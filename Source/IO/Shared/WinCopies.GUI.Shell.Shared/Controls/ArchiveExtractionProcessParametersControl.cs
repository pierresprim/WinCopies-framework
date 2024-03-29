﻿/* Copyright © Pierre Sprimont, 2021
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

using static WinCopies.Util.Desktop.UtilHelpers;

namespace WinCopies.GUI.IO.Controls.Process
{
    public class ArchiveExtractionProcessParametersControl : ArchiveProcessParametersControl
    {
        public static readonly DependencyProperty PreserveDirectoryStructureProperty = Register<bool, ArchiveExtractionProcessParametersControl>(nameof(PreserveDirectoryStructure), new PropertyMetadata(true));

        public bool PreserveDirectoryStructure { get => (bool)GetValue(PreserveDirectoryStructureProperty); set => SetValue(PreserveDirectoryStructureProperty, value); }

        static ArchiveExtractionProcessParametersControl() => DefaultStyleKeyProperty.OverrideMetadata(typeof(ArchiveExtractionProcessParametersControl), new FrameworkPropertyMetadata(typeof(ArchiveExtractionProcessParametersControl)));
    }
}
