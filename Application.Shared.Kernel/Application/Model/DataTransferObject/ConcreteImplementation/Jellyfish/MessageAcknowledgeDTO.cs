using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

using Application.Shared.Kernel.Application.Model.DataTransferObject;

namespace Application.Shared.Kernel.Application.Model.DataTransferObject.ConcreteImplementation.Jellyfish
{
    public class MessageAcknowledgeDTO : DataTransferModelAbstract
    {
        [JsonPropertyName("message_uuid")]
        [Required]
        public Guid MessageUuid { get; set; } = Guid.Empty;

        [JsonPropertyName("chat_uuid")]
        [Required]
        public Guid ChatUuid { get; set; } = Guid.Empty;
    }
}
