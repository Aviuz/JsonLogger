using JsonLogger.Enums;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xunit;

namespace JsonLogger.EndTest
{
    public class Logging
    {
        [Fact]
        public void CreateNewLogWithSingleEntry()
        {
            using (var repo = new TemporaryFileSetUp())
            {
                var logger = new JsonLoggerBuilder(repo.LogFilePath)
                    .Build();

                logger.Warning("Test warning");

                string contentOfFile = repo.ReadAllText()
                    .Trim();
                AssertLogEntry("Test warning", null, LogLevel.Warning, DataType.Text, contentOfFile);
            }
        }

        [Fact]
        public void CreateNewLogWithMessageAndExceptionEntries()
        {
            using (var repo = new TemporaryFileSetUp())
            {
                var logger = new JsonLoggerBuilder(repo.LogFilePath)
                    .Build();

                logger.Log(new NullReferenceException());

                logger.Warning("Test warning");


                var contentOfFile = repo.ReadAllText()
                     .Split('\n')
                     .Select(str => str.Trim())
                     .ToArray();

                Assert.Contains("NullReferenceException", contentOfFile[0]);
                AssertLogEntry("Test warning", null, LogLevel.Warning, DataType.Text, contentOfFile[1]);
            }
        }

        [Theory]
        [InlineData("./test/relativeFile1.log", "test/relativeFile1.log")]
        [InlineData("./relativeFile2.log", "relativeFile2.log")]
        [InlineData("relativeFile3.log", "relativeFile3.log")]
        public void RelativePath(string testedRelativePath, string relativePathForCheck)
        {
            string concreteDirectory = "C:/temp";
            string destinationFilePath = Path.Combine(concreteDirectory, relativePathForCheck);

            File.Delete(destinationFilePath);

            string currentDirectory = Environment.CurrentDirectory;
            Environment.CurrentDirectory = concreteDirectory;

            try
            {
                var logger = new JsonLoggerBuilder(testedRelativePath)
                    .Build();
                logger.Error("tubaluga");
            }
            catch (Exception) { }
            finally
            {
                Environment.CurrentDirectory = currentDirectory;
            }

            Assert.True(File.Exists(destinationFilePath));

            string contentOfFile = File.ReadAllText(destinationFilePath).Trim();
            AssertLogEntry("tubaluga", null, LogLevel.Critical, DataType.Text, contentOfFile);

            File.Delete(destinationFilePath);
        }

        [Fact]
        public void LogObject()
        {
            using (var repo = new TemporaryFileSetUp())
            {
                var logger = new JsonLoggerBuilder(repo.LogFilePath)
                    .Build();

                logger.Log("test", new { p1 = 2, p2 = "text" });

                string contentOfFile = repo.ReadAllText().Trim();
                AssertLogEntry("test", "{\"p1\":2,\"p2\":\"text\"}", LogLevel.Info, DataType.Object, contentOfFile);
            }
        }

        [Fact]
        public void LogException()
        {
            using (var repo = new TemporaryFileSetUp())
            {
                var logger = new JsonLoggerBuilder(repo.LogFilePath)
                    .Build();

                logger.Log("exception", new NullReferenceException("in exception"));

                string contentOfFile = repo.ReadAllText().Trim();
                AssertLogEntry("exception", "\\{\"exceptionType\":\"System\\.NullReferenceException\",\"exception\":\\{\"message\":\"in exception\",\"data\":\\{\\},\"hResult\":-2147467261\\}\\}", LogLevel.Critical, DataType.Exception, contentOfFile);
            }
        }

        [Fact]
        public void LogRecursive()
        {
            using (var repo = new TemporaryFileSetUp())
            {
                var logger = new JsonLoggerBuilder(repo.LogFilePath)
                    .Build();

                var list = new List<object>();
                list.Add(list);

                logger.Log(list);

                string contentOfFile = repo.ReadAllText().Trim();
                Assert.Contains("List", contentOfFile);
            }
        }

        [Fact]
        public void LogArray()
        {
            using (var repo = new TemporaryFileSetUp())
            {
                var logger = new JsonLoggerBuilder(repo.LogFilePath)
                    .Build();

                logger.Log(new string[] { "aaaa", "bbbbb", "ccccc" });

                string contentOfFile = repo.ReadAllText().Trim();
                AssertLogEntry("String[]", @"\[""aaaa"",""bbbbb"",""ccccc""\]", LogLevel.Info, DataType.Object, contentOfFile);
            }
        }

        [Fact]
        public void LogList()
        {
            using (var repo = new TemporaryFileSetUp())
            {
                var logger = new JsonLoggerBuilder(repo.LogFilePath)
                    .Build();

                logger.Log("list", new List<string>() { "aaaa", "bbbbb", "ccccc" });

                string contentOfFile = repo.ReadAllText().Trim();
                AssertLogEntry("list", @"\[""aaaa"",""bbbbb"",""ccccc""\]", LogLevel.Info, DataType.Object, contentOfFile);
            }
        }

        [Fact]
        public void LogHiddenString()
        {
            using (var repo = new TemporaryFileSetUp())
            {
                var logger = new JsonLoggerBuilder(repo.LogFilePath)
                    .Build();

                object hiddenString = "hello world";

                logger.Log(hiddenString);

                string contentOfFile = repo.ReadAllText().Trim();
                AssertLogEntry("String", "\"hello world\"", LogLevel.Info, DataType.Object, contentOfFile);
            }
        }

        [Fact]
        public void LogDouble()
        {
            using (var repo = new TemporaryFileSetUp())
            {
                var logger = new JsonLoggerBuilder(repo.LogFilePath)
                    .Build();

                double val = 0.3;

                logger.Log(val);

                string contentOfFile = repo.ReadAllText().Trim();
                AssertLogEntry("Double", "0.3", LogLevel.Info, DataType.Object, contentOfFile);
            }
        }

        [Fact]
        public void LogBool()
        {
            using (var repo = new TemporaryFileSetUp())
            {
                var logger = new JsonLoggerBuilder(repo.LogFilePath)
                    .Build();

                bool val = true;

                logger.Log(val);

                string contentOfFile = repo.ReadAllText().Trim();
                AssertLogEntry("Boolean", "true", LogLevel.Info, DataType.Object, contentOfFile);
            }
        }

        private void AssertLogEntry(string title, string data, LogLevel level, DataType dataType, string validatedEntry)
        {
            string titleStr = title != null ? $"\"title\":\"{title}\"" : null;
            string dataStr = data != null ? $"\"data\":{data}" : null;
            string levelStr = $"\"level\":\"{level.ToString()}\"";
            string typeStr = $"\"type\":\"{dataType.ToString()}\"";
            string timeStr = @"""time"":""[0-9\-T\:\.\+]+""";

            string[] strings = { titleStr, dataStr, levelStr, typeStr, timeStr };

            Assert.Matches(@$"^\{{{string.Join(",", strings.Where(s => s != null).ToArray())}\}}$", validatedEntry);
        }
    }
}