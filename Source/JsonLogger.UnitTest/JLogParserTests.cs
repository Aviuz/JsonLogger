using JsonLogger.Enums;
using JsonLogger.Formatting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Xunit;

namespace JsonLogger.UnitTest
{
    public class JLogParserTests
    {
        [Fact]
        public void TestDeserializeFromMemmoryStream()
        {
            var entryFormatter = new EntryFormatter(new DataFormatter());

            List<object> logs;

            using (var stream = new MemoryStream())
            {
                using (var streamWriter = new StreamWriter(stream, Encoding.UTF8, leaveOpen: true))
                    streamWriter.Write(entryFormatter.FormatEntryText("log title", "log description", LogLevel.Warning, DataType.Text, DateTime.Now));

                stream.Seek(0, SeekOrigin.Begin);

                logs = JLogParser.Parse(stream, Encoding.UTF8).ToList();
            }

            Assert.Single(logs);
        }
    }
}
