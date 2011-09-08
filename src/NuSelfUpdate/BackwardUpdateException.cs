using System;

namespace NuSelfUpdate
{
    public class BackwardUpdateException : Exception
    {
        public BackwardUpdateException(Version installedVersion, Version targetVersion)
            : base(string.Format("Can only update to a newer version. Installed Version: {0}, Update Version: {1}", installedVersion, targetVersion))
        {

        }
    }
}