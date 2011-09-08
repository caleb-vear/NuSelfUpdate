using System;
using System.Collections.Generic;
using System.Linq;
using NSubstitute;
using NuGet;

namespace NuSelfUpdate.Tests.Helpers
{
    public static class Packages
    {
        public static IEnumerable<IPackage> FromVersions(string packageId, params Version[] versions)
        {
            var maxVersion = versions.Max();

            var packages = versions.Select(v =>
                                               {
                                                   var package = Substitute.For<IPackage>();
                                                   package.Id.Returns(packageId);
                                                   package.Version.Returns(v);
                                                   package.IsLatestVersion.Returns(v == maxVersion);
                                                   return package;
                                               });

            return packages.ToArray();
        }
    }
}