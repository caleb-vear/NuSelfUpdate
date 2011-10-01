using System;
using NSubstitute;
using NuSelfUpdate.Tests.Helpers;
using Shouldly;

namespace NuSelfUpdate.Tests.AppUpdaterBehaviour.ApplyPreparedUpdateScenarios
{
    public class PreparedUpdateIsForInstalledAppVersion
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
            _appUpdater = new AppUpdaterBuilder(TestConstants.AppPackageId)
                .SetupWithTestValues(_installedVersion)
                .Build();
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