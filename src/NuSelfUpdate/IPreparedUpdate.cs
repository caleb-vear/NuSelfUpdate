using System;
using System.Collections.Generic;

namespace NuSelfUpdate
{
    public interface IPreparedUpdate
    {
        Version Version { get; }
        IEnumerable<string> Files { get; }        
    }
}