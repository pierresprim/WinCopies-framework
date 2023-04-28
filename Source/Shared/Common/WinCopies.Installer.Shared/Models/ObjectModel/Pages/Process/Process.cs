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
    public interface IProcessPage : IBrowsableForwardCommonPage<IEndPage, IProcessData>
    {
        // Left empty.
    }

    public abstract class ProcessPage : CommonInstallerPageBase<IEndPage, IProcessData>, IProcessPage
    {
        public override string Title => "Installing...";

        public override string Description => $"Please wait while {Installer.ProgramName} is being installed on your computer.";

        protected ProcessPage(in Installer installer) : base(installer) { /* Left empty. */ }
    }
}
