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

namespace WinCopies.Installer
{
    public abstract class CommonInstallerPageBase<TNext, TData> : InstallerPage<TNext>, ICommonPage where TNext : IInstallerPage where TData : IInstallerPageData
    {
        public TData Data { get; }

        IInstallerPageData ICommonPage.Data => Data;

        protected CommonInstallerPageBase(in Installer installer) : base(installer) => Data = GetData();

        protected abstract TData GetData();
    }

    public abstract class CommonInstallerPage<TPrevious, TNext, TData> : CommonInstallerPageBase<TNext, TData> where TPrevious : InstallerPage where TNext : IInstallerPage where TData : IInstallerPageData
    {
        public TPrevious PreviousPage { get; }

        protected CommonInstallerPage(in TPrevious previousPage) : base(previousPage.Installer) => PreviousPage = previousPage;
    }

    public interface ICommonPage : IInstallerPage
    {
        IInstallerPageData Data { get; }

        Icon Icon { get; }
    }

    public interface ICommonPage<out T> : ICommonPage where T : IInstallerPageData
    {
        new T Data { get; }
#if CS8
        IInstallerPageData ICommonPage.Data => Data;
#endif
    }
}
