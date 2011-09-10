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

        public override string AppDirectory
        {
            get { return @"c:\app\"; }
        }

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
}