using System.Collections.Generic;
using System.Linq;
using NSubstitute;
using NuGet;
using NUnit.Framework;
using System;
using Shouldly;

namespace NuSelfUpdate.Tests
{    
    public class CheckForUpdateTests
    {
        static IVersionLocator CreateVersionLocator(Version currentVersion)
        {
            var versionLocator = Substitute.For<IVersionLocator>();
            versionLocator.CurrentVersion.Returns(currentVersion);

            return versionLocator;
        }

        static IPackageRepositoryFactory CreatePackageRepositoryFactory(IEnumerable<IPackage> packages)
        {
            var packageRepositoryFactory = Substitute.For<IPackageRepositoryFactory>();

            packageRepositoryFactory
                .CreateRepository("repository")
                .GetPackages()
                .Returns(packages.AsQueryable());
            return packageRepositoryFactory;
        }

        static IEnumerable<IPackage> CreatePacakges(params Version[] versions)
        {
            var maxVersion = versions.Max();

            var packages = versions.Select(v =>
                                               {
                                                   var package = Substitute.For<IPackage>();
                                                   package.Id.Returns("package");
                                                   package.Version.Returns(v);
                                                   package.IsLatestVersion.Returns(v == maxVersion);
                                                   return package;
                                               });

            return packages.ToArray();
        }

        [TestFixture]
        public class WhenNoUpdatesAvailable
        {
            [Test]
            public void UpdateAvailableShouldBeFalse()
            {
                var installedVersion = new Version(1, 0, 0, 0);

                var packages = CreatePacakges(installedVersion);
                var packageRepositoryFactory = CreatePackageRepositoryFactory(packages);

                var updater = new AppUpdater("repository", "package", packageRepositoryFactory, CreateVersionLocator(installedVersion));

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

                var packages = CreatePacakges(installedVersion, newVersion);
                var packageRepositoryFactory = CreatePackageRepositoryFactory(packages);

                var updater = new AppUpdater("repository", "package", packageRepositoryFactory, CreateVersionLocator(installedVersion));

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

                var packages = CreatePacakges(installedVersion, newVersion, newestVersion);
                var packageRepositoryFactory = CreatePackageRepositoryFactory(packages);

                var updater = new AppUpdater("repository", "package", packageRepositoryFactory, CreateVersionLocator(installedVersion));

                var updateCheck = updater.CheckForUpdate();

                updateCheck.UpdateAvailable.ShouldBe(true);
            }
        }
    }
}