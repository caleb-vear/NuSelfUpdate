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
        readonly IAppVersionProvider _appVersionProvider;
        readonly IPrepDirectoryStrategy _prepDirectoryStrategy;
        readonly IExtendedFileSystem _fileSystem;

        public AppUpdater(string packageSource, string appPackageId, IPackageRepositoryFactory packageRepositoryFactory, IAppVersionProvider appVersionProvider, IPrepDirectoryStrategy prepDirectoryStrategy, IExtendedFileSystem fileSystem)
        {
            _packageSource = packageSource;
            _appPackageId = appPackageId;
            _packageRepositoryFactory = packageRepositoryFactory;
            _appVersionProvider = appVersionProvider;
            _prepDirectoryStrategy = prepDirectoryStrategy;
            _fileSystem = fileSystem;
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

            foreach (var packageFile in package.GetFiles("app"))
            {
                var targetPath = Path.Combine(prepDirectory, Path.GetFileName(packageFile.Path));
                _fileSystem.AddFile(targetPath, packageFile.GetStream());
            }

            return null;
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

    public interface IPreparedUpdate
    {

    }
}