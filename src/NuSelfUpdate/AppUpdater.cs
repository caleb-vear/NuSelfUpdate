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
        readonly ICommandLineWrapper _commandLineWrapper;
        readonly IProcessWrapper _processWrapper;
        readonly string _oldVersionDir;

        public AppUpdater(AppUpdaterConfig config)
        {
            _packageSource = config.PackageSource;
            _appPackageId = config.AppPackageId;
            _packageRepositoryFactory = config.PackageRepositoryFactory;
            _appVersionProvider = config.AppVersionProvider;
            _fileSystem = config.FileSystem;
            _appDirectory = config.AppDirectory;
            _commandLineWrapper = config.CommandLineWrapper;
            _processWrapper = config.ProcessWrapper;
            _oldVersionDir = Path.Combine(_appDirectory, ".old");
        }

        public bool OldVersionExists
        {
            get
            {
                return _fileSystem.DirectoryExists(_oldVersionDir);
            }
        }

        public IUpdateCheck CheckForUpdate()
        {
            var currentVersion = _appVersionProvider.CurrentVersion;
            var repository = _packageRepositoryFactory.CreateRepository(_packageSource);
            var latestPackage = repository.FindPackage(_appPackageId);

            if (latestPackage == null || currentVersion >= latestPackage.Version)
                return new UpdateNotFound();

            return new UpdateFound(latestPackage);
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
                var targetPath = Path.Combine(prepDirectory, Get(packageFile.Path, relativeTo: "app"));
                _fileSystem.AddFile(targetPath, packageFile.GetStream());

                preparedFiles.Add(targetPath);
            }

            return new PreparedUpdate(package.Version, preparedFiles);
        }

        public InstalledUpdate ApplyPreparedUpdate(IPreparedUpdate preparedUpdate)
        {
            AssertCanUpdate(preparedUpdate.Version);

            var oldVersionDir = _oldVersionDir;
            var basePrepDir = Path.Combine(_appDirectory, ".updates");
            var prepDir = Path.Combine(basePrepDir, preparedUpdate.Version.ToString());

            if (_fileSystem.DirectoryExists(oldVersionDir))
                _fileSystem.DeleteDirectory(oldVersionDir, true);

            foreach (var filePath in preparedUpdate.Files)
            {
                var fileName = Get(filePath, relativeTo: prepDir);
                var appFilePath = Path.Combine(_appDirectory, fileName);
                if (_fileSystem.FileExists(appFilePath))
                {
                    _fileSystem.MoveFile(appFilePath, Path.Combine(oldVersionDir, fileName));
                }

                _fileSystem.MoveFile(filePath, appFilePath);
            }

            _fileSystem.DeleteDirectory(basePrepDir, true);

            return new InstalledUpdate(_appVersionProvider.CurrentVersion, preparedUpdate.Version);
        }

        public InstalledUpdate LaunchInstalledUpdate(InstalledUpdate installedUpdate)
        {
            if (installedUpdate == null)
                throw new ArgumentNullException("installedUpdate");

            var fullCmdLine = _commandLineWrapper.Full;
            var arguments = _commandLineWrapper.Arguments;

            var filename = arguments[0];
            var cmdArguments = fullCmdLine.Substring(fullCmdLine.IndexOf(filename) + filename.Length);

            if (cmdArguments.StartsWith("\""))
                cmdArguments = cmdArguments.Substring(1);

            _processWrapper.Start(filename, cmdArguments.Trim());

            return installedUpdate;
        }

        public void RemoveOldVersionFiles()
        {
            _fileSystem.DeleteDirectory(_oldVersionDir, true);
        }

        string Get(string path, string relativeTo)
        {
            var pathSegments = new List<string>();
            var relativeToParentDir = Path.GetDirectoryName(relativeTo);

            var ignoreCase = StringComparison.InvariantCultureIgnoreCase;
            while (!relativeToParentDir.Equals(Path.GetDirectoryName(path), ignoreCase))
            {
                pathSegments.Add(Path.GetFileName(path));
                path = Path.GetDirectoryName(path);
            }

            return Path.Combine(pathSegments.AsEnumerable().Reverse().ToArray());
        }

        void AssertCanUpdate(Version targetVersion)
        {
            if (targetVersion <= _appVersionProvider.CurrentVersion)
                throw new BackwardUpdateException(_appVersionProvider.CurrentVersion, targetVersion);
        }
    }
}