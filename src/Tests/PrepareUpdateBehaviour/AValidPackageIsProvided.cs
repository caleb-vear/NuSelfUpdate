using System;
using System.Collections.Generic;
using System.Linq;
using NSubstitute;
using NuGet;
using NuSelfUpdate.Tests.Helpers;

namespace NuSelfUpdate.Tests.PrepareUpdateBehaviour
{
    public class AValidPackageIsProvided : BddifyTest
    {
        Version _installedVersion;
        AppUpdater _appUpdater;
        IPackage _package;
        IEnumerable<IPackageFile> _appFiles;
        IEnumerable<IPackageFile> _otherFiles;
        IPackageFileSaver _packageFileSaver;

        void GivenAnInstalledVersion()
        {
            _installedVersion = new Version(1, 0);
        }

        void AndGivenAnAppUpdater()
        {
            _packageFileSaver = Substitute.For<IPackageFileSaver>();
            _appUpdater = AppUpdaters.Build(_installedVersion, Enumerable.Empty<IPackage>(), _packageFileSaver);
        }

        void AndGivenAPackageForANewerVersionOfTheApp()
        {
            _package = Packages.FromVersions(AppUpdaters.DefaultPackageId, new Version(1, 1)).Single();
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
            var prepDirectory = TestPrepDirectoryStrategy.Instance.GetFor(_package.Version);

            foreach (var file in _appFiles)
                _packageFileSaver.Received().Save(file, prepDirectory);
        }

        void AndNoOtherFilesWillHaveBeenSaved()
        {
            foreach (var file in _otherFiles)
                _packageFileSaver.DidNotReceive().Save(file, Arg.Any<string>());
        }

        IEnumerable<IPackageFile> GetAppFileSubstitutes(string directory, params string[] fileNames)
        {
            foreach (var fileName in fileNames)
            {
                var file = Substitute.For<IPackageFile>();
                file.Path.Returns(System.IO.Path.Combine(directory, fileName));

                yield return file;
            }
        }
    }
}