using JsonLogger.Controllers;
using JsonLogger.Sinks;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace JsonLogger.UnitTest.Sinks
{
    public class FileSinkTests
    {
        [Fact]
        public void FileControllerGetsCorrectString()
        {
            bool fileControllerGotLog = false;

            var fileController = new Mock<IFileController>();
            fileController.Setup(f => f.AppendLogEntry("this is correct log"))
                .Callback(() => fileControllerGotLog = true);

            var sink = new FileSink(fileController.Object);

            sink.Emit("this is not correct log", Enums.LogLevel.Critical);

            Assert.False(fileControllerGotLog);

            sink.Emit("this is correct log", Enums.LogLevel.Trace);

            Assert.True(fileControllerGotLog);
        }
    }
}
