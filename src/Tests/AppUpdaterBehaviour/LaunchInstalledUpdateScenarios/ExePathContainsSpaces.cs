using System;
using NSubstitute;
using NuSelfUpdate.Tests.Helpers;
using Shouldly;

namespace NuSelfUpdate.Tests.AppUpdaterBehaviour.LaunchInstalledUpdateScenarios
{
    public class ExePathContainsSpaces
    {
        Version _installedVersion;
        AppUpdater _appUpdater;
        InstalledUpdate _installedUpdate;
        AppUpdaterBuilder _builder;
        InstalledUpdate _returnedInstalledUpdate;
        ICommandLineWrapper _commandLineWrapper;
        IProcessWrapper _processWrapper;

        void GivenTheAppWasRunWithAPathThatContainsSpacesAndArgumentsThatDoContainSpaces()
        {
            _installedVersion = new Version(1, 0);
            _builder = new AppUpdaterBuilder(TestConstants.AppPackageId)
                .SetupWithTestValues(_installedVersion);

            _commandLineWrapper = _builder.GetSubsituteCommandLineWrapper();
            _processWrapper = _builder.GetSubsituteProcessWrapper();

            _commandLineWrapper.Full.Returns("\"C:\\Program Files\\myapp.exe\" -v1 -updatemode \"auto update\"");
            _commandLineWrapper.Arguments.Returns(new[] { "C:\\Program Files\\myapp.exe", "-v1", "-updatemode", "auto update" });
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
            _processWrapper.Received().Start(@"C:\Program Files\myapp.exe", "-v1 -updatemode" + " \"auto update\"");
        }

        void AndTheInstalledUpdateReturnedWillBeTheSameAsTheOnePassedIn()
        {
            _returnedInstalledUpdate.ShouldBe(_installedUpdate);
        }
    }
}