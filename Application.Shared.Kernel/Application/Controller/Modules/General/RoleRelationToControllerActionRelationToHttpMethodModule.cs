using System;
using Application.Shared.Kernel.Application.Model.Database.MySQL.Schema.ApiGateway.Table;

namespace Application.Shared.Kernel.Application.Controller.Modules
{
    public class RoleRelationToControllerActionRelationToHttpMethodModule : AbstractBackendModule<RoleRelationToControllerActionRelationToHttpMethodModel>
    {
        #region Private
        #endregion
        #region Public

        #endregion
        #region Ctor & Dtor
        public RoleRelationToControllerActionRelationToHttpMethodModule(ISingletonDatabaseHandler databaseHandler, ICachingHandler cache, IMysqlDapperContext mysqlDapperContext) : base(databaseHandler, cache, mysqlDapperContext)
        {

        }
        #endregion
        #region Methods

        #endregion
    }
}
