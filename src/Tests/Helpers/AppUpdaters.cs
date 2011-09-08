using System;
using System.Collections.Generic;
using NuGet;

namespace NuSelfUpdate.Tests.Helpers
{
    public static class AppUpdaters
    {
        public const string DefaultRepositoryName = "repository";
        public const string DefaultPackageId = "package";

        public static AppUpdater Build(Version installedVersion, IEnumerable<IPackage> availablePackages)
        {
            var packageRepositoryFactory = PackageRepositoryFactories.Create(availablePackages);
            var versionLocator = VersionLocators.Create(installedVersion);

            return new AppUpdater(DefaultRepositoryName, DefaultPackageId, packageRepositoryFactory, versionLocator);
        }
    }
}