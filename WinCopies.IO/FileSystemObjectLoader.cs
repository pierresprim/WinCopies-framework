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

using System.Collections.Generic;
using WinCopies.Util;

namespace WinCopies.IO
{

    public interface IFileSystemObjectLoader : IBrowsableObjectInfoLoader
    {

        FileTypes FileTypes { get; set; }

    }

    public abstract class FileSystemObjectLoader<TPath, TItems, TSubItems, TFactory> : BrowsableObjectInfoLoader<TPath, TItems, TSubItems, TFactory>, IFileSystemObjectLoader where TPath : FileSystemObjectInfo where TItems : FileSystemObjectInfo where TSubItems : FileSystemObjectInfo where TFactory : BrowsableObjectInfoFactory
    {

        private readonly FileTypes _fileTypes = Util.Util.GetAllEnumFlags<FileTypes>();

        public FileTypes FileTypes { get => _fileTypes; set => this.SetBackgroundWorkerProperty(nameof(FileTypes), nameof(_fileTypes), value, typeof(FileSystemObjectLoader<TPath, TItems, TSubItems, TFactory>), true); }

        protected FileSystemObjectLoader( BrowsableObjectTreeNode< TPath, TItems, TFactory > path, FileTypes fileTypes, bool workerReportsProgress, bool workerSupportsCancellation) : this(path, fileTypes, new FileSystemObjectComparer<IFileSystemObject>(), workerReportsProgress, workerSupportsCancellation) { }

        protected FileSystemObjectLoader( BrowsableObjectTreeNode< TPath, TItems, TFactory > path, FileTypes fileTypes, IFileSystemObjectComparer<IFileSystemObject> browsableObjectInfoComparer, bool workerReportsProgress, bool workerSupportsCancellation) : base(path, browsableObjectInfoComparer, workerReportsProgress, workerSupportsCancellation) => _fileTypes = fileTypes;

    }
}
