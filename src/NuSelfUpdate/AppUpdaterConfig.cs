using System.IO;
using System.Reflection;
using NuGet;

namespace NuSelfUpdate
{
    public class AppUpdaterConfig
    {
        public string PackageSource { get; set; }
        public string AppPackageId { get; set; }
        public IPackageRepositoryFactory PackageRepositoryFactory { get; set; }
        public IAppVersionProvider AppVersionProvider { get; set; }
        public IPrepDirectoryStrategy UpdatePrepDirectoryStrategy { get; set; }
        public IExtendedFileSystem FileSystem { get; set; }
        public virtual string AppDirectory
        {
            get
            {
                return Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            }
        }
    }
}