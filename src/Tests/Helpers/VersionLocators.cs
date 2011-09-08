using System;
using NSubstitute;

namespace NuSelfUpdate.Tests.Helpers
{
    public static class VersionLocators
    {
        public static IVersionLocator Create(Version currentVersion)
        {
            var versionLocator = Substitute.For<IVersionLocator>();
            versionLocator.CurrentVersion.Returns(currentVersion);

            return versionLocator;
        }
    }
}