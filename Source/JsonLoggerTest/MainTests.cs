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
