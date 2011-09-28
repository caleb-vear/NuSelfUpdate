using System;
using NSubstitute;
using NuSelfUpdate.Tests.Helpers;
using Shouldly;

namespace NuSelfUpdate.Tests.AppUpdaterBehaviour.LaunchInstalledUpdateScenarios
{
    public class CommandLineArgumentsWithSpaces
    {
        Version _installedVersion;
        AppUpdater _appUpdater;
        InstalledUpdate _installedUpdate;
        TestUpdaterConfig _config;
        InstalledUpdate _returnedInstalledUpdate;

        void GivenTheAppWasRunWithArgumentsThatDoContainSpaces()
        {
            _installedVersion = new Version(1, 0);
            _config = new TestUpdaterConfig(_installedVersion);

            _config.CommandLineWrapper.Full.Returns(".\\myapp.exe -v1 -updatemode \"auto update\"");
            _config.CommandLineWrapper.Arguments.Returns(new[] { @".\myapp.exe", "-v1", "-updatemode", "auto update" });
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
            _returnedInstalledUpdate = _appUpdater.LaunchInstalledUpdate(_installedUpdate);
        }

        void ThenTheApplicationWillBeLaunchedWithNoArguments()
        {
            _config.ProcessWrapper.Received().Start(@".\myapp.exe", "-v1 -updatemode" + " \"auto update\"");
        }

        void AndTheInstalledUpdateReturnedWillBeTheSameAsTheOnePassedIn()
        {
            _returnedInstalledUpdate.ShouldBe(_installedUpdate);
        }
    }
}