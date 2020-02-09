/* Copyright © Pierre Sprimont, 2019
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

namespace WinCopies.IO
{
    public partial class Generic : ResourceDictionary
    {

        public static string GetStringResource(object key) => (string)ResourceDictionary[key];

        public static ResourceDictionary ResourceDictionary { get; } = WinCopies.Util.Generic.AddNewDictionary("/WinCopies.IO;component/Generic.xaml");

        /// <summary>
        /// Gets the NotBrowsableObject resource.
        /// </summary>
        public static string NotBrowsableObject => GetStringResource(nameof(NotBrowsableObject));

        /// <summary>
        /// Gets the FileTypeAndSpecialFolderNotCorrespond resource.
        /// </summary>
        public static string FileTypeAndSpecialFolderNotCorrespond => GetStringResource(nameof(FileTypeAndSpecialFolderNotCorrespond));

        /// <summary>
        /// Gets the BackgroundWorkerIsBusy resource.
        /// </summary>
        public static string BackgroundWorkerIsBusy => GetStringResource(nameof(BackgroundWorkerIsBusy));

        /// <summary>
        /// Gets the NotValidRegistryKey resource.
        /// </summary>
        public static string NotValidRegistryKey => GetStringResource(nameof(NotValidRegistryKey));

        /// <summary>
        /// Gets the RegistryKeyNotExists resource.
        /// </summary>
        public static string RegistryKeyNotExists => GetStringResource(nameof(RegistryKeyNotExists));

    }
}
