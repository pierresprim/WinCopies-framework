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
using System.Diagnostics;
using System.Drawing;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

using TsudaKageyu;

using static WinCopies.Util.Util;
using IfCT = WinCopies.Util.Util.ComparisonType;
using IfCM = WinCopies.Util.Util.ComparisonMode;
using IfComp = WinCopies.Util.Util.Comparison;
using System.Linq;
using System.Collections.Generic;
using WinCopies.Util;

namespace WinCopies.IO
{

    /// <summary>
    /// Represents an archive item.
    /// </summary>
    public interface IArchiveItemInfo : IArchiveItemInfoProvider
    {

        /// <summary>
        /// The <see cref="SevenZip.ArchiveFileInfo"/> that this <see cref="IArchiveItemInfo"/> represents.
        /// </summary>
        ArchiveFileInfo? ArchiveFileInfo { get; }

    }

    public delegate ArchiveFileInfo? ArchiveFileInfoDeepClone(ArchiveFileInfo? obj, string archivePath);

    /// <summary>
    /// Represents an archive item.
    /// </summary>
    public class ArchiveItemInfo/*<TItems, TFactory>*/ : ArchiveItemInfoProvider/*<TItems, TFactory>*/, IArchiveItemInfo // where TItems : BrowsableObjectInfo, IArchiveItemInfo where TFactory : BrowsableObjectInfoFactory, IArchiveItemInfoFactory
    {

        // public override bool IsRenamingSupported => false;

        #region Fields

        private DeepClone<ArchiveFileInfo?> _archiveFileInfoDelegate;

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value that indicates whether this object needs to reconstruct objects on deep cloning.
        /// </summary>
        public override bool NeedsObjectsOrValuesReconstruction => true; // True because of the ShellObjectInfo's ShellObject

        public static ArchiveFileInfoDeepClone DefaultArchiveFileInfoDeepClone { get; } = (ArchiveFileInfo? archiveFileInfo, string archivePath) => archiveFileInfo.HasValue
                ? (ArchiveFileInfo?)new SevenZipExtractor(archivePath).ArchiveFileData.First(item => item.FileName.ToLower() == archiveFileInfo.Value.FileName)
                : null;

        /// <summary>
        /// Gets the localized path of this <see cref="ArchiveItemInfo"/>.
        /// </summary>
        public override string LocalizedName => ArchiveShellObject.LocalizedName;

        /// <summary>
        /// Gets the name of this <see cref="ArchiveItemInfo"/>.
        /// </summary>
        public override string Name => System.IO.Path.GetFileName(Path);

        /// <summary>
        /// Gets the small <see cref="BitmapSource"/> of this <see cref="ArchiveItemInfo"/>.
        /// </summary>
        public override BitmapSource SmallBitmapSource => TryGetBitmapSource(new System.Drawing.Size(16, 16));

        /// <summary>
        /// Gets the medium <see cref="BitmapSource"/> of this <see cref="ArchiveItemInfo"/>.
        /// </summary>
        public override BitmapSource MediumBitmapSource => TryGetBitmapSource(new System.Drawing.Size(48, 48));

        /// <summary>
        /// Gets the large <see cref="BitmapSource"/> of this <see cref="ArchiveItemInfo"/>.
        /// </summary>
        public override BitmapSource LargeBitmapSource => TryGetBitmapSource(new System.Drawing.Size(128, 128));

        /// <summary>
        /// Gets the extra large <see cref="BitmapSource"/> of this <see cref="ArchiveItemInfo"/>.
        /// </summary>
        public override BitmapSource ExtraLargeBitmapSource => TryGetBitmapSource(new System.Drawing.Size(256, 256));

        /// <summary>
        /// Gets a value that indicates whether this <see cref="ArchiveItemInfo"/> is browsable.
        /// </summary>
        public override bool IsBrowsable => If(IfCT.Or, IfCM.Logical, IfComp.Equal, FileType, FileType.Folder, FileType.Drive, FileType.Archive);

        //IShellObjectInfo IArchiveItemInfoProvider.ArchiveShellObject => ArchiveShellObjectOverride;

        /// <summary>
        /// The <see cref="SevenZip.ArchiveFileInfo"/> that this <see cref="IArchiveItemInfo"/> represents.
        /// </summary>
        public ArchiveFileInfo? ArchiveFileInfo { get; private set; }

        /// <summary>
        /// The parent <see cref="IShellObjectInfo"/> of the current archive item.
        /// </summary>
        public override IShellObjectInfo ArchiveShellObject { get; }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="ArchiveItemInfo"/> class using a custom factory for <see cref="ArchiveItemInfo"/>s.
        /// </summary>
        /// <param name="archiveShellObject">The <see cref="IShellObjectInfo"/> that correspond to the root path of the archive</param>
        /// <param name="archiveFileInfoDelegate">The <see cref="SevenZip.ArchiveFileInfo"/> that correspond to this archive item in the archive. Note: leave this parameter null if this <see cref="ArchiveItemInfo"/> represents a folder that exists implicitly in the archive.</param>
        /// <param name="path">The full path to this archive item</param>
        /// <param name="fileType">The file type of this archive item</param>
        public ArchiveItemInfo(string path, FileType fileType, IShellObjectInfo archiveShellObject, ArchiveFileInfo? archiveFileInfo, DeepClone<ArchiveFileInfo?> archiveFileInfoDelegate) : base(path, fileType)

