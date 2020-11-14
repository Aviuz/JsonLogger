using JsonLogger.Enums;
using System;
using System.Text.Json.Serialization;

namespace JsonLogger.Formatting
{
    public class LogEntry
    {
        [JsonPropertyName("title")]
        public string Title { get; set; }
        [JsonPropertyName("data")]
        public object Data { get; set; }
        [JsonPropertyName("level")]
        public string Level { get; set; }
        [JsonPropertyName("type")]
        public string DataType { get; set; }
        [JsonPropertyName("time")]
        public DateTime DateTime { get; set; }
    }
}