using System;
using System.IO;
using NuSelfUpdate.Tests.Helpers;
using Shouldly;

namespace NuSelfUpdate.Tests.AppUpdaterBehaviour.RemoveOldVersionFilesScenarios
{
    public class OldFilesExist : BaseRemoveOldVerisionFilesScenario
    {
        Version _installedVersion;
        MockFileSystem _fileSystem;
        string[] _appFiles;
        AppUpdater _appUpdater;
        AppUpdaterBuilder _builder;

        void GivenAnApplicationDirectoryContainingAppFiles()
        {
            _installedVersion = new Version(1, 0);
            _builder = new AppUpdaterBuilder(TestConstants.AppPackageId)
                .SetupWithTestValues(_installedVersion);

            _fileSystem = _builder.GetMockFileSystem();

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

        void AndGivenAnAppUpdater()
        {
            _appUpdater = _builder.Build();
        }

        void WhenRemoveOldVersionFilesIsCalled()
        {
            _appUpdater.RemoveOldVersionFiles();
        }

        void ThenAllTheOldDirectoryWillBeDeleted()
        {
            _fileSystem.DirectoryExists(OldDir).ShouldBe(false);
        }

        void AndAllCurrentVersionAppFilesWillRemain()
        {
            foreach (var file in _appFiles)
                VerifyFile(_fileSystem, Path.Combine(AppDirectory, file), _installedVersion);  
        }
    }
}