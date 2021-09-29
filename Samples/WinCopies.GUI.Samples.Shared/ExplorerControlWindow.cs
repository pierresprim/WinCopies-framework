/* Copyright © Pierre Sprimont, 2021
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

using WinCopies.GUI.Shell;

namespace WinCopies.GUI.Samples
{
    public partial class ExplorerControlWindow : BrowsableObjectInfoWindow
    {
        public ExplorerControlWindow() : base(GetDefaultDataContext()) { /* Left empty. */ }

        public static BrowsableObjectInfoWindowViewModel GetDefaultDataContext()
        {
            var dataContext = new BrowsableObjectInfoWindowViewModel();

            dataContext.Paths.IsCheckBoxVisible = true;

            return dataContext;
        }

        protected override BrowsableObjectInfoWindow GetNewBrowsableObjectInfoWindow() => new ExplorerControlWindow();

        protected override void OnAboutWindowRequested() { }

        protected override void OnDelete() { }

        protected override void OnEmpty() { }

        protected override void OnPaste() { }

        protected override void OnQuit() { }

        protected override void OnRecycle() { }

        protected override void OnSubmitABug()
        {
            string url = "https://github.com/pierresprim/WinCopies/issues";

            _ = UtilHelpers.StartProcessNetCore(url);
        }
    }
}
