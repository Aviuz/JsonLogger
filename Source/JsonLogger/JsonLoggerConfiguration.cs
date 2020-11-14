using JsonLogger.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace JsonLogger
{
    public class JsonLoggerConfiguration
    {
        public JsonLoggerConfiguration(string filePath)
        {
            FilePath = filePath;
        }

        public string FilePath { get; set; }

        public bool FileSplitting { get; set; }
        public bool CompactMode { get; set; }

        public LogLevel MinimumLogLevel { get; set; } = LogLevel.Info;
        public LogLevel MaximumLogLevel { get; set; } = LogLevel.Fatal;

        public string SplittingDirectory { get; set; } = "Logs/";
        public long MaxFileSize { get; set; } = 1000 * 1000;

        public bool Backtrackng { get; set; }
        public TimeSpan BacktrackingTime { get; set; } = TimeSpan.FromMinutes(2);
        public LogLevel MinimumLevelForBacktracking { get; set; } = LogLevel.Trace;
        public LogLevel TriggeringLevelForBacktracking { get; set; } = LogLevel.Critical;
    }
}