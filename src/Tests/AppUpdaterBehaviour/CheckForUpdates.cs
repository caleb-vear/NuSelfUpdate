using Bddify;
using Bddify.Core;
using NUnit.Framework;
using NuSelfUpdate.Tests.AppUpdaterBehaviour.CheckForUpdateScenarios;

namespace NuSelfUpdate.Tests.AppUpdaterBehaviour
{
    [TestFixture, Story(
        AsA = "As an application",
        IWant = "I want to check and see if there are new updates available",
        SoThat = "I can start the process of updating my self")]
    public class CheckForUpdates
    {
        [Test]
        public void NoPackagesHaveEverBeenPublished()
        {
            new NoPackagesHaveEverBeenPublished().Bddify();
        }

        [Test]
        public void NoUpdatesAvailable()
        {
            new NoUpdatesAvailable().Bddify();
        }

        [Test]
        public void ASingleUpdateIsAvailable()
        {
            new ASinlgeUpdateIsAvailable().Bddify();
        }

        [Test]
        public void MultipleUpdatesAreAvailable()
        {
            new MultipleUpdatesAreAvailable().Bddify();
        }
    }
}