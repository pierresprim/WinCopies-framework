namespace WinCopies.Installer
{
    public interface IBrowsableForwardInstallerPage<out T> : IInstallerPage where T : IInstallerPage
    {
        T NextPage { get; }
    }

    public interface IBrowsableForwardCommonPage<out T> : IBrowsableForwardInstallerPage<T>, ICommonPage where T : IInstallerPage
    {
        // Left empty.
    }

    public interface IBrowsableForwardCommonPage<out TNext, out TData> : IBrowsableForwardInstallerPage<TNext>, ICommonPage<TData> where TNext : IInstallerPage where TData : IInstallerPageData
    {
        // Left empty.
    }

    public interface IBrowsableInstallerPage<out TPrevious, out TNext> : IBrowsableForwardInstallerPage<TNext> where TPrevious : IInstallerPage where TNext : IInstallerPage
    {
        TPrevious PreviousPage { get; }
    }

    public interface IBrowsableCommonPage<out TPrevious, out TNext> : IBrowsableForwardCommonPage<TNext>, IBrowsableInstallerPage<TPrevious, TNext> where TPrevious : IInstallerPage where TNext : IInstallerPage
    {
        // Left empty.
    }

    public interface IBrowsableCommonPage<out TPrevious, out TNext, out TData> : IBrowsableForwardCommonPage<TNext, TData>, IBrowsableCommonPage<TPrevious, TNext> where TPrevious : IInstallerPage where TNext : IInstallerPage where TData : IInstallerPageData
    {
        // Left empty.
    }
}
