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

namespace WebApiFunction.Application.Model.Database.MySQL.Jellyfish
{
    [Serializable]
    public class ChatModel : AbstractModel
    {

        [DataType(DataType.Text, ErrorMessage = DataValidationMessageStruct.WrongDataTypeGivenMsg)]
        [MinLength(1, ErrorMessage = DataValidationMessageStruct.StringMinLengthExceededMsg), MaxLength(45, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonPropertyName("name")]
        [DatabaseColumnProperty("name", MySqlDbType.String)]
        public string Name { get; set; }

        [JsonIgnore]
        [DatabaseColumnProperty("picture", MySqlDbType.Blob)]
        public virtual byte[] Picture { get; set; }

        [JsonPropertyName("picture")]
        public virtual string PictureBase64 { get => this.Picture != null ? Convert.ToBase64String(this.Picture) : null; }
    }
}
