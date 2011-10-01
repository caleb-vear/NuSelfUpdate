using System;
using System.IO;
using System.Reflection;
using NuGet;

namespace NuSelfUpdate
{
    public class AppUpdaterBuilder
    {
        readonly string _appPackageId;
        string _packageSource;
        IPackageRepositoryFactory _repositoryFactory;
        IAppVersionProvider _versionProvider;
        IExtendedFileSystem _fileSystem;
        ICommandLineWrapper _commandLineWrapper;
        IProcessWrapper _processWrapper;

        /// <param name="appPackageId">The id of the package this applications updates are published to.</param>
        public AppUpdaterBuilder(string appPackageId)
        {
            if (string.IsNullOrEmpty(appPackageId))
                throw new ArgumentNullException("appPackageId");

            _appPackageId = appPackageId;
        }

        /// <summary>
        /// Set the <paramref name="packageSource"/> which will be used when creating the package repository.
        /// The default is to use the public NuGet repository.
        /// </summary>
        public AppUpdaterBuilder SourceUpdatesFrom(string packageSource)
        {
            _packageSource = packageSource;
            return this;
        }

        /// <summary>
        /// Provide an alternative <see cref="IPackageRepositoryFactory"/> if you need to customize how
        /// the <see cref="AppUpdater"/> will create a package repository <see cref="IPackageRepository"/>.
        /// </summary>
        public AppUpdaterBuilder CreatePackageRepositoriesWith(IPackageRepositoryFactory repositoryFactory)
        {
            _repositoryFactory = repositoryFactory;
            return this;
        }

        /// <summary>
        /// Control how the <see cref="AppUpdater"/> will determine what the current version of the application is.
        /// The default is to use the assembly version of the entry assembly.
        /// </summary>
        public AppUpdaterBuilder CurrentVersionProvidedBy(IAppVersionProvider appVersionProvider)
        {
            _versionProvider = appVersionProvider;
            return this;
        }

        /// <summary>
        /// Control how the <see cref="AppUpdater"/> will access the file system.
        /// </summary>
        public AppUpdaterBuilder FileSystemAccessedThrough(IExtendedFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
            return this;
        }

        /// <summary>
        /// Control how the <see cref="AppUpdater"/> will determine what the current command line arguments
        /// are.  This use used for the LaunchInstalledUpdate feature.
        /// </summary>
        public AppUpdaterBuilder CommandLineInfoProvidedBy(ICommandLineWrapper commandLineWrapper)
        {
            _commandLineWrapper = commandLineWrapper;
            return this;
        }

        /// <summary>
        /// Control how the <see cref="AppUpdater"/> will launch the new process when using the
        /// LaunchInstalledUpdater feature.
        /// </summary>
        public AppUpdaterBuilder RelaunchAppWith(IProcessWrapper processWrapper)
        {
            _processWrapper = processWrapper;
            return this;
        }

        /// <summary>
        /// Build the AppUpdater with the configured options.
        /// </summary>
        /// <returns></returns>
        public AppUpdater Build()
        {
            var nugetConfig = new NuGetConfig
                                  {
                                      AppPackageId = _appPackageId,
                                      PackageSource = _packageSource ?? NuGetConstants.DefaultFeedUrl,
                                      RepositoryFactory = _repositoryFactory ?? new AppUpdaterRepositoryFactory(),
                                  };

            var versionProvider = _versionProvider ?? new EntryAssemblyAppVersionProvider();
            var fileSystem = _fileSystem ?? new ExtendedPhysicalFileSystem(GetAppDir());
            var commandLineWrapper = _commandLineWrapper ?? new CommandLineWrapper();
            var processWrapper = _processWrapper ?? new ProcessWrapper();

            return new AppUpdater(nugetConfig, versionProvider, fileSystem, commandLineWrapper, processWrapper);
        }

        static string GetAppDir()
        {
            return Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
        }
    }
}