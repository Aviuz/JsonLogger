using JsonLogger.Enums;
using JsonLogger.Formatting;
using JsonLogger.Sinks;
using System;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("JsonLogger.UnitTest")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]

namespace JsonLogger
{
    public partial class JLogger
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
}