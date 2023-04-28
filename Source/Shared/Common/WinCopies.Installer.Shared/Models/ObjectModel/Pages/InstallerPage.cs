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

using System.Drawing;
using System.Windows.Media;

namespace WinCopies.Installer
{
    public interface IInstallerPage
    {
        IInstaller Installer { get; }

        string Title { get; }

        string Description { get; }

        string NextStepName { get; }

        ImageSource ImageSource { get; }
    }

    public interface IInstallerPageData
    {
        IInstaller Installer { get; }

        string
#if CS8
            ?
#endif
            Error
        { get; }
    }

    public abstract class InstallerPage : IInstallerPage
    {
        public Installer Installer { get; }

        IInstaller IInstallerPage.Installer => Installer;

        public abstract string Title { get; }

        public abstract string Description { get; }

        public abstract Icon Icon { get; }

        public virtual string NextStepName => "_Next";

        public abstract ImageSource ImageSource { get; }

        protected InstallerPage(in Installer installer) => Installer = installer;
    }

    public abstract class InstallerPage<T> : InstallerPage where T : IInstallerPage
    {
        private bool _nextPageLoaded = false;

        private T
#if CS9
            ?
#endif
            _nextPage;

        public T
#if CS9
            ?
#endif
            NextPage
        {
            get
            {
                if (_nextPageLoaded) 
                    
                    return _nextPage;

                _nextPageLoaded = true;

                return _nextPage = GetNextPage();
            }
        }

        protected InstallerPage(in Installer installer) : base(installer) { }

        protected abstract T
#if CS9
            ?
#endif
            GetNextPage();
    }

    public abstract class InstallerPageData : IInstallerPageData
    {
        public Installer Installer { get; }

        IInstaller IInstallerPageData.Installer => Installer;

        public virtual string
#if CS8
            ?
#endif
            Error => null;

        protected InstallerPageData(in Installer installer) => Installer = installer;
    }
}
