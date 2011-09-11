using System;
using NuGet;

namespace NuSelfUpdate
{
    public class UpdateFound : IUpdateCheck
    {
        public bool UpdateAvailable { get { return UpdatePackage != null; } }

        public IPackage UpdatePackage { get; private set; }

        public UpdateFound(IPackage updatePackage)
        {
            UpdatePackage = updatePackage;
        }

        public override string ToString()
        {
            return "UpdateCheck: v" + UpdatePackage.Version + " update found";
        }
    }
}