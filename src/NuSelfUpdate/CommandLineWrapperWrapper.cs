using System;

namespace NuSelfUpdate
{
    public class CommandLineWrapperWrapper : ICommandLineWrapper
    {
        public string Full
        {
            get { return Environment.CommandLine; }
        }

        public string[] Arguments
        {
            get { return Environment.GetCommandLineArgs(); }
        }
    }
}