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

namespace WinCopies.Installer
{
    public class StartPageViewModel : CommonPageViewModel<IStartPage>, IStartPage
    {
        public sealed override bool CanBrowseBack => false;

        public ILicenseAgreementPage NextPage => ModelGeneric.NextPage;

        public StartPageViewModel(in InstallerViewModel installer) : base(installer.StartPage, installer) { /* Left empty. */ }
        internal StartPageViewModel(in IStartPage startPage, in InstallerViewModel installer) : base(startPage, installer) { /* Left empty. */ }

        public sealed override IInstallerPageViewModel GetPrevious() => throw new InvalidOperationException();

        public sealed override IInstallerPageViewModel GetNext() => new LicenseAgreementPageViewModel(ModelGeneric.NextPage, Installer);
    }

    public class EndPageViewModel : CommonPageViewModel<IEndPage>, IEndPage
    {
        public override bool CanBrowseBack => false;
        public override bool CanBrowseForward => false;
        public override bool CanCancel => false;

        internal EndPageViewModel(in IEndPage installerPage, in InstallerViewModel installer) : base(installerPage, installer) { /* Left empty. */ }

        public override IInstallerPageViewModel GetPrevious() => throw new InvalidOperationException();
        public override IInstallerPageViewModel GetNext() => throw new InvalidOperationException();
    }
}