        {

            if (fileType == FileType.SpecialFolder)

                // todo:

                throw new ArgumentException("'fileType' can't be a SpecialFolder.");

            _archiveFileInfoDelegate = archiveFileInfoDelegate;

            ArchiveFileInfo = archiveFileInfo;

            if (ArchiveFileInfo.HasValue && !path.EndsWith(ArchiveFileInfo.Value.FileName))

                // todo:

                throw new ArgumentException($"'{nameof(path)}' must end with '{nameof(ArchiveFileInfo.Value.FileName)}'");

            ArchiveShellObject = archiveShellObject;

#if DEBUG 

            Debug.WriteLine("shellObject == null: " + (archiveShellObject == null).ToString());

#endif

            // Path = path;

#if DEBUG

            Debug.WriteLine("path: " + path);

#endif

        }

        ///// <summary>
        ///// Loads the items of this <see cref="ArchiveItemInfo"/> using custom worker behavior options.
        ///// </summary>
        ///// <param name="workerReportsProgress">Whether the worker reports progress</param>
        ///// <param name="workerSupportsCancellation">Whether the worker supports cancellation.</param>
        //public override void LoadItems(bool workerReportsProgress, bool workerSupportsCancellation) => LoadItems((IBrowsableObjectInfoLoader)new ArchiveLoader<ArchiveItemInfo, TItems, TFactory>(this, GetAllEnumFlags<FileTypes>(), workerReportsProgress, workerSupportsCancellation));

        ///// <summary>
        ///// Loads the items of this <see cref="BrowsableObjectInfo"/> asynchronously using custom worker behavior options.
        ///// </summary>
        ///// <param name="workerReportsProgress">Whether the worker reports progress</param>
        ///// <param name="workerSupportsCancellation">Whether the worker supports cancellation.</param>
        //public override void LoadItemsAsync(bool workerReportsProgress, bool workerSupportsCancellation) => LoadItemsAsync((IBrowsableObjectInfoLoader)new ArchiveLoader<ArchiveItemInfo, TItems, TFactory>(this, GetAllEnumFlags<FileTypes>(), workerReportsProgress, workerSupportsCancellation));

        #region Protected methods

        /// <summary>
        /// When overridden in a derived class, returns the parent of this <see cref="ArchiveItemInfo"/>.
        /// </summary>
        /// <returns>the parent of this <see cref="ArchiveItemInfo"/>.</returns>
        protected override IBrowsableObjectInfo GetParent()
        {

            IBrowsableObjectInfo result;

            if (Path.Length > ArchiveShellObject.Path.Length)

            {

                string path = Path.Substring(0, Path.LastIndexOf(IO.Path.PathSeparator));

                ArchiveFileInfo? archiveFileInfo = null;

                using (var extractor = new SevenZipExtractor(ArchiveShellObject.Path))

                    foreach (ArchiveFileInfo item in extractor.ArchiveFileData)

                        if (item.FileName.ToLower() == path.ToLower())

                            archiveFileInfo = item;

                result = new ArchiveItemInfo(path, FileType.Folder, ArchiveShellObject, archiveFileInfo, _archiveFileInfo => ArchiveItemInfo.DefaultArchiveFileInfoDeepClone(_archiveFileInfo, ArchiveShellObject.Path) /*archiveParentFileInfo.Value*/);

            }

            else

                result = ArchiveShellObject;

            return result /*&& Path.Contains(IO.Path.PathSeparator)*/;

        }

        /// <summary>
        /// Gets a deep clone of this <see cref="ArchiveItemInfo"/>. The <see cref="BrowsableObjectInfo.OnDeepClone(BrowsableObjectInfo)"/> method already has an implementation for deep cloning from constructor and not from an <see cref="object.MemberwiseClone"/> operation. If you perform a deep cloning operation using an <see cref="object.MemberwiseClone"/> operation in <see cref="DeepCloneOverride()"/>, you'll have to override this method if your class has to reinitialize members.
        /// </summary>
        protected override BrowsableObjectInfo DeepCloneOverride() => new ArchiveItemInfo(Path, FileType, (IShellObjectInfo)ArchiveShellObject.DeepClone(), _archiveFileInfoDelegate(ArchiveFileInfo), _archiveFileInfoDelegate);

        // public virtual IBrowsableObjectInfo GetBrowsableObjectInfo(IBrowsableObjectInfo browsableObjectInfo) => browsableObjectInfo;

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        protected override void Dispose(bool disposing)
        {

            base.Dispose(disposing);

            if (disposing)

            {

                ArchiveFileInfo = null;

                _archiveFileInfoDelegate = null;

            }

        }

        #endregion

        // public override string ToString() => System.IO.Path.GetFileName(Path);

        #region Private methods

        private Icon TryGetIcon(System.Drawing.Size size) =>

            // if (System.IO.Path.HasExtension(Path))

            Microsoft.WindowsAPICodePack.Shell.FileOperation.GetFileInfo(System.IO.Path.GetExtension(Path), Microsoft.WindowsAPICodePack.Shell.FileAttributes.Normal, Microsoft.WindowsAPICodePack.Shell.GetFileInfoOptions.Icon | Microsoft.WindowsAPICodePack.Shell.GetFileInfoOptions.UseFileAttributes).Icon?.TryGetIcon(size, 32, true, true) ?? TryGetIcon(FileType == FileType.Folder ? 3 : 0, "SHELL32.dll", size);// else// return TryGetIcon(FileType == FileType.Folder ? 3 : 0, "SHELL32.dll", size);

        private BitmapSource TryGetBitmapSource(System.Drawing.Size size)

        {

            using (Icon icon = TryGetIcon(size))

                return icon == null ? null : Imaging.CreateBitmapSourceFromHIcon(icon.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());

        }

        #endregion

    }

}
