using JsonLogger.Enums;
using JsonLogger.Formatting;
using JsonLogger.Sinks;
using System;
using System.Runtime.CompilerServices;

namespace JsonLogger
{
    partial class JLogger
    {
        /// <summary>
        /// Logs single-line text with specified level
        /// </summary>
        public void Log(string text, LogLevel logLevel = LogLevel.Info)
            => LogText(text, null, logLevel);

        /// <inheritdoc cref="JLogger.LogText(string, string, LogLevel)" />
        public void Log(string title, string description, LogLevel logLevel = LogLevel.Info)
            => LogText(title, description, logLevel);

        /// <summary>
        /// Logs exception with it's properties as JSON object with default 'Critical' level
        /// </summary>
        public void Log(string title, Exception exception, LogLevel logLevel = LogLevel.Critical)
            => LogException(title, exception, logLevel);

        /// <summary>
        /// Logs exception with it's properties as JSON object with default 'Critical' level
        /// </summary>
        public void Log(Exception exception, LogLevel logLevel = LogLevel.Critical)
            => LogException(null, exception, logLevel);

        /// <summary>
        /// Logs objects with it's public properties and fields as JSON object
        /// </summary>
        /// <param name="title">Title for entry</param>
        /// <param name="item">Object that will be logged</param>
        /// <param name="logLevel">Level for entry</param>
        public void Log(string title, object item, LogLevel logLevel = LogLevel.Info)
            => LogObject(title, item, logLevel);

        /// <summary>
        /// Logs objects with it's public properties and fields as JSON object
        /// </summary>
        /// <param name="item">Object that will be logged</param>
        /// <param name="logLevel">Category for entry</param>
        public void Log(object item, LogLevel logLevel = LogLevel.Info)
            => LogObject(null, item, logLevel);

        /// <summary>
        /// Logs single-line text with 'Warning' level
        /// </summary>
        public void Warning(string text)
            => LogText(text, null, LogLevel.Warning);

        /// <summary>
        /// Logs single-line text with 'Warning' level
        /// </summary>
        public void Error(string text)
            => LogText(text, null, LogLevel.Critical);

        /// <inheritdoc cref="JLogger.LogException(string, Exception, LogLevel)" />
        public void Exception(string title, Exception exception, LogLevel logLevel = LogLevel.Info)
            => LogException(title, exception, logLevel);

        /// <inheritdoc cref="JLogger.LogObject(string, object, LogLevel)" />
        public void Object(string title, object data, LogLevel logLevel = LogLevel.Info)
            => LogObject(title, data, logLevel);
    }
}
