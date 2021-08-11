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
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;

using WinCopies.Collections.Generic;
using WinCopies.GUI.Drawing;
using WinCopies.IO.AbstractionInterop;
using WinCopies.IO.Enumeration;
using WinCopies.IO.Process;
using WinCopies.IO.PropertySystem;
using WinCopies.IO.Selectors;
using WinCopies.IO.Shell.ObjectModel;
using WinCopies.Linq;
using WinCopies.PropertySystem;

using static WinCopies.UtilHelpers;
using static WinCopies.ThrowHelper;

namespace WinCopies.IO
{
    public static class IconImageInfoHelper
    {
        public static string GetFriendlyBitDepth(PixelFormat pixelFormat)
        {
            switch (pixelFormat)
            {
                case PixelFormat.Format1bppIndexed:
                    return "1-bit B/W";
                case PixelFormat.Format24bppRgb:
                    return "24-bit True Colors";
                case PixelFormat.Format32bppArgb:
                case PixelFormat.Format32bppRgb:
                    return "32-bit Alpha Channel";
                case PixelFormat.Format8bppIndexed:
                    return "8-bit 256 Colors";
                case PixelFormat.Format4bppIndexed:
                    return "4-bit 16 Colors";
            }

            return "Unknown";
        }
    }

    namespace PropertySystem
    {
        public interface IIconImageInfoProperties : WinCopies.DotNetFix.IDisposable
        {
            string FriendlyBitDepth { get; }

            PixelFormat PixelFormat { get; }

            int ColorsInPalette { get; }

            IconImageFormat IconImageFormat { get; }
        }

        public class IconImageInfoProperties : BrowsableObjectInfoProperties<IconImage>, IIconImageInfoProperties
        {
            public string FriendlyBitDepth => IconImageInfoHelper.GetFriendlyBitDepth(InnerObject.PixelFormat);

            public PixelFormat PixelFormat => InnerObject.PixelFormat;

            public int ColorsInPalette => InnerObject.ColorsInPalette;

            public IconImageFormat IconImageFormat => InnerObject.IconImageFormat;

            public IconImageInfoProperties(in IconImage icon) : base(icon) { /* Left empty. */ }
        }
    }

    namespace ObjectModel
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
            {
                switch (System.IO.Path.GetExtension(path).ToLower())
                {
                    case ".ico":
                    case ".icl":
                    case ".dll":
                    case ".exe":
                    case ".ocx":
                    case ".cpl":
                    case ".src":

                        return true;
                }

                return false;
            }

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

            protected override IBrowsableObjectInfoBitmapSources BitmapSourcesOverride => _shellObjectInfo.BitmapSources;

            protected override string ItemTypeNameOverride => _shellObjectInfo.ItemTypeName;

            protected override string DescriptionOverride => _shellObjectInfo.Description;

            protected override bool IsSpecialItemOverride => _shellObjectInfo.IsSpecialItem;

            public override string LocalizedName => ShellObjectInfo.LocalizedName;

            public override string Name => ShellObjectInfo.Name;

            System.IO.Stream IShellObjectInfoBase.ArchiveFileStream => null;

            IShellObjectInfoBase IArchiveItemInfoProvider.ArchiveShellObject => this;

            ShellObject IEncapsulatorBrowsableObjectInfo<ShellObject>.InnerObject => ShellObjectInfo.InnerObject;

            protected override System.Collections.Generic.IEnumerable<IProcessInfo> CustomProcessesOverride => null;

            protected internal MultiIconInfo(in IShellObjectInfo shellObjectInfo) : base(shellObjectInfo.InnerObject.ParsingName, shellObjectInfo.ClientVersion)
            {
                _shellObjectInfo = shellObjectInfo;

                (_multiIcon = new MultiIcon()).Load(Path);
            }

            public static MultiIconInfo From(in IShellObjectInfo shellObjectInfo) => new MultiIconInfo(IsValidFormat((shellObjectInfo.InnerObject is ShellFile shellFile
                ? shellFile.IsFileSystemObject
                    ? shellObjectInfo
                    : throw new ArgumentException("The given ShellObject must be a file system object.", nameof(shellObjectInfo))
                : throw new ArgumentException("The given ShellObject must be a ShellFile.", nameof(shellObjectInfo))).Path)
                ? shellObjectInfo
                : throw new ArgumentException("The file format of the given ShellObject is not supported."));

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

