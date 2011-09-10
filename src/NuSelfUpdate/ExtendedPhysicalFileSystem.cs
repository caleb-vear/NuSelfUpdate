using System.IO;
using NuGet;

namespace NuSelfUpdate
{
    public class ExtendedPhysicalFileSystem : PhysicalFileSystem, IExtendedFileSystem
    {
        public ExtendedPhysicalFileSystem(string root)
            : base(root)
        {
        }

        public void MoveFile(string sourcePath, string destinationPath)
        {
            EnsureDirectory(Path.GetDirectoryName(destinationPath));

            File.Move(sourcePath, destinationPath);
        }
    }
}