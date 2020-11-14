using JsonLogger.Controllers;
using JsonLogger.Enums;

namespace JsonLogger.Sinks
{
    internal class FileSink : ISink
    {
        private readonly IFileController fileController;

        public FileSink(IFileController fileController)
        {
            this.fileController = fileController;
        }

        public void Emit(string logEntry, LogLevel level)
        {
            _ = fileController.AppendLogEntry(logEntry);
        }
    }
}