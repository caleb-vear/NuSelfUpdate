using System;
using System.Linq;
using NSubstitute;
using NUnit.Framework;
using NuSelfUpdate.Tests.Helpers;
using Shouldly;

namespace NuSelfUpdate.Tests
{
    public class PerpareUpdateTests
    {
        [TestFixture]
        public class WhenUpdateIsNull
        {
            [Test]            
            public void ANullReferenceExceptionWillBeThrown()
            {
                var installedVersion = new Version(1, 0, 0, 0);
                var newVersion = new Version(1, 1, 0, 0);

                var updater = AppUpdaters.Build(installedVersion, Packages.FromVersions(AppUpdaters.DefaultPackageId, installedVersion, newVersion));

                IUpdate update = null;
                Should.Throw<ArgumentNullException>(() => updater.PrepareUpdate(update));
            }
        }

        [TestFixture]
        public class WhenUpdatePackageIsForTheCurrentlyInstalledVersion
        {
            [Test]
            public void AnBackwardUpdateExceptionWillBeThrown()
            {
                var installedVersion = new Version(1, 0);
                var packages = Packages.FromVersions(AppUpdaters.DefaultPackageId, installedVersion);
                var updater = AppUpdaters.Build(installedVersion, packages);

                var update = Substitute.For<IUpdate>();
                update.Package.Returns(packages.First());

                Should.Throw<BackwardUpdateException>(() => updater.PrepareUpdate(update));
            }
        }

        [TestFixture]
        public class WhenUpdatePackageIsForTheAPreviousVersion
        {
            [Test]
            public void AnBackwardUpdateExceptionWillBeThrown()
            {
                var oldVersion = new Version(0, 1);
                var installedVersion = new Version(1, 0);
                var packages = Packages.FromVersions(AppUpdaters.DefaultPackageId, oldVersion, installedVersion);
                var updater = AppUpdaters.Build(installedVersion, packages);

                var update = Substitute.For<IUpdate>();
                var oldPackage = packages.First(p => p.Version == oldVersion);
                update.Package.Returns(oldPackage);

                Should.Throw<BackwardUpdateException>(() => updater.PrepareUpdate(update));
            }
        }
    }
}