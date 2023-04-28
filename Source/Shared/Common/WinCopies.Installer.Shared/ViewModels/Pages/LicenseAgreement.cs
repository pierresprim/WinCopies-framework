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
    public class LicenseAgreementPageViewModel : CommonPageViewModel3<IStartPage, ILicenseAgreementPage, IUserGroupPage, ILicenseAgreementData>, ILicenseAgreementPage
    {
        protected class LicenseAgreementDataViewModel : InstallerPageDataViewModel<ILicenseAgreementData>, ILicenseAgreementDataViewModel
        {
            private FlowDocument _document;

            public DataFormat DataFormat => ModelGeneric.DataFormat;

            public FlowDocument Document
            {
                get
                {
                    if (_document == null)
                    {
                        _document = new FlowDocument();

                        using
#if !CS8
                            (
#endif
                            System.IO.Stream s = GetText()
#if CS8
                            ;
#else
                            )
#endif
                        new TextRange(_document.ContentStart, _document.ContentEnd).Load(s, DataFormat.Name);
                    }

                    return _document;
                }
            }

            public LicenseAgreementDataViewModel(in LicenseAgreementPageViewModel licenseAgreementPage) : base(licenseAgreementPage.ModelGeneric.Data, licenseAgreementPage.Installer) { /* Left empty. */ }

            public System.IO.Stream GetText() => ModelGeneric.GetText();
        }

        internal LicenseAgreementPageViewModel(in ILicenseAgreementPage licenseAgreementPage, in InstallerViewModel installer) : base(licenseAgreementPage, installer) { /* Left empty. */ }

        public override ILicenseAgreementData GetData() => new LicenseAgreementDataViewModel(this);

        public override IInstallerPageViewModel GetPrevious() => new StartPageViewModel(ModelGeneric.PreviousPage, Installer);
        public override IInstallerPageViewModel GetNext() => new UserGroupPageViewModel(ModelGeneric.NextPage, Installer);
    }
}
