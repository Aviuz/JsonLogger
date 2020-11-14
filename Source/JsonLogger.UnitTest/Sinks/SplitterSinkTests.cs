using JsonLogger.Controllers;
using JsonLogger.Enums;
using JsonLogger.Sinks;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace JsonLogger.UnitTest.Sinks
{
    public class SplitterSinkTests
    {
        [Fact]
        public void TetstIfCallsCopyAdterSizeChange()
        {
            var logsRecorded = new List<string>();
            string copiedTo = null;

            var fileController = new Mock<IFileController>();

            fileController.Setup(f => f.AppendLogEntry(It.IsAny<string>()))
                .Callback((string str) => logsRecorded.Add(str));

            fileController.Setup(f => f.TransferTo(It.IsAny<string>()))
                .Callback((string filePath) => copiedTo = filePath);

            fileController.Setup(f => f.GetExtension()).Returns(".log");
            fileController.Setup(f => f.GetSize()).Returns(1);

            var splitterSink = new SplitterSink(fileController.Object, "log-test/", 3);

            splitterSink.Emit("test1", LogLevel.Trace);

            Assert.Null(copiedTo);

            fileController.Setup(f => f.GetSize()).Returns(3);

            splitterSink.Emit("test2", LogLevel.Trace);

            Assert.Matches(@"^log-test/\d\d\.\d\d\.\d\d\d\d \d\d\.\d\d\.log$", copiedTo);
        }
    }
}
