using JsonLogger.Enums;
using JsonLogger.Formatting;
using JsonLogger.Sinks;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace JsonLogger.UnitTest
{
    public class ErrorHandling
    {
        [Fact]
        public void InternalLogging()
        {
            bool insideCallback = false;
            string internalMessage = null;
            LogLevel internalLevel = LogLevel.Trace;

            var sink = new Mock<ISink>();

            sink.Setup(s => s.Emit(It.IsAny<string>(), Enums.LogLevel.Info))
                .Throws<NullReferenceException>();
            sink.Setup(s => s.Emit(It.IsAny<string>(), LogLevel.Critical))
                .Callback((string msg, LogLevel level) =>
                {
                    insideCallback = true;
                    internalMessage = msg;
                    internalLevel = level;
                });

            var logger = new JLogger(sink.Object, new EntryFormatter(new DataFormatter()));

            logger.Log("test");

            Assert.True(insideCallback);
            Assert.Equal(LogLevel.Critical, internalLevel);
            Assert.Contains("StackTrace", internalMessage);
            Assert.Contains("Failed to log entry", internalMessage);
        }
    }
}
