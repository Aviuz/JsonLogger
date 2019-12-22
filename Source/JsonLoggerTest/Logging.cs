using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;

namespace JsonLoggerTest
{
    [TestClass]
    public class Logging
    {
        [TestMethod]
        public void InitializeFile()
        {
            FileMockUp.CleanFile(FileMockUp.TestFilePath);

            var logger = new JsonLogger.JLogger(FileMockUp.TestFilePath);
            logger.SaveChanges();

            string contentOfFile = File.ReadAllText(FileMockUp.TestFilePath);
            Assert.AreEqual("[\r\n]", contentOfFile);

            FileMockUp.CleanFile(FileMockUp.TestFilePath);
        }

        [TestMethod]
        public void CreateNewLogWithSingleEntry()
        {
            FileMockUp.CleanFile(FileMockUp.TestFilePath);

            var logger = new JsonLogger.JLogger(FileMockUp.TestFilePath);

            logger.Warning("Test warning");

            logger.SaveChanges();

            var contentOfFile = logger.LogJson;
            Assert.AreEqual(1, contentOfFile.Count);

            FileMockUp.CleanFile(FileMockUp.TestFilePath);
        }

        [TestMethod]
        public void CreateNewLogWithMessageAndExceptionEntries()
        {
            FileMockUp.CleanFile(FileMockUp.TestFilePath);

            var logger = new JsonLogger.JLogger(FileMockUp.TestFilePath);

            logger.Log(new NullReferenceException());

            logger.Warning("Test warning");

            logger.SaveChanges();

            var contentOfFile = logger.LogJson;
            Assert.AreEqual(2, contentOfFile.Count);

            FileMockUp.CleanFile(FileMockUp.TestFilePath);
        }

        [TestMethod]
        public void RewriteLogFile()
        {
            FileMockUp.CleanFile(FileMockUp.TestFilePath);

            var logger = new JsonLogger.JLogger(FileMockUp.TestFilePath);

            logger.Warning("Test warning");

            logger.SaveChanges();

            logger.Log(new NullReferenceException());

            logger.SaveChanges();

            var contentOfFile = logger.LogJson;
            Assert.AreEqual(2, contentOfFile.Count);

            FileMockUp.CleanFile(FileMockUp.TestFilePath);
        }

        [TestMethod]
        public void AppendToEmptyArrayLogFile()
        {
            FileMockUp.CleanFile(FileMockUp.TestFilePath);

            var logger = new JsonLogger.JLogger(FileMockUp.TestFilePath);

            logger.SaveChanges();

            logger.Log(new NullReferenceException());

            logger.SaveChanges();

            var contentOfFile = logger.LogJson;
            Assert.AreEqual(1, contentOfFile.Count);

            FileMockUp.CleanFile(FileMockUp.TestFilePath);
        }

        [TestMethod]
        public void SaveText()
        {
            FileMockUp.CleanFile(FileMockUp.TestFilePath);

            var logger = new JsonLogger.JLogger(FileMockUp.TestFilePath);

            logger.Log("title");

            logger.SaveChanges();

            var contentOfFile = logger.LogJson;
            Assert.AreEqual(JsonLogger.DataType.Text.ToString(), contentOfFile[0]["dataType"].ToString());

            FileMockUp.CleanFile(FileMockUp.TestFilePath);
        }

        [TestMethod]
        public void SaveLongText()
        {
            FileMockUp.CleanFile(FileMockUp.TestFilePath);

            var logger = new JsonLogger.JLogger(FileMockUp.TestFilePath);

            logger.Log("title", "very long text");

            logger.SaveChanges();

            var contentOfFile = logger.LogJson;
            Assert.AreEqual(JsonLogger.DataType.Text.ToString(), contentOfFile[0]["dataType"].ToString());
            Assert.AreEqual("very long text", contentOfFile[0]["data"].ToString());

            FileMockUp.CleanFile(FileMockUp.TestFilePath);
        }

        [TestMethod]
        public void SaveObject()
        {
            FileMockUp.CleanFile(FileMockUp.TestFilePath);

            var logger = new JsonLogger.JLogger(FileMockUp.TestFilePath);

            logger.Log(new object());

            logger.SaveChanges();

            var contentOfFile = logger.LogJson;
            Assert.AreEqual(JsonLogger.DataType.Object.ToString(), contentOfFile[0]["dataType"].ToString());

            FileMockUp.CleanFile(FileMockUp.TestFilePath);
        }

