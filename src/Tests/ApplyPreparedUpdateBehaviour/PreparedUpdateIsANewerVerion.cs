using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NSubstitute;
using NuSelfUpdate.Tests.Helpers;
using Shouldly;

namespace NuSelfUpdate.Tests.ApplyPreparedUpdateBehaviour
{
    public class PreparedUpdateIsANewerVerion : BddifyTest
    {
        const string PrepDir = @"c:\app\.updates\1.1\";
        const string AppDirectory = @"c:\app\";
        const string OldDir = @"c:\app\.old\";
        Version _installedVersion;
        TestUpdaterConfig _config;
        MockFileSystem _fileSystem;
        string[] _appFiles;
        IPreparedUpdate _preparedUpdate;
        AppUpdater _appUpdater;
        string[] _newAppFiles;
        Version _newVersion;

        void GivenAnInstalledVersion()
        {
            _installedVersion = new Version(1, 0);
            _config = new TestUpdaterConfig(_installedVersion);
            _fileSystem = (MockFileSystem)_config.FileSystem;

            _appFiles = new[] { "app.exe", "app.exe.config", "nuget.dll", "data.db" };

            foreach (var file in _appFiles)
            {
                _fileSystem.AddFile(Path.Combine(AppDirectory, file), MockFileContent(file, _installedVersion));
            }
        }

        void AndGivenAPreparedUpdateForANewerVersion()
        {
            _preparedUpdate = Substitute.For<IPreparedUpdate>();
            _newVersion = new Version(1, 1);
            _preparedUpdate.Version.Returns(_newVersion);

            _newAppFiles = new[] { "app.exe", "app.exe.config", "nuget.dll", "app.core.dll" };
            _fileSystem.CreateDirectory(@"c:\app\.updates\1.1");

            foreach (var file in _newAppFiles)
            {
                _fileSystem.AddFile(Path.Combine(PrepDir, file), MockFileContent(file, _newVersion));
            }

            _preparedUpdate.Files.Returns(_newAppFiles.Select(file => Path.Combine(PrepDir, file)));
        }

        void AndGivenAnAppUpdater()
        {
            _appUpdater = new AppUpdater(_config);
        }

        void WhenThePreparedUpdateIsApplied()
        {
            _appUpdater.ApplyPreparedUpdate(_preparedUpdate);
        }

        void ThenAllAppFilesThatHaveNewerVersionsWillBeMovedIntoTheOldDirectory()
        {
            var expectedOldDirFiles = new Dictionary<string, Version>
                                          {
                                              {"app.exe", _installedVersion},
                                              {"app.exe.config", _installedVersion},
                                              {"nuget.dll", _installedVersion},
                                          };

            VerifyDirectoryFiles(OldDir, expectedOldDirFiles);
        }

        void AndAllPreparedFilesWillHaveBeenMovedToTheAppDirectoryLeavingFilesThatDidNotHaveNewerVerionsIntact()
        {
            var expectedAppDirFiles = new Dictionary<string, Version>
                                          {
                                              {AppDirectory + "app.exe", _newVersion},
                                              {AppDirectory + "app.exe.config", _newVersion},
                                              {AppDirectory + "nuget.dll", _newVersion},
                                              {AppDirectory + "app.core.dll", _newVersion},
                                              {AppDirectory + "data.db", _installedVersion},
                                          };

            VerifyDirectoryFiles(AppDirectory, expectedAppDirFiles);
        }

        void AndThePrepDirectoryWillBeDeleted()
        {
            _fileSystem.DirectoryExists(PrepDir).ShouldBe(false);
        }

        void VerifyFile(string file, Version version)
        {
            _fileSystem.ReadAllText(file).ShouldBe(MockFileContent(Path.GetFileName(file), version));
        }

        void VerifyDirectoryFiles(string directory, IDictionary<string, Version> expectedFiles)
        {
            foreach (var file in expectedFiles)
            {
                VerifyFile(Path.Combine(directory, file.Key), file.Value);
            }

            ShouldBeTestExtensions.ShouldBe(_fileSystem.GetFiles(directory)
                              .All(expectedFiles.ContainsKey), true);
        }

        static string MockFileContent(string file, Version version)
        {
            return file + " - v" + version;
        }
    }
}