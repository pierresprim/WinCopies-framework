/* Copyright © Pierre Sprimont, 2020
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

#region Usings
#region Namespaces
using Microsoft.WindowsAPICodePack.Shell;

#region WinCopies
using WinCopies.IO.ObjectModel;
using WinCopies.IO.PropertySystem;
#endregion WinCopies
#endregion Namespaces

#region Static Usings
using static Microsoft.WindowsAPICodePack.Shell.KnownFolders;

using static WinCopies.IO.BrowsableObjectInfoCallbackReason;
using static WinCopies.IO.FileType;
#endregion Static Usings
#endregion

namespace WinCopies.IO
{
    public struct ShellObjectInfoMonitoring<TObjectProperties, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> where TObjectProperties : IFileSystemObjectInfoProperties where TSelectorDictionary : IEnumerableSelectorDictionary<TDictionaryItems, IBrowsableObjectInfo>
    {
        public ShellObjectInfo<TObjectProperties, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> ShellObjectInfo { get; }

        public ShellObjectWatcher ShellObjectWatcher { get; private set; }

        public ShellObjectInfoMonitoring(in ShellObjectInfo<TObjectProperties, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> shellObjectInfo)
        {
            ShellObjectInfo = shellObjectInfo;

            ShellObjectWatcher = null;
        }

        public void Start()
        {
            if (ShellObjectWatcher == null)
            {
                ShellObjectWatcher = new ShellObjectWatcher(ShellObjectInfo.InnerObjectGeneric, false);

                if (ShellObjectInfo.InnerObjectGeneric.ParsingName == Computer.ParsingName)
                {
                    ShellObjectWatcher.DriveAdded += DirectoryCreated;
                    ShellObjectWatcher.DriveRemoved += DirectoryDeleted;
                }

                else
                {
                    ShellObjectWatcher.DirectoryCreated += DirectoryCreated;
                    ShellObjectWatcher.DirectoryRenamed += DirectoryRenamed;
                    ShellObjectWatcher.DirectoryDeleted += DirectoryDeleted;

                    ShellObjectWatcher.ItemCreated += ItemCreated;
                    ShellObjectWatcher.ItemRenamed += ItemRenamed;
                    ShellObjectWatcher.ItemDeleted += ItemDeleted;
                }

                ShellObjectWatcher.Start();
            }
        }

        public void Stop()
        {
            if (ShellObjectWatcher == null)

                return;

            ShellObjectWatcher.Dispose();

            if (ShellObjectInfo.InnerObjectGeneric.ParsingName == Computer.ParsingName)
            {
                ShellObjectWatcher.DriveAdded -= DirectoryCreated;
                ShellObjectWatcher.DriveRemoved -= DirectoryDeleted;
            }

            else
            {
                ShellObjectWatcher.DirectoryCreated -= DirectoryCreated;
                ShellObjectWatcher.DirectoryRenamed -= DirectoryRenamed;
                ShellObjectWatcher.DirectoryDeleted -= DirectoryDeleted;

                ShellObjectWatcher.ItemCreated -= ItemCreated;
                ShellObjectWatcher.ItemRenamed -= ItemRenamed;
                ShellObjectWatcher.ItemDeleted -= ItemDeleted;
            }

            ShellObjectWatcher = null;
        }

        private IBrowsableObjectInfo Create(in string path, in bool directory) => new ShellObjectInfo(path, directory ? Folder : FileType.File, ShellObjectFactory.Create(path), ShellObjectInfo.ClientVersion);

        private void _RaiseCallback(in string path, in string newPath, in bool directory, in bool update)
        {
            try
            {
                IBrowsableObjectInfo obj = Create(newPath, directory);

                ShellObjectInfo.RaiseCallbacks(new BrowsableObjectInfoCallbackArgs(path, obj, update ? Updated : Added));
            }

            catch
            {
                // Left empty.
            }
        }



        private void DirectoryCreated(object sender, ShellObjectChangedEventArgs e) => _RaiseCallback(e.Path, e.Path, true, false);

        private void DirectoryRenamed(object sender, ShellObjectRenamedEventArgs e) => _RaiseCallback(e.Path, e.NewPath, true, true);

        private void DirectoryDeleted(object sender, ShellObjectChangedEventArgs e) => ShellObjectInfo.RaiseCallbacks(new BrowsableObjectInfoCallbackArgs(e.Path, null, Removed));



        private void ItemCreated(object sender, ShellObjectChangedEventArgs e) => _RaiseCallback(e.Path, e.Path, false, false);

        private void ItemRenamed(object sender, ShellObjectRenamedEventArgs e) => _RaiseCallback(e.Path, e.NewPath, false, true);

        private void ItemDeleted(object sender, ShellObjectChangedEventArgs e) => ShellObjectInfo.RaiseCallbacks(new BrowsableObjectInfoCallbackArgs(e.Path, null, Removed));
    }
}
