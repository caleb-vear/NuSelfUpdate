using System;
using System.IO;
using NuSelfUpdate.Tests.Helpers;
using Shouldly;

namespace NuSelfUpdate.Tests.AppUpdaterBehaviour.OldVersionExistsScenarios
{
    public class AnOldVersionExists
    {
        Version _installedVersion;
        TestUpdaterConfig _config;
        MockFileSystem _fileSystem;
        string[] _appFiles;
        AppUpdater _appUpdater;
        protected const string AppDirectory = @"c:\app\";
        protected const string OldDir = @"c:\app\.old\";

        void GivenAnApplicationDirectoryContainingAppFiles()
        {
            _installedVersion = new Version(1, 0);
            _config = new TestUpdaterConfig(_installedVersion);
            _fileSystem = (MockFileSystem)_config.FileSystem;

            _appFiles = new[] { "app.exe", "app.exe.config", "nuget.dll", "data.db", "content\\logo.png" };

            foreach (var file in _appFiles)
                _fileSystem.AddFile(Path.Combine(AppDirectory, file), MockFileContent(file, _installedVersion));            
        }

        void AndGivenAndOldVersionOfTheAppExistsInTheOldDirectory()
        {
            var oldVersion = new Version(0, 9);

            foreach (var file in _appFiles)
                _fileSystem.AddFile(Path.Combine(OldDir, file), MockFileContent(file, _installedVersion));
        }

        void WhenAnAppUpdaterIsCreated()
        {
            _appUpdater = new AppUpdater(_config);
        }

        void ThenTheOldVersionExistsPropertyWillBeTrue()
        {
            _appUpdater.OldVersionExists.ShouldBe(true);
        }

        protected static string MockFileContent(string file, Version version)
        {
            return Path.GetFileName(file) + " - v" + version;
        }
    }
}