using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace WebApiApplicationService.Models.Database
{
    public class ClassRelationModel : AbstractModel
    {

        public Type EntityOneNetType;
        public Type EntityTwoNetType;

        public string RelationName { get => "rel_"+EntityOne+"_"+ EntityOneKeyCol+"<--->" + EntityTwo+"_"+ EntityTwoKeyCol; }

        [Required(ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg)]
        [JsonPropertyName("direction")]
        [DatabaseColumnPropertyAttribute("direction", MySql.Data.MySqlClient.MySqlDbType.String)]
        public string Direction { get; set; } = null;

        [Required(ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg)]
        [JsonPropertyName("entity_one")]
        [DatabaseColumnPropertyAttribute("entity_one", MySql.Data.MySqlClient.MySqlDbType.String)]
        public string EntityOne { get; set; } = null;

        [Required(ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg)]
        [JsonPropertyName("entity_one_key_col")]
        [DatabaseColumnPropertyAttribute("entity_one_key_col", MySql.Data.MySqlClient.MySqlDbType.String)]
        public string EntityOneKeyCol { get; set; } = null;

        [Required(ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg)]
        [JsonPropertyName("entity_one_relationship_type_uuid")]
        [DatabaseColumnPropertyAttribute("entity_one_relationship_type_uuid", MySql.Data.MySqlClient.MySqlDbType.String)]
        public Guid EntityOneRelationshipTypeUuid { get; set; } = Guid.Empty;

        [Required(ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg)]
        [JsonPropertyName("entity_two_relationship_type_uuid")]
        [DatabaseColumnPropertyAttribute("entity_two_relationship_type_uuid", MySql.Data.MySqlClient.MySqlDbType.String)]
        public Guid EntityTwoRelationshipTypeUuid { get; set; } = Guid.Empty;

        [Required(ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg)]
        [JsonPropertyName("entity_one_class_uuid")]
        [DatabaseColumnPropertyAttribute("entity_one_class_uuid", MySql.Data.MySqlClient.MySqlDbType.String)]
        public Guid EntityOneClassUuid { get; set; } = Guid.Empty;

        [Required(ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg)]
        [JsonPropertyName("entity_two_class_uuid")]
        [DatabaseColumnPropertyAttribute("entity_two_class_uuid", MySql.Data.MySqlClient.MySqlDbType.String)]
        public Guid EntityTwoClassUuid { get; set; } = Guid.Empty;

        [Required(ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg)]
        [JsonPropertyName("entity_two")]
        [DatabaseColumnPropertyAttribute("entity_two", MySql.Data.MySqlClient.MySqlDbType.String)]
        public string EntityTwo { get; set; } = null;

        [Required(ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg)]
        [JsonPropertyName("entity_two_key_col")]
        [DatabaseColumnPropertyAttribute("entity_two_key_col", MySql.Data.MySqlClient.MySqlDbType.String)]
        public string EntityTwoKeyCol { get; set; } = null;

        public override string ToString()
        {
            return EntityOne+Direction+EntityTwo;
        }
    }
}
