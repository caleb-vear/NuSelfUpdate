using System;
using NSubstitute;
using NuSelfUpdate.Tests.Helpers;
using Shouldly;

namespace NuSelfUpdate.Tests.ApplyPreparedUpdateBehaviour
{
    public class PreparedUpdateIsForAnOlderAppVersion : BddifyTest
    {
        Version _installedVersion;
        IPreparedUpdate _preparedUpdate;
        Exception _exception;
        AppUpdater _appUpdater;

        void GivenAnInstaledVersion()
        {
            _installedVersion = new Version(1, 1);
        }

        void AndGivenAPreparedUpdateForAnOlderVersion()
        {
            _preparedUpdate = Substitute.For<IPreparedUpdate>();
            _preparedUpdate.Version.Returns(new Version(1, 0));
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
            var backwardUpdate = _exception as BackwardUpdateException;

            backwardUpdate.InstalledVersion.ShouldBe(_installedVersion);
            backwardUpdate.TargetVersion.ShouldBe(_preparedUpdate.Version);
        }
    }
}