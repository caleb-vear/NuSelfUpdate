using System;
using System.Linq;
using NuGet;
using NuSelfUpdate.Tests.Helpers;
using Shouldly;

namespace NuSelfUpdate.Tests.PrepareUpdateBehaviour
{
    public class NoPackageIsProvided : BddifyTest
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
            _exception.As<ArgumentNullException>().ParamName.ShouldBe("package");
        }
    }
}