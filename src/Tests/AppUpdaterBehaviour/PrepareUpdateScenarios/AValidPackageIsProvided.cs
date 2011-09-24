using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NSubstitute;
using NuGet;
using NuSelfUpdate.Tests.Helpers;
using Shouldly;

namespace NuSelfUpdate.Tests.AppUpdaterBehaviour.PrepareUpdateScenarios
{
    public class AValidPackageIsProvided : BddifyTest
    {
        Version _installedVersion;
        AppUpdater _appUpdater;
        IPackage _package;
        IEnumerable<IPackageFile> _appFiles;
        IEnumerable<IPackageFile> _otherFiles;
        TestUpdaterConfig _config;
        MockFileSystem _fileSystem;
        IPreparedUpdate _preparedUpdate;
        List<string> _expectedFiles;

        void GivenAnInstalledVersion()
        {
            _installedVersion = new Version(1, 0);
        }

        void AndGivenAnAppUpdater()
        {
            _config = new TestUpdaterConfig(_installedVersion);
            _fileSystem = (MockFileSystem) _config.FileSystem;
            _appUpdater = new AppUpdater(_config);
        }

        void AndGivenAPackageForANewerVersionOfTheApp()
        {
            _package = Packages.FromVersions(_config.AppPackageId, new Version(1, 1)).Single();
            _appFiles = GetAppFileSubstitutes("app", "app.exe", "app.exe.config", "nuget.dll", @"content\logo.png").ToArray();
            _otherFiles = GetAppFileSubstitutes("", "README.md").ToArray();

            var packageFiles = _appFiles.Concat(_otherFiles);
            _package.GetFiles().Returns(packageFiles);
        }

        void WhenTheUpdateIsPrepared()
        {
            _preparedUpdate =  _appUpdater.PrepareUpdate(_package);
        }
        
        void ThenAllFilesInThePackagesAppDirectoryWillBeSavedToTheUpgradePrepPath()
        {
            _expectedFiles = new List<string>()
                                 {
                                     @"c:\app\.updates\1.1\app.exe",
                                     @"c:\app\.updates\1.1\app.exe.config",
                                     @"c:\app\.updates\1.1\nuget.dll",
                                     @"c:\app\.updates\1.1\content\logo.png",
                                 };

            foreach (var expectedFile in _expectedFiles)
            {
                _fileSystem.ReadAllText(expectedFile)
                    .ShouldBe(Encoding.UTF8.GetString(GetMockFileBytes(expectedFile)));
            }
        }

        void AndNoOtherFilesWillHaveBeenSaved()
        {
            _fileSystem.Paths.Where(f => f.Value != null).Count().ShouldBe(_expectedFiles.Count);
        }

        void AndThePreparedUpdateWillHaveTheCorrectVersion()
        {
            _preparedUpdate.Version.ShouldBe(_package.Version);
        }

        void AndThePreparedUpdateWillListTheSavedFiles()
        {
            var sortedExpextedFiles = _expectedFiles.OrderBy(f => f);
            _preparedUpdate.Files
                .OrderBy(f => f)
                .ShouldBe(sortedExpextedFiles);
        }

        IEnumerable<IPackageFile> GetAppFileSubstitutes(string directory, params string[] fileNames)
        {
            foreach (var fileName in fileNames)
            {
                var file = Substitute.For<IPackageFile>();
                file.Path.Returns(System.IO.Path.Combine(directory, fileName));

                file.GetStream().Returns(callInfo => new System.IO.MemoryStream(GetMockFileBytes(file.Path)));
                yield return file;
            }
        }

        static byte[] GetMockFileBytes(string fileName)
        {
            fileName = System.IO.Path.GetFileName(fileName);
            return Encoding.UTF8.GetBytes(fileName.Length + " - " + fileName);
        }
    }
}