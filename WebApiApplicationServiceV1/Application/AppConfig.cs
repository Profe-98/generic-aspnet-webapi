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
using WebApiApplicationService;
using WebApiApplicationService.Handler;
using System.IO;

namespace WebApiApplicationService 
{

    public class AppConfig : IAppconfig
    {
        #region Private
        private readonly IConfiguration _configuration = null;
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

        public AppConfig(IConfiguration configuration,ISingletonJsonHandler jsonHandler, Microsoft.AspNetCore.Hosting.IWebHostEnvironment env)
        {
            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
            _configuration = configuration;
            _jsonHandler = jsonHandler;
            string basePath = Path.Combine(env.ContentRootPath, "Config");
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

        private void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            Save();
        }

        public void Load()
        {

            string json = File.ReadAllText(this.ConfigPath);
            AppServiceConfigurationModel appServiceConfigurationModel = _jsonHandler.JsonDeserialize<AppServiceConfigurationModel>(json);

            WebApiConfigurationModel webApiConfigurationModel = appServiceConfigurationModel.WebApiConfigurationModel;
            ApiSecurityConfigurationModel apiSecurityConfigurationModel = appServiceConfigurationModel.ApiSecurityConfigurationModel;
            LogConfigurationModel logConfigurationModel = appServiceConfigurationModel.LogConfigurationModel;
            MailConfigurationModel mailConfigurationModel = appServiceConfigurationModel.MailConfigurationModel;
            AntivirusConfigurationModel antiVirusConfigurationModel = appServiceConfigurationModel.AntivirusConfigurationModel;
            DatabaseConfigurationModel databaseConfigurationModel = appServiceConfigurationModel.DatabaseConfigurationModel;
            CacheConfigurationModel cacheConfigurationModel = appServiceConfigurationModel.CacheConfigurationModel;
            AmpqConfigurationModel ampqConfigurationModel = appServiceConfigurationModel.RabbitMqConfigurationModel;

            AppServiceConfigurationModel tmp = new AppServiceConfigurationModel();
            tmp.AntivirusConfigurationModel = antiVirusConfigurationModel;
            tmp.DatabaseConfigurationModel = databaseConfigurationModel;
            tmp.ApiSecurityConfigurationModel = apiSecurityConfigurationModel;
            tmp.WebApiConfigurationModel = webApiConfigurationModel;
            tmp.LogConfigurationModel = logConfigurationModel;
            tmp.MailConfigurationModel = mailConfigurationModel;
            tmp.CacheConfigurationModel = cacheConfigurationModel;
            tmp.RabbitMqConfigurationModel = ampqConfigurationModel;


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
