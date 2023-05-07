using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApiApplicationService.Models;
using WebApiApplicationService.InternalModels;
using WebApiApplicationService.Models.Database;

namespace WebApiApplicationService.Modules
{
    public class SchemaDataCommandModule : CustomBackendModule<SchemaDataCommandModel>
    {
        #region Private
        #endregion
        #region Public

        #endregion
        #region Ctor & Dtor
        public SchemaDataCommandModule(IScopedDatabaseHandler databaseHandler, WebApiApplicationService.ICachingHandler cache) : base(databaseHandler, cache)
        {

        }
        #endregion
        #region Methods
        #endregion
    }
}
