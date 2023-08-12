using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using MySql.Data.MySqlClient;
using Application.Shared.Kernel.Configuration.Const;
using Application.Shared.Kernel.Application.Model.Database.MySQL;

namespace Application.Shared.Kernel.Application.Model.Database.MySQL.Schema.ApiGateway.Table
{
    public class ClassRelationModel : AbstractModel
    {

        public Type EntityOneNetType;
        public Type EntityTwoNetType;

        public string RelationName { get => "rel_" + EntityOne + "_" + EntityOneKeyCol + "<--->" + EntityTwo + "_" + EntityTwoKeyCol; }

        [Required(ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg)]
        [JsonPropertyName("direction")]
        [DatabaseColumnProperty("direction", MySqlDbType.String)]
        public string Direction { get; set; } = null;

        [Required(ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg)]
        [JsonPropertyName("entity_one")]
        [DatabaseColumnProperty("entity_one", MySqlDbType.String)]
        public string EntityOne { get; set; } = null;

        [Required(ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg)]
        [JsonPropertyName("entity_one_key_col")]
        [DatabaseColumnProperty("entity_one_key_col", MySqlDbType.String)]
        public string EntityOneKeyCol { get; set; } = null;

        [Required(ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg)]
        [JsonPropertyName("entity_one_relationship_type_uuid")]
        [DatabaseColumnProperty("entity_one_relationship_type_uuid", MySqlDbType.String)]
        public Guid EntityOneRelationshipTypeUuid { get; set; } = Guid.Empty;

        [Required(ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg)]
        [JsonPropertyName("entity_two_relationship_type_uuid")]
        [DatabaseColumnProperty("entity_two_relationship_type_uuid", MySqlDbType.String)]
        public Guid EntityTwoRelationshipTypeUuid { get; set; } = Guid.Empty;

        [Required(ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg)]
        [JsonPropertyName("entity_one_class_uuid")]
        [DatabaseColumnProperty("entity_one_class_uuid", MySqlDbType.String)]
        public Guid EntityOneClassUuid { get; set; } = Guid.Empty;

        [Required(ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg)]
        [JsonPropertyName("entity_two_class_uuid")]
        [DatabaseColumnProperty("entity_two_class_uuid", MySqlDbType.String)]
        public Guid EntityTwoClassUuid { get; set; } = Guid.Empty;

        [Required(ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg)]
        [JsonPropertyName("entity_two")]
        [DatabaseColumnProperty("entity_two", MySqlDbType.String)]
        public string EntityTwo { get; set; } = null;

        [Required(ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg)]
        [JsonPropertyName("entity_two_key_col")]
        [DatabaseColumnProperty("entity_two_key_col", MySqlDbType.String)]
        public string EntityTwoKeyCol { get; set; } = null;

        public override string ToString()
        {
            return EntityOne + Direction + EntityTwo;
        }
    }
}
