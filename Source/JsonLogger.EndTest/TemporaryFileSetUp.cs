using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace JsonLogger.EndTest
{
    public class TemporaryFileSetUp : IDisposable
    {
        public string LogFilePath { get; } = $"C:/temp/{Guid.NewGuid()}.txt";
        public string SplittingFolder { get; } = $"C:/temp/transferredLogs_{Guid.NewGuid()}";

        public TemporaryFileSetUp()
        {
            if (!Directory.Exists(Path.GetDirectoryName(LogFilePath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(LogFilePath));
            }
            Directory.CreateDirectory(SplittingFolder);
        }

        public string ReadAllText()
        {
            DateTime timeout = DateTime.Now.AddSeconds(10);
            while (DateTime.Now < timeout)
            {
                try
                {
                    return File.ReadAllText(LogFilePath);
                }
                catch (IOException) { }
            }

            throw new TimeoutException();
        }

        public void Dispose()
        {
            File.Delete(LogFilePath);
            Directory.Delete(SplittingFolder, true);
        }
    }
}
