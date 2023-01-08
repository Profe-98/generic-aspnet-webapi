﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApiApplicationService.Handler;
using WebApiApplicationService.Models;
using WebApiApplicationService.InternalModels;
using MySql.Data.MySqlClient;
using System.Data;
using System.IO;
using System.Reflection;
using WebApiApplicationService.Models.Database;

namespace WebApiApplicationService.Modules
{
    public class AuthModule : CustomBackendModule<AuthModel>
    {
        #region Private
        #endregion
        #region Public

        #endregion
        #region Ctor & Dtor
        public AuthModule(IScopedDatabaseHandler databaseHandler, WebApiApplicationService.ICachingHandler cache) : base(databaseHandler, cache)
        {

        }
        #endregion
        #region Methods
        
        #endregion
    }
}
