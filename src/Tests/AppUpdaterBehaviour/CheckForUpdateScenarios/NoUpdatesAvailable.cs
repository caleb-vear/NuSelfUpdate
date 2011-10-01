using System;
using System.Collections.Generic;
using System.Linq;
using NuGet;
using NuSelfUpdate.Tests.Helpers;
using Shouldly;

namespace NuSelfUpdate.Tests.AppUpdaterBehaviour.CheckForUpdateScenarios
{
    public class NoUpdatesAvailable
    {
        Version _installedVersion;
        IEnumerable<IPackage> _packages;
        AppUpdater _updater;
        IUpdateCheck _updateCheck;
        AppUpdaterBuilder _builder;

        void GivenAnInstalledVersion()
        {
            _installedVersion = new Version(1, 0, 0, 0);
        }

        void AndGivenNoNewerPackagesHaveBeenPublishedWithTheAppPackageId()
        {
            _packages = Packages.FromVersions(TestConstants.AppPackageId, _installedVersion);
        }

        void AndGivenPackagesWithOtherPackageIdsOfANewerVersionHaveBeenPublished()
        {
            _packages = _packages.Concat(Packages.FromVersions("Other.Package", new Version(1, 1, 0, 0)));

            _builder = new AppUpdaterBuilder(TestConstants.AppPackageId)
                .SetupWithTestValues(_installedVersion)
                .SetPublishedPackages(_packages);
        }

        void AndGivenAnAppUpdater()
        {
            _updater = _builder.Build();
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