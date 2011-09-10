using System;
using System.Collections.Generic;

namespace NuSelfUpdate
{
    public class PreparedUpdate : IPreparedUpdate
    {
        public Version Version { get; private set; }
        public IEnumerable<string> Files { get; private set; }

        public PreparedUpdate(Version version, IEnumerable<string> files)
        {
            Version = version;
            Files = files;
        }
    }
}