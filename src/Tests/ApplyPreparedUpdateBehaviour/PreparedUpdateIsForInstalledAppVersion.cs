using System;
using NSubstitute;
using NuSelfUpdate.Tests.Helpers;
using Shouldly;

namespace NuSelfUpdate.Tests.ApplyPreparedUpdateBehaviour
{
    public class PreparedUpdateIsForInstalledAppVersion : BddifyTest
    {
        Version _installedVersion;
        IPreparedUpdate _preparedUpdate;
        Exception _exception;
        AppUpdater _appUpdater;

        void GivenAnInstaledVersion()
        {
            _installedVersion = new Version(1, 1);
        }

        void AndGivenAPreparedUpdateForTheInstalledVersion()
        {
            _preparedUpdate = Substitute.For<IPreparedUpdate>();
            _preparedUpdate.Version.Returns(_installedVersion);
        }

        void AndGivenAnAppUpdater()
        {
            _appUpdater = new AppUpdater(new TestUpdaterConfig(_installedVersion));
        }

        void WhenApplyingPreparedUpdate()
        {
            _exception = Run.CatchingException(() => _appUpdater.ApplyPreparedUpdate(_preparedUpdate));
        }

        void ThenABackwardUpdateExceptionWillBeThrown()
        {
            _exception.ShouldBeTypeOf<BackwardUpdateException>();
            var backwardUpdate = (BackwardUpdateException)_exception;

            backwardUpdate.InstalledVersion.ShouldBe(_installedVersion);
            backwardUpdate.TargetVersion.ShouldBe(_installedVersion);
        }
    }
}