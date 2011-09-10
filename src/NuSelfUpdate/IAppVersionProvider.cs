using System;

namespace NuSelfUpdate
{
    public interface IAppVersionProvider
    {
        Version CurrentVersion { get; }
    }
}