using JsonLogger.Controllers;
using JsonLogger.Enums;
using JsonLogger.Formatting;
using JsonLogger.Sinks;
using System;
using System.IO;

namespace JsonLogger
{
    public class JsonLoggerBuilder
    {
        public JsonLoggerConfiguration Configuration { get; }

        public JsonLoggerBuilder(string filePath)
        {
            Configuration = new JsonLoggerConfiguration(filePath);
        }

        public JsonLoggerBuilder(JsonLoggerConfiguration configuration)
        {
            Configuration = configuration;
        }

        public JsonLoggerBuilder EnableFileSplitting(int maxFileSize, string directory)
        {
            Configuration.FileSplitting = true;
            Configuration.MaxFileSize = maxFileSize;
            Configuration.SplittingDirectory = directory;

            return this;
        }

        public JsonLoggerBuilder EnableCompactMode()
        {
            Configuration.CompactMode = true;

            return this;
        }

        public JsonLoggerBuilder EnableBacktracking(LogLevel minimumLevel = LogLevel.Trace, LogLevel triggeringLevel = LogLevel.Critical, TimeSpan backtrackingTime = default)
        {
            if (backtrackingTime != default)
                Configuration.BacktrackingTime = backtrackingTime;

            Configuration.MinimumLevelForBacktracking = minimumLevel;
            Configuration.TriggeringLevelForBacktracking = triggeringLevel;

            Configuration.Backtrackng = true;

            return this;
        }

        public JLogger Build()
        {
            EnsureDirectory(Path.GetDirectoryName(Configuration.FilePath));

            var fileController = new FileController(Configuration.FilePath);
            ISink sink = new FileSink(fileController);

            if (Configuration.FileSplitting)
            {
                sink = new BundleSink(
                     sink,
                     new SplitterSink(fileController, Configuration.SplittingDirectory, Configuration.MaxFileSize)
                 );
                EnsureDirectory(Configuration.SplittingDirectory);
            }

            if (Configuration.MinimumLogLevel > LogLevel.Trace || Configuration.MaximumLogLevel < LogLevel.Fatal)
                sink = new FilterSink(sink, Configuration.MinimumLogLevel, Configuration.MaximumLogLevel);

            if (Configuration.Backtrackng)
            {
                sink = new BundleSink(
                    sink,
                    new BacktrackSink(new FileSink(fileController), Configuration.MinimumLevelForBacktracking, Configuration.TriggeringLevelForBacktracking, Configuration.BacktrackingTime)
                );
            }

            return new JLogger(sink, new EntryFormatter(new DataFormatter()));
        }

        private void EnsureDirectory(string directory)
        {
            if (!string.IsNullOrWhiteSpace(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }
    }
}