        [TestMethod]
        public void SaveException()
        {
            FileMockUp.CleanFile(FileMockUp.TestFilePath);

            var logger = new JsonLogger.JLogger(FileMockUp.TestFilePath);

            logger.Log(new FormatException());

            logger.SaveChanges();

            var contentOfFile = logger.LogJson;
            Assert.AreEqual(JsonLogger.DataType.Exception.ToString(), contentOfFile[0]["dataType"].ToString());

            FileMockUp.CleanFile(FileMockUp.TestFilePath);
        }

        [TestMethod]
        public void ReadingBeforeSaving()
        {
            FileMockUp.CleanFile(FileMockUp.TestFilePath);

            var logger = new JsonLogger.JLogger(FileMockUp.TestFilePath);

            logger.Log("before saving 1");
            logger.Log("before saving 2");

            logger.SaveChanges();

            logger.Log("after saving 1");
            logger.Log("after saving 2");

            Assert.AreEqual(logger.LogJson.Count, 4);
        }

        [TestMethod]
        public void LogArray()
        {
            FileMockUp.CleanFile(FileMockUp.TestFilePath);

            var logger = new JsonLogger.JLogger(FileMockUp.TestFilePath);

            logger.Log(new string[] { "aaaa", "bbbbb", "ccccc" });

            logger.SaveChanges();

            var contentOfFile = logger.LogJson;
            Assert.AreEqual(1, contentOfFile.Count);

            FileMockUp.CleanFile(FileMockUp.TestFilePath);
        }

        [TestMethod]
        public void LogList()
        {
            FileMockUp.CleanFile(FileMockUp.TestFilePath);

            var logger = new JsonLogger.JLogger(FileMockUp.TestFilePath);

            logger.Log(new List<string>() { "aaaa", "bbbbb", "ccccc" });

            logger.SaveChanges();

            var contentOfFile = logger.LogJson;
            Assert.AreEqual(1, contentOfFile.Count);

            FileMockUp.CleanFile(FileMockUp.TestFilePath);
        }

        [TestMethod]
        public void LogHiddenString()
        {
            FileMockUp.CleanFile(FileMockUp.TestFilePath);

            var logger = new JsonLogger.JLogger(FileMockUp.TestFilePath);

            object hiddenString = "hello world";

            logger.Log(hiddenString);

            logger.SaveChanges();

            var contentOfFile = logger.LogJson;
            Assert.AreEqual(1, contentOfFile.Count);

            FileMockUp.CleanFile(FileMockUp.TestFilePath);
        }

        [TestMethod]
        public void LogDouble()
        {
            FileMockUp.CleanFile(FileMockUp.TestFilePath);

            var logger = new JsonLogger.JLogger(FileMockUp.TestFilePath);

            double val = 0.3;

            logger.Log(val);

            logger.SaveChanges();

            var contentOfFile = logger.LogJson;
            Assert.AreEqual(1, contentOfFile.Count);

            FileMockUp.CleanFile(FileMockUp.TestFilePath);
        }

        [TestMethod]
        public void LogBool()
        {
            FileMockUp.CleanFile(FileMockUp.TestFilePath);

            var logger = new JsonLogger.JLogger(FileMockUp.TestFilePath);

            bool val = true;

            logger.Log(val);

            logger.SaveChanges();

            var contentOfFile = logger.LogJson;
            Assert.AreEqual(1, contentOfFile.Count);

            FileMockUp.CleanFile(FileMockUp.TestFilePath);
        }

        [TestMethod]
        public void LogRecursive()
        {
            FileMockUp.CleanFile(FileMockUp.TestFilePath);

            var logger = new JsonLogger.JLogger(FileMockUp.TestFilePath);

            var list = new List<object>();
            list.Add(list);

            logger.Log(list);

            logger.SaveChanges();

            var contentOfFile = logger.LogJson;
            Assert.AreEqual(2, contentOfFile.Count);

            FileMockUp.CleanFile(FileMockUp.TestFilePath);
        }
    }
}
