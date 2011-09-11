using System;
using System.Collections.Generic;
using System.Linq;
using NuGet;
using NuSelfUpdate.Tests.Helpers;
using Shouldly;

namespace NuSelfUpdate.Tests.CheckForUpdateBehaviour
{
    public class NoPackagesHaveEverBeenPublished : BddifyTest
    {
        Version _installedVersion;
        IEnumerable<IPackage> _packages;
        AppUpdater _updater;
        IUpdateCheck _updateCheck;
        TestUpdaterConfig _config;

        void GivenAnInstalledVersion()
        {
            _installedVersion = new Version(1, 0, 0, 0);
            _config = new TestUpdaterConfig(_installedVersion);

        }

        void AndGivenNoNewerPackagesHaveBeenPublishedWithTheAppPackageId()
        {
            _packages = Packages.FromVersions(_config.AppPackageId, _installedVersion);
        }

        void AndGivenNoPackagesHaveBeenPublished()
        {
            _packages = Enumerable.Empty<IPackage>();
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

        void ThenTheUpdateChecksUpdateAvailableWillBeFalse()
        {
            _updateCheck.UpdateAvailable.ShouldBe(false);
        }

        void AndTheUpdateCheckPackageWillBeNull()
        {
            _updateCheck.UpdatePackage.ShouldBe(null);
        }        
    }
}