using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using nClam;
using System.Threading;

namespace WebApiApplicationService
{
    public interface IVulnerablityHandler 
    {

        public Task<Dictionary<string, ClamScanResult>> CheckFile(string[] filePaths);
        public void ScoreAndClean(KeyValuePair<string, ClamScanResult> clamScanResult);
        public Task<ClamScanResult> Scan(byte[] binary);
        public Task<KeyValuePair<string, ClamScanResult>> CheckFile(string filePath);
        public Task<bool> CheckConnection();
    }

    public interface ISingletonVulnerablityHandler : IVulnerablityHandler
    {

    }
    public interface IScopedVulnerablityHandler : IVulnerablityHandler
    {

    }
}
