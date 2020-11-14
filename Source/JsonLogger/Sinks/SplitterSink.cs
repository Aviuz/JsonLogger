using JsonLogger.Controllers;
using JsonLogger.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace JsonLogger.Sinks
{
    internal class SplitterSink : ISink
    {
        private readonly IFileController fileControllerWatched;
        private readonly string targetDirectory;
        private readonly long splittingFileSize;

        public SplitterSink(IFileController fileControllerWatched, string targetDirectory, long splittingFileSize)
        {
            this.fileControllerWatched = fileControllerWatched;
            this.targetDirectory = targetDirectory;
            this.splittingFileSize = splittingFileSize;
        }

        public void Emit(string logEntry, LogLevel level)
        {
            if (fileControllerWatched.GetSize() >= splittingFileSize)
                _ = Split();
        }

        private async Task Split()
        {
            string destinationFilePath = Path.Combine(
                targetDirectory,
                $"{DateTime.Now:dd.MM.yyyy HH.mm}{fileControllerWatched.GetExtension()}"
            );

            var task = fileControllerWatched.TransferTo(destinationFilePath);
            await task;
        }
    }
}
