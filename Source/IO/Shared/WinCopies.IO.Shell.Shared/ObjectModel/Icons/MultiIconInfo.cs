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

using Microsoft.WindowsAPICodePack.Shell;

using System;
using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;

using WinCopies.Collections.Generic;
using WinCopies.GUI.Drawing;
using WinCopies.IO.AbstractionInterop;
using WinCopies.IO.Enumeration;
using WinCopies.IO.Process;
using WinCopies.IO.PropertySystem;
using WinCopies.IO.Selectors;
using WinCopies.Linq;
using WinCopies.PropertySystem;
using WinCopies.Temp;

namespace WinCopies.IO.ObjectModel
{
    public interface IMultiIconInfo : IBrowsableObjectInfo<IFileSystemObjectInfoProperties, MultiIcon, MultiIconInfoItemProvider, IEnumerableSelectorDictionary<MultiIconInfoItemProvider, IBrowsableObjectInfo>, MultiIconInfoItemProvider>, IShellObjectInfo<IFileSystemObjectInfoProperties, ShellObjectInfoEnumeratorStruct, IEnumerableSelectorDictionary<ShellObjectInfoItemProvider, IBrowsableObjectInfo>, ShellObjectInfoItemProvider>
    {
        // Left empty.
    }

    public class MultiIconInfo : BrowsableObjectInfo<IFileSystemObjectInfoProperties, MultiIcon, MultiIconInfoItemProvider, IEnumerableSelectorDictionary<MultiIconInfoItemProvider, IBrowsableObjectInfo>, MultiIconInfoItemProvider>, IMultiIconInfo
    {
        protected internal class BrowsabilityPath : IBrowsabilityPath<IShellObjectInfoBase>
        {
            string IBrowsabilityPathBase.Name => "Icon Manager";

            IBrowsableObjectInfo IBrowsabilityPath<IShellObjectInfoBase>.GetPath(IShellObjectInfoBase browsableObjectInfo) => new MultiIconInfo((IShellObjectInfo)browsableObjectInfo);

            bool IBrowsabilityPath<IShellObjectInfoBase>.IsValidFor(IShellObjectInfoBase browsableObjectInfo) => System.IO.File.Exists(browsableObjectInfo.Path) && IsValidFormat(browsableObjectInfo.Path);
        }

        private MultiIcon _multiIcon;
        private IShellObjectInfo _shellObjectInfo;

        public static bool IsValidFormat(in string path)
#if CS9
                =>
#else
        {
            switch (
#endif
                System.IO.Path.GetExtension(path).ToLower()
#if CS9
                    switch
#else
                    )
#endif
            {
#if !CS9
                case
#endif
                    ".ico"
#if CS9
                        or
#else
                        :
                case
#endif
                        ".icl"
#if CS9
                        or
#else
                        :
                case
#endif
                        ".dll"
#if CS9
                        or
#else
                        :
                case
#endif
                        ".exe"
#if CS9
                        or
#else
                        :
                case
#endif
                        ".ocx"
#if CS9
                        or
#else
                        :
                case
#endif
                        ".cpl"
#if CS9
                        or
#else
                        :
                case
#endif
                        ".src"
#if CS9
                    =>
#else
                    :
                    return
#endif
                    true
#if CS9
                    ,

                    _ =>
#else
                    ;
                default:
                    return
#endif
                    false
#if CS9
                    ,
#else
                    ;
#endif
            };
#if !CS9
        }
#endif

        protected override bool IsLocalRootOverride => false;

        protected IShellObjectInfo ShellObjectInfo => GetValueIfNotDisposed(_shellObjectInfo);

        protected sealed override IFileSystemObjectInfoProperties ObjectPropertiesGenericOverride => _shellObjectInfo.ObjectProperties;

        protected sealed override IProcessFactory ProcessFactoryOverride => _shellObjectInfo.ProcessFactory;

        public static IEnumerableSelectorDictionary<MultiIconInfoItemProvider, IBrowsableObjectInfo> DefaultItemSelectorDictionary { get; } = new MultiIconInfoSelectorDictionary();

        protected override IBrowsabilityOptions BrowsabilityOverride => BrowsabilityOptions.BrowsableByDefault;

        protected override MultiIcon InnerObjectGenericOverride => _multiIcon;

        protected override System.Collections.Generic.IEnumerable<IBrowsabilityPath> BrowsabilityPathsOverride => _shellObjectInfo.BrowsabilityPaths;

        protected override IPropertySystemCollection<PropertyId, ShellPropertyGroup> ObjectPropertySystemOverride => _shellObjectInfo.ObjectPropertySystem;

        protected override bool IsRecursivelyBrowsableOverride => _shellObjectInfo.IsRecursivelyBrowsable;

        protected override IBrowsableObjectInfo ParentOverride => _shellObjectInfo.Parent;

