using System.Linq;
using NSubstitute;
using NuGet;
using NUnit.Framework;
using System;
using Shouldly;

namespace NuSelfUpdate.Tests
{    
    public class CheckForUpdate
    {
        [TestFixture]
        public class WhenNoUpdatesAvailable
        {
            [Test]
            public void UpdateAvailableShouldBeFalse()
            {
                var installedVersion = new Version(1, 0, 0, 0);

                var packages = new[] {Substitute.For<IPackage>()};
                packages[0].Version.Returns(installedVersion);
                packages[0].IsLatestVersion.Returns(true);
                packages[0].Id.Returns("package");

                var packageRepositoryFactory = Substitute.For<IPackageRepositoryFactory>();

                packageRepositoryFactory
                    .CreateRepository("repository")
                    .GetPackages()
                    .Returns(packages.AsQueryable());
                
                var versionLocator = Substitute.For<IVersionLocator>();
                versionLocator.CurrentVersion.Returns(installedVersion);

                var updater = new AppUpdater("repository", "package", packageRepositoryFactory, versionLocator);

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

                var packages = new[] { Substitute.For<IPackage>(), Substitute.For<IPackage>() };
                packages[0].Version.Returns(installedVersion);
                packages[0].IsLatestVersion.Returns(true);
                packages[0].Id.Returns("package");
                packages[1].Version.Returns(newVersion);
                packages[1].IsLatestVersion.Returns(false);
                packages[1].Id.Returns("package");

                var packageRepositoryFactory = Substitute.For<IPackageRepositoryFactory>();

                packageRepositoryFactory
                    .CreateRepository("repository")
                    .GetPackages()
                    .Returns(packages.AsQueryable());

                var versionLocator = Substitute.For<IVersionLocator>();
                versionLocator.CurrentVersion.Returns(installedVersion);

                var updater = new AppUpdater("repository", "package", packageRepositoryFactory, versionLocator);

                var updateCheck = updater.CheckForUpdate();

                updateCheck.UpdateAvailable.ShouldBe(true);
            }
        }
    }
}