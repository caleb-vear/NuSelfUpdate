using System.Linq;
using NUnit.Framework;
using System;
using NuSelfUpdate.Tests.Helpers;
using Shouldly;

namespace NuSelfUpdate.Tests
{    
    public class CheckForUpdateTests
    {
        [TestFixture]
        public class WhenNoUpdatesAvailable
        {
            [Test]
            public void UpdateAvailableShouldBeFalse()
            {
                var installedVersion = new Version(1, 0, 0, 0);
                var packageId = "package";

                var packages = Packages.FromVersions(packageId, installedVersion).Concat(Packages.FromVersions("Other.Package", new Version(1,1,0,0)));
                var packageRepositoryFactory = PackageRepositoryFactories.Create(packages);

                var updater = new AppUpdater("repository", packageId, packageRepositoryFactory, VersionLocators.Create(installedVersion));

                var updateCheck = updater.CheckForUpdate();

                updateCheck.UpdateAvailable.ShouldBe(false);
            }
        }

        [TestFixture]
        public class WhenAnUpdateIsAvailable
        {
            [Test]
            public void UpdateAvailableShouldBeTrue()
            {
                var installedVersion = new Version(1, 0, 0, 0);
                var newVersion = new Version(1, 1, 0, 0);

                var packages = Packages.FromVersions("package", installedVersion, newVersion);
                var packageRepositoryFactory = PackageRepositoryFactories.Create(packages);

                var updater = new AppUpdater("repository", "package", packageRepositoryFactory, VersionLocators.Create(installedVersion));

                var updateCheck = updater.CheckForUpdate();

                updateCheck.UpdateAvailable.ShouldBe(true);
            }
        }

        [TestFixture]
        public class WhenMultipleUpdatesAreaAvailable
        {
            [Test]
            public void UpdateAvailableShouldBeTrue()
            {
                var installedVersion = new Version(1, 0, 0, 0);
                var newVersion = new Version(1, 1, 0, 0);
                var newestVersion = new Version(1, 2, 0, 0);

                var packages = Packages.FromVersions("package", installedVersion, newVersion, newestVersion);
                var packageRepositoryFactory = PackageRepositoryFactories.Create(packages);

                var updater = new AppUpdater("repository", "package", packageRepositoryFactory, VersionLocators.Create(installedVersion));

                var updateCheck = updater.CheckForUpdate();

                updateCheck.UpdateAvailable.ShouldBe(true);
            }
        }
    }
}