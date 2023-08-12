using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Application.Shared.Kernel.Configuration.Model.Abstraction;

namespace Application.Shared.Kernel.Configuration.Model.ConcreteImplementation
{

    public class LogConfigurationModel : AbstractConfigurationModel
    {

        [JsonPropertyName("log_level")]
        public General.MESSAGE_LEVEL LogLevel { get; set; } = General.MESSAGE_LEVEL.LEVEL_INFO;
        [JsonPropertyName("log_date_format")]
        public string LogdateFormat { get; set; } = "yyyy-MM-dd";
        [JsonPropertyName("log_time_format")]
        public string LogtimeFormat { get; set; } = "HH:mm:ss";
        [JsonPropertyName("userinterface_date_format")]
        public string UserInterfaceDateFormat { get; set; } = "yyyy-MM-dd";
        [JsonPropertyName("userinterface_time_format")]
        public string UserInterfaceTimeFormat { get; set; } = "HH:mm:ss";
    }
}
