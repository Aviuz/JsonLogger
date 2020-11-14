using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace JsonLogger.Formatting
{
    public class DataFormatter
    {
        private JsonSerializerOptions defaultOptions;
        private JsonSerializerOptions preserveOptions;

        public DataFormatter()
        {
            defaultOptions = new JsonSerializerOptions()
            {
                WriteIndented = false,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                MaxDepth = 15,
            };
            preserveOptions = new JsonSerializerOptions()
            {
                WriteIndented = false,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                ReferenceHandler = ReferenceHandler.Preserve,
            };
        }

        public string Format(object data) 
        {
            try
            {
                return JsonSerializer.Serialize(data, defaultOptions);
            }
            catch (JsonException)
            {
                return JsonSerializer.Serialize(data, preserveOptions);
            }
        }
    }
}