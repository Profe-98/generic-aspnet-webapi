using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Shared.Kernel.Configuration.Service;

namespace Application.Shared.Kernel.Application.Model.Dapper.Mysql.Context
{
    public class AbstractDapperContext : IMysqlDapperContext
    {
        private readonly IAppconfig _config;
        private readonly string _connectionString;
        private MySqlConnection _connection;
        public AbstractDapperContext(IAppconfig appconfig)
        {
            _config = appconfig;
            _connectionString = _config.AppServiceConfiguration.DatabaseConfigurationModel.MysqlConnectionString.ToString();

        }
        public MySqlConnection GetConnection() => _connection = _connection ?? new MySqlConnection(_connectionString);
    }
}
