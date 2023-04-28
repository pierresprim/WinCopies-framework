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

using WinCopies.Util.Data;

namespace WinCopies.Installer
{
    public class ActionsViewModel : ViewModel<OnlineInstallerViewModel>
    {
        public Actions Actions { get => ModelGeneric.Actions; set { ModelGeneric.Actions = value; OnPropertyChanged(nameof(Actions)); } }

        public ActionsViewModel(in OnlineInstallerViewModel model) : base(model) { /* Left empty. */ }
    }

    public class TemporaryDestinationViewModel : ViewModel<OnlineInstallerViewModel>
    {
        public IInstaller Installer => ModelGeneric;

        public string
#if CS8
            ?
#endif
            TemporaryDirectory
        { get => ModelGeneric.TemporaryDirectory; set { ModelGeneric.TemporaryDirectory = value; OnPropertyChanged(nameof(TemporaryDirectory)); } }

        public TemporaryDestinationViewModel(in OnlineInstallerViewModel model) : base(model) { /* Left empty. */ }
    }

    public class OnlineInstallerViewModel : InstallerViewModel
    {
        public OnlineInstallerViewModel(in Installer model) : base(model) { /* Left empty. */ }

        protected override ExtraData GetExtraData() => new
#if !CS9
            ExtraData
#endif
            (new ActionsViewModel(this), new TemporaryDestinationViewModel(this));
    }
}
