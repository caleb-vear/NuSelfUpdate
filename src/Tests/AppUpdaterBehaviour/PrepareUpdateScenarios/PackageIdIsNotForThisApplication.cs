using System;
using System.Linq;
using NuGet;
using NuSelfUpdate.Tests.Helpers;
using Shouldly;

namespace NuSelfUpdate.Tests.AppUpdaterBehaviour.PrepareUpdateScenarios
{
    public class PackageIdIsNotForThisApplication
    {
        Version _installedVersion;
        AppUpdater _appUpdater;
        IPackage _incorrectPackage;
        Exception _exception;
        TestUpdaterConfig _config;

        void GivenAnInstalledVersion()
        {
            _installedVersion = new Version(1, 0);
        }

        void AndGivenAnAppUpdater()
        {
            _config = new TestUpdaterConfig(_installedVersion);
            _appUpdater = new AppUpdater(_config);
        }

        void AndGivenAPackageWithANewerVersionNumberButAnIdOtherThanTheAppsId()
        {
            _incorrectPackage = Packages.FromVersions("Not.Target.Id", new Version(1, 1)).First();
        }

        void WhenTheUpdateIsPrepared()
        {
            _exception = Run.CatchingException(() => _appUpdater.PrepareUpdate(_incorrectPackage));
        }

        void ThenAnArgumentExceptionWillBeThrown()
        {
            _exception.ShouldBeTypeOf<ArgumentException>();
            ((ArgumentException)_exception).ParamName.ShouldBe("package");
        }
    }
}