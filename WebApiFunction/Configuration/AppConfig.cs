using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Text.Json.Serialization;
using System.Globalization;
using MySql.Data.MySqlClient;
using Microsoft.Extensions.Hosting;
using MimeKit;
using MailKit;
using MailKit.Search;
using MailKit.Security;
using MailKit.Net.Imap;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using System.IO;
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
using Microsoft.AspNetCore.Hosting;


namespace WebApiFunction.Configuration
{

    public class AppConfig : IAppconfig
    {
        #region Private
        private readonly string _configurationFilePath;
        private readonly ISingletonJsonHandler _jsonHandler = null;
        private AppServiceConfigurationModel _appServiceConfigurationModel;
        #endregion
        #region Public
        
        public AppServiceConfigurationModel AppServiceConfiguration
        {
            get
            {
                return _appServiceConfigurationModel;
            }
            set
            {
                _appServiceConfigurationModel = value;

            }
        }
        public string ConfigPath
        { get; private set; }
        #endregion
        #region Ctor & Dtor

        public AppConfig(ISingletonJsonHandler jsonHandler, string configFilePath)
        {
            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
            _jsonHandler = jsonHandler;
            _configurationFilePath = configFilePath;
            string basePath = Path.Combine(_configurationFilePath, "Config");
            string configPath = Path.Combine(basePath, "appserviceconfiguration.json");
            ConfigPath = configPath;
            /*string antiVirusConfigurationModelStr = _configuration["anti_virus_configuration"];
            string databaseConfigurationModelStr = _configuration["database_configuration"];
            string apiSecurityConfigurationModelStr = _configuration["api_security_configuration"];
            string webApiConfigurationModelStr = _configuration["web_api_configuration"];
            string logConfigurationModelStr = _configuration["logging_configuration"];
            string mailConfigurationModelStr = _configuration["mail_configuration"];*/
            Load();
        }
        public AppConfig(ISingletonJsonHandler jsonHandler, IWebHostEnvironment env):this(jsonHandler, env.ContentRootPath)
        {

        }

        private void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            Save();
        }

        public void Load()
        {

            string json = File.ReadAllText(ConfigPath);
            AppServiceConfigurationModel appServiceConfigurationModel = _jsonHandler.JsonDeserialize<AppServiceConfigurationModel>(json);

            WebApiConfigurationModel webApiConfigurationModel = appServiceConfigurationModel.WebApiConfigurationModel;
            ApiSecurityConfigurationModel apiSecurityConfigurationModel = appServiceConfigurationModel.ApiSecurityConfigurationModel;
            LogConfigurationModel logConfigurationModel = appServiceConfigurationModel.LogConfigurationModel;
            MailConfigurationModel mailConfigurationModel = appServiceConfigurationModel.MailConfigurationModel;
            AntivirusConfigurationModel antiVirusConfigurationModel = appServiceConfigurationModel.AntivirusConfigurationModel;
            DatabaseConfigurationModel databaseConfigurationModel = appServiceConfigurationModel.DatabaseConfigurationModel;
            DatabaseConfigurationModel databaseNodeManagerConfigurationModel = appServiceConfigurationModel.NodeManagerDatabaseConfigurationModel;
            CacheConfigurationModel cacheConfigurationModel = appServiceConfigurationModel.CacheConfigurationModel;
            AmpqConfigurationModel ampqConfigurationModel = appServiceConfigurationModel.RabbitMqConfigurationModel;
            SignalRConfigurationModel signalRConfigurationModel = appServiceConfigurationModel.SignalRHubConfigurationModel;

            AppServiceConfigurationModel tmp = new AppServiceConfigurationModel(_configurationFilePath);
            tmp.AntivirusConfigurationModel = antiVirusConfigurationModel;
            tmp.DatabaseConfigurationModel = databaseConfigurationModel;
            tmp.NodeManagerDatabaseConfigurationModel = databaseNodeManagerConfigurationModel;
            tmp.ApiSecurityConfigurationModel = apiSecurityConfigurationModel;
            tmp.WebApiConfigurationModel = webApiConfigurationModel;
            tmp.LogConfigurationModel = logConfigurationModel;
            tmp.MailConfigurationModel = mailConfigurationModel;
            tmp.CacheConfigurationModel = cacheConfigurationModel;
            tmp.RabbitMqConfigurationModel = ampqConfigurationModel;
            tmp.SignalRHubConfigurationModel = signalRConfigurationModel;


            _appServiceConfigurationModel = tmp;
        }
        public void Save()
        {
            string json = _jsonHandler.JsonSerialize<AppServiceConfigurationModel>(_appServiceConfigurationModel);
            File.WriteAllText(ConfigPath, json);
        }
        #endregion
    }
}
