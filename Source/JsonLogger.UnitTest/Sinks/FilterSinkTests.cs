using JsonLogger.Enums;
using JsonLogger.Sinks;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace JsonLogger.UnitTest.Sinks
{
    public class FilterSinkTests
    {
        [Theory]
        [InlineData(LogLevel.Trace, LogLevel.Fatal, LogLevel.Info, true)]
        [InlineData(LogLevel.Info, LogLevel.Fatal, LogLevel.Info, true)]
        [InlineData(LogLevel.Warning, LogLevel.Warning, LogLevel.Warning, true)]
        [InlineData(LogLevel.Critical, LogLevel.Info, LogLevel.Critical, false)]
        [InlineData(LogLevel.Info, LogLevel.Fatal, LogLevel.Trace, false)]
        [InlineData(LogLevel.Info, LogLevel.Critical, LogLevel.Fatal, false)]
        public void TestFilterRanges(LogLevel minimum, LogLevel maximum, LogLevel logLevel, bool expectedCapture)
        {
            var logsRecorded = new List<(string, LogLevel)>();

            var innerSink = new Mock<ISink>();
            innerSink.Setup(s => s.Emit(It.IsAny<string>(), It.IsAny<LogLevel>()))
                .Callback((string str, LogLevel level) => logsRecorded.Add((str, level)));

            var filterSink = new FilterSink(innerSink.Object, minimum, maximum);

            filterSink.Emit("test log", logLevel);

            if (expectedCapture)
            {
                Assert.Single(logsRecorded, ("test log", logLevel));
            }
            else
            {
                Assert.Empty(logsRecorded);
            }
        }
    }
}
