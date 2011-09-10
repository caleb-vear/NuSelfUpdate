using System;

namespace NuSelfUpdate
{
    public class InstalledUpdate
    {
        public Version OldVersion { get; private set; }
        public Version NewVersion { get; private set; }

        public InstalledUpdate(Version old, Version newVersion)
        {
            OldVersion = old;
            NewVersion = newVersion;
        }
    }
}