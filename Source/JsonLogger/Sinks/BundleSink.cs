using JsonLogger.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace JsonLogger.Sinks
{
    internal class BundleSink : ISink
    {
        private List<ISink> innerSinks;

        public BundleSink(params ISink[] sinks)
        {
            innerSinks = new List<ISink>(sinks);
        }

        public void Emit(string logEntry, LogLevel level)
        {
            foreach (var sink in innerSinks)
                sink.Emit(logEntry, level);
        }
    }
}