        protected override IBitmapSourceProvider BitmapSourceProviderOverride => _shellObjectInfo.BitmapSourceProvider;

        protected override string ItemTypeNameOverride => _shellObjectInfo.ItemTypeName;

        protected override string DescriptionOverride => _shellObjectInfo.Description;

        protected override bool IsSpecialItemOverride => _shellObjectInfo.IsSpecialItem;

        public override string LocalizedName => ShellObjectInfo.LocalizedName;

        public override string Name => ShellObjectInfo.Name;

        IShellObjectInfoBase IArchiveItemInfoProvider.ArchiveShellObject => this;

        ShellObject IEncapsulatorBrowsableObjectInfo<ShellObject>.InnerObject => ShellObjectInfo.InnerObject;

        protected override System.Collections.Generic.IEnumerable<IProcessInfo> CustomProcessesOverride => null;

        bool IShellObjectInfoBase.IsArchiveOpen => false;

        protected internal MultiIconInfo(in IShellObjectInfo shellObjectInfo) : base(shellObjectInfo.InnerObject.ParsingName, shellObjectInfo.ClientVersion)
        {
            _shellObjectInfo = shellObjectInfo;

            (_multiIcon = new MultiIcon()).Load(Path);
        }

        public static MultiIconInfo From(in IShellObjectInfo shellObjectInfo) => new
#if !CS9
                MultiIconInfo
#endif
                (IsValidFormat((shellObjectInfo.InnerObject is ShellFile shellFile
            ? shellFile.IsFileSystemObject
                ? shellObjectInfo
                : throw new ArgumentException("The given ShellObject must be a file system object.", nameof(shellObjectInfo))
            : throw new ArgumentException("The given ShellObject must be a ShellFile.", nameof(shellObjectInfo))).Path)
            ? shellObjectInfo
            : throw new ArgumentException("The file format of the given ShellObject is not supported."));

        StreamInfo IShellObjectInfoBase.GetArchiveFileStream() => null;

        protected override IEnumerableSelectorDictionary<MultiIconInfoItemProvider, IBrowsableObjectInfo> GetSelectorDictionaryOverride() => DefaultItemSelectorDictionary;

        protected override System.Collections.Generic.IEnumerable<MultiIconInfoItemProvider> GetItemProviders()
        {
            for (int i = 0; i < InnerObjectGeneric.Count; i++)

                yield return new MultiIconInfoItemProvider(_multiIcon[i], this);
        }

        protected override System.Collections.Generic.IEnumerable<MultiIconInfoItemProvider> GetItemProviders(Predicate<MultiIconInfoItemProvider> predicate) => predicate == null ? GetItemProviders() : GetItemProviders().WherePredicate(predicate);

        protected override ArrayBuilder<IBrowsableObjectInfo> GetRootItemsOverride() => FileSystemObjectInfo.GetRootItems();

        protected override void DisposeUnmanaged()
        {
            _shellObjectInfo = null;

            base.DisposeUnmanaged();
        }

        protected override void DisposeManaged()
        {
            _multiIcon.Clear();
            _multiIcon = null;

            base.DisposeManaged();
        }

        protected override System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> GetSubRootItemsOverride() => ShellObjectInfo.GetSubRootItems();

        void IShellObjectInfoBase.OpenArchive(FileMode fileMode, FileAccess fileAccess, FileShare fileShare, int? bufferSize, FileOptions fileOptions) => ShellObjectInfo.OpenArchive(fileMode, fileAccess, fileShare, bufferSize, fileOptions);

        void IShellObjectInfoBase.CloseArchive() => ShellObjectInfo.CloseArchive();

        Icon IFileSystemObjectInfo.TryGetIcon(in int size) => ShellObjectInfo.TryGetIcon(size);

        BitmapSource IFileSystemObjectInfo.TryGetBitmapSource(in int size) => ShellObjectInfo.TryGetBitmapSource(size);

        System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> IBrowsableObjectInfo<IFileSystemObjectInfoProperties, ShellObject, ShellObjectInfoEnumeratorStruct, IEnumerableSelectorDictionary<ShellObjectInfoItemProvider, IBrowsableObjectInfo>, ShellObjectInfoItemProvider>.GetItems(Predicate<ShellObjectInfoEnumeratorStruct> predicate) => _shellObjectInfo.GetItems(predicate);

        IEnumerableSelectorDictionary<ShellObjectInfoItemProvider, IBrowsableObjectInfo> IBrowsableObjectInfo<IFileSystemObjectInfoProperties, ShellObject, ShellObjectInfoEnumeratorStruct, IEnumerableSelectorDictionary<ShellObjectInfoItemProvider, IBrowsableObjectInfo>, ShellObjectInfoItemProvider>.GetSelectorDictionary() => _shellObjectInfo.GetSelectorDictionary();
    }
}
