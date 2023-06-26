using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using MimeKit;
using WebApiFunction.Configuration;
using WebApiFunction.Database;
using MySql.Data.MySqlClient;
using WebApiFunction.Application.Model.DataTransferObject;

namespace WebApiFunction.Application.Model.Database.MySQL.Jellyfish
{
    [Serializable]
    public class ChatDTO : DataTransferModelAbstract
    {

    }
}
