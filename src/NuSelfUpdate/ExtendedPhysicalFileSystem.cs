using System;
using System.IO;
using System.Reflection;
using NuGet;

namespace NuSelfUpdate
{
    public class ExtendedPhysicalFileSystem : PhysicalFileSystem, IExtendedFileSystem
    {
        public ExtendedPhysicalFileSystem(string applicationDirectory)
            : base(applicationDirectory)
        {
        }

        public string AppDirectory
        {
            get { return Root; }
        }

        public void MoveFile(string sourcePath, string destinationPath)
        {
            EnsureDirectory(Path.GetDirectoryName(destinationPath));

            File.Move(sourcePath, destinationPath);
        }
    }
}