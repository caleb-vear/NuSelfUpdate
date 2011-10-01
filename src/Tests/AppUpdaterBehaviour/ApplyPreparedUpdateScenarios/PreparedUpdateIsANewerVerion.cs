using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NSubstitute;
using NuSelfUpdate.Tests.Helpers;
using Shouldly;

namespace NuSelfUpdate.Tests.AppUpdaterBehaviour.ApplyPreparedUpdateScenarios
{
    public class PreparedUpdateIsANewerVerion : BaseApplyUpdateScenario
    {
        Version _preUpdateVersion;
        string[] _appFiles;
        IPreparedUpdate _preparedUpdate;
        AppUpdater _appUpdater;
        string[] _newAppFiles;
        Version _newVersion;
        InstalledUpdate _installedUpdated;
        AppUpdaterBuilder _builder;

        void GivenAnInstalledVersion()
        {
            _preUpdateVersion = new Version(1, 0);
            _builder = new AppUpdaterBuilder(TestConstants.AppPackageId)
                .SetupWithTestValues(_preUpdateVersion);

            FileSystem = _builder.GetMockFileSystem();

            _appFiles = new[] { "app.exe", "app.exe.config", "nuget.dll", "data.db", "content\\logo.png" };

            foreach (var file in _appFiles)
            {
                FileSystem.AddFile(Path.Combine(AppDirectory, file), MockFileContent(file, _preUpdateVersion));
            }
        }

        void AndGivenAPreparedUpdateForANewerVersion()
        {
            _preparedUpdate = Substitute.For<IPreparedUpdate>();
            _newVersion = new Version(1, 1);
            _preparedUpdate.Version.Returns(_newVersion);

            _newAppFiles = new[] { "app.exe", "app.exe.config", "nuget.dll", "app.core.dll", "content\\logo.png" };
            FileSystem.CreateDirectory(@"c:\app\.updates\1.1");

            foreach (var file in _newAppFiles)
            {
                FileSystem.AddFile(Path.Combine(PrepDir, file), MockFileContent(file, _newVersion));
            }

            _preparedUpdate.Files.Returns(_newAppFiles.Select(file => Path.Combine(PrepDir, file)));
        }

        void AndGivenAnAppUpdater()
        {
            _appUpdater = _builder.Build();
        }

        void WhenThePreparedUpdateIsApplied()
        {
            _installedUpdated = _appUpdater.ApplyPreparedUpdate(_preparedUpdate);
        }

        void ThenAllAppFilesThatHaveNewerVersionsWillBeMovedIntoTheOldDirectory()
        {
            var expectedOldDirFiles = new Dictionary<string, Version>
                                          {
                                              {"app.exe", _preUpdateVersion},
                                              {"app.exe.config", _preUpdateVersion},
                                              {"nuget.dll", _preUpdateVersion},
                                              {"content\\logo.png", _preUpdateVersion},
                                          };

            VerifyDirectoryFiles(OldDir, expectedOldDirFiles);
        }

        void AndAllPreparedFilesWillHaveBeenMovedToTheAppDirectoryLeavingFilesThatDidNotHaveNewerVerionsIntact()
        {
            var expectedAppDirFiles = new Dictionary<string, Version>
                                          {
                                              {"app.exe", _newVersion},
                                              {"app.exe.config", _newVersion},
                                              {"nuget.dll", _newVersion},
                                              {"app.core.dll", _newVersion},
                                              {"content\\logo.png", _newVersion},
                                              {"data.db", _preUpdateVersion},
                                          };

            VerifyDirectoryFiles(AppDirectory, expectedAppDirFiles);
        }

        void AndThePrepDirectoryWillBeDeleted()
        {
            FileSystem.DirectoryExists(PrepDir).ShouldBe(false);
        }

        void AndTheInstalledUpdateWillHaveTheOldVersionAndNewVersionPropertiesSetCorrectly()
        {
            _installedUpdated.OldVersion.ShouldBe(_preUpdateVersion);
            _installedUpdated.NewVersion.ShouldBe(_newVersion);
        }
    }
}