        public interface ISingleIconInfo : IBrowsableObjectInfo<object, SingleIcon, SingleIconInfoItemProvider, IEnumerableSelectorDictionary<SingleIconInfoItemProvider, IBrowsableObjectInfo>, SingleIconInfoItemProvider>
        {
            // Left empty.
        }

        public class SingleIconInfo : BrowsableObjectInfo<object, SingleIcon, SingleIconInfoItemProvider, IEnumerableSelectorDictionary<SingleIconInfoItemProvider, IBrowsableObjectInfo>, SingleIconInfoItemProvider>, ISingleIconInfo
        {
            #region Classes
            protected class BrowsableObjectInfoBitmapSources : BrowsableObjectInfoBitmapSources<SingleIcon>
            {
                public static BitmapSource TryGetBitmapSource(in Icon icon, in ushort size) => Shell.ObjectModel.BrowsableObjectInfo.TryGetBitmapSource(Shell.ObjectModel.BrowsableObjectInfo.TryGetIcon(icon, size));

                protected override BitmapSource SmallBitmapSourceOverride => TryGetBitmapSource(InnerObject.Icon, SmallIconSize);

                protected override BitmapSource MediumBitmapSourceOverride => TryGetBitmapSource(InnerObject.Icon, MediumIconSize);

                protected override BitmapSource LargeBitmapSourceOverride => TryGetBitmapSource(InnerObject.Icon, LargeIconSize);

                protected override BitmapSource ExtraLargeBitmapSourceOverride => TryGetBitmapSource(InnerObject.Icon, ExtraLargeIconSize);

                public BrowsableObjectInfoBitmapSources(in SingleIcon singleIcon) : base(singleIcon) { /* Left empty. */ }
            }
            #endregion

            private static readonly BrowsabilityPathStack<ISingleIconInfo> __browsabilityPathStack = new BrowsabilityPathStack<ISingleIconInfo>();
            private static readonly WriteOnlyBrowsabilityPathStack<ISingleIconInfo> _browsabilityPathStack = __browsabilityPathStack.AsWriteOnly();

            public static IBrowsabilityPathStack<ISingleIconInfo> BrowsabilityPathStack => _browsabilityPathStack;

            private SingleIcon _singleIcon;
            private IBrowsableObjectInfo _parent;
            private IBrowsableObjectInfoBitmapSources _bitmapSources;

            protected override SingleIcon InnerObjectGenericOverride => _singleIcon;

            protected override object ObjectPropertiesGenericOverride => null;

            protected override System.Collections.Generic.IEnumerable<IBrowsabilityPath> BrowsabilityPathsOverride => __browsabilityPathStack.GetBrowsabilityPaths(this);

            protected override IProcessFactory ProcessFactoryOverride => Process.ProcessFactory.DefaultProcessFactory;

            protected override IPropertySystemCollection<PropertyId, ShellPropertyGroup> ObjectPropertySystemOverride => null;

            protected override IBrowsabilityOptions BrowsabilityOverride => BrowsabilityOptions.BrowsableByDefault;

            protected override bool IsRecursivelyBrowsableOverride => true;

            protected override IBrowsableObjectInfo ParentOverride => _parent
#if CS8
            ??=
#else
            ?? (_parent =
#endif
            new MultiIconInfo(ShellObjectInfo.From(Path.Remove(Path.LastIndexOf("\\"))))
#if !CS8
            )
#endif
            ;

            protected override IBrowsableObjectInfoBitmapSources BitmapSourcesOverride => _bitmapSources
#if CS8
            ??=
#else
            ?? (_bitmapSources =
#endif
            new BrowsableObjectInfoBitmapSources(InnerObjectGeneric)
#if !CS8
            )
#endif
            ;

            protected override string ItemTypeNameOverride => "Icon";

            protected override string DescriptionOverride => NotApplicable;

            protected override bool IsSpecialItemOverride => false;

