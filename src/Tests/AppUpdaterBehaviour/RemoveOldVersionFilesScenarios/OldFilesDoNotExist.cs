using System;
using System.IO;
using NuSelfUpdate.Tests.Helpers;
using Shouldly;

namespace NuSelfUpdate.Tests.AppUpdaterBehaviour.RemoveOldVersionFilesScenarios
{
    public class OldFilesDoNotExist : BaseRemoveOldVerisionFilesScenario
    {
        Version _installedVersion;
        TestUpdaterConfig _config;
        MockFileSystem _fileSystem;
        string[] _appFiles;
        AppUpdater _appUpdater;

        void GivenAnApplicationDirectoryContainingAppFiles()
        {
            _installedVersion = new Version(1, 0);
            _config = new TestUpdaterConfig(_installedVersion);
            _fileSystem = (MockFileSystem)_config.FileSystem;

            _appFiles = new[] { "app.exe", "app.exe.config", "nuget.dll", "data.db", "content\\logo.png" };

            foreach (var file in _appFiles)
                _fileSystem.AddFile(Path.Combine(AppDirectory, file), MockFileContent(file, _installedVersion));
        }

        void AndGivenNoOldDirectoryExists()
        {
            _fileSystem.DirectoryExists(OldDir).ShouldBe(false);
        }

        void AndGivenAnAppUpdater()
        {
            _appUpdater = new AppUpdater(_config);
        }

        void WhenRemoveOldVersionFilesIsCalled()
        {
            _appUpdater.RemoveOldVersionFiles();
        }

        void ThenAllCurrentVersionAppFilesWillRemain()
        {
            foreach (var file in _appFiles)
                VerifyFile(_fileSystem, Path.Combine(AppDirectory, file), _installedVersion);
        }
    }
}