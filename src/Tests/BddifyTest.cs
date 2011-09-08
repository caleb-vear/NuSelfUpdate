using Bddify;
using NUnit.Framework;
using System;
using NuSelfUpdate.Tests.Helpers;
using Shouldly;

namespace NuSelfUpdate.Tests
{   
    [TestFixture]
    public abstract class BddifyTest
    {
        [Test]
        public void VerifyBehaviour()
        {
            this.Bddify();
        }
    }
}