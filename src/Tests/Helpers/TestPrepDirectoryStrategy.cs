using System;

namespace NuSelfUpdate.Tests.Helpers
{
    public class TestPrepDirectoryStrategy : IPrepDirectoryStrategy
    {
        public static readonly IPrepDirectoryStrategy Instance = new TestPrepDirectoryStrategy();

        private TestPrepDirectoryStrategy() {}

        public string GetFor(Version updateVersion)
        {
            return System.IO.Path.Combine("c:\\app-updates", updateVersion.ToString());
        }
    }
}