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

using System.Windows;
using System.Windows.Documents;

namespace WinCopies.Installer
{
    public interface ILicenseAgreementData : IInstallerPageData
    {
        DataFormat DataFormat { get; }

        System.IO.Stream GetText();
    }

    public interface ILicenseAgreementDataViewModel : ILicenseAgreementData
    {
        FlowDocument Document { get; }
    }

    public interface ILicenseAgreementPage : IBrowsableCommonPage<IStartPage, IUserGroupPage, ILicenseAgreementData>
    {
        // Left empty.
    }

    public abstract class LicenseAgreementPage : CommonInstallerPage<StartPage, IUserGroupPage, ILicenseAgreementData>, ILicenseAgreementPage
    {
        public override string Title => "License Agreement";

        public override string Description => $"Please read the terms of the license of {Installer.ProgramName} before starting the installation.";

        public override string NextStepName => "I _Agree";

        IStartPage IBrowsableInstallerPage<IStartPage, IUserGroupPage>.PreviousPage => PreviousPage;

        protected LicenseAgreementPage(in StartPage previousPage) : base(previousPage) { /* Left empty. */ }
    }
}
