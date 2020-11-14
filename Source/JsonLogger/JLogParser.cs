using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

namespace JsonLogger
{
    public class JLogParser
    {
        public static IEnumerable<object> Parse(Stream stream, Encoding encoding)
        {
            using (var reader = new StreamReader(stream, encoding))
            {
                while (!reader.EndOfStream)
                {
                    string logEntry = reader.ReadLine();

                    yield return JsonSerializer.Deserialize<object>(logEntry);
                }
            }
        }
    }
}
