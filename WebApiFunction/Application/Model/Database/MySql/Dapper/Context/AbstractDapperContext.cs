using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiFunction.Configuration;

namespace WebApiFunction.Application.Model.Database.MySql.Dapper.Context
{
    public class MysqlDapperContext
    {
        private readonly IAppconfig _config;
        private readonly string _connectionString;
        public MysqlDapperContext(IAppconfig appconfig)
        {
            _config = appconfig;
            _connectionString = _config.AppServiceConfiguration.DatabaseConfigurationModel.MysqlConnectionString.ToString();

        }
        public IDbConnection GetConnection() => new MySqlConnection(_connectionString);
    }
}
