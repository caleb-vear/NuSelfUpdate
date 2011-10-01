using System.Diagnostics;

namespace NuSelfUpdate
{
    public class ProcessWrapper : IProcessWrapper
    {
        public void Start(string fileName, string arguments)
        {
            if (string.IsNullOrEmpty(arguments))
                Process.Start(fileName);

            Process.Start(fileName, arguments);
        }
    }
}