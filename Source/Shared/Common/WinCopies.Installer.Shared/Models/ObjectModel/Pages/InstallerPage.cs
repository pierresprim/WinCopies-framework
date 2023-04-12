using System.Windows.Media;

namespace WinCopies.Installer
{
    public interface IInstallerPage
    {
        IInstaller Installer { get; }

        string Title { get; }

        string Description { get; }

        string NextStepName { get; }

        ImageSource ImageSource { get; }
    }

    public interface IInstallerPageData
    {
        IInstaller Installer { get; }

        string
#if CS8
            ?
#endif
            Error
        { get; }
    }

    public abstract class InstallerPage : IInstallerPage
    {
        public Installer Installer { get; }

        IInstaller IInstallerPage.Installer => Installer;

        public abstract string Title { get; }

        public abstract string Description { get; }

        public virtual string NextStepName => "Next";

        public abstract ImageSource ImageSource { get; }

        protected InstallerPage(in Installer installer) => Installer = installer;
    }

    public abstract class InstallerPage<T> : InstallerPage where T : IInstallerPage
    {
        private bool _nextPageLoaded = false;

        private T
#if CS9
            ?
#endif
            _nextPage;

        public T
#if CS9
            ?
#endif
            NextPage
        {
            get
            {
                if (_nextPageLoaded) return _nextPage;

                _nextPageLoaded = true;

                return _nextPage = GetNextPage();
            }
        }

        protected InstallerPage(in Installer installer) : base(installer) { }

        protected abstract T
#if CS9
            ?
#endif
            GetNextPage();
    }

    public abstract class InstallerPageData : IInstallerPageData
    {
        public Installer Installer { get; }

        IInstaller IInstallerPageData.Installer => Installer;

        public virtual string
#if CS8
            ?
#endif
            Error => null;

        protected InstallerPageData(in Installer installer) => Installer = installer;
    }
}
