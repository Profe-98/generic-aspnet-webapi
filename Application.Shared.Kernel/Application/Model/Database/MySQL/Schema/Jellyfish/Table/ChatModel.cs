using System;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using MySql.Data.MySqlClient;
using Application.Shared.Kernel.Configuration.Const;

namespace Application.Shared.Kernel.Application.Model.Database.MySQL.Schema.Jellyfish.Table
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
        public virtual string PictureBase64 { get => Picture != null ? Convert.ToBase64String(Picture) : null; }
    }
}
