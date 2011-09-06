using System;

namespace NuSelfUpdate
{
    public interface IVersionLocator
    {
        Version CurrentVersion { get; }
    }
}