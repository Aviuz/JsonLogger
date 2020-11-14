using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace JsonLogger.EndTest
{
    public class Transferring
    {
        [Fact]
        public void ShouldTransgerLog()
        {
            using (var repository = new TemporaryFileSetUp())
            {
                var logger = new JsonLoggerBuilder(repository.LogFilePath)
                    .EnableFileSplitting(100, repository.SplittingFolder)
                    .Build();

                logger.Log("test");

                string contentOfFile = repository.ReadAllText().Trim();

                Assert.False(string.IsNullOrWhiteSpace(contentOfFile));

                logger.Log("testing long message");

                contentOfFile = repository.ReadAllText().Trim();
                Assert.True(string.IsNullOrWhiteSpace(contentOfFile));

                Assert.Single(Directory.EnumerateFiles(repository.SplittingFolder));
            }
        }
    }
}
