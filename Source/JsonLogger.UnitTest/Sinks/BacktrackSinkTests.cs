using JsonLogger.Enums;
using JsonLogger.Sinks;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace JsonLogger.UnitTest.Sinks
{
    public class BacktrackSinkTests
    {
        [Fact]
        public void TriggerBacktrack()
        {
            var logsRecorded = new List<(string, LogLevel)>();

            var innerSink = new Mock<ISink>();
            innerSink.Setup(s => s.Emit(It.IsAny<string>(), It.IsAny<LogLevel>()))
                .Callback((string str, LogLevel level) => logsRecorded.Add((str, level)));

            var backTrackSink = new BacktrackSink(innerSink.Object, LogLevel.Trace, LogLevel.Critical, TimeSpan.FromSeconds(30));

            backTrackSink.Emit("entry 1", LogLevel.Trace);
            backTrackSink.Emit("entry 2", LogLevel.Trace);

            Assert.Empty(logsRecorded);

            backTrackSink.Emit("entry 3", LogLevel.Critical);

            Assert.Equal(3, logsRecorded.Count);
            Assert.Equal("entry 1", logsRecorded[0].Item1);
            Assert.Equal("entry 2", logsRecorded[1].Item1);
            Assert.Equal("entry 3", logsRecorded[2].Item1);
        }

        [Fact]
        public void FilterLogsFromBottom()
        {
            var logsRecorded = new List<(string, LogLevel)>();

            var innerSink = new Mock<ISink>();
            innerSink.Setup(s => s.Emit(It.IsAny<string>(), It.IsAny<LogLevel>()))
                .Callback((string str, LogLevel level) => logsRecorded.Add((str, level)));

            var backTrackSink = new BacktrackSink(innerSink.Object, LogLevel.Info, LogLevel.Critical, TimeSpan.FromSeconds(30));

            backTrackSink.Emit("entry 1", LogLevel.Trace);
            backTrackSink.Emit("entry 2", LogLevel.Info);

            Assert.Empty(logsRecorded);

            backTrackSink.Emit("entry 3", LogLevel.Critical);

            Assert.Equal(2, logsRecorded.Count);
            Assert.Equal("entry 2", logsRecorded[0].Item1);
            Assert.Equal("entry 3", logsRecorded[1].Item1);
        }

        [Fact]
        public void TestIfChangingTriggeringLevelWorks()
        {
            var logsRecorded = new List<(string, LogLevel)>();

            var innerSink = new Mock<ISink>();
            innerSink.Setup(s => s.Emit(It.IsAny<string>(), It.IsAny<LogLevel>()))
                .Callback((string str, LogLevel level) => logsRecorded.Add((str, level)));

            var backTrackSink = new BacktrackSink(innerSink.Object, LogLevel.Info, LogLevel.Fatal, TimeSpan.FromSeconds(30));

            backTrackSink.Emit("entry 1", LogLevel.Trace);
            backTrackSink.Emit("entry 2", LogLevel.Info);

            Assert.Empty(logsRecorded);

            backTrackSink.Emit("entry 3", LogLevel.Critical);

            Assert.Empty(logsRecorded);
        }

        [Fact]
        public async Task DeleteOldUnrecordedLogs()
        {
            var logsRecorded = new List<(string, LogLevel)>();

            var innerSink = new Mock<ISink>();
            innerSink.Setup(s => s.Emit(It.IsAny<string>(), It.IsAny<LogLevel>()))
                .Callback((string str, LogLevel level) => logsRecorded.Add((str, level)));

            var backTrackSink = new BacktrackSink(innerSink.Object, LogLevel.Trace, LogLevel.Critical, TimeSpan.FromMilliseconds(50));

            backTrackSink.Emit("entry 1", LogLevel.Trace);
            await Task.Delay(100);
            backTrackSink.Emit("entry 2", LogLevel.Trace);

            Assert.Empty(logsRecorded);

            backTrackSink.Emit("entry 3", LogLevel.Critical);

            Assert.Equal(2, logsRecorded.Count);
            Assert.Equal("entry 2", logsRecorded[0].Item1);
            Assert.Equal("entry 3", logsRecorded[1].Item1);
        }

        [Fact]
        public void ThrowsNull()
        {
            var logsRecorded = new List<(string, LogLevel)>();

            var backTrackSink = new BacktrackSink(null, LogLevel.Trace, LogLevel.Critical, TimeSpan.FromMilliseconds(50));

            Assert.Throws<NullReferenceException>(() => backTrackSink.Emit("entry 1", LogLevel.Critical));
        }

        [Fact]
        public void InvertedLevelsDontThrow()
        {
            var logsRecorded = new List<(string, LogLevel)>();

            var innerSink = new Mock<ISink>();
            innerSink.Setup(s => s.Emit(It.IsAny<string>(), It.IsAny<LogLevel>()))
                .Callback((string str, LogLevel level) => logsRecorded.Add((str, level)));

            var backTrackSink = new BacktrackSink(innerSink.Object, LogLevel.Critical, LogLevel.Info, TimeSpan.FromSeconds(30));

            backTrackSink.Emit("entry 1", LogLevel.Trace);
            backTrackSink.Emit("entry 2", LogLevel.Trace);

            Assert.Empty(logsRecorded);

            backTrackSink.Emit("entry 3", LogLevel.Critical);

            Assert.Single(logsRecorded);
            Assert.Equal("entry 3", logsRecorded[0].Item1);
        }
    }
}
