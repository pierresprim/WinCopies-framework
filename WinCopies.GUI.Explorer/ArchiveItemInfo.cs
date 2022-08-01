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

using SevenZip;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using WinCopies.IO;

namespace WinCopies.GUI.Explorer
{

    public class ArchiveItemInfo : IO.ArchiveItemInfo, IBrowsableObjectInfo, IBrowsableObjectInfoInternal, IBrowsableObjectInfoHelper

    {

        public bool IsSelected { get; set; }

        public IBrowsableObjectInfo SelectedItem { get; set; }

        public bool IsCheckBoxEnabled { get; set; }

        public ReadOnlyObservableCollection<IBrowsableObjectInfo> SelectedItems { get; internal set; } = null;

        ReadOnlyObservableCollection<IBrowsableObjectInfo> IBrowsableObjectInfoHelper.SelectedItems { set => SelectedItems = value; }

        ObservableCollection<IBrowsableObjectInfo> IBrowsableObjectInfoInternal.SelectedItems { get; set; } = new ObservableCollection<IBrowsableObjectInfo>();

        public ArchiveItemInfo(IO.ShellObjectInfo archiveShellObject, ArchiveFileInfo? archiveFileInfo, string path, FileType fileType) : base(archiveShellObject, archiveFileInfo, path, fileType) => BrowsableObjectInfoHelper.Init(this);

        public ArchiveItemInfo(IO.ArchiveItemInfo archiveItemInfo) : this(archiveItemInfo.ArchiveShellObject, archiveItemInfo.ArchiveFileInfo, archiveItemInfo.Path, archiveItemInfo.FileType) => BrowsableObjectInfoHelper.Init(this);

        public override IO.IBrowsableObjectInfo GetBrowsableObjectInfo(IO.ShellObjectInfo archiveShellObject, ArchiveFileInfo? archiveFileInfo, string path, FileType fileType) => new ArchiveItemInfo(archiveShellObject, archiveFileInfo, path, fileType);

    }
}
