using System;
using System.Collections.Generic;
using System.Linq;
using NuGet;
using NuSelfUpdate.Tests.Helpers;
using Shouldly;
using Enumerable = System.Linq.Enumerable;

namespace NuSelfUpdate.Tests.CheckForUpdateBehaviour
{
    public class UpdateIsAvailable : BddifyTest
    {
        Version _installedVersion;
        Version _newVersion;
        IEnumerable<IPackage> _packages;
        AppUpdater _updater;
        IUpdateCheck _updateCheck;
        TestUpdaterConfig _config;

        void GivenAnInstalledVersion()
        {
            _installedVersion = new Version(1, 0);
            _config = new TestUpdaterConfig(_installedVersion);
        }

        void AndGivenAPackageForANewerVersionHasBeenPublished()
        {
            _newVersion = new Version(1, 1);
            _packages = Packages.FromVersions(_config.AppPackageId, _installedVersion, _newVersion).ToList();
            _config.PublishedPackages = _packages;
        }

        void AndGivenAnAppUpdater()
        {
            _updater = new AppUpdater(_config);
        }

        void WhenCheckForUpdateIsCalled()
        {
            _updateCheck = _updater.CheckForUpdate();
        }

        void ThenUpdateAvailableWillBeTrue()
        {
            _updateCheck.UpdateAvailable.ShouldBe(true);
        }

        void AndTheUpdatePackageWillBeTheNewVersion()
        {
            var newPackage = _packages.Single(p => p.Version == _newVersion);
            _updateCheck.UpdatePackage.ShouldBe(newPackage);
        }
    }
}