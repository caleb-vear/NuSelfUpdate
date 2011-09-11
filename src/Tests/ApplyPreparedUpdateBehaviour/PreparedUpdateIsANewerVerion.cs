using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NSubstitute;
using NuSelfUpdate.Tests.Helpers;
using Shouldly;

namespace NuSelfUpdate.Tests.ApplyPreparedUpdateBehaviour
{
    public class PreparedUpdateIsANewerVerion : ApplyUpdateTest
    {
        Version _preUpdateVersion;
        TestUpdaterConfig _config;
        string[] _appFiles;
        IPreparedUpdate _preparedUpdate;
        AppUpdater _appUpdater;
        string[] _newAppFiles;
        Version _newVersion;
        InstalledUpdate _installedUpdated;

        void GivenAnInstalledVersion()
        {
            _preUpdateVersion = new Version(1, 0);
            _config = new TestUpdaterConfig(_preUpdateVersion);
            FileSystem = (MockFileSystem)_config.FileSystem;

            _appFiles = new[] { "app.exe", "app.exe.config", "nuget.dll", "data.db" };

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

            _newAppFiles = new[] { "app.exe", "app.exe.config", "nuget.dll", "app.core.dll" };
            FileSystem.CreateDirectory(@"c:\app\.updates\1.1");

            foreach (var file in _newAppFiles)
            {
                FileSystem.AddFile(Path.Combine(PrepDir, file), MockFileContent(file, _newVersion));
            }

            _preparedUpdate.Files.Returns(_newAppFiles.Select(file => Path.Combine(PrepDir, file)));
        }

        void AndGivenAnAppUpdater()
        {
            _appUpdater = new AppUpdater(_config);
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