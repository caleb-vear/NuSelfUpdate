using NuGet;
namespace NuSelfUpdate
{
    public interface IUpdateCheck
    {
        bool UpdateAvailable { get; }
        IPackage UpdatePackage { get; }
    }
}