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
    public interface IDestinationData : IInstallerPageData
    {
        string Location { get; set; }
    }

    public interface IDestinationPage : IBrowsableCommonPage<IUserGroupPage, IOptionsPage, IDestinationData>
    {
        // Left empty.
    }

    public abstract class DestinationPage : CommonInstallerPage<UserGroupPage, IOptionsPage, IDestinationData>, IDestinationPage
    {
        protected class DestinationData : InstallerPageData, IDestinationData
        {
            public string Location { get => Installer.Location; set => Installer.Location = value; }

            public DestinationData(in Installer installer) : base(installer) { /* Left empty. */ }
        }

        public override string Title => "Destination directory";

        public override string Description => $"Please choose the directory to which to install {Installer.ProgramName}.";

        IUserGroupPage IBrowsableInstallerPage<IUserGroupPage, IOptionsPage>.PreviousPage => PreviousPage;

        protected DestinationPage(in UserGroupPage previousPage) : base(previousPage) { /* Left empty. */ }

        protected override IDestinationData GetData() => new DestinationData(Installer);
    }
}
