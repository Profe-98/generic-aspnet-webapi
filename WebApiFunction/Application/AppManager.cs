using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using WebApiFunction.Application.Controller.Modules;
using WebApiFunction.Application.Model.Internal;
using WebApiFunction.Application.Model.Database.MySql;
using WebApiFunction.Application.Model.Database.MySql.Entity;

using WebApiFunction.Cache.Distributed.RedisCache;
using WebApiFunction.Ampq.Rabbitmq.Data;
using WebApiFunction.Ampq.Rabbitmq;
using WebApiFunction.Antivirus;
using WebApiFunction.Antivirus.nClam;
using WebApiFunction.Application.Model.DataTransferObject.Frontend.Transfer;
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
using WebApiFunction.Web.AspNet.ActionResult;

namespace WebApiFunction.Application
{
    public static class AppManager
    {
        /// <summary>
        /// Allgemeine Klasse zum Verwalten von der App
        /// </summary>
        public enum MESSAGE_LEVEL : int
        {
            LEVEL_INVALID = -1,
            LEVEL_NORMAL = 0,
            LEVEL_INFO = 1,
            LEVEL_NOTE = 2,
            LEVEL_WARNING = 3,
            LEVEL_CRITICAL = 4
        }

        private static List<ApiModel> _controller = new List<ApiModel>();
        private static ThreadManager _theadManager = null;
        private static TranslationManager _translationManager = new TranslationManager();

        static AppManager()
        {
        }

        public static List<ApiModel> Api
        {
            get
            {
                return _controller;
            }
        }
        public static List<SystemMessageUserModel> SystemUsedMediumAccess { get; set; }
        public static ThreadManager Th = ThreadManager;//alias
        public static ThreadManager ThreadManager
        {
            get
            {
                InitThreadManager();
                return _theadManager;
            }
        }
        private static void InitThreadManager()
        {
            if (_theadManager == null)
            {
                _theadManager = new ThreadManager();
            }
        }
        public static TranslationManager T = TranslationManager;//alias
        public static TranslationManager TranslationManager
        {
            get { return _translationManager; }
        }


        #region Methods
        public static void InitRegisterApi(List<ApiModel> endpoints)
        {
            if (_controller.Count == 0)
                _controller = endpoints;
        }
        #endregion Methods
    }
}
