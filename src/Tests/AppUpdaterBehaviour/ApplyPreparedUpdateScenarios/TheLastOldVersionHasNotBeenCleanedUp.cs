using System;
using System.IO;
using System.Linq;
using NSubstitute;
using NuSelfUpdate.Tests.Helpers;
using Shouldly;

namespace NuSelfUpdate.Tests.AppUpdaterBehaviour.ApplyPreparedUpdateScenarios
{
    public class TheLastOldVersionHasNotBeenCleanedUp : BaseApplyUpdateScenario
    {
        Version _preUpdateVersion;
        string _appFile;
        IPreparedUpdate _preparedUpdate;
        Version _newVersion;
        AppUpdater _appUpdater;
        string _cruftFile;
        AppUpdaterBuilder _builder;

        void GivenAnInstalledVersion()
        {
            _preUpdateVersion = new Version(1, 0);
            _builder = new AppUpdaterBuilder(TestConstants.AppPackageId)
                .SetupWithTestValues(_preUpdateVersion);

            FileSystem = _builder.GetMockFileSystem();

            _appFile = "app.exe";

            FileSystem.AddFile(Path.Combine(AppDirectory, _appFile), MockFileContent(_appFile, _preUpdateVersion));
        }

        void AndGivenThereIsAnOldCopyStillInOldDirectoryAlongWithACruftFile()
        {
            var oldVersion = new Version(0, 9);
            _cruftFile = "cruft.dll";
            FileSystem.AddFile(Path.Combine(OldDir, _cruftFile), MockFileContent(_cruftFile, oldVersion));
            FileSystem.AddFile(Path.Combine(OldDir, _appFile), MockFileContent(_appFile, oldVersion));
        }

        void AndGivenAPreparedUpdateForANewerVersion()
        {
            _preparedUpdate = Substitute.For<IPreparedUpdate>();
            _newVersion = new Version(1, 1);
            _preparedUpdate.Version.Returns(_newVersion);

            FileSystem.CreateDirectory(@"c:\app\.updates\1.1");

            FileSystem.AddFile(Path.Combine(PrepDir, _appFile), MockFileContent(_appFile, _newVersion));
            _preparedUpdate.Files.Returns(new[] { Path.Combine(PrepDir, _appFile) });
        }

        void AndGivenAnAppUpdater()
        {
            _appUpdater = _builder.Build();
        }

        void WhenThePreparedUpdateIsApplied()
        {
            _appUpdater.ApplyPreparedUpdate(_preparedUpdate);
        }

        void ThenTheOldDirectoryAppFileWillHaveBeenOverridenByThePreUpdateAppFile()
        {
            VerifyFile(Path.Combine(OldDir, _appFile), _preUpdateVersion);
        }

        void AndTheCruftFileWillHaveBeenDeleted()
        {
            FileSystem.FileExists(_cruftFile).ShouldBe(false);
            ShouldBeTestExtensions.ShouldBe(FileSystem.GetFiles(OldDir).Count(), 1);
        }
    }
}