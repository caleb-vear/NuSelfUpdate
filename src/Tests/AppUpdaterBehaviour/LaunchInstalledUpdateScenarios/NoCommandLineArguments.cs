using System;
using NSubstitute;
using NuSelfUpdate.Tests.Helpers;
using Shouldly;

namespace NuSelfUpdate.Tests.AppUpdaterBehaviour.LaunchInstalledUpdateScenarios
{
    public class NoCommandLineArguments
    {
        Version _installedVersion;
        AppUpdater _appUpdater;
        InstalledUpdate _installedUpdate;
        InstalledUpdate _returnedInstalledUpdate;
        AppUpdaterBuilder _builder;
        ICommandLineWrapper _commandLineWrapper;
        IProcessWrapper _processWrapper;

        void GivenTheAppWasRunWithNoArguments()
        {
            _installedVersion = new Version(1, 0);
            _builder = new AppUpdaterBuilder(TestConstants.AppPackageId)
                .SetupWithTestValues(_installedVersion);

            _commandLineWrapper = _builder.GetSubsituteCommandLineWrapper();
            _processWrapper = _builder.GetSubsituteProcessWrapper();

            _commandLineWrapper.Full.Returns(@"\.myapp.exe");
            _commandLineWrapper.Arguments.Returns(new[] { @"\.myapp.exe" });
        }

        void AndGivenAnAppUpdater()
        {
            _appUpdater = _builder.Build();
        }

        void AndGivenAnInstalledUpdate()
        {
            _installedUpdate = new InstalledUpdate(_installedVersion, new Version(2, 0));
        }

        void WhenLaunchInstalledUpdateIsCalled()
        {
            _returnedInstalledUpdate = _appUpdater.LaunchInstalledUpdate(_installedUpdate);
        }

        void ThenTheApplicationWillBeLaunchedWithNoArguments()
        {
            _processWrapper.Received().Start(@"\.myapp.exe", string.Empty);
        }

        void AndTheInstalledUpdateReturnedWillBeTheSameAsTheOnePassedIn()
        {
            _returnedInstalledUpdate.ShouldBe(_installedUpdate);
        }
    }
}