using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace JsonLoggerTest
{
    [TestClass]
    public class MainTests
    {
        public const string TestFilePath = "C:/temp/JsonLoggerTestingFile.txt";

        [TestMethod]
        public void InitializeFile()
        {
            CleanFile(TestFilePath);

            var logger = new JsonLogger.JLogger(TestFilePath);
            logger.SaveChanges();

            string contentOfFile = File.ReadAllText(TestFilePath);
            Assert.AreEqual("[\r\n]", contentOfFile);

            CleanFile(TestFilePath);
        }

        [TestMethod]
        public void CreateNewLogWithSingleEntry()
        {
            CleanFile(TestFilePath);

            var logger = new JsonLogger.JLogger(TestFilePath);

            logger.Warning("Test warning");

            logger.SaveChanges();

            var contentOfFile = logger.LogJson;
            Assert.AreEqual(1, contentOfFile.Count);

            CleanFile(TestFilePath);
        }

        [TestMethod]
        public void CreateNewLogWithMessageAndExceptionEntries()
        {
            CleanFile(TestFilePath);

            var logger = new JsonLogger.JLogger(TestFilePath);

            logger.Log(new NullReferenceException());

            logger.Warning("Test warning");

            logger.SaveChanges();

            var contentOfFile = logger.LogJson;
            Assert.AreEqual(2, contentOfFile.Count);

            CleanFile(TestFilePath);
        }

        [TestMethod]
        public void RewriteLogFile()
        {
            CleanFile(TestFilePath);

            var logger = new JsonLogger.JLogger(TestFilePath);

            logger.Warning("Test warning");

            logger.SaveChanges();

            logger.Log(new NullReferenceException());

            logger.SaveChanges();

            var contentOfFile = logger.LogJson;
            Assert.AreEqual(2, contentOfFile.Count);

            CleanFile(TestFilePath);
        }

        [TestMethod]
        public void AppendToEmptyArrayLogFile()
        {
            CleanFile(TestFilePath);

            var logger = new JsonLogger.JLogger(TestFilePath);

            logger.SaveChanges();

            logger.Log(new NullReferenceException());

            logger.SaveChanges();

            var contentOfFile = logger.LogJson;
            Assert.AreEqual(1, contentOfFile.Count);

            CleanFile(TestFilePath);
        }

        [TestMethod]
        public void SaveText()
        {
            CleanFile(TestFilePath);

            var logger = new JsonLogger.JLogger(TestFilePath);

            logger.Log("title");

            logger.SaveChanges();

            var contentOfFile = logger.LogJson;
            Assert.AreEqual(JsonLogger.DataType.Text.ToString(), contentOfFile[0]["dataType"].ToString());

            CleanFile(TestFilePath);
        }

        [TestMethod]
        public void SaveLongText()
        {
            CleanFile(TestFilePath);

            var logger = new JsonLogger.JLogger(TestFilePath);

            logger.Log("title", "very long text");

            logger.SaveChanges();

            var contentOfFile = logger.LogJson;
            Assert.AreEqual(JsonLogger.DataType.Text.ToString(), contentOfFile[0]["dataType"].ToString());
            Assert.AreEqual("very long text", contentOfFile[0]["data"].ToString());

            CleanFile(TestFilePath);
        }

        [TestMethod]
        public void SaveObject()
        {
            CleanFile(TestFilePath);

            var logger = new JsonLogger.JLogger(TestFilePath);

            logger.Log(new object());

            logger.SaveChanges();

            var contentOfFile = logger.LogJson;
            Assert.AreEqual(JsonLogger.DataType.Object.ToString(), contentOfFile[0]["dataType"].ToString());

            CleanFile(TestFilePath);
        }

        [TestMethod]
        public void SaveException()
        {
            CleanFile(TestFilePath);

            var logger = new JsonLogger.JLogger(TestFilePath);

            logger.Log(new FormatException());

            logger.SaveChanges();

            var contentOfFile = logger.LogJson;
            Assert.AreEqual(JsonLogger.DataType.Exception.ToString(), contentOfFile[0]["dataType"].ToString());

            CleanFile(TestFilePath);
        }

        [TestMethod]
        public void ReadingBeforeSaving()
        {
            CleanFile(TestFilePath);

            var logger = new JsonLogger.JLogger(TestFilePath);

            logger.Log("before saving 1");
            logger.Log("before saving 2");

            logger.SaveChanges();

            logger.Log("after saving 1");
            logger.Log("after saving 2");

            Assert.AreEqual(logger.LogJson.Count, 4);
        }

        private static void CleanFile(string path)
        {
            if (!Directory.Exists(Path.GetDirectoryName(path)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path));
            }
            else if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
    }
}
