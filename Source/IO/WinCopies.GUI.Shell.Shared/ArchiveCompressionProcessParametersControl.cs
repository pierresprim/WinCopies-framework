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

using SevenZip;

using System.Windows;
using System.Windows.Controls;

using static WinCopies.Util.Desktop.UtilHelpers;

namespace WinCopies.GUI.IO.Controls.Process
{
    public class ArchiveCompressionProcessParametersControl : Control
    {
        private static DependencyProperty Register<T>(in string propertyName, in PropertyMetadata propertyMetadata) => Register<T, ArchiveCompressionProcessParametersControl>(propertyName, propertyMetadata);
        
        public static readonly DependencyProperty ArchiveFormatProperty = Register<OutArchiveFormat>(nameof(ArchiveFormat), new PropertyMetadata(OutArchiveFormat.Zip));

        public OutArchiveFormat ArchiveFormat { get => (OutArchiveFormat)GetValue(ArchiveFormatProperty); set => SetValue(ArchiveFormatProperty, value); }

        public static readonly DependencyProperty CompressionLevelProperty = Register<CompressionLevel>(nameof(CompressionLevel), new PropertyMetadata(CompressionLevel.Normal));

        public CompressionLevel CompressionLevel { get => (CompressionLevel)GetValue(CompressionLevelProperty); set => SetValue(CompressionLevelProperty, value); }

        public static readonly DependencyProperty CompressionMethodProperty = Register<CompressionMethod>(nameof(CompressionMethod), new PropertyMetadata(CompressionMethod.Default));

        public CompressionMethod CompressionMethod { get => (CompressionMethod)GetValue(CompressionMethodProperty); set => SetValue(CompressionMethodProperty, value); }

        public static readonly DependencyProperty CompressionModeProperty = Register<CompressionMode>(nameof(CompressionMode), new PropertyMetadata(CompressionMode.Append));

        public CompressionMode CompressionMode { get => (CompressionMode)GetValue(CompressionModeProperty); set => SetValue(CompressionModeProperty, value); }

        public static readonly DependencyProperty DestinationPathProperty = Register<string, ArchiveCompressionProcessParametersControl>(nameof(DestinationPath));

        public string DestinationPath { get => (string)GetValue(DestinationPathProperty); set => SetValue(DestinationPathProperty, value); }

        public static readonly DependencyProperty FastCompressionProperty = Register<bool>(nameof(FastCompression), new PropertyMetadata(true));

        public bool FastCompression { get => (bool)GetValue(FastCompressionProperty); set => SetValue(FastCompressionProperty, value); }

        public static readonly DependencyProperty IncludeEmptyDirectoriesProperty = Register<bool>(nameof(IncludeEmptyDirectories), new PropertyMetadata(false));

        public bool IncludeEmptyDirectories { get => (bool)GetValue(IncludeEmptyDirectoriesProperty); set => SetValue(IncludeEmptyDirectoriesProperty, value); }

        public static readonly DependencyProperty PreserveDirectoryRootProperty = Register<bool>(nameof(PreserveDirectoryRoot), new PropertyMetadata(false));

        public bool PreserveDirectoryRoot { get => (bool)GetValue(PreserveDirectoryRootProperty); set => SetValue(PreserveDirectoryRootProperty, value); }

        static ArchiveCompressionProcessParametersControl() => DefaultStyleKeyProperty.OverrideMetadata(typeof(ArchiveCompressionProcessParametersControl), new FrameworkPropertyMetadata(typeof(ArchiveCompressionProcessParametersControl)));
    }
}
