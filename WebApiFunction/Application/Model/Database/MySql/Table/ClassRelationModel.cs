using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using MySql.Data.MySqlClient;
using WebApiFunction.Application.Model.Database.MySql;
using WebApiFunction.Application.Model.Database.MySql.Entity;
using WebApiFunction.Cache.Distributed.RedisCache;
using WebApiFunction.Ampq.Rabbitmq.Data;
using WebApiFunction.Ampq.Rabbitmq;
using WebApiFunction.Antivirus;
using WebApiFunction.Antivirus.nClam;
using WebApiFunction.Application.Model.DataTransferObject.Frontend.Transfer;
using WebApiFunction.Application.Model.DataTransferObject;
using WebApiFunction.Application.Model;
using WebApiFunction.Configuration;
using WebApiFunction.Controller;
using WebApiFunction.Data;
using WebApiFunction.Data.Web;
using WebApiFunction.Data.Format.Json;
using WebApiFunction.Data.Web.Api.Abstractions.JsonApiV1;
using WebApiFunction.Database;
using WebApiFunction.Filter;
using WebApiFunction.Formatter;
using WebApiFunction.Healthcheck;
using WebApiFunction.LocalSystem.IO.File;
using WebApiFunction.Log;
using WebApiFunction.Metric;
using WebApiFunction.Metric.Influxdb;
using WebApiFunction.MicroService;
using WebApiFunction.Network;
using WebApiFunction.Security;
using WebApiFunction.Security.Encryption;
using WebApiFunction.Threading;
using WebApiFunction.Threading.Service;
using WebApiFunction.Threading.Task;
using WebApiFunction.Utility;
using WebApiFunction.Web;
using WebApiFunction.Web.AspNet;
using WebApiFunction.Web.Authentification;
using WebApiFunction.Web.Http.Api.Abstractions.JsonApiV1;

namespace WebApiFunction.Application.Model.Database.MySql.Entity
{
    public class ClassRelationModel : AbstractModel
    {

        public Type EntityOneNetType;
        public Type EntityTwoNetType;

        public string RelationName { get => "rel_" + EntityOne + "_" + EntityOneKeyCol + "<--->" + EntityTwo + "_" + EntityTwoKeyCol; }

        [Required(ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg)]
        [JsonPropertyName("direction")]
        [DatabaseColumnPropertyAttribute("direction", MySqlDbType.String)]
        public string Direction { get; set; } = null;

        [Required(ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg)]
        [JsonPropertyName("entity_one")]
        [DatabaseColumnPropertyAttribute("entity_one", MySqlDbType.String)]
        public string EntityOne { get; set; } = null;

        [Required(ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg)]
        [JsonPropertyName("entity_one_key_col")]
        [DatabaseColumnPropertyAttribute("entity_one_key_col", MySqlDbType.String)]
        public string EntityOneKeyCol { get; set; } = null;

        [Required(ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg)]
        [JsonPropertyName("entity_one_relationship_type_uuid")]
        [DatabaseColumnPropertyAttribute("entity_one_relationship_type_uuid", MySqlDbType.String)]
        public Guid EntityOneRelationshipTypeUuid { get; set; } = Guid.Empty;

        [Required(ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg)]
        [JsonPropertyName("entity_two_relationship_type_uuid")]
        [DatabaseColumnPropertyAttribute("entity_two_relationship_type_uuid", MySqlDbType.String)]
        public Guid EntityTwoRelationshipTypeUuid { get; set; } = Guid.Empty;

        [Required(ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg)]
        [JsonPropertyName("entity_one_class_uuid")]
        [DatabaseColumnPropertyAttribute("entity_one_class_uuid", MySqlDbType.String)]
        public Guid EntityOneClassUuid { get; set; } = Guid.Empty;

        [Required(ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg)]
        [JsonPropertyName("entity_two_class_uuid")]
        [DatabaseColumnPropertyAttribute("entity_two_class_uuid", MySqlDbType.String)]
        public Guid EntityTwoClassUuid { get; set; } = Guid.Empty;

        [Required(ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg)]
        [JsonPropertyName("entity_two")]
        [DatabaseColumnPropertyAttribute("entity_two", MySqlDbType.String)]
        public string EntityTwo { get; set; } = null;

        [Required(ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg)]
        [JsonPropertyName("entity_two_key_col")]
        [DatabaseColumnPropertyAttribute("entity_two_key_col", MySqlDbType.String)]
        public string EntityTwoKeyCol { get; set; } = null;

        public override string ToString()
        {
            return EntityOne + Direction + EntityTwo;
        }
    }
}
