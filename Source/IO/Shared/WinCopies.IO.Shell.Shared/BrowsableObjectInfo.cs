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

using Microsoft.WindowsAPICodePack.Shell;
using Microsoft.WindowsAPICodePack.Win32Native.Shell;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Media.Imaging;
using WinCopies.Collections.Generic;
using WinCopies.GUI.Drawing;
using WinCopies.IO.ObjectModel;
using WinCopies.IO.Process;
using WinCopies.IO.PropertySystem;
using WinCopies.Linq;
using WinCopies.PropertySystem;
using static Microsoft.WindowsAPICodePack.Shell.KnownFolders;

using static WinCopies.IO.ObjectModel.BrowsableObjectInfo;

namespace WinCopies.IO.Shell
{
    public class BrowsableObjectInfoIconBitmapSources : BitmapSources<Icon>
    {
        protected override BitmapSource SmallOverride => TryGetBitmapSource(InnerObject);

        protected override BitmapSource MediumOverride => TryGetBitmapSource(InnerObject);

        protected override BitmapSource LargeOverride => TryGetBitmapSource(InnerObject);

        protected override BitmapSource ExtraLargeOverride => TryGetBitmapSource(InnerObject);

        public BrowsableObjectInfoIconBitmapSources(in Icon icon) : base(icon) { /* Left empty. */ }
    }

    public class BrowsableObjectInfoPlugin : IO.BrowsableObjectInfoPlugin
    {
        public override IBitmapSourceProvider BitmapSourceProvider => null;

        public override System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> GetStartPages(ClientVersion clientVersion) => Collections.Generic.Enumerable<IBrowsableObjectInfo>.GetEnumerable(ShellObjectInfo.GetDefault(clientVersion), new RegistryItemInfo(), new WMIItemInfo());

        public override System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> GetProtocols(IBrowsableObjectInfo parent, ClientVersion clientVersion) => Collections.Generic.Enumerable<IBrowsableObjectInfo>.GetEnumerable(new ObjectModel.FileProtocolInfo(parent, clientVersion), new ObjectModel.ShellProtocolInfo(parent, clientVersion));

        public BrowsableObjectInfoPlugin()
        {
            RegisterBrowsabilityPathsStack.Push(ShellObjectInfo.RegisterDefaultBrowsabilityPaths);

            RegisterBrowsableObjectInfoSelectorsStack.Push(ShellObjectInfo.RegisterDefaultBrowsabilityPaths);

            RegisterProcessSelectorsStack.Push(ShellObjectInfo.RegisterDefaultProcessSelectors);
        }
    }

    namespace ObjectModel
    {
        public class FileProtocolInfo : ProtocolInfo
        {
            public FileProtocolInfo(in IBrowsableObjectInfo parent, in ClientVersion clientVersion) : base(null, parent, clientVersion) { /* Left empty. */ }

            protected override System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> GetItemsOverride()
            {
                ShellObjectInfo getShellObjectInfo(in IKnownFolder knownFolder) => new ShellObjectInfo(knownFolder, ClientVersion);

                return Collections.Generic.Enumerable<ShellObjectInfo>.GetEnumerable(getShellObjectInfo(KnownFolders.Desktop), getShellObjectInfo(Computer), getShellObjectInfo(UsersFiles), getShellObjectInfo(UsersLibraries));
            }
        }

        public class ShellProtocolInfo : ProtocolInfo
        {
            public ShellProtocolInfo(in IBrowsableObjectInfo parent, in ClientVersion clientVersion) : base("shell", parent, clientVersion) { /* Left empty. */ }

            protected override System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> GetItemsOverride()
            {
                ShellObjectInfo shellObjectInfo;

                void trySetShellObjectInfo(in IKnownFolder _knownFolder)
                {
                    try
                    {
                        shellObjectInfo = new ShellObjectInfo(_knownFolder, ClientVersion);
                    }

                    catch (ShellException ex)
                    {
                        System.Type type = ex.GetType();

                        shellObjectInfo = null;
                    }
                }

                foreach (IKnownFolder knownFolder in All)
                {
                    trySetShellObjectInfo(knownFolder);

                    if (shellObjectInfo != null)

                        yield return shellObjectInfo;
                }
            }
        }

        public static class BrowsableObjectInfo
        {
            public static Action RegisterDefaultBrowsableObjectInfoSelector { get; private set; } = () =>
            {
                DefaultBrowsableObjectInfoSelectorDictionary.Push(item => Predicate(item, typeof(Consts.Guids.Shell.Process.Shell)), TryGetBrowsableObjectInfo

                // System.Reflection.Assembly.GetExecutingAssembly().DefinedTypes.FirstOrDefault(t => t.Namespace.StartsWith(typeof(Process.ObjectModel.IProcess).Namespace) && t.GetCustomAttribute<ProcessGuidAttribute>().Guid == guid);
                );

                RegisterDefaultBrowsableObjectInfoSelector = EmptyVoid;
            };

            private static void EmptyVoid() { /* Left empty. */ }

            public static IBrowsableObjectInfo GetBrowsableObjectInfo(string path) => ShellObjectInfo.From(ShellObjectFactory.Create(path));

            public static IBrowsableObjectInfo TryGetBrowsableObjectInfo(BrowsableObjectInfoURL3 url)
            {
                BrowsableObjectInfoURL2 _url = url.URL;

                switch (_url.Protocol)
                {
                    case Consts.Protocols.SHELL:

                        return GetBrowsableObjectInfo(_url.URL.URI);

                    case Consts.Protocols.REGISTRY:

                        return new RegistryItemInfo(_url.URL.URI, url.ClientVersion);

                    case Consts.Protocols.WMI:

                        return WMIItemInfo.GetWMIItemInfo(WMIItemInfo.RootPath, _url.URL.URI, null, url.ClientVersion);
                }

                return null;
            }

            public static IBrowsableObjectInfo GetBrowsableObjectInfo(BrowsableObjectInfoURL3 url) => TryGetBrowsableObjectInfo(url) ?? throw new InvalidOperationException(Resources.ExceptionMessages.NoBrowsableObjectInfoCouldBeGenerated);

            public static IBrowsableObjectInfoPlugin GetPlugInParameters() => new BrowsableObjectInfoPlugin();

            public static Icon TryGetIcon(in int iconIndex, in string dll, in System.Drawing.Size size) => IO.ObjectModel.BrowsableObjectInfo.TryGetIcon(new IconExtractor(IO.Path.GetRealPathFromEnvironmentVariables(IO.Path.System32Path + dll)).GetIcon(iconIndex), size);

            public static BitmapSource TryGetBitmapSource(in int iconIndex, in string dllName, in int size)
            {
                using
#if !CS8
            (
#endif
                Icon icon = TryGetIcon(iconIndex, dllName, new System.Drawing.Size(size, size))
#if CS8
            ;
#else
            )
#endif

                return IO.ObjectModel.BrowsableObjectInfo.TryGetBitmapSource(icon);
            }
        }

        public abstract class AppBrowsableObjectInfo<T> : BrowsableObjectInfo3<T>
        {
            public sealed override string Protocol => "about";

            public override string URI => Path;

            protected AppBrowsableObjectInfo(in T innerObject, in string path, in ClientVersion clientVersion) : base(innerObject, path, clientVersion) { /* Left empty. */ }
        }

        public abstract class PluginInfo<T> : BrowsableObjectInfo3<T>, IEncapsulatorBrowsableObjectInfo<IBrowsableObjectInfoPlugin> where T : IBrowsableObjectInfoPlugin
        {
            public override string LocalizedName => GetName(InnerObjectGenericOverride.GetType().Assembly);

            public override string Name => LocalizedName;

            protected override bool IsLocalRootOverride => false;

            protected override IBitmapSourceProvider BitmapSourceProviderOverride => InnerObjectGenericOverride.BitmapSourceProvider;

            protected override IBrowsabilityOptions BrowsabilityOverride => BrowsabilityOptions.BrowsableByDefault;

            protected override System.Collections.Generic.IEnumerable<IBrowsabilityPath> BrowsabilityPathsOverride => null;

            protected override System.Collections.Generic.IEnumerable<IProcessInfo> CustomProcessesOverride => null;

            protected override string DescriptionOverride => InnerObjectGeneric.GetType().Assembly.GetCustomAttributes<AssemblyDescriptionAttribute>().FirstOrDefault()?.Description;

            protected override bool IsRecursivelyBrowsableOverride => true;

            protected override bool IsSpecialItemOverride => false;

            protected override string ItemTypeNameOverride => "Plug-in Start Page";

            protected override object ObjectPropertiesOverride => null;

            protected override IPropertySystemCollection<PropertyId, ShellPropertyGroup> ObjectPropertySystemOverride => null;

            protected override IProcessFactory ProcessFactoryOverride => null;

            public override string Protocol => "plugin";

            public override string URI { get; }

            IBrowsableObjectInfoPlugin IEncapsulatorBrowsableObjectInfo<IBrowsableObjectInfoPlugin>.InnerObject => InnerObjectGeneric;

            private PluginInfo(in T plugin, in ClientVersion clientVersion, Assembly assembly) : base(plugin, GetName(assembly), clientVersion) => URI = GetURI(assembly);

            protected PluginInfo(in T plugin, in ClientVersion clientVersion) : this(plugin, clientVersion, (plugin
#if CS8
                ??
#else
                == null ?
#endif
                throw ThrowHelper.GetArgumentNullException(nameof(plugin))
#if !CS8
                : plugin
#endif
                ).GetType().Assembly)
            { /* Left empty. */ }

            public static string GetName(in Assembly assembly)
            {
                AssemblyName name = (assembly ?? throw ThrowHelper.GetArgumentNullException(nameof(assembly))).GetName();

                return name.Name ?? name.FullName;
            }

            public static string GetURI(in Assembly assembly) => assembly.GetCustomAttributes<GuidAttribute>().FirstOrDefault()?.Value ?? GetName(assembly);

            protected override System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> GetItemsOverride() => InnerObjectGenericOverride.GetStartPages(ClientVersion).AppendValues(InnerObjectGenericOverride.GetProtocols(this, ClientVersion));

            protected override ArrayBuilder<IBrowsableObjectInfo> GetRootItemsOverride() => null;

            protected override System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> GetSubRootItemsOverride() => null;
        }

        public abstract class BrowsableObjectInfoStartPage<T> : AppBrowsableObjectInfo<System.Collections.Generic.IEnumerable<T>> where T : IEncapsulatorBrowsableObjectInfo<IBrowsableObjectInfoPlugin>
        {
            public override string LocalizedName => "Start Here";

            public override string Name => LocalizedName;

            protected override bool IsLocalRootOverride => true;

            protected override IBrowsabilityOptions BrowsabilityOverride => BrowsabilityOptions.BrowsableByDefault;

            protected override System.Collections.Generic.IEnumerable<IBrowsabilityPath> BrowsabilityPathsOverride => null;

            protected override System.Collections.Generic.IEnumerable<IProcessInfo> CustomProcessesOverride => null;

            protected override string DescriptionOverride => "This is the start page of the Explorer window. Here you can find all the root browsable items from the plug-ins you have installed.";

            protected override bool IsRecursivelyBrowsableOverride => true;

            protected override bool IsSpecialItemOverride => false;

            protected override string ItemTypeNameOverride => "Start Page";

            protected override object ObjectPropertiesOverride => null;

            protected override IPropertySystemCollection<PropertyId, ShellPropertyGroup> ObjectPropertySystemOverride => null;

            protected override IBrowsableObjectInfo ParentOverride => null;

            protected override IProcessFactory ProcessFactoryOverride => null;

            protected BrowsableObjectInfoStartPage(in System.Collections.Generic.IEnumerable<T> plugins, in ClientVersion clientVersion) : base(plugins, "start", clientVersion) { /* Left empty. */ }

            protected override System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> GetItemsOverride() => InnerObjectGenericOverride.As<IBrowsableObjectInfo>();

            protected override ArrayBuilder<IBrowsableObjectInfo> GetRootItemsOverride() => null;

            protected override System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> GetSubRootItemsOverride() => null;
        }
    }
}
