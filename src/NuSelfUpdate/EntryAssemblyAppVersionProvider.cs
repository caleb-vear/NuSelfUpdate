using System;
using System.Reflection;

namespace NuSelfUpdate
{
    public class EntryAssemblyAppVersionProvider : IAppVersionProvider
    {
        public Version CurrentVersion
        {
            get { return Assembly.GetEntryAssembly().GetName().Version; }
        }
    }
}