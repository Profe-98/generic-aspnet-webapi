using Microsoft.AspNetCore.Hosting;
using Application.Shared.Kernel.Configuration.Extension;
using Application.Shared.Kernel.Configuration.Model.ConcreteImplementation;

using Application.Shared.Kernel.Data.Format.Json;

namespace Application.Shared.Kernel.Configuration.Service
{

    public class AppConfig : IAppconfig
    {
        #region Private
        private readonly string _configurationFilePath;
        private readonly ISingletonJsonHandler _jsonHandler = null;
        private MainConfigurationModel _appServiceConfigurationModel;
        #endregion
        #region Public

        public MainConfigurationModel AppServiceConfiguration
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
        public AppConfig(ISingletonJsonHandler jsonHandler, IWebHostEnvironment env) : this(jsonHandler, env.ContentRootPath)
        {

        }

        private void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            Save();
        }

        public void Load()
        {

            string json = File.ReadAllText(ConfigPath);
            MainConfigurationModel appServiceConfigurationModel = _jsonHandler.JsonDeserialize<MainConfigurationModel>(json);

            WebApiConfigurationModel webApiConfigurationModel = appServiceConfigurationModel.WebApiConfigurationModel;
            ApiSecurityConfigurationModel apiSecurityConfigurationModel = appServiceConfigurationModel.ApiSecurityConfigurationModel;
            LogConfigurationModel logConfigurationModel = appServiceConfigurationModel.LogConfigurationModel;
            MailConfigurationModel mailConfigurationModel = appServiceConfigurationModel.MailConfigurationModel;
            ClamAvConfigurationModel antiVirusConfigurationModel = appServiceConfigurationModel.AntivirusConfigurationModel;
            DatabaseConfigurationModel databaseConfigurationModel = appServiceConfigurationModel.DatabaseConfigurationModel;
            DatabaseConfigurationModel databaseNodeManagerConfigurationModel = appServiceConfigurationModel.NodeManagerDatabaseConfigurationModel;
            CacheConfigurationModel cacheConfigurationModel = appServiceConfigurationModel.CacheConfigurationModel;
            AmpqConfigurationModel ampqConfigurationModel = appServiceConfigurationModel.RabbitMqConfigurationModel;
            SignalRConfigurationModel signalRConfigurationModel = appServiceConfigurationModel.SignalRHubConfigurationModel;

            MainConfigurationModel tmp = new MainConfigurationModel(_configurationFilePath);
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
            string json = _jsonHandler.JsonSerialize(_appServiceConfigurationModel);
            File.WriteAllText(ConfigPath, json);
        }
        #endregion
    }
}
