using Bddify;
using Bddify.Core;
using NUnit.Framework;
using NuSelfUpdate.Tests.AppUpdaterBehaviour.RemoveOldVersionFilesScenarios;

namespace NuSelfUpdate.Tests.AppUpdaterBehaviour
{
    [TestFixture, Story(
        AsA = "As an application",
        IWant = "I want to delete previous versions of my self",
        SoThat = "I do not fill my users computer with cruft")]
    public class RemoveOldVersionFiles
    {
        [Test]
        public void OldFilesExist()
        {
            new OldFilesExist().Bddify();
        }

        [Test]
        public void OldDirectoryExistsButDoesNotContainFiles()
        {
            new OldDirectoryExistsButDoesNotContainFiles().Bddify();
        }

        [Test]
        public void OldFilesDoNotExist()
        {
            new OldFilesDoNotExist().Bddify();
        }
    }
}