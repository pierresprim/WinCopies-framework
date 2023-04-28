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

namespace WinCopies.Installer
{
    public class DestinationPageViewModel : CommonPageViewModel3<IUserGroupPage, IDestinationPage, IOptionsPage, IDestinationData>, IDestinationPage
    {
        protected class DestinationDataViewModel : InstallerPageDataViewModel<IDestinationData>, IDestinationData
        {
            public string Location
            {
                get => ModelGeneric.Location;

                set
                {
                    ModelGeneric.Location = value;

                    OnInstallerPropertyChanged(nameof(Location));
                }
            }

            public DestinationDataViewModel(in DestinationPageViewModel destinationPage) : base(destinationPage.ModelGeneric.Data, destinationPage.Installer) { /* Left empty. */ }
        }

        internal DestinationPageViewModel(in IDestinationPage destinationPage, in InstallerViewModel installer) : base(destinationPage, installer) { /* Left empty. */ }

        public override IDestinationData GetData() => new DestinationDataViewModel(this);

        public override IInstallerPageViewModel GetPrevious() => new UserGroupPageViewModel(ModelGeneric.PreviousPage, Installer);
        public override IInstallerPageViewModel GetNext() => Installer.OptionsPage
#if CS8
            ??=
#else
            ?? (Installer.OptionsPage =
#endif
            new OptionsPageViewModel(ModelGeneric.NextPage, Installer)
#if !CS8
            )
#endif
            ;
    }
}
