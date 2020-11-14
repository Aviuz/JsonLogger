using JsonLogger.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace JsonLogger.Sinks
{
    internal class BacktrackSink : ISink
    {
        private readonly ISink innerSink;
        private readonly LogLevel minimumLogLevel;
        private readonly LogLevel triggeringLogLevel;
        private readonly TimeSpan timeLimit;

        private readonly List<(DateTime, string, LogLevel)> cashedLogEntries = new List<(DateTime, string, LogLevel)>();

        public BacktrackSink(ISink innerSink, LogLevel minimumLogLevel, LogLevel triggeringLogLevel, TimeSpan timeLimit)
        {
            this.innerSink = innerSink;
            this.minimumLogLevel = minimumLogLevel;
            this.triggeringLogLevel = triggeringLogLevel;
            this.timeLimit = timeLimit;
        }

        public void Emit(string logEntry, LogLevel level)
        {
            ClearOldLogs();

            if (level >= triggeringLogLevel)
                CaptureLogEntry(logEntry, level);
            else if (level >= minimumLogLevel)
                cashedLogEntries.Add((DateTime.Now, logEntry, level));
        }

        private void CaptureLogEntry(string logEntry, LogLevel logLevel)
        {
            foreach (var (_, backtrackEntry, backtrackLevel) in cashedLogEntries)
                innerSink.Emit(backtrackEntry, backtrackLevel);

            innerSink.Emit(logEntry, logLevel);
        }

        private void ClearOldLogs()
        {
            for (int i = 0; i < cashedLogEntries.Count; i++)
            {
                var (dateTime, _, _) = cashedLogEntries[i];
                if (dateTime < DateTime.Now.Subtract(timeLimit))
                {
                    cashedLogEntries.RemoveAt(i--);
                }
                else
                {
                    return;
                }
            }
        }
    }
}
