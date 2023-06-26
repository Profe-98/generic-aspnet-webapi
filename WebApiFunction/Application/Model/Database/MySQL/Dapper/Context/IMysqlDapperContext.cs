using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiFunction.Application.Model.Database.MySQL.Dapper.Context
{
    public interface IMysqlDapperContext
    {
        public MySqlConnection GetConnection();
    }
}
