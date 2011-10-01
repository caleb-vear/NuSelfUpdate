using NuGet;

namespace NuSelfUpdate
{
    public class NuGetConfig
    {
        public string PackageSource { get; set; }
        public string AppPackageId { get; set; }
        public IPackageRepositoryFactory RepositoryFactory { get; set; }
    }
}