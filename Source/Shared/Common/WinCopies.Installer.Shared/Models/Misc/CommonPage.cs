using System.Drawing;

namespace WinCopies.Installer
{
    public abstract class CommonInstallerPageBase<TNext, TData> : InstallerPage<TNext>, ICommonPage where TNext : IInstallerPage where TData : IInstallerPageData
    {
        public abstract Icon Icon { get; }

        public TData Data { get; }

        IInstallerPageData ICommonPage.Data => Data;

        protected CommonInstallerPageBase(in Installer installer) : base(installer) => Data = GetData();

        protected abstract TData GetData();
    }

    public abstract class CommonInstallerPage<TPrevious, TNext, TData> : CommonInstallerPageBase<TNext, TData> where TPrevious : InstallerPage where TNext : IInstallerPage where TData : IInstallerPageData
    {
        public TPrevious PreviousPage { get; }

        protected CommonInstallerPage(in TPrevious previousPage) : base(previousPage.Installer) => PreviousPage = previousPage;
    }

    public interface ICommonPage : IInstallerPage
    {
        IInstallerPageData Data { get; }

        Icon Icon { get; }
    }

    public interface ICommonPage<out T> : ICommonPage where T : IInstallerPageData
    {
        new T Data { get; }

#if CS8
        IInstallerPageData ICommonPage.Data => Data;
#endif
    }
}
