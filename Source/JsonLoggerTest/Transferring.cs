using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace JsonLoggerTest
{
    [TestClass]
    public class Transferring
    {
        [TestMethod]
        public void TransferLogFile()
        {
            FileMockUp.CleanFile(FileMockUp.TestFilePath);
            FileMockUp.CleanFile(FileMockUp.TestFilePathSecondary);

            var logger = new JsonLogger.JLogger(FileMockUp.TestFilePath);

            logger.Warning("Test warning");

            logger.SaveChanges();

            logger.Log(new NullReferenceException());

            logger.SaveChanges();

            logger.TranferLogToFile(FileMockUp.TestFilePathSecondary, true);

            var sourceLogContent = logger.LogJson;
            Assert.AreEqual(0, sourceLogContent.Count);

            var tranferredContent = JArray.Parse(File.ReadAllText(FileMockUp.TestFilePathSecondary));
            Assert.AreEqual(2, tranferredContent.Count);

            FileMockUp.CleanFile(FileMockUp.TestFilePath);
            FileMockUp.CleanFile(FileMockUp.TestFilePathSecondary);
        }

        [TestMethod]
        public void AutomaticTransferring()
        {
            FileMockUp.CleanFile(FileMockUp.TestFilePath);
            FileMockUp.CleanFile(FileMockUp.TestFilePathSecondary);

            var logger = new JsonLogger.JLogger(FileMockUp.TestFilePath);
            logger.TriggerAutomaticTransferSize = 500;
            logger.CustomAutomaticTransferFileName += Logger_CustomAutomaticTransferFileName;

            logger.Warning("Test warning");

            logger.SaveChanges();

            logger.Log(new NullReferenceException());

            logger.SaveChanges();

            var sourceLogContent = logger.LogJson;
            Assert.AreEqual(0, sourceLogContent.Count);

            var tranferredContent = JArray.Parse(File.ReadAllText(FileMockUp.TestFilePathSecondary));
            Assert.AreEqual(2, tranferredContent.Count);

            FileMockUp.CleanFile(FileMockUp.TestFilePath);
            FileMockUp.CleanFile(FileMockUp.TestFilePathSecondary);
        }
        public static void Logger_CustomAutomaticTransferFileName(object sender, JsonLogger.EventArguments.AutomaticTransferingEvent e)
        {
            e.FilePath = FileMockUp.TestFilePathSecondary;
        }
    }
}
