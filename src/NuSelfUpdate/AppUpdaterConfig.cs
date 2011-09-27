using System.IO;
using System.Reflection;
using NuGet;

namespace NuSelfUpdate
{
    public class AppUpdaterConfig
    {
        public string PackageSource { get; set; }
        public string AppPackageId { get; private set; }
        public IPackageRepositoryFactory PackageRepositoryFactory { get; set; }
        public IAppVersionProvider AppVersionProvider { get; set; }
        public IExtendedFileSystem FileSystem { get; set; }
        public ICommandLineWrapper CommandLineWrapper { get; set; }
        public IProcessWrapper ProcessWrapper { get; set; }
        public virtual string AppDirectory
        {
            get
            {
                return Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            }
        }

        public AppUpdaterConfig(string appPackageId)
        {
            AppPackageId = appPackageId;
            PackageSource = NuGetConstants.DefaultFeedUrl;
            PackageRepositoryFactory = new AppUpdaterRepositoryFactory();
            AppVersionProvider = new EntryAssemblyAppVersionProvider();
            FileSystem = new ExtendedPhysicalFileSystem(AppDirectory);
            CommandLineWrapper = new CommandLineWrapperWrapper();
            ProcessWrapper = new ProcessWrapperWrapper();
        }
    }
}