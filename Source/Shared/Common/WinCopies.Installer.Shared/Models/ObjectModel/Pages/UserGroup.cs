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
    public interface IUserGroupData : IInstallerPageData
    {
        bool InstallForCurrentUserOnly { get; set; }
    }

    public interface IUserGroupPage : IBrowsableCommonPage<ILicenseAgreementPage, IDestinationPage, IUserGroupData>
    {
        // Left empty.
    }

    public abstract class UserGroupPage : CommonInstallerPage<LicenseAgreementPage, IDestinationPage, IUserGroupData>, IUserGroupPage
    {
        protected class UserGroupData : InstallerPageData, IUserGroupData
        {
            public bool InstallForCurrentUserOnly { get => Installer.InstallForCurrentUserOnly; set => Installer.InstallForCurrentUserOnly = value; }

            public UserGroupData(in Installer installer) : base(installer) { /* Left empty. */ }
        }

        public override string Title => "User Group";

        public override string Description => $"Choose between the two user groups the one {Installer.ProgramName} will be installed for.";

        ILicenseAgreementPage IBrowsableInstallerPage<ILicenseAgreementPage, IDestinationPage>.PreviousPage => PreviousPage;

        protected UserGroupPage(in LicenseAgreementPage previousPage) : base(previousPage) { /* Left empty. */ }

        protected override IUserGroupData GetData() => new UserGroupData(Installer);
    }
}
