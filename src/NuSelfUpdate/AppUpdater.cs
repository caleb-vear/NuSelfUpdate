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
        readonly ICommandLineWrapper _commandLineWrapper;
        readonly IProcessWrapper _processWrapper;
        readonly string _oldVersionDir;

        public AppUpdater(NuGetConfig nugetConfig, IAppVersionProvider appVersionProvider, IExtendedFileSystem fileSystem, ICommandLineWrapper commandLineWrapper, IProcessWrapper processWrapper)
        {
            _appVersionProvider = appVersionProvider;
            _fileSystem = fileSystem;
            _commandLineWrapper = commandLineWrapper;
            _processWrapper = processWrapper;
            _packageSource = nugetConfig.PackageSource;
            _appPackageId = nugetConfig.AppPackageId;
            _packageRepositoryFactory = nugetConfig.RepositoryFactory;

            _oldVersionDir = Path.Combine(_fileSystem.AppDirectory, ".old");
        }

        public bool OldVersionExists
        {
            get
            {
                return _fileSystem.DirectoryExists(_oldVersionDir);
            }
        }

        public Version CurrentVersion
        {
            get { return _appVersionProvider.CurrentVersion; }
        }

        public IUpdateCheck CheckForUpdate()
        {
            var repository = _packageRepositoryFactory.CreateRepository(_packageSource);
            var latestPackage = repository.FindPackage(_appPackageId);

            if (latestPackage == null || CurrentVersion >= latestPackage.Version)
                return new UpdateNotFound();

            return new UpdateFound(latestPackage);
        }

        public IPreparedUpdate PrepareUpdate(IPackage package)
        {
            if (package == null || package.Id != _appPackageId)
                throw new ArgumentNullException("package");

            AssertCanUpdate(package.Version);

            var prepDirectory = Path.Combine(_fileSystem.AppDirectory, ".updates", package.Version.ToString());
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
            var basePrepDir = Path.Combine(_fileSystem.AppDirectory, ".updates");
            var prepDir = Path.Combine(basePrepDir, preparedUpdate.Version.ToString());

            if (_fileSystem.DirectoryExists(oldVersionDir))
                _fileSystem.DeleteDirectory(oldVersionDir, true);

            foreach (var filePath in preparedUpdate.Files)
            {
                var fileName = Get(filePath, relativeTo: prepDir);
                var appFilePath = Path.Combine(_fileSystem.AppDirectory, fileName);
                if (_fileSystem.FileExists(appFilePath))
                {
                    _fileSystem.MoveFile(appFilePath, Path.Combine(oldVersionDir, fileName));
                }

                _fileSystem.MoveFile(filePath, appFilePath);
            }

            _fileSystem.DeleteDirectory(basePrepDir, true);

            return new InstalledUpdate(CurrentVersion, preparedUpdate.Version);
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
            if (targetVersion <= CurrentVersion)
                throw new BackwardUpdateException(CurrentVersion, targetVersion);
        }
    }
}