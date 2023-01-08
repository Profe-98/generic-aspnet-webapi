﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApiApplicationService.Models;
using WebApiApplicationService.InternalModels;
using WebApiApplicationService.Models.Database;

namespace WebApiApplicationService.Modules
{
    public class OdbcDriverModule : CustomBackendModule<OdbcDriverModel>
    {
        #region Private
        #endregion
        #region Public

        #endregion
        #region Ctor & Dtor
        public OdbcDriverModule(IScopedDatabaseHandler databaseHandler, WebApiApplicationService.ICachingHandler cache) : base(databaseHandler, cache)
        {

        }
        #endregion
        #region Methods
        #endregion
    }
}
