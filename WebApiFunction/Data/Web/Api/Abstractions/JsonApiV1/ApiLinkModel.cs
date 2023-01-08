using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using System.Text.Json;
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

namespace WebApiFunction.Data.Web.Api.Abstractions.JsonApiV1
{
    [Serializable]
    public class ApiLinkModel
    {
        #region Private
        private string _self = null;
        private string _next = null;
        private string _previous = null;
        private string _last = null;
        private string _related = null;
        #endregion
        #region Public
        [JsonPropertyName("self")]
        public string Self
        {
            get
            {
                return _self;
            }
            set
            {
                _self = string.IsNullOrEmpty(value) ? null : value.ToLower();
            }
        }
        [JsonPropertyName("next")]
        public string Next
        {
            get
            {
                return _next;
            }
            set
            {
                _next = string.IsNullOrEmpty(value) ? null : value.ToLower();
            }
        }
        [JsonPropertyName("previous")]
        public string Previous
        {
            get
            {
                return _previous;
            }
            set
            {
                _previous = string.IsNullOrEmpty(value) ? null : value.ToLower();
            }
        }
        [JsonPropertyName("last")]
        public string Last
        {
            get
            {
                return _last;
            }
            set
            {
                _last = string.IsNullOrEmpty(value) ? null : value.ToLower();
            }
        }
        [JsonPropertyName("related")]
        public string Related
        {
            get
            {
                return _related;
            }
            set
            {
                _related = string.IsNullOrEmpty(value) ? null : value.ToLower();
            }
        }
        [JsonPropertyName("meta")]
        public ApiMetaModel Meta { get; set; }
        #endregion

        #region Ctor
        #endregion

    }
}
