using JsonLogger.Formatting;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace JsonLogger.UnitTest
{
    public class Formatting
    {
        [Fact]
        public void FormatExceptionHasNoWhiteSpace()
        {
            var formatter = new DataFormatter();
            string serialized = formatter.Format(new NullReferenceException("this is null exception"));

            Assert.False(serialized.Contains('\n'));
        }

        [Fact]
        public void FormatNull()
        {
            var formatter = new DataFormatter();
            string serialized = formatter.Format(null);

            Assert.Equal("null", serialized);
        }

        [Fact]
        public void SkipNullsInEntries()
        {
            var formatter = new EntryFormatter(new DataFormatter());
            string serialized = formatter.FormatEntryText("title with no description", null, Enums.LogLevel.Info, Enums.DataType.Text, DateTime.Now);

            Assert.DoesNotContain("\"data\"", serialized);
            Assert.DoesNotContain("null", serialized);
        }
    }
}
