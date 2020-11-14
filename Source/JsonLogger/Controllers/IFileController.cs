using System.Collections.Generic;
using System.Threading.Tasks;

namespace JsonLogger.Controllers
{
    public interface IFileController
    {
        Task AppendLogEntry(string serializedLog);
        Task TransferTo(string targetFilePath);
        string GetExtension();
        long GetSize();
        IEnumerable<string> ReadLogEntries();
    }
}