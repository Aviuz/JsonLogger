using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace JsonLogger.Controllers
{
    public class FileController : IFileController
    {
        private string filePath;

        public FileController(string filePath)
        {
            this.filePath = filePath;
        }

        public async Task AppendLogEntry(string serializedLog)
        {
            using (var fileStream = new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
            using (var textWriter = new StreamWriter(fileStream, Encoding.UTF8))
            {
                await textWriter.WriteLineAsync(serializedLog);
            }
        }

        public IEnumerable<string> ReadLogEntries()
        {
            using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            using (var textReader = new StreamReader(fileStream, Encoding.UTF8))
            {
                while (!textReader.EndOfStream)
                    yield return textReader.ReadLine();
            }
        }

        public async Task TransferTo(string targetFilePath)
        {
            using (var sourceFileStream = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
            using (var destinationFileStream = new FileStream(targetFilePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
            {
                await sourceFileStream.CopyToAsync(destinationFileStream);

                sourceFileStream.Seek(0, SeekOrigin.Begin);
                sourceFileStream.SetLength(0);
                sourceFileStream.Flush();
            }
        }

        public long GetSize()
        {
            return new FileInfo(filePath).Length;
        }

        public string GetExtension()
        {
            return Path.GetExtension(filePath);
        }
    }
}