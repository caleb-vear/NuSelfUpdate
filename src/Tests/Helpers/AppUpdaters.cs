using System;
using System.Collections.Generic;
using NSubstitute;
using NuGet;

namespace NuSelfUpdate.Tests.Helpers
{
    public static class AppUpdaters
    {
        public const string DefaultRepositoryName = "repository";
        public const string DefaultPackageId = "package";

        public static AppUpdater Build(Version installedVersion, IEnumerable<IPackage> availablePackages)
        {
            return Build(installedVersion, availablePackages, Substitute.For<IPackageFileSaver>());
        }

        public static AppUpdater Build(Version installedVersion, IEnumerable<IPackage> availablePackages, IPackageFileSaver packageFileSaver)
        {
            var packageRepositoryFactory = PackageRepositoryFactories.Create(availablePackages);
            var versionLocator = VersionLocators.Create(installedVersion);

            return new AppUpdater(DefaultRepositoryName, DefaultPackageId, packageRepositoryFactory, versionLocator, TestPrepDirectoryStrategy.Instance, packageFileSaver);
        }
    }
}