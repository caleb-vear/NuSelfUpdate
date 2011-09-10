using NuGet;

namespace NuSelfUpdate
{
    public interface IExtendedFileSystem : IFileSystem
    {
        void MoveFile(string sourcePath, string destinationPath);
    }
}