using System;
using NuGet;

namespace NuSelfUpdate
{
    public class AppUpdater
    {
        readonly string _packageSource;
        readonly string _appPackageId;
        readonly IPackageRepositoryFactory _packageRepositoryFactory;
        readonly IVersionLocator _versionLocator;
        readonly IPrepDirectoryStrategy _prepDirectoryStrategy;
        readonly IPackageFileSaver _packageFileSaver;

        public AppUpdater(string packageSource, string appPackageId, IPackageRepositoryFactory packageRepositoryFactory, IVersionLocator versionLocator, IPrepDirectoryStrategy prepDirectoryStrategy, IPackageFileSaver packageFileSaver)
        {
            _packageSource = packageSource;
            _appPackageId = appPackageId;
            _packageRepositoryFactory = packageRepositoryFactory;
            _versionLocator = versionLocator;
            _prepDirectoryStrategy = prepDirectoryStrategy;
            _packageFileSaver = packageFileSaver;
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

            foreach (var file in package.GetFiles("app"))
            {
                _packageFileSaver.Save(file, prepDirectory);
            }

            return null;
        }

        private void AssertCanUpdate(Version currentVersion, Version targetVersion)
        {
            if (targetVersion <= _versionLocator.CurrentVersion)
                throw new BackwardUpdateException(_versionLocator.CurrentVersion, targetVersion);
        }
    }

    public interface IPackageFileSaver
    {
        void Save(IPackageFile file, string directory);
    }

    public interface IPrepDirectoryStrategy
    {
        string GetFor(Version updateVersion);
    }

    public interface IPreparedUpdate
    {

    }
}