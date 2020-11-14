using JsonLogger.Enums;
using JsonLogger.Sinks;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace JsonLogger.UnitTest.Sinks
{
    public class BundleSinkTests
    {
        [Fact]
        public void EmptyBundleDontThrow()
        {
            var sink = new BundleSink();

            sink.Emit("test", LogLevel.Info);
        }

        [Fact]
        public void ThreeSinksGetLogs()
        {
            var innerLogs1 = new List<string>();
            var innerSink1 = new Mock<ISink>();
            innerSink1.Setup(s => s.Emit(It.IsAny<string>(), It.IsAny<LogLevel>()))
                .Callback((string str, LogLevel level) => innerLogs1.Add(str));

            var innerLogs2 = new List<string>();
            var innerSink2 = new Mock<ISink>();
            innerSink2.Setup(s => s.Emit(It.IsAny<string>(), It.IsAny<LogLevel>()))
                .Callback((string str, LogLevel level) => innerLogs2.Add(str));

            var innerLogs3 = new List<string>();
            var innerSink3 = new Mock<ISink>();
            innerSink3.Setup(s => s.Emit(It.IsAny<string>(), It.IsAny<LogLevel>()))
                .Callback((string str, LogLevel level) => innerLogs3.Add(str));

            var sink = new BundleSink(innerSink1.Object, innerSink2.Object, innerSink3.Object);

            sink.Emit("test", LogLevel.Info);

            Assert.Single(innerLogs1);
            Assert.Equal("test", innerLogs1[0]);
            Assert.Single(innerLogs2);
            Assert.Equal("test", innerLogs2[0]);
            Assert.Single(innerLogs3);
            Assert.Equal("test", innerLogs3[0]);
        }
    }
}
