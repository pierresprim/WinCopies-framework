/* Copyright © Pierre Sprimont, 2022
 *
 * This file is part of the WinCopies Framework.
 *
 * The WinCopies Framework is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * The WinCopies Framework is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with the WinCopies Framework.  If not, see <https://www.gnu.org/licenses/>. */

using System;
using System.Collections.Generic;
using System.Windows.Media;

using WinCopies.Util.Data;
using static Microsoft.WindowsAPICodePack.COMNative.Shell.PropertySystem.SystemProperties.System;

namespace WinCopies.Installer
{
    public interface IInstallerPageViewModel : IInstallerPage
    {
        bool CanBrowseBack { get; }

        bool CanBrowseForward { get; }

        bool CanCancel { get; }

        IInstallerPageViewModel GetPrevious();

        IInstallerPageViewModel GetNext();

        void MovePrevious();

        void MoveNext();
    }

    public interface IInstallerModel : IInstaller
    {
        new IInstallerPageViewModel Current { get; }
    }

    public readonly struct ExtraData
    {
        public object
#if CS8
                ?
#endif
                UserGroup
        { get; }

        public object
#if CS8
                ?
#endif
                Destination
        { get; }

        public ExtraData(in object userGroup, in object destination)
        {
            UserGroup = userGroup;
            Destination = destination;
        }
    }

    public interface IProcessPageViewModel : IProcessPage, IInstallerPageViewModel
    {
        // Left empty.
    }

    public abstract class InstallerViewModel : ViewModel<Installer>, IInstallerModel
    {
        private IInstallerPageViewModel _current;

        protected internal OptionsPageViewModel OptionsPage { get; internal set; }

        public string ProgramName => ModelGeneric.ProgramName;

        public bool Is32Bit => ModelGeneric.Is32Bit;

        public bool InstallForCurrentUserOnly => ModelGeneric.InstallForCurrentUserOnly;

        public string Location => ModelGeneric.Location;

        public string
#if CS8
            ?
#endif
            TemporaryDirectory
        { get => ModelGeneric.TemporaryDirectory; set => ModelGeneric.TemporaryDirectory = value; }

        public Actions Actions { get => ModelGeneric.Actions; set => ModelGeneric.Actions = value; }

        public bool Completed => ModelGeneric.Completed;

        public IStartPage StartPage => ModelGeneric.StartPage;

        public IInstallerPageViewModel Current
        {
            get => _current;

            protected internal set
            {
                ModelGeneric.Current = value;

                _ = UpdateValue(ref _current, value, nameof(Current));
            }
        }

        IInstallerPage IInstaller.Current { get => Current; set => throw new NotSupportedException(); }

        public IOptionsData Options => ModelGeneric.Options;

        public Error Error => ModelGeneric.Error;

        public bool RequiresRestart => ModelGeneric.RequiresRestart;

        internal new Installer Model => ModelGeneric;

        public ExtraData? ExtraData { get; }

        public IEnumerable<KeyValuePair<string, IFile>>
#if CS8
            ?
#endif
            Files => ModelGeneric.Files;

        public InstallerViewModel(in Installer model) : base(model)
        {
            ModelGeneric.Current = (_current = GetFirstPage());
            ExtraData = GetExtraData();
        }

        protected virtual IInstallerPageViewModel GetFirstPage() => new StartPageViewModel(this);

        protected abstract ExtraData GetExtraData();

        internal new void OnPropertyChanged(in string propertyName) => base.OnPropertyChanged(propertyName);

        protected virtual IProcessPageViewModel GetProcessPageViewModel(IProcessPage processPage) => processPage is ProcessPageViewModel _processPage ? _processPage : new ProcessPageViewModel(processPage, this);
    }

    public class InstallerPageDataViewModel<T> : ViewModel<T>, IInstallerPageData where T : IInstallerPageData
    {
        public InstallerViewModel Installer { get; }

        IInstaller IInstallerPageData.Installer => Installer.Model;

        public string
#if CS8
            ?
#endif
            Error => ModelGeneric.Error;

        public InstallerPageDataViewModel(in T model, in InstallerViewModel installerViewModel) : base(model) => Installer = installerViewModel.Model == model.Installer ? installerViewModel : throw new ArgumentException($"{nameof(installerViewModel)}'s inner installer is not the same as {nameof(model)}'s installer.");

        protected void OnInstallerPropertyChanged(in string propertyName)
        {
            OnPropertyChanged(propertyName);
            Installer.OnPropertyChanged(propertyName);
        }
    }

    public abstract class InstallerPageViewModel<T> : ViewModel<T>, IInstallerPageViewModel where T : IInstallerPage
    {
        public InstallerViewModel Installer { get; }

        public abstract bool CanBrowseBack { get; }
        public abstract bool CanBrowseForward { get; }
        public abstract bool CanCancel { get; }

        public string Title => ModelGeneric.Title;

        public string Description => ModelGeneric.Description;

        public ImageSource ImageSource => ModelGeneric.ImageSource;

        IInstaller IInstallerPage.Installer => Installer.Model;

        public string NextStepName => ModelGeneric.NextStepName;

        protected InstallerPageViewModel(in T installerPage, in InstallerViewModel installer) : base(installerPage) => Installer = installer;

        internal void MarkAsCompleted() => Installer.Model.MarkAsCompleted();

        public abstract IInstallerPageViewModel GetPrevious();
        public abstract IInstallerPageViewModel GetNext();

        public void MovePrevious() => Installer.Current = GetPrevious();
        public void MoveNext() => Installer.Current = GetNext();
    }
}
