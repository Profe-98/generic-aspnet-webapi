using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using System.Net;
using System.ComponentModel.DataAnnotations;
using MySql.Data.MySqlClient;
using WebApiFunction.Converter;
using WebApiFunction.Data.Web.MIME;
using WebApiFunction.Application.Model.Internal;
using WebApiFunction.Cache.Distributed.RedisCache;
using WebApiFunction.Ampq.Rabbitmq.Data;
using WebApiFunction.Ampq.Rabbitmq;
using WebApiFunction.Antivirus;
using WebApiFunction.Antivirus.nClam;
using WebApiFunction.Application.Model.DataTransferObject.Helix.Frontend.Transfer;
using WebApiFunction.Application.Model.DataTransferObject;
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
using WebApiFunction.Application.Model.Database.MySQL.Jellyfish;
using Microsoft.AspNetCore.Authorization;

namespace WebApiFunction.Application.Model.Database.MySQL.Jellyfish
{
    [Serializable]
    public class AuthModel : Table.AuthModel
    {
        #region Private
        #endregion Private
        #region Public
        #endregion Public

        [JsonIgnore]
        [DatabaseColumnProperty("uuid", MySqlDbType.String)]
        public override Guid Uuid { get; set; } = Guid.Empty;


        [JsonPropertyName("test")]
        [SensitiveDataAttribute("user,admin,root")]
        public string Test { get; set; } = "testeintrag";

        #region Ctor & Dtor
        public AuthModel()
        {

        }
        public AuthModel(Table.AuthModel authModel)
        {
            if (authModel == null)
                return;
            Uuid = authModel.Uuid;
            UserUuid = authModel.UserUuid;
            Ipv4 = authModel.Ipv4;
            Ipv6 = authModel.Ipv6;
            RemotePort = authModel.RemotePort;
            LocalPort = authModel.LocalPort;
            Ipv4Local = authModel.Ipv4Local;
            Ipv6Local = authModel.Ipv6Local;
            Token = authModel.Token;
            TokenExpires = authModel.TokenExpires;
            RefreshToken = authModel.RefreshToken;
            RefreshTokenExpires = authModel.RefreshTokenExpires;
            UserAgent = authModel.UserAgent;
            LogoutTime = authModel.LogoutTime;
            IsAdmin = authModel.IsAdmin;
            UserModel = new UserModel(authModel.UserModel);
        }

        #endregion Ctor & Dtor
        #region Methods
        private IPAddress ConvertStringToIp(string ipStr)
        {
            if (IPAddress.TryParse(Ipv4Local, out IPAddress address))
            {
                return address;
            }
            return null;
        }
        #endregion Methods
    }
}
