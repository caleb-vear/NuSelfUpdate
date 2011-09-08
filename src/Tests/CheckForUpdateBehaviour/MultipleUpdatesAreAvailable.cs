using System;
using System.Collections.Generic;
using System.Linq;
using NuGet;
using NuSelfUpdate.Tests.Helpers;
using Shouldly;
using Enumerable = System.Linq.Enumerable;

namespace NuSelfUpdate.Tests.CheckForUpdateBehaviour
{
    public class MultipleUpdatesAreAvailable : BddifyTest
    {
        Version _installedVersion;
        Version _newVersion;
        Version _newestVersion;
        IEnumerable<IPackage> _packages;
        AppUpdater _updater;
        IUpdateCheck _updateCheck;

        void GivenAnInstalledVersion()
        {
            _installedVersion = new Version(1, 0);
        }

        void AndGivenAPackageForMultipleNewerVersionHasBeenPublished()
        {
            _newVersion = new Version(1, 1);
            _newestVersion = new Version(1, 2);
            _packages = Enumerable.ToList<IPackage>(Packages.FromVersions(AppUpdaters.DefaultPackageId, _installedVersion, _newVersion, _newestVersion));
        }

        void AndGivenAnAppUpdater()
        {
            _updater = AppUpdaters.Build(_installedVersion, _packages);
        }

        void WhenCheckForUpdateIsCalled()
        {
            _updateCheck = _updater.CheckForUpdate();
        }

        void ThenUpdateAvailableWillBeTrue()
        {
            _updateCheck.UpdateAvailable.ShouldBe(true);
        }

        void AndTheUpdatePackageWillBeTheNewestVersion()
        {
            var newPackage = _packages.Single(p => p.Version == _newestVersion);
            _updateCheck.UpdatePackage.ShouldBe(newPackage);
        }
    }
}