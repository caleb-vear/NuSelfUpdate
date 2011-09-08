using System;

namespace NuSelfUpdate
{
    public class BackwardUpdateException : Exception
    {
        public Version InstalledVersion { get; private set; }
        public Version TargetVersion { get; private set; }

        public BackwardUpdateException(Version installedVersion, Version targetVersion)
            : base(string.Format("Can only update to a newer version. Installed Version: {0}, Update Version: {1}", installedVersion, targetVersion))
        {
            InstalledVersion = installedVersion;
            TargetVersion = targetVersion;
        }
    }
}