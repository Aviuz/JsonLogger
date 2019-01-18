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
        public void Log()
        {
            string path = "C:/tmp/log.txt";

            var logger = new JsonLogger.JsonLogger(path);

            logger.LogMessage("test");

            logger.SaveChanges();

            string contentOfFile = File.ReadAllText(path);
            Assert.AreEqual("[\r\n]", contentOfFile);
        }
    }
}
