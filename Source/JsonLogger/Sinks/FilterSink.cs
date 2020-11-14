using JsonLogger.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace JsonLogger.Sinks
{
    internal class FilterSink : ISink
    {
        private readonly ISink innerSink;
        private readonly LogLevel minimumLevel;
        private readonly LogLevel maximumLevel;

        public FilterSink(ISink innerSink, LogLevel minimumLevel, LogLevel maximumLevel)
        {
            this.innerSink = innerSink;
            this.minimumLevel = minimumLevel;
            this.maximumLevel = maximumLevel;
        }

        public void Emit(string logEntry, LogLevel level)
        {
            if (level >= minimumLevel && level <= maximumLevel)
                innerSink.Emit(logEntry, level);
        }
    }
}
