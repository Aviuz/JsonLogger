using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JsonLoggerTest
{
    [TestClass]
    public class MainTests
    {
        [TestMethod]
        public void InitializeFile()
        {
            string path = "C:/tmp/empty.txt";

            File.Delete(path);

            var logger = new JsonLogger.JsonLogger(path);
            logger.SaveChanges();

            string contentOfFile = File.ReadAllText(path);
            Assert.AreEqual("[\r\n]", contentOfFile);
        }

        [TestMethod]
        public void CreateNewLogWithSingleEntry()
        {
            string path = "C:/tmp/log.txt";

            File.Delete(path);

            var logger = new JsonLogger.JsonLogger(path);

            logger.LogWarning("Test warning");

            logger.SaveChanges();

            var contentOfFile = logger.LogJson;
            Assert.AreEqual(1, contentOfFile.Count);
        }

        [TestMethod]
        public void CreateNewLogWithMessageAndExceptionEntries()
        {
            string path = "C:/tmp/log.txt";

            File.Delete(path);

            var logger = new JsonLogger.JsonLogger(path);

            logger.LogException(new NullReferenceException());

            logger.LogWarning("Test warning");

            logger.SaveChanges();

            var contentOfFile = logger.LogJson;
            Assert.AreEqual(2, contentOfFile.Count);
        }

        [TestMethod]
        public void RewriteLogFile()
        {
            string path = "C:/tmp/log.txt";

            File.Delete(path);

            var logger = new JsonLogger.JsonLogger(path);

            logger.LogWarning("Test warning");

            logger.SaveChanges();

            logger.LogException(new NullReferenceException());

            logger.SaveChanges();

            var contentOfFile = logger.LogJson;
            Assert.AreEqual(2, contentOfFile.Count);
        }

        [TestMethod]
        public void AppendToEmptyArrayLogFile()
        {
            string path = "C:/tmp/log.txt";

            File.Delete(path);

            var logger = new JsonLogger.JsonLogger(path);

            logger.SaveChanges();

            logger.LogException(new NullReferenceException());

            logger.SaveChanges();

            var contentOfFile = logger.LogJson;
            Assert.AreEqual(1, contentOfFile.Count);
        }
    }
}
