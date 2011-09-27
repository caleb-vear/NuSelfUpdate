namespace NuSelfUpdate
{
    public interface ICommandLineWrapper
    {
        string Full { get; }
        string[] Arguments { get; }
    }
}