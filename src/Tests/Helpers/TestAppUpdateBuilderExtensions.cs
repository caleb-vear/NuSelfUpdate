using System;
using System.Collections.Generic;
using System.Linq;
using NSubstitute;
using NuGet;

namespace NuSelfUpdate.Tests.Helpers
{
    public static class TestAppUpdateBuilderExtensions
    {
        public static AppUpdaterBuilder SetupWithTestValues(this AppUpdaterBuilder builder, Version installedVersion)
        {
            var versionProvider = Substitute.For<IAppVersionProvider>();
            versionProvider.CurrentVersion.Returns(installedVersion);

            return builder
                .SourceUpdatesFrom("repository")
                .CreatePackageRepositoriesWith(PackageRepositoryFactories.Create(Enumerable.Empty<IPackage>()))
                .CurrentVersionProvidedBy(versionProvider)
                .FileSystemAccessedThrough(new MockFileSystem(TestConstants.AppDirectory))
                .CommandLineInfoProvidedBy(Substitute.For<ICommandLineWrapper>())
                .RelaunchAppWith(Substitute.For<IProcessWrapper>());
        }

        public static AppUpdaterBuilder SetPublishedPackages(this AppUpdaterBuilder builder, IEnumerable<IPackage> publishedPackages)
        {
            return builder.CreatePackageRepositoriesWith(PackageRepositoryFactories.Create(publishedPackages));
        }

        /// <summary>
        /// NOTE: This should only be called once per test as each time you call it you will get a new instance.
        /// </summary>
        public static ICommandLineWrapper GetSubsituteCommandLineWrapper(this AppUpdaterBuilder builder)
        {
            var clw = Substitute.For<ICommandLineWrapper>();
            builder.CommandLineInfoProvidedBy(clw);

            return clw;
        }

        /// <summary>
        /// NOTE: This should only be called once per test as each time you call it you will get a new instance.
        /// </summary>
        public static IProcessWrapper GetSubsituteProcessWrapper(this AppUpdaterBuilder builder)
        {
            var pw = Substitute.For<IProcessWrapper>();
            builder.RelaunchAppWith(pw);

            return pw;
        }

        /// <summary>
        /// NOTE: This should only be called once per test as each time you call it you will get a new instance.
        /// </summary>
        public static MockFileSystem GetMockFileSystem(this AppUpdaterBuilder builder)
        {
            var fileSystem = new MockFileSystem(TestConstants.AppDirectory);
            builder.FileSystemAccessedThrough(fileSystem);

            return fileSystem;
        }
    }
}