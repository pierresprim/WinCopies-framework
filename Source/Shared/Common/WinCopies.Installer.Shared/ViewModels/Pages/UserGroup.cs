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
    public class UserGroupPageViewModel : CommonPageViewModel3<ILicenseAgreementPage, IUserGroupPage, IDestinationPage, IUserGroupData>, IUserGroupPage
    {
        protected class UserGroupDataViewModel : InstallerPageDataViewModel<IUserGroupData>, IUserGroupData
        {
            public bool InstallForCurrentUserOnly
            {
                get => ModelGeneric.InstallForCurrentUserOnly;

                set
                {
                    ModelGeneric.InstallForCurrentUserOnly = value;

                    OnInstallerPropertyChanged(nameof(InstallForCurrentUserOnly));
                }
            }

            public object
#if CS8
                ?
#endif
                ExtraData => Installer.ExtraData?.UserGroup;

            public UserGroupDataViewModel(in UserGroupPageViewModel userGroupPage) : base(userGroupPage.ModelGeneric.Data, userGroupPage.Installer) { /* Left empty. */ }
        }

        internal UserGroupPageViewModel(in IUserGroupPage userGroupPage, in InstallerViewModel installer) : base(userGroupPage, installer) { /* Left empty. */ }

        public override IUserGroupData GetData() => new UserGroupDataViewModel(this);

        public override IInstallerPageViewModel GetPrevious() => new LicenseAgreementPageViewModel(ModelGeneric.PreviousPage, Installer);
        public override IInstallerPageViewModel GetNext() => new DestinationPageViewModel(ModelGeneric.NextPage, Installer);
    }
}
