using System;
using System.Collections.Generic;
using System.Linq;
using NSubstitute;
using NuGet;

namespace NuSelfUpdate.Tests.Helpers
{
    public class TestUpdaterConfig : AppUpdaterConfig
    {
        public IEnumerable<IPackage> PublishedPackages {set { PackageRepositoryFactory = PackageRepositoryFactories.Create(value); }}

        public TestUpdaterConfig(Version installedVersion)
        {
            PackageSource = "repository";
            AppPackageId = "package";
            PackageRepositoryFactory = PackageRepositoryFactories.Create(Enumerable.Empty<IPackage>());
            AppVersionProvider = Substitute.For<IAppVersionProvider>();
            AppVersionProvider.CurrentVersion.Returns(installedVersion);
            UpdatePrepDirectoryStrategy = new TestPrepDirectoryStrategy();
            FileSystem = new MockFileSystem();
        }
    }

    //public static class AppUpdaters
    //{
    //    public const string DefaultRepositoryName = "repository";
    //    public const string DefaultPackageId = "package";

    //    public static AppUpdater Build(Version installedVersion, IEnumerable<IPackage> availablePackages)
    //    {
    //        return Build(installedVersion, availablePackages, new MockFileSystem());
    //    }

    //    public static AppUpdater Build(Version installedVersion, IEnumerable<IPackage> availablePackages, IExtendedFileSystem fileSystem)
    //    {
    //        var packageRepositoryFactory = PackageRepositoryFactories.Create(availablePackages);
    //        var versionLocator = AppVersionProviders.Create(installedVersion);

    //        return new AppUpdater(DefaultRepositoryName, DefaultPackageId, packageRepositoryFactory, versionLocator, TestPrepDirectoryStrategy.Instance, fileSystem);
    //    }
    //}
}