using System;
using NuGet;
using System.IO;

namespace NuSelfUpdate
{
    public class AppUpdater
    {
        readonly string _packageSource;
        readonly string _appPackageId;
        readonly IPackageRepositoryFactory _packageRepositoryFactory;
        readonly IVersionLocator _versionLocator;
        readonly IPrepDirectoryStrategy _prepDirectoryStrategy;
        readonly IFileSystem _fileSystem;

        public AppUpdater(string packageSource, string appPackageId, IPackageRepositoryFactory packageRepositoryFactory, IVersionLocator versionLocator, IPrepDirectoryStrategy prepDirectoryStrategy, IFileSystem fileSystem)
        {
            _packageSource = packageSource;
            _appPackageId = appPackageId;
            _packageRepositoryFactory = packageRepositoryFactory;
            _versionLocator = versionLocator;
            _prepDirectoryStrategy = prepDirectoryStrategy;
            _fileSystem = fileSystem;
        }

        public IUpdateCheck CheckForUpdate()
        {
            var currentVersion = _versionLocator.CurrentVersion;
            var repository = _packageRepositoryFactory.CreateRepository(_packageSource);
            var latestPackage = repository.FindPackage(_appPackageId);

            return currentVersion < latestPackage.Version ? new UpdateFound(latestPackage) : (IUpdateCheck)new UpdateNotFound();
        }

        public IPreparedUpdate PrepareUpdate(IPackage package)
        {
            if (package == null || package.Id != _appPackageId)
                throw new ArgumentNullException("package");

            AssertCanUpdate(_versionLocator.CurrentVersion, package.Version);

            var prepDirectory = _prepDirectoryStrategy.GetFor(package.Version);

            foreach (var packageFile in package.GetFiles("app"))
            {
                var targetPath = Path.Combine(prepDirectory, Path.GetFileName(packageFile.Path));
                _fileSystem.AddFile(targetPath, packageFile.GetStream());
            }

            return null;
        }

        private void AssertCanUpdate(Version currentVersion, Version targetVersion)
        {
            if (targetVersion <= _versionLocator.CurrentVersion)
                throw new BackwardUpdateException(_versionLocator.CurrentVersion, targetVersion);
        }
    }

    public interface IPrepDirectoryStrategy
    {
        string GetFor(Version updateVersion);
    }

    public interface IPreparedUpdate
    {

    }
}