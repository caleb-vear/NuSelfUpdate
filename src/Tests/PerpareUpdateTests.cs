using System;
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

                var packages = Packages.FromVersions("package", installedVersion, newVersion);
                var packageRepositoryFactory = PackageRepositoryFactories.Create(packages);

                var updater = new AppUpdater("repository", "package", packageRepositoryFactory, VersionLocators.Create(installedVersion));

                IUpdate update = null;
                Should.Throw<ArgumentNullException>(() => updater.PrepareUpdate(update));
            }
        }
    }
}