using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using WinCopies.Util;

namespace WinCopies.Installer
{
    public abstract class CommonPageViewModel<T> : InstallerPageViewModel<T> where T : IInstallerPage
    {
        private byte _bools = 0b11;

        public override bool CanBrowseForward => _bools.GetBit(0);

        public override bool CanCancel => _bools.GetBit(1);

        protected CommonPageViewModel(in T installerPage, in InstallerViewModel installer) : base(installerPage, installer) { /* Left empty. */ }

        private void SetBit(in byte pos, in bool value, in string propertyName)
        {
            UtilHelpers.SetBit(ref _bools, pos, value);

            OnPropertyChanged(nameof(propertyName));
        }

        protected void MarkAsBusy()
        {
            void setBit(in byte pos, in string propertyName) => SetBit(pos, false, propertyName);

            setBit(0, nameof(CanBrowseForward));
            setBit(1, nameof(CanCancel));
        }

        protected void MarkAsCompleted()
        {
            SetBit(0, true, nameof(CanBrowseForward));

            base.MarkAsCompleted();
        }
    }

    public abstract class CommonPageViewModel2<TCurrent, TNext, TData> : CommonPageViewModel<TCurrent>, IBrowsableForwardCommonPage<TNext, TData> where TCurrent : IBrowsableForwardCommonPage<TNext, TData> where TNext : IInstallerPage where TData : IInstallerPageData
    {
        public Icon Icon => ModelGeneric.Icon;

        public TData Data { get; }

#if !CS8
        IInstallerPageData ICommonPage.Data => Data;
#endif

        protected CommonPageViewModel2(in TCurrent installerPage, in InstallerViewModel installer) : base(installerPage, installer) => Data = GetData();

        public abstract TData GetData();

        public TNext NextPage => ModelGeneric.NextPage;
    }

    public abstract class CommonPageViewModel3<TPrevious, TCurrent, TNext, TData> : CommonPageViewModel2<TCurrent, TNext, TData>, IBrowsableCommonPage<TPrevious, TNext, TData> where TPrevious : IInstallerPage where TCurrent : IBrowsableCommonPage<TPrevious, TNext, TData> where TNext : IInstallerPage where TData : IInstallerPageData
    {
        public sealed override bool CanBrowseBack => true;

        public TPrevious PreviousPage => ModelGeneric.PreviousPage;

        public CommonPageViewModel3(in TCurrent installerPage, in InstallerViewModel installer) : base(installerPage, installer) { /* Left empty. */ }
    }
}
