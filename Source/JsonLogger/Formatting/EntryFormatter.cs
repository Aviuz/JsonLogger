using JsonLogger.Enums;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace JsonLogger.Formatting
{
    internal class EntryFormatter
    {
        private readonly DataFormatter dataFormatter;
        
        public EntryFormatter(DataFormatter dataFormatter)
        {
            this.dataFormatter = dataFormatter;
        }

        internal string FormatEntryText(string title, string description, LogLevel level, DataType dataType, DateTime dateTime)
        {
            var entry = new LogEntry()
            {
                Title = title,
                Data = description,
                Level = level.ToString(),
                DataType = dataType.ToString(),
                DateTime = dateTime,
            };
            return dataFormatter.Format(entry);
        }

        internal string FormatEntryException(string title, Exception exception, LogLevel level, DataType dataType, DateTime dateTime)
        {
            var entry = new LogEntry()
            {
                Title = title,
                Data = new { ExceptionType = exception.GetType().FullName, Exception = exception },
                Level = level.ToString(),
                DataType = dataType.ToString(),
                DateTime = dateTime,
            };
            return dataFormatter.Format(entry);
        }

        internal string FormatEntryObject(string title, object data, LogLevel level, DataType dataType, DateTime dateTime)
        {
            var entry = new LogEntry()
            {
                Title = title,
                Data = data,
                Level = level.ToString(),
                DataType = dataType.ToString(),
                DateTime = dateTime,
            };
            return dataFormatter.Format(entry);
        }
    }
}