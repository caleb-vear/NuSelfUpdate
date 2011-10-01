using System;

namespace NuSelfUpdate
{
    public class CommandLineWrapper : ICommandLineWrapper
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