            protected override ArrayBuilder<IBrowsableObjectInfo> GetRootItemsOverride()
            {
                var arrayBuilder = new ArrayBuilder<IBrowsableObjectInfo>();

                _=arrayBuilder.AddLast(Parent);

                return arrayBuilder;
            }

            public override string LocalizedName => GetValueIfNotDisposed(_singleIcon).Name;

            public override string Name => LocalizedName;

            public static IEnumerableSelectorDictionary<SingleIconInfoItemProvider, IBrowsableObjectInfo> DefaultItemSelectorDictionary { get; } = new SingleIconInfoSelectorDictionary();

            protected override System.Collections.Generic.IEnumerable<IProcessInfo> CustomProcessesOverride => null;

            public SingleIconInfo(in string parentPath, in ClientVersion clientVersion, in SingleIcon icon) : base($"{parentPath}\\{icon.Name}", clientVersion) => _singleIcon = icon;

            protected override IEnumerableSelectorDictionary<SingleIconInfoItemProvider, IBrowsableObjectInfo> GetSelectorDictionaryOverride() => DefaultItemSelectorDictionary;

            protected override System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> GetSubRootItemsOverride() => GetItemsOverride();

            protected override System.Collections.Generic.IEnumerable<SingleIconInfoItemProvider> GetItemProviders() => _singleIcon.Select(icon => new SingleIconInfoItemProvider(icon, this));

            protected override System.Collections.Generic.IEnumerable<SingleIconInfoItemProvider> GetItemProviders(Predicate<SingleIconInfoItemProvider> predicate) => predicate == null ? GetItemProviders() : GetItemProviders().WherePredicate(predicate);

            protected override void DisposeUnmanaged()
            {
                _singleIcon = null;

                _parent = null;

                base.DisposeUnmanaged();
            }

            protected override void DisposeManaged()
            {
                _bitmapSources = null;

                base.DisposeManaged();
            }
        }

        public interface IIconImageInfo : IBrowsableObjectInfo<object, IconImage, IconImageInfoItemProvider, IEnumerableSelectorDictionary<IconImageInfoItemProvider, IBrowsableObjectInfo>, IconImageInfoItemProvider>
        {
            // Left empty.
        }

        public class IconImageInfo : BrowsableObjectInfo<IIconImageInfoProperties, IconImage, IconImageInfoItemProvider, IEnumerableSelectorDictionary<IconImageInfoItemProvider, IBrowsableObjectInfo>, IconImageInfoItemProvider>, IIconImageInfo
        {
            private ISingleIconInfo _parent;
            private IBrowsableObjectInfoBitmapSources _bitmapSources;
            private IconImage _iconImage;

            protected override IProcessFactory ProcessFactoryOverride => null;

            protected override IconImage InnerObjectGenericOverride => _iconImage;

            protected override IIconImageInfoProperties ObjectPropertiesGenericOverride => null;

            protected override System.Collections.Generic.IEnumerable<IBrowsabilityPath> BrowsabilityPathsOverride => null;

            protected override IPropertySystemCollection<PropertyId, ShellPropertyGroup> ObjectPropertySystemOverride => null;

            protected override IBrowsabilityOptions BrowsabilityOverride => BrowsabilityOptions.BrowsableByDefault;

            protected override bool IsRecursivelyBrowsableOverride => true;

            protected override IBrowsableObjectInfo ParentOverride => _parent;

            protected override IBrowsableObjectInfoBitmapSources BitmapSourcesOverride => _bitmapSources
#if CS8
            ??=
#else
            ?? (_bitmapSources =
#endif
            new BrowsableObjectInfoIconBitmapSources(InnerObjectGeneric.Icon)
#if !CS8
            )
#endif
            ;

            protected override string ItemTypeNameOverride => "Icon image";

            protected override string DescriptionOverride => NotApplicable;

            protected override bool IsSpecialItemOverride => false;

            public override string LocalizedName => $"{InnerObjectGeneric.Size} {ObjectPropertiesGeneric.FriendlyBitDepth}";

            public override string Name => LocalizedName;

            protected override System.Collections.Generic.IEnumerable<IProcessInfo> CustomProcessesOverride => null;

            public static IEnumerableSelectorDictionary<IconImageInfoItemProvider, IBrowsableObjectInfo> DefaultItemSelectorDictionary { get; } = new IconImageInfoSelectorDictionary();

            public IconImageInfo(in IconImage icon, in ISingleIconInfo parent) : base($"{(parent ?? throw GetArgumentNullException(nameof(parent))).Path}\\{(icon ?? throw GetArgumentNullException(nameof(icon))).Size} - {icon.IconImageFormat}", parent.ClientVersion) => _parent = parent;

            protected override IEnumerableSelectorDictionary<IconImageInfoItemProvider, IBrowsableObjectInfo> GetSelectorDictionaryOverride() => DefaultItemSelectorDictionary;

            protected override System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> GetSubRootItemsOverride() => GetItemsOverride();

            protected override System.Collections.Generic.IEnumerable<IconImageInfoItemProvider> GetItemProviders() => new IconImageInfoItemProvider[] { new IconImageInfoItemProvider("Image", InnerObjectGeneric.Image, this), new IconImageInfoItemProvider("Mask", InnerObjectGeneric.Mask, this), new IconImageInfoItemProvider("Icon", InnerObjectGeneric.Icon, this) };

            protected override System.Collections.Generic.IEnumerable<IconImageInfoItemProvider> GetItemProviders(Predicate<IconImageInfoItemProvider> predicate) => predicate == null ? GetItemProviders() : GetItemProviders().WherePredicate(predicate);

            protected override ArrayBuilder<IBrowsableObjectInfo> GetRootItemsOverride()
            {
                var arrayBuilder = new ArrayBuilder<IBrowsableObjectInfo>();

                arrayBuilder.AddLast(Parent.Parent);

                return arrayBuilder;
            }

            protected override void DisposeManaged()
            {
                _iconImage = null;

                base.DisposeManaged();
            }

            protected override void DisposeUnmanaged()
            {
                _bitmapSources.Dispose();
                _bitmapSources = null;

                _parent = null;

                base.DisposeUnmanaged();
            }
        }

        public interface IBitmapInfo : IBrowsableObjectInfo<object, Bitmap, object, IEnumerableSelectorDictionary<object, IBrowsableObjectInfo>, object>
        {
            // Left empty.
        }

        public class BitmapInfo : BrowsableObjectInfo<object, Bitmap, object, IEnumerableSelectorDictionary<object, IBrowsableObjectInfo>, object>, IBitmapInfo
        {
            private Bitmap _bitmap;
            private IBrowsableObjectInfo _parent;
            private IBrowsableObjectInfoBitmapSources _bitmapSources;

            public override string Name => LocalizedName;

            protected override IBrowsableObjectInfo ParentOverride => _parent;

            protected override Bitmap InnerObjectGenericOverride => _bitmap;

            protected override object ObjectPropertiesGenericOverride => null;

            protected override System.Collections.Generic.IEnumerable<IBrowsabilityPath> BrowsabilityPathsOverride => null;

            protected override IProcessFactory ProcessFactoryOverride => Process.ProcessFactory.DefaultProcessFactory;

            protected override IPropertySystemCollection<PropertyId, ShellPropertyGroup> ObjectPropertySystemOverride => null;

            protected override IBrowsabilityOptions BrowsabilityOverride => BrowsabilityOptions.BrowsableByDefault;

            protected override bool IsRecursivelyBrowsableOverride => false;

            protected override IBrowsableObjectInfoBitmapSources BitmapSourcesOverride => GetValueIfNotDisposed(_bitmapSources
#if CS8
            ??=
#else
            ?? (_bitmapSources =
#endif
            new BrowsableObjectInfoBitmapBitmapSources(InnerObjectGeneric)
#if !CS8
            )
#endif
            );

            protected override string ItemTypeNameOverride => "Bitmap";

            protected override string DescriptionOverride => "Bitmap";

            protected override bool IsSpecialItemOverride => false;

            public override string LocalizedName { get; }

            protected override System.Collections.Generic.IEnumerable<IProcessInfo> CustomProcessesOverride => null;

            public BitmapInfo(IIconImageInfo parent, in string name, in Bitmap bitmap) : base($"{parent.Path}\\{name}", parent.ClientVersion)
            {
                LocalizedName = name;

                _bitmap = bitmap;

                _parent = parent;
            }

            protected override ArrayBuilder<IBrowsableObjectInfo> GetRootItemsOverride()
            {
                var arrayBuilder = new ArrayBuilder<IBrowsableObjectInfo>();

                _ = arrayBuilder.AddLast(Parent.Parent.Parent);

                return arrayBuilder;
            }

            protected override void DisposeUnmanaged()
            {
                _bitmapSources.Dispose();
                _bitmapSources = null;

                _parent = null;

                base.DisposeUnmanaged();
            }

            protected override void DisposeManaged()
            {
                _bitmap = null;

                base.DisposeManaged();
            }

            protected override IEnumerableSelectorDictionary<object, IBrowsableObjectInfo> GetSelectorDictionaryOverride() => null;

            protected override System.Collections.Generic.IEnumerable<object> GetItemProviders() => null;

            protected override System.Collections.Generic.IEnumerable<object> GetItemProviders(Predicate<object> predicate) => null;

            protected override System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> GetSubRootItemsOverride() => null;
        }

        public interface IIconInfo : IBrowsableObjectInfo<object, Icon, object, IEnumerableSelectorDictionary<object, IBrowsableObjectInfo>, object>
        {
            // Left empty.
        }

        public class IconInfo : BrowsableObjectInfo<object, Icon, object, IEnumerableSelectorDictionary<object, IBrowsableObjectInfo>, object>, IIconInfo
        {
            private Icon _icon;
            private IBrowsableObjectInfo _parent;
            private IBrowsableObjectInfoBitmapSources _bitmapSources;

            public override string Name => LocalizedName;

            protected override IBrowsableObjectInfo ParentOverride => _parent;

            protected override Icon InnerObjectGenericOverride => _icon;

            protected override object ObjectPropertiesGenericOverride => null;

            protected override System.Collections.Generic.IEnumerable<IBrowsabilityPath> BrowsabilityPathsOverride => null;

            protected override IProcessFactory ProcessFactoryOverride => Process.ProcessFactory.DefaultProcessFactory;

            protected override IPropertySystemCollection<PropertyId, ShellPropertyGroup> ObjectPropertySystemOverride => null;

            protected override IBrowsabilityOptions BrowsabilityOverride => BrowsabilityOptions.BrowsableByDefault;

            protected override bool IsRecursivelyBrowsableOverride => false;

            protected override IBrowsableObjectInfoBitmapSources BitmapSourcesOverride => GetValueIfNotDisposed(_bitmapSources
#if CS8
            ??=
#else
            ?? (_bitmapSources =
#endif
            new BrowsableObjectInfoIconBitmapSources(InnerObjectGeneric)
#if !CS8
            )
#endif
            );

            protected override string ItemTypeNameOverride => "Icon";

            protected override string DescriptionOverride => "Icon";

            protected override bool IsSpecialItemOverride => false;

            public override string LocalizedName { get; }

            protected override System.Collections.Generic.IEnumerable<IProcessInfo> CustomProcessesOverride => null;

            public IconInfo(in IIconImageInfo parent, in string name, in Icon icon) : base($"{parent.Path}\\{name}", parent.ClientVersion)
            {
                LocalizedName = name;

                _icon = icon;

                _parent = parent;
            }

            protected override ArrayBuilder<IBrowsableObjectInfo> GetRootItemsOverride()
            {
                var arrayBuilder = new ArrayBuilder<IBrowsableObjectInfo>();

                _ = arrayBuilder.AddLast(Parent.Parent.Parent);

                return arrayBuilder;
            }

            protected override void DisposeUnmanaged()
            {
                _bitmapSources.Dispose();
                _bitmapSources = null;

                _parent = null;

                base.DisposeUnmanaged();
            }

            protected override void DisposeManaged()
            {
                _icon = null;

                base.DisposeManaged();
            }

            protected override IEnumerableSelectorDictionary<object, IBrowsableObjectInfo> GetSelectorDictionaryOverride() => null;

            protected override System.Collections.Generic.IEnumerable<object> GetItemProviders() => null;

            protected override System.Collections.Generic.IEnumerable<object> GetItemProviders(Predicate<object> predicate) => null;

            protected override System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> GetSubRootItemsOverride() => null;
        }
    }
}
