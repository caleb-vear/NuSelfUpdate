using System;
using NSubstitute;

namespace NuSelfUpdate.Tests.Helpers
{
    public static class AppVersionProviders
    {
        public static IAppVersionProvider Create(Version currentVersion)
        {
            var versionLocator = Substitute.For<IAppVersionProvider>();
            versionLocator.CurrentVersion.Returns(currentVersion);

            return versionLocator;
        }
    }
}