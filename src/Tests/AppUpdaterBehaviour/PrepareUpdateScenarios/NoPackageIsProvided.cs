using System;
using NuSelfUpdate.Tests.Helpers;
using Shouldly;

namespace NuSelfUpdate.Tests.AppUpdaterBehaviour.PrepareUpdateScenarios
{
    public class NoPackageIsProvided
    {
        AppUpdater _appUpdater;
        Exception _exception;

        void GivenAnAppUpdater()
        {
            _appUpdater = new AppUpdater(new TestUpdaterConfig(new Version(1, 0)));
        }

        void WhenPrepareUpdateIsCalledWithoutProvidingAPackage()
        {
            _exception = Run.CatchingException(() =>_appUpdater.PrepareUpdate(null));
        }

        void ThenAnArgumentNullExceptionWillBeThrown()
        {
            _exception.ShouldBeTypeOf<ArgumentNullException>();
            ((ArgumentNullException)_exception).ParamName.ShouldBe("package");
        }
    }
}