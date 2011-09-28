using Bddify;
using Bddify.Core;
using NUnit.Framework;
using NuSelfUpdate.Tests.AppUpdaterBehaviour.LaunchInstalledUpdateScenarios;

namespace NuSelfUpdate.Tests.AppUpdaterBehaviour
{
    [TestFixture, Story(
        AsA = "As an application",
        IWant = "I want to run an updated version of my self after installing an update",
        SoThat = "So that my users can make use of the nice new features and bug fixes in the new version.")]
    public class LaunchInstalledUpdate
    {
        [Test]
        public void NoCommandLineArguments()
        {
            new NoCommandLineArguments().Bddify();
        }

        [Test]
        public void CommandLineArgumentsWithoutSpaces()
        {
            new CommandLineArgumentsWithoutSpaces().Bddify();
        }

        [Test]
        public void CommandLineArgumentsWithSpaces()
        {
            new CommandLineArgumentsWithSpaces().Bddify();
        }

        [Test]
        public void ExePathContainsSpaces()
        {
            new ExePathContainsSpaces().Bddify();
        }

        [Test]
        public void InstalledUpdateIsNull()
        {
            new InstalledUpdateIsNull().Bddify();
        }
    }
}