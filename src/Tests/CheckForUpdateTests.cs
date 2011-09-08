using System.Collections.Generic;
using System.Linq;
using NuGet;
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

                var packages = Packages.FromVersions(AppUpdaters.DefaultPackageId, installedVersion)
                    .Concat(Packages.FromVersions("Other.Package", new Version(1,1,0,0)));

                var updater = AppUpdaters.Build(installedVersion, packages);

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

                var packages = Packages.FromVersions(AppUpdaters.DefaultPackageId, installedVersion, newVersion);
                var updater = AppUpdaters.Build(installedVersion, packages);

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

                var packages = Packages.FromVersions(AppUpdaters.DefaultPackageId, installedVersion, newVersion, newestVersion);
                var updater = AppUpdaters.Build(installedVersion, packages);

                var updateCheck = updater.CheckForUpdate();

                updateCheck.UpdateAvailable.ShouldBe(true);
            }
        }
    }
}