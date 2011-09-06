using NuGet;

namespace NuSelfUpdate
{
    public class AppUpdater
    {
        readonly string _packageSource;
        readonly string _packageName;
        readonly IPackageRepositoryFactory _packageRepositoryFactory;
        readonly IVersionLocator _versionLocator;

        public AppUpdater(string packageSource, string packageName, IPackageRepositoryFactory packageRepositoryFactory, IVersionLocator versionLocator)
        {
            _packageSource = packageSource;
            _packageName = packageName;
            _packageRepositoryFactory = packageRepositoryFactory;
            _versionLocator = versionLocator;
        }

        public IUpdateCheck CheckForUpdate()
        {
            var currentVersion = _versionLocator.CurrentVersion;
            var repository = _packageRepositoryFactory.CreateRepository(_packageSource);
            var latestPackage = repository.FindPackage(_packageName);

            return new UpdateCheck { UpdateAvailable = currentVersion < latestPackage.Version };
        }
    }
}