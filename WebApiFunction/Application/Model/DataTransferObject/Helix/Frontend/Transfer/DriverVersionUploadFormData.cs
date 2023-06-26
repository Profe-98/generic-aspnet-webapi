using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using WebApiFunction.Data.Web.MIME;
using WebApiFunction.Application.Model.Internal;


using WebApiFunction.Cache.Distributed.RedisCache;
using WebApiFunction.Ampq.Rabbitmq.Data;
using WebApiFunction.Ampq.Rabbitmq;
using WebApiFunction.Antivirus;
using WebApiFunction.Antivirus.nClam;
using WebApiFunction.Configuration;
using WebApiFunction.Collections;
using WebApiFunction.Web.AspNet.Controller;
using WebApiFunction.Data;
using WebApiFunction.Data.Web;
using WebApiFunction.Data.Format.Json;
using WebApiFunction.Data.Web.Api.Abstractions.JsonApiV1;
using WebApiFunction.Database;
using WebApiFunction.Application.Model.Database.MySQL;
using WebApiFunction.Application.Model.Database.MySQL.Data;
using WebApiFunction.Web.AspNet.Filter;
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

namespace WebApiFunction.Application.Model.DataTransferObject.Helix.Frontend.Transfer
{
    public class DriverVersionUploadFormData : GeneralMimeFileFormData
    {
        public new IFormFile File { get => FileLibrary; set => FileLibrary = value; }
        public new string FileName { get => FileNameLibrary; set => FileNameLibrary = value; }

        [Required()]
        public IFormFile FileLibrary { get; set; }
        [Required()]
        public string FileNameLibrary { get; set; }
        [Required()]
        public IFormFile FileSetup { get; set; }
        [Required()]
        public string FileNameSetup { get; set; }

        public DriverVersionUploadFormData() : base()
        {

        }
    }
}
