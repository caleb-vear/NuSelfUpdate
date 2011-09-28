using System;

namespace NuSelfUpdate
{
    public class InstalledUpdate
    {
        public Version OldVersion { get; private set; }
        public Version NewVersion { get; private set; }

        public InstalledUpdate(Version old, Version newVersion)
        {
            if (old >= newVersion)
                throw new BackwardUpdateException(old, newVersion);

            OldVersion = old;
            NewVersion = newVersion;
        }

        public override string ToString()
        {
            return "Installed Update: " + OldVersion + " -> " + NewVersion;
        }
    }
}