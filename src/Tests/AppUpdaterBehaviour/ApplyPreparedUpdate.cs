using Bddify;
using Bddify.Core;
using NUnit.Framework;
using NuSelfUpdate.Tests.AppUpdaterBehaviour.ApplyPreparedUpdateScenarios;

namespace NuSelfUpdate.Tests.AppUpdaterBehaviour
{
    [TestFixture, Story(
        AsA = "As an application",
        IWant = "I want to apply a prepared update",
        SoThat = "So that I can give my users sweet new features and fix bugs.")]
    public class ApplyPreparedUpdate
    {
        [Test]
        public void PreparedUpdateIsANewerVerion()
        {
            new PreparedUpdateIsANewerVerion().Bddify();
        }

        [Test]
        public void PreparedUpdateIsForAnOlderAppVersion()
        {
            new PreparedUpdateIsForAnOlderAppVersion().Bddify();
        }

        [Test]
        public void PreparedUpdateIsForInstalledAppVersion()
        {
            new PreparedUpdateIsForInstalledAppVersion().Bddify();
        }

        [Test]
        public void TheLastOldVersionHasNotBeenCleanedUp()
        {
            new TheLastOldVersionHasNotBeenCleanedUp().Bddify();
        }
    }
}