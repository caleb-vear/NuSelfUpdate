using System;
using System.Collections.Generic;
using System.Linq;
using NuGet;
using System.IO;

namespace NuSelfUpdate
{
    public class AppUpdater
    {
        readonly string _packageSource;
        readonly string _appPackageId;
        readonly IPackageRepositoryFactory _packageRepositoryFactory;
        readonly IAppVersionProvider _appVersionProvider;
        readonly IExtendedFileSystem _fileSystem;
        readonly string _appDirectory;

        public AppUpdater(AppUpdaterConfig config)
        {
            _packageSource = config.PackageSource;
            _appPackageId = config.AppPackageId;
            _packageRepositoryFactory = config.PackageRepositoryFactory;
            _appVersionProvider = config.AppVersionProvider;
            _fileSystem = config.FileSystem;
            _appDirectory = config.AppDirectory;
        }

        public IUpdateCheck CheckForUpdate()
        {
            var currentVersion = _appVersionProvider.CurrentVersion;
            var repository = _packageRepositoryFactory.CreateRepository(_packageSource);
            var latestPackage = repository.FindPackage(_appPackageId);

            return currentVersion < latestPackage.Version ? new UpdateFound(latestPackage) : (IUpdateCheck)new UpdateNotFound();
        }

        public IPreparedUpdate PrepareUpdate(IPackage package)
        {
            if (package == null || package.Id != _appPackageId)
                throw new ArgumentNullException("package");

            AssertCanUpdate(package.Version);

            var prepDirectory = Path.Combine(_appDirectory, ".updates", package.Version.ToString());
            var preparedFiles = new List<string>();

            foreach (var packageFile in package.GetFiles("app"))
            {
                var targetPath = Path.Combine(prepDirectory, Path.GetFileName(packageFile.Path));                
                _fileSystem.AddFile(targetPath, packageFile.GetStream());

                preparedFiles.Add(targetPath);
            }

            return new PreparedUpdate(package.Version, preparedFiles);
        }

        public InstalledUpdate ApplyPreparedUpdate(IPreparedUpdate preparedUpdate)
        {
            AssertCanUpdate(preparedUpdate.Version);

            foreach (var filePath in preparedUpdate.Files)
            {
                var fileName = Path.GetFileName(filePath);
                var appFilePath = Path.Combine(_appDirectory, fileName);
                if (_fileSystem.FileExists(appFilePath))
                {
                    _fileSystem.MoveFile(appFilePath, Path.Combine(_appDirectory, ".old", fileName));
                }

                _fileSystem.MoveFile(filePath, appFilePath);
            }

            _fileSystem.DeleteDirectory(Path.Combine(_appDirectory, ".updates"), true);

            return new InstalledUpdate(_appVersionProvider.CurrentVersion, preparedUpdate.Version);
        }

        private void AssertCanUpdate(Version targetVersion)
        {
            if (targetVersion <= _appVersionProvider.CurrentVersion)
                throw new BackwardUpdateException(_appVersionProvider.CurrentVersion, targetVersion);
        }        
    }
}