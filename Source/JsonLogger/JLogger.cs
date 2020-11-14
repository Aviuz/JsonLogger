using JsonLogger.Enums;
using JsonLogger.Formatting;
using JsonLogger.Sinks;
using System;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("JsonLogger.UnitTest")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]

namespace JsonLogger
{
    public class JLogger
    {
        private ISink sink;
        private readonly EntryFormatter entryFormatter;

        internal JLogger(ISink sink, EntryFormatter entryFormatter)
        {
            this.sink = sink;
            this.entryFormatter = entryFormatter;
        }

        /// <summary>
        /// Logs block of text with specified level
        /// </summary>
        public void LogText(string title, string description, LogLevel logLevel)
        {
            try
            {
                sink.Emit(
                    entryFormatter.FormatEntryText(title, description, logLevel, DataType.Text, DateTime.Now),
                    logLevel
                );
            }
            catch (Exception ex)
            {
                InternalLog(ex);
            }
        }

        public void LogException(string title, Exception exception, LogLevel logLevel)
        {
            try
            {
                if (title == null)
                    title = exception.GetType().Name;

                sink.Emit(
                    entryFormatter.FormatEntryException(title, exception, logLevel, DataType.Exception, DateTime.Now),
                    logLevel
                );
            }
            catch (Exception ex)
            {
                InternalLog(ex);
            }
        }

        public void LogObject(string title, object data, LogLevel logLevel)
        {
            try
            {
                if (title == null)
                    title = data.GetType().Name;

                sink.Emit(
                    entryFormatter.FormatEntryObject(title, data, logLevel, DataType.Object, DateTime.Now),
                    logLevel
                );
            }
            catch (Exception ex)
            {
                InternalLog(ex);
            }
        }

        private void InternalLog(Exception ex)
        {
            LogText("Failed to log entry", $"StackTrace: {ex.StackTrace}", LogLevel.Critical);
        }
    }

    public static class JLoggerShortcuts
    {
        /// <summary>
        /// Logs single-line text with specified level
        /// </summary>
        public static void Log(this JLogger logger, string text, LogLevel logLevel = LogLevel.Info)
            => logger.LogText(text, null, logLevel);

        /// <inheritdoc cref="JLogger.LogText(string, string, LogLevel)" />
        public static void Log(this JLogger logger, string title, string description, LogLevel logLevel = LogLevel.Info)
            => logger.LogText(title, description, logLevel);

        /// <summary>
        /// Logs exception with it's properties as JSON object with default 'Critical' level
        /// </summary>
        public static void Log(this JLogger logger, string title, Exception exception, LogLevel logLevel = LogLevel.Critical)
            => logger.LogException(title, exception, logLevel);

        /// <summary>
        /// Logs exception with it's properties as JSON object with default 'Critical' level
        /// </summary>
        public static void Log(this JLogger logger, Exception exception, LogLevel logLevel = LogLevel.Critical)
            => logger.LogException(null, exception, logLevel);

        /// <summary>
        /// Logs objects with it's public properties and fields as JSON object
        /// </summary>
        /// <param name="title">Title for entry</param>
        /// <param name="item">Object that will be logged</param>
        /// <param name="logLevel">Level for entry</param>
        public static void Log(this JLogger logger, string title, object item, LogLevel logLevel = LogLevel.Info)
            => logger.LogObject(title, item, logLevel);

        /// <summary>
        /// Logs objects with it's public properties and fields as JSON object
        /// </summary>
        /// <param name="item">Object that will be logged</param>
        /// <param name="logLevel">Category for entry</param>
        public static void Log(this JLogger logger, object item, LogLevel logLevel = LogLevel.Info)
            => logger.LogObject(null, item, logLevel);

        /// <summary>
        /// Logs single-line text with 'Warning' level
        /// </summary>
        public static void Warning(this JLogger logger, string text)
            => logger.LogText(text, null, LogLevel.Warning);

        /// <summary>
        /// Logs single-line text with 'Warning' level
        /// </summary>
        public static void Error(this JLogger logger, string text)
            => logger.LogText(text, null, LogLevel.Critical);

        /// <inheritdoc cref="JLogger.LogException(string, Exception, LogLevel)" />
        public static void Exception(this JLogger logger, string title, Exception exception, LogLevel logLevel = LogLevel.Info)
            => logger.LogException(title, exception, logLevel);

        /// <inheritdoc cref="JLogger.LogObject(string, object, LogLevel)" />
        public static void Object(this JLogger logger, string title, object data, LogLevel logLevel = LogLevel.Info)
            => logger.LogObject(title, data, logLevel);
    }
}