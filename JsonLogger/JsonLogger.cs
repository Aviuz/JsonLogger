using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Text;

namespace JsonLogger
{
    public class JsonLogger : IDisposable
    {
        private static readonly byte[] ArrayTemplate;
        private static readonly byte[] EndOfArrayTemplate;

        private StringBuilder uncommitedChanges;

        public string LogFilePath { get; set; }

        static JsonLogger()
        {
            ArrayTemplate = Encoding.UTF8.GetBytes("[\r\n]");
            EndOfArrayTemplate = Encoding.UTF8.GetBytes("]");
        }

        public JsonLogger(string filePath)
        {
            LogFilePath = filePath;

            uncommitedChanges = new StringBuilder();
        }

        public void LogMessage(string message)
        {
            var entry = new JObject();
            entry["title"] = message;
            entry["category"] = "Message";
            entry["dataType"] = "Text";
            entry["date"] = DateTime.Now.ToShortDateString();
            entry["time"] = DateTime.Now.TimeOfDay.ToString();
            uncommitedChanges.Append(entry.ToString() + ",\r\n");
        }

        public void LogWarning(string message)
        {
            var entry = new JObject();
            entry["title"] = message;
            entry["category"] = "Message";
            entry["dataType"] = "Text";
            entry["date"] = DateTime.Now.ToShortDateString();
            entry["time"] = DateTime.Now.TimeOfDay.ToString();
            uncommitedChanges.Append(entry.ToString() + ",\r\n");
        }

        public JArray GetFullLog()
        {
            return ReverseOrderOfArray(JArray.Parse(File.ReadAllText(LogFilePath)));
        }

        private static JArray ReverseOrderOfArray(JArray array)
        {
            var output = new JArray();
            for (int i = array.Count - 1; i >= 0; i--)
            {
                output.Add(array[i]);
            }
            return output;
        }

        public void SaveChanges()
        {
            InitializeFile(LogFilePath);

            if (uncommitedChanges.Length == 0)
                return;

            byte[] buffer = Encoding.UTF8.GetBytes(uncommitedChanges.ToString());

            using (var stream = new FileStream(LogFilePath, FileMode.Open, FileAccess.ReadWrite))
            {
                stream.Seek(-1, SeekOrigin.End);
                stream.Write(buffer, 0, buffer.Length);
                stream.Write(EndOfArrayTemplate, 0, EndOfArrayTemplate.Length);
                stream.Close();
            }
        }

        private static void InitializeFile(string path)
        {
            bool isJsonArray;

            if (!File.Exists(path))
            {
                using (var stream = File.Create(path))
                {
                    stream.Close();
                }
                isJsonArray = false;
            }
            else
            {
                using (var stream = File.OpenText(path))
                {
                    int firstSymbol = stream.Read();
                    if ((char)firstSymbol == '[')
                    {
                        isJsonArray = true;
                    }
                    else
                    {
                        isJsonArray = false;
                    }
                    stream.Close();
                }
            }

            if (!isJsonArray)
            {
                using (var stream = File.OpenWrite(path))
                {
                    stream.Write(ArrayTemplate, 0, ArrayTemplate.Length);
                    stream.Close();
                }
            }
        }

        public void Dispose()
        {
            SaveChanges();
        }
    }
}
