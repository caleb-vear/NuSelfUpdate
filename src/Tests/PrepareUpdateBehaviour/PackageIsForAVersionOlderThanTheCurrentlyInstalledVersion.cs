﻿using System;
using System.Linq;
using NuGet;
using NuSelfUpdate.Tests.Helpers;
using Shouldly;

namespace NuSelfUpdate.Tests.PrepareUpdateBehaviour
{
    public class PackageIsForAVersionOlderThanTheCurrentlyInstalledVersion : BddifyTest
    {
        Version _installedVersion;
        AppUpdater _appUpdater;
        IPackage _oldVersionPackage;
        Exception _exception;

        void GivenAnInstalledVersion()
        {
            _installedVersion = new Version(1, 0);
        }

        void AndGivenAnAppUpdater()
        {
            _appUpdater = AppUpdaters.Build(_installedVersion, Enumerable.Empty<IPackage>());
        }

        void AndGivenAPackageForAnOlderVersion()
        {
            _oldVersionPackage = Packages.FromVersions(AppUpdaters.DefaultPackageId, new Version(0,1)).Single();
        }

        void WhenPreparePackageIsCalled()
        {
            _exception = Run.CatchingException(() => _appUpdater.PrepareUpdate(_oldVersionPackage));
        }

        void ThenABackwardUpdateExceptionWillBeThrown()
        {
            _exception.ShouldBeTypeOf<BackwardUpdateException>();
            var backwardUpdate = _exception.As<BackwardUpdateException>();
            backwardUpdate.InstalledVersion.ShouldBe(_installedVersion);
            backwardUpdate.TargetVersion.ShouldBe(_oldVersionPackage.Version);
        }
    }
}