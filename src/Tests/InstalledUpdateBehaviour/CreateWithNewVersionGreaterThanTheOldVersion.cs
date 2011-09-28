using System;
using Shouldly;

namespace NuSelfUpdate.Tests.InstalledUpdateBehaviour
{
    public class CreateWithNewVersionGreaterThanTheOldVersion : BddifyTest
    {
        Version _oldVersion;
        Version _newVersion;
        InstalledUpdate _installedUpdate;

        void GivenAnOldVersion()
        {
            _oldVersion = new Version(1, 0);
        }

        void AndGivenANewVersionThatIsGreaterThanTheOld()
        {
            _newVersion = new Version(2, 0);
        }

        void WhenAttemptingToCreateAnInstalledUpdateWithThoseVersions()
        {
             _installedUpdate = new InstalledUpdate(_oldVersion, _newVersion);
        }

        void ThenTheOldVersionPropertyWillBeCorrect()
        {
            _installedUpdate.OldVersion.ShouldBe(_oldVersion);            
        }            

        void AndTheNewVersionPropertyWillBeCorrect()
        {
            _installedUpdate.NewVersion.ShouldBe(_newVersion);
        }
    }
}