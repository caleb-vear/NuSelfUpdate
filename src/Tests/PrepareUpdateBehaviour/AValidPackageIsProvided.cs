using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NSubstitute;
using NuGet;
using NuSelfUpdate.Tests.Helpers;
using Shouldly;

namespace NuSelfUpdate.Tests.PrepareUpdateBehaviour
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
            _appFiles = GetAppFileSubstitutes("app", "app.exe", "app.exe.config", "nuget.dll").ToArray();
            _otherFiles = GetAppFileSubstitutes("", "README.md").ToArray();

            var packageFiles = _appFiles.Concat(_otherFiles);
            _package.GetFiles().Returns(packageFiles);
        }

        void WhenTheUpdateIsPrepared()
        {
            _appUpdater.PrepareUpdate(_package);
        }
        
        void ThenAllFilesInThePackagesAppDirectoryWillBeSavedToTheUpgradePrepPath()
        {
            var expectedFiles = new Dictionary<string, string>()
                                    {
                                        {@"c:\app\.updates\1.1\app.exe", "0 - app.exe"},
                                        {@"c:\app\.updates\1.1\app.exe.config", "1 - app.exe.config"},
                                        {@"c:\app\.updates\1.1\nuget.dll", "2 - nuget.dll"},
                                    };

            foreach (var expectedFile in expectedFiles)
            {
                _fileSystem.ReadAllText(expectedFile.Key)
                    .ShouldBe(expectedFile.Value);
            }
        }

        void AndNoOtherFilesWillHaveBeenSaved()
        {
            _fileSystem.Paths.Where(f => f.Value != null).Count().ShouldBe(3);
        }

        IEnumerable<IPackageFile> GetAppFileSubstitutes(string directory, params string[] fileNames)
        {
            var index = 0;

            foreach (var fileName in fileNames)
            {
                var file = Substitute.For<IPackageFile>();
                file.Path.Returns(System.IO.Path.Combine(directory, fileName));
                var fileBytes = Encoding.UTF8.GetBytes(index++ + " - " + fileName);

                file.GetStream().Returns(callInfo => new System.IO.MemoryStream(fileBytes));
                yield return file;
            }
        }
    }
}