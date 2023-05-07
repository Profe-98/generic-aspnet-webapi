using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using nClam;
using System.Net;
using System.IO;
using System.Text;
using Microsoft.Extensions.Logging;
using System.Reflection;
using System.Threading;
using MySql.Data.MySqlClient;
using WebApiFunction.Data.Web.MIME;
using WebApiFunction.Application.Model.Internal;
using WebApiFunction.Application.Model.Database.MySql;
using WebApiFunction.Application.Model.Database.MySql.Entity;
using WebApiFunction.Cache.Distributed.RedisCache;
using WebApiFunction.Ampq.Rabbitmq.Data;
using WebApiFunction.Ampq.Rabbitmq;
using WebApiFunction.Antivirus;
using WebApiFunction.Antivirus.nClam;
using WebApiFunction.Application.Model.DataTransferObject.Helix.Frontend.Transfer;
using WebApiFunction.Application.Model.DataTransferObject;
using WebApiFunction.Application.Model;
using WebApiFunction.Configuration;
using WebApiFunction.Collections;
using WebApiFunction.Controller;
using WebApiFunction.Data;
using WebApiFunction.Data.Web;
using WebApiFunction.Data.Format.Json;
using WebApiFunction.Data.Web.Api.Abstractions.JsonApiV1;
using WebApiFunction.Database;
using WebApiFunction.Database.MySQL;
using WebApiFunction.Database.MySQL.Data;
using WebApiFunction.Filter;
using WebApiFunction.Formatter;
using WebApiFunction.LocalSystem.IO.File;
using WebApiFunction.Log;
using WebApiFunction.Metric;
using WebApiFunction.Metric.Influxdb;
using WebApiFunction.MicroService;
using WebApiFunction.Network;
using WebApiFunction.Security;
using WebApiFunction.Security.Encryption;
using WebApiFunction.Threading;
using WebApiFunction.Threading.Service;
using WebApiFunction.Threading.Task;
using WebApiFunction.Utility;
using WebApiFunction.Web;
using WebApiFunction.Web.AspNet;
using WebApiFunction.Web.Authentification;
using WebApiFunction.Web.Http.Api.Abstractions.JsonApiV1;
using WebApiFunction.Web.Http;

namespace WebApiFunction.Antivirus.nClam
{
    /// <summary>
    /// Anti Virus protector for File Upload and general real time check of filesystem to protect the environment from maliscious malware
    /// The Antivirus Software for protection is the open source (Apache 2) av software: ClamAV, net lib is nClam
    /// IMPORTANT: For Eicar Testfile disable regular system av solution, than you can test successfully with eicar
    /// Download: https://www.clamav.net/downloads
    /// Documentation: https://www.clamav.net/documents/clam-antivirus-user-manual
    /// 
    /// ** Linux Ubuntu **
    /// Installation of ClamAV:
    /// apt-get install clamav
    /// Daemon: apt-get install clamav-daemon
    /// Support for Scanning Archives like RAR: apt-get install libclamunrar6
    /// Or you run it on a docker container, docker images are existing a lot in the net
    /// 
    /// ** Windows 10 x64 **
    /// Installation:
    /// Download the executable 'https://www.clamav.net/downloads' select Win64, download and install
    /// 
    /// ClamAV-Serving: clamd --install
    /// Virus-Database Update: reshclam --install
    /// 
    /// 
    /// 
    /// Use:
    /// ClamAV serving an api to check file for vulnerablities.
    /// For updating the virus database, execute in the installation dir of clamav: freshclam.exe or the linux equivalent 
    /// 
    /// 
    /// 
    /// 
    /// 
    /// Example use in c#:
    /// 
    /// var clam = new ClamClient("localhost", 3310);
    /// var scanResult = clam.ScanFileOnServer("C:\\test.txt");  //any file you would like!
    ///  
    /// switch (scanResult.Result)
    /// {
    ///     case ClamScanResults.Clean:
    ///         Console.WriteLine("The file is clean!");
    ///         break;
    ///     case ClamScanResults.VirusDetected:
    ///         Console.WriteLine("Virus Found!");
    ///         Console.WriteLine("Virus name: {0}", scanResult.InfectedFiles.First().VirusName);
    ///         break;
    ///     case ClamScanResults.Error:
    ///         Console.WriteLine("Woah an error occured! Error: {0}", scanResult.RawResult);
    ///         break;
    /// }
    /// </summary>
    public class VulnerablityHandler : IScopedVulnerablityHandler, ISingletonVulnerablityHandler
    {
        /*
         * For checks: create a txt-file with following content: X5O!P%@AP[4\PZX54(P^)7CC)7}$EICAR-STANDARD-ANTIVIRUS-TEST-FILE!$H+H*
         */
        public readonly string Host;
        public readonly int Port;

        private readonly System.Timers.Timer _pingTimeoutTimer = new System.Timers.Timer();
        public CancellationTokenSource CancellationTokenSourcePing;
        public readonly ClamClient ClamClient;
        public readonly string QuarantineDirectory;
        public readonly bool DeleteInfectedFilesPermantly;
        private readonly ILogger<VulnerablityHandler> _logger;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ip">Default localhost</param>
        /// <param name="port">Default 3310 for ClamAV API</param>
        /// <param name="quarantineDirectory"></param>
        /// <param name="deleteInfectedFilesPermantly"></param>
        public VulnerablityHandler(string avQuarantinePath, string host = "127.0.0.1", int port = 3310, bool deleteFilesPermantly = true)
        {
            _logger = new LoggerFactory().CreateLogger<VulnerablityHandler>();
            Host = host;
            Port = port;
            string quarantineDirectory = avQuarantinePath;
            bool deleteInfectedFilesPermantly = deleteFilesPermantly;
            if (!IPAddress.TryParse(Host, out IPAddress addr))
            {
                addr = Dns.GetHostEntry(Host).AddressList.FirstOrDefault(IPAddress.Loopback);
            }
            IPAddress avEndpoint = addr;

            if (!Directory.Exists(quarantineDirectory))
            {
                Directory.CreateDirectory(quarantineDirectory);
            }
            QuarantineDirectory = quarantineDirectory;
            DeleteInfectedFilesPermantly = deleteInfectedFilesPermantly;
            ClamClient = new ClamClient(avEndpoint, Port);
            Init();
        }


        private async void Init()
        {
            bool ping = await CheckConnection();
            if (!ping)
            {
                _logger.Logging(LogLevel.Error, "antivirus host not available, host seems down", MethodBase.GetCurrentMethod(), GetType().Name);
            }
            else
            {
                _logger.Logging(LogLevel.Information, "antivirus host successfully connected", MethodBase.GetCurrentMethod(), GetType().Name);

            }
        }

        public async Task<bool> CheckConnection()
        {
            CancellationTokenSourcePing = new CancellationTokenSource();
            CancellationTokenSource CancellationTokenSourcePingTimeout = new CancellationTokenSource();
            try
            {

                _pingTimeoutTimer.Start();
                Task<bool> ping = ClamClient.PingAsync(CancellationTokenSourcePing.Token);
                bool pingR = await Task.WhenAny(ping, Task.Delay(1000, CancellationTokenSourcePingTimeout.Token)) == ping;
                if (!pingR)
                {
                    CancellationTokenSourcePing.Cancel();
                    return false;
                }
                else
                {
                    CancellationTokenSourcePingTimeout.Cancel();
                    return await ping;
                }

            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<ClamScanResult> Scan(byte[] binary)
        {
            ClamScanResult clamScanResult = await ClamClient.SendAndScanFileAsync(new MemoryStream(binary));

            return clamScanResult;
        }

        public async Task<KeyValuePair<string, ClamScanResult>> CheckFile(string filePath)
        {
            KeyValuePair<string, ClamScanResult> clamScanResult = new KeyValuePair<string, ClamScanResult>();
            if (File.Exists(filePath))
            {
                byte[] binary = File.ReadAllBytes(filePath);
                ClamScanResult tmp = await Scan(binary);

                clamScanResult = new KeyValuePair<string, ClamScanResult>(filePath, tmp);
                ScoreAndClean(clamScanResult);
            }
            return clamScanResult;
        }
        public async Task<Dictionary<string, ClamScanResult>> CheckFile(string[] filePaths)
        {
            Dictionary<string, ClamScanResult> clamScanResult = new Dictionary<string, ClamScanResult>();
            foreach (string path in filePaths)
            {
                if (File.Exists(path))
                {
                    KeyValuePair<string, ClamScanResult> tmp = await CheckFile(path);
                    clamScanResult.Add(tmp.Key, tmp.Value);
                }
            }
            return clamScanResult;
        }

        public void ScoreAndClean(KeyValuePair<string, ClamScanResult> clamScanResult)
        {
            bool moveToQuarantine = false;
            switch (clamScanResult.Value.Result)
            {
                case ClamScanResults.Clean://file is ok
                    moveToQuarantine = true;
                    break;
                case ClamScanResults.Error://move to quarantine
                    moveToQuarantine = true;
                    break;
                case ClamScanResults.Unknown://move to quarantine
                    moveToQuarantine = true;
                    break;
                case ClamScanResults.VirusDetected://delete vulnerablity
                    if (DeleteInfectedFilesPermantly)
                        DeleteFile(clamScanResult.Key);
                    break;
            }
            if (moveToQuarantine)
            {
                string source = clamScanResult.Key;
                string sourceFileName = Path.GetFileName(source);
                string dest = Path.Combine(QuarantineDirectory, sourceFileName);
                File.Move(source, dest);
            }
        }


        private bool DeleteFile(string filePath)
        {
            try
            {
                File.Delete(filePath);
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }


    }
}
