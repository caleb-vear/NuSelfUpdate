using System.Diagnostics;

namespace NuSelfUpdate
{
    public class ProcessWrapperWrapper : IProcessWrapper
    {
        public void Start(string fileName, string arguments)
        {
            if (string.IsNullOrEmpty(arguments))
                Process.Start(fileName);

            Process.Start(fileName, arguments);
        }
    }
}