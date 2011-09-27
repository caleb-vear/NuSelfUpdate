using System;
using NSubstitute;
using NuSelfUpdate.Tests.Helpers;

namespace NuSelfUpdate.Tests.AppUpdaterBehaviour.LaunchInstalledUpdateScenarios
{
    public class NoCommandLineArguments
    {
        Version _installedVersion;
        AppUpdater _appUpdater;
        InstalledUpdate _installedUpdate;
        TestUpdaterConfig _config;

        void GivenTheAppWasRunWithNoArguments()
        {
            _installedVersion = new Version(1, 0);
            _config = new TestUpdaterConfig(_installedVersion);

            _config.CommandLineWrapper.Full.Returns(@"\.myapp.exe");
            _config.CommandLineWrapper.Arguments.Returns(new[] { @"\.myapp.exe" });
        }

        void AndGivenAnAppUpdater()
        {
            _appUpdater = new AppUpdater(_config);
        }

        void AndGivenAnInstalledUpdate()
        {
            _installedUpdate = new InstalledUpdate(_installedVersion, new Version(2, 0));
        }

        void WhenLaunchInstalledUpdateIsCalled()
        {
            _appUpdater.LaunchInstalledUpdate(_installedUpdate);
        }

        void ThenTheApplicationWillBeLaunchedWithNoArguments()
        {
            _config.ProcessWrapper.Received().Start(@"\.myapp.exe", string.Empty);
        }
    }
}