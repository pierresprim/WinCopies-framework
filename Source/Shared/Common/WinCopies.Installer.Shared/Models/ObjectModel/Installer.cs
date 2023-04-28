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

using System.Collections.Generic;

using WinCopies.Util;

namespace WinCopies.Installer
{
    public interface IInstaller
    {
        string ProgramName { get; }

        bool Is32Bit { get; }

        bool InstallForCurrentUserOnly { get; }

        string
#if CS8
            ?
#endif
            Location
        { get; }

        string
#if CS8
           ?
#endif
           TemporaryDirectory
        { get; }

        Actions Actions { get; }

        IEnumerable<KeyValuePair<string, IFile>>
#if CS8
            ?
#endif
            Files
        { get; }

        bool Completed { get; }

        bool RequiresRestart { get; }

        IStartPage
#if CS8
            ?
#endif
            StartPage
        { get; }

        IOptionsData
#if CS8
            ?
#endif
            Options
        { get; }

        Error Error { get; }

        IInstallerPage
#if CS8
            ?
#endif
            Current
        { get; set; }

        ExtraData? ExtraData { get; }
    }

    public abstract class Installer : IInstaller
    {
        private byte _bools;
        private IStartPage
#if CS8
            ?
#endif
            _startPage;

        public abstract string ProgramName { get; }

        public abstract bool Is32Bit { get; }

        public bool InstallForCurrentUserOnly { get => GetBit(0); internal set => SetBit(0, value); }

        public string
#if CS8
            ?
#endif
            Location
        { get; internal set; }

        public string
#if CS8
            ?
#endif
            TemporaryDirectory
        { get; internal set; }

        public Actions Actions { get; internal set; }

        public IEnumerable<KeyValuePair<string, IFile>>
#if CS8
            ?
#endif
            Files
        { get; internal set; }

        public bool Completed { get => GetBit(1); private set => SetBit(1, value); }

        public IStartPage
#if CS8
            ?
#endif
            StartPage => _startPage
#if CS8
            ??=
#else
            ?? (_startPage =
#endif
            GetStartPage()
#if !CS8
                )
#endif
            ;

        public IInstallerPage
#if CS8
            ?
#endif
            Current
        { get; set; }

        public IOptionsData
#if CS8
            ?
#endif
            Options
        { get; internal set; }

        public Error Error { get; internal set; }

        public abstract bool RequiresRestart { get; }

        ExtraData? IInstaller.ExtraData => null;

        public Installer() { /* Left empty. */ }

        private bool GetBit(in byte pos) => _bools.GetBit(pos);
        private void SetBit(in byte pos, in bool value) => UtilHelpers.SetBit(ref _bools, pos, value);

        internal void MarkAsCompleted() => Completed = true;

        protected abstract IStartPage
#if CS8
            ?
#endif
            GetStartPage();
    }
}
