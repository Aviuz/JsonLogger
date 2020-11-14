using JsonLogger.Enums;

namespace JsonLogger.Sinks
{
    internal interface ISink
    {
        void Emit(string logEntry, LogLevel level);
    }
}