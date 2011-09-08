using System.Collections.Generic;
using System.Linq;
using NSubstitute;
using NuGet;

namespace NuSelfUpdate.Tests.Helpers
{
    public static class PackageRepositoryFactories
    {
        public static IPackageRepositoryFactory Create(IEnumerable<IPackage> packages)
        {
            var packageRepositoryFactory = Substitute.For<IPackageRepositoryFactory>();

            packageRepositoryFactory
                .CreateRepository("repository")
                .GetPackages()
                .Returns(packages.AsQueryable());
            return packageRepositoryFactory;
        }
    }
}