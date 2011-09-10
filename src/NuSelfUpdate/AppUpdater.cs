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
        readonly IPrepDirectoryStrategy _prepDirectoryStrategy;
        readonly IExtendedFileSystem _fileSystem;

        public AppUpdater(AppUpdaterConfig config)
        {
            _packageSource = config.PackageSource;
            _appPackageId = config.AppPackageId;
            _packageRepositoryFactory = config.PackageRepositoryFactory;
            _appVersionProvider = config.AppVersionProvider;
            _prepDirectoryStrategy = config.UpdatePrepDirectoryStrategy;
            _fileSystem = config.FileSystem;
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

            AssertCanUpdate(_appVersionProvider.CurrentVersion, package.Version);

            var prepDirectory = _prepDirectoryStrategy.GetFor(package.Version);
            var preparedFiles = new List<string>();

            foreach (var packageFile in package.GetFiles("app"))
            {
                var targetPath = Path.Combine(prepDirectory, Path.GetFileName(packageFile.Path));                
                _fileSystem.AddFile(targetPath, packageFile.GetStream());

                preparedFiles.Add(targetPath);
            }

            return new PreparedUpdate(package.Version, preparedFiles);
        }

        private void AssertCanUpdate(Version currentVersion, Version targetVersion)
        {
            if (targetVersion <= _appVersionProvider.CurrentVersion)
                throw new BackwardUpdateException(_appVersionProvider.CurrentVersion, targetVersion);
        }
    }

    public interface IPrepDirectoryStrategy
    {
        string GetFor(Version updateVersion);
    }
}