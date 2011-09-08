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

                var updater = AppUpdaters.Build(installedVersion, Packages.FromVersions(AppUpdaters.DefaultPackageId, installedVersion, newVersion));

                IUpdate update = null;
                Should.Throw<ArgumentNullException>(() => updater.PrepareUpdate(update));
            }
        }
    }
}