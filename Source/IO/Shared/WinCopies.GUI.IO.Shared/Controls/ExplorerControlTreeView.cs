﻿/* Copyright © Pierre Sprimont, 2020
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
using System.Windows.Controls;

namespace WinCopies.GUI.IO.Controls
{
    // todo: should be in WinCopies.Util.Desktop
    public enum CloseTabsTo : sbyte
    {
        Left = 1,

        Right = 2
    }

    public class ExplorerControlTreeView : TreeView
    {
        protected override DependencyObject GetContainerForItemOverride() => new ExplorerControlTreeViewItem();
    }
}
