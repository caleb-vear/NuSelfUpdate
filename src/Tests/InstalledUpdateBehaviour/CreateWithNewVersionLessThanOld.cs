using System;
using NuSelfUpdate.Tests.Helpers;
using Shouldly;

namespace NuSelfUpdate.Tests.InstalledUpdateBehaviour
{
    public class CreateWithNewVersionLessThanOld : BddifyTest
    {
        Version _oldVersion;
        Version _newVersion;
        Exception _exception;

        void GivenAnOldVersion()
        {
            _oldVersion = new Version(1, 0);
        }

        void AndGivenANewVersionThatIsLessThanTheOld()
        {
            _newVersion = new Version(0, 9);
        }

        void WhenAttemptingToCreateAnInstalledUpdateWithThoseVersions()
        {
            _exception = Run.CatchingException(() => new InstalledUpdate(_oldVersion, _newVersion));
        }

        void ThenABackwardUpdateExceptionWillBeThrown()
        {
            _exception.ShouldNotBe(null);
            _exception.ShouldBeTypeOf<BackwardUpdateException>();
            var backwardUpdateEx = (BackwardUpdateException)_exception;
            backwardUpdateEx.InstalledVersion.ShouldBe(_oldVersion);
            backwardUpdateEx.TargetVersion.ShouldBe(_newVersion);
        }
    }
}