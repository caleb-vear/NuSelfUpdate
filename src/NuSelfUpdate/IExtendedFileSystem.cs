using NuGet;

namespace NuSelfUpdate
{
    public interface IExtendedFileSystem : IFileSystem
    {
        string AppDirectory { get; }
        void MoveFile(string sourcePath, string destinationPath);
    }
}