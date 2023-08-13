﻿using System;
using Application.Shared.Kernel.Application.Model.Database.MySQL.Schema.ApiGateway.Table;

namespace Application.Shared.Kernel.Application.Controller.Modules
{
    public class ControllerActionModule : AbstractBackendModule<ControllerActionModel>
    {
        #region Private
        #endregion
        #region Public

        #endregion
        #region Ctor & Dtor
        public ControllerActionModule(ISingletonDatabaseHandler databaseHandler, ICachingHandler cache, IMysqlDapperContext mysqlDapperContext) : base(databaseHandler, cache, mysqlDapperContext)
        {

        }
        #endregion
        #region Methods
        #endregion
    }
}