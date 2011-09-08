using System;
using NuGet;

namespace NuSelfUpdate
{
    public class AppUpdater
    {
        readonly string _packageSource;
        readonly string _packageId;
        readonly IPackageRepositoryFactory _packageRepositoryFactory;
        readonly IVersionLocator _versionLocator;

        public AppUpdater(string packageSource, string packageId, IPackageRepositoryFactory packageRepositoryFactory, IVersionLocator versionLocator)
        {
            _packageSource = packageSource;
            _packageId = packageId;
            _packageRepositoryFactory = packageRepositoryFactory;
            _versionLocator = versionLocator;
        }

        public IUpdateCheck CheckForUpdate()
        {
            var currentVersion = _versionLocator.CurrentVersion;
            var repository = _packageRepositoryFactory.CreateRepository(_packageSource);
            var latestPackage = repository.FindPackage(_packageId);

            return new UpdateCheck { UpdateAvailable = currentVersion < latestPackage.Version };
        }

        public IPreparedUpdate PrepareUpdate(IUpdate update)
        {
            if (update == null)
                throw new ArgumentNullException("update");

            return null;
        }
    }

    public interface IUpdate
    {

    }

    public interface IPreparedUpdate
    {
        
    }
}