using System;
using NuSelfUpdate.Tests.Helpers;
using Shouldly;

namespace NuSelfUpdate.Tests.AppUpdaterBehaviour.LaunchInstalledUpdateScenarios
{
    public class InstalledUpdateIsNull
    {
        InstalledUpdate _installedUpdate;
        AppUpdater _appUpdater;
        Exception _exception;

        void GivenANullInstalledUpdate()
        {
            _installedUpdate = default(InstalledUpdate);
        }

        void AndGivenAnAppUpdater()
        {
            _appUpdater = new AppUpdater(new TestUpdaterConfig(new Version(1, 0)));
        }

        void WhenLaunchInstalledUpdateIsCalled()
        {
            _exception = Run.CatchingException(() => _appUpdater.LaunchInstalledUpdate(_installedUpdate));
        }

        void ThenAnArgumentNullExceptionWillBeThrown()
        {
            _exception.ShouldNotBe(null);
            _exception.ShouldBeTypeOf<ArgumentNullException>();
            var argEx = (ArgumentNullException) _exception;
            argEx.ParamName.ShouldBe("installedUpdate");
        }
    }
}