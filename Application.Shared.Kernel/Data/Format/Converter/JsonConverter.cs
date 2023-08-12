using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Application.Shared.Kernel.Data.Format.Converter
{
    public static class JsonConverter
    {
        public class JsonBoolConverter : JsonConverter<bool>
        {
            public JsonBoolConverter()
            {

            }
            public override bool Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                bool response = false;
                try
                {
                    response = reader.GetBoolean();

                }
                catch (Exception ex2)
                {

                    try
                    {
                        string tokenValue = reader.GetString();
                        if (tokenValue != null)
                        {
                            if (tokenValue.ToLower() == "true")
                            {
                                response = true;
                            }
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                }
                return response;
            }


            public override void Write(Utf8JsonWriter writer, bool value, JsonSerializerOptions options)
            {
                writer.WriteBooleanValue(value);
            }
        }
        public class JsonDateTimeToIsoConverter : JsonConverter<DateTime>
        {
            public JsonDateTimeToIsoConverter()
            {

            }
            public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                DateTime response = DateTime.MinValue;
                string val = reader.GetString();
                if (val != null)
                {
                    if (DateTime.TryParse(val, out response))
                    {

                    }
                }
                return response;
            }


            public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
            {
                writer.WriteStringValue(value.ToString("yyyy-MM-ddTHH:mm:ssZ"));
            }
        }
    }
}
