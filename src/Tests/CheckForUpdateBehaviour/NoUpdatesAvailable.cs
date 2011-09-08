using System;
using System.Collections.Generic;
using System.Linq;
using NuGet;
using NuSelfUpdate.Tests.Helpers;
using Shouldly;

namespace NuSelfUpdate.Tests.CheckForUpdateBehaviour
{
    public class NoUpdatesAvailable : BddifyTest
    {
        Version _installedVersion;
        IEnumerable<IPackage> _packages;
        AppUpdater _updater;
        IUpdateCheck _updateCheck;

        void GivenAnInstalledVersion()
        {
            _installedVersion = new Version(1, 0, 0, 0);
        }

        void AndGivenNoNewerPackagesHaveBeenPublishedWithTheAppPackageId()
        {
            _packages = Packages.FromVersions(AppUpdaters.DefaultPackageId, _installedVersion);
        }

        void AndGivenPackagesWithOtherPackageIdsOfANewerVersionHaveBeenPublished()
        {
            _packages = _packages.Concat(Packages.FromVersions("Other.Package", new Version(1, 1, 0, 0)));
        }

        void AndGivenAnAppUpdater()
        {
            _updater = AppUpdaters.Build(_installedVersion, _packages);
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