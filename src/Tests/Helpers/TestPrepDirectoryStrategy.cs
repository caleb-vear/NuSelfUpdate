using System;

namespace NuSelfUpdate.Tests.Helpers
{
    public class TestPrepDirectoryStrategy : IPrepDirectoryStrategy
    {
        public string GetFor(Version updateVersion)
        {
            return System.IO.Path.Combine(@"c:\app\.updates\", updateVersion.ToString());
        }
    }
}