﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Data;
using System.IO;
using System.Reflection;
using WebApiFunction.Controller;
using WebApiFunction.Antivirus.nClam;
using WebApiFunction.Database;
using WebApiFunction.Web.Authentification;
using WebApiFunction.Mail;
using WebApiFunction.Web.Http.Api.Abstractions.JsonApiV1;
using WebApiFunction.Threading.Service;
using WebApiFunction.Data.Format.Json;
using WebApiFunction.Cache.Distributed.RedisCache;
using WebApiFunction.Ampq.Rabbitmq;
using WebApiFunction.Configuration;
using WebApiFunction.MicroService;
using WebApiFunction.Security.Encryption;
using WebApiFunction.Application.Model.Database.MySql.Helix;

namespace WebApiFunction.Application.Controller.Modules.Helix
{
    public class MessageAttachmentModule : AbstractBackendModule<MessageAttachmentModel>
    {
        #region Private
        #endregion
        #region Public

        #endregion
        #region Ctor & Dtor
        public MessageAttachmentModule(IScopedDatabaseHandler databaseHandler, ICachingHandler cache, Model.Database.MySql.Dapper.Context.MysqlDapperContext mysqlDapperContext) : base(databaseHandler, cache, mysqlDapperContext)
        {

        }
        #endregion
        #region Methods

        #endregion
    }
}