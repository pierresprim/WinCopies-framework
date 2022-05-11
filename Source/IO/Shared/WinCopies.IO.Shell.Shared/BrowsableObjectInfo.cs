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
using System.Windows.Media.Imaging;

using WinCopies.GUI.Drawing;
using WinCopies.IO.ObjectModel;

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

        public override IEnumerable<IBrowsableObjectInfo> GetStartPages(ClientVersion clientVersion) => Collections.Generic.Enumerable<IBrowsableObjectInfo>.GetEnumerable(ShellObjectInfo.GetDefault(clientVersion), new RegistryItemInfo(), new WMIItemInfo());

        public override IEnumerable<IBrowsableObjectInfo> GetProtocols(IBrowsableObjectInfo parent, ClientVersion clientVersion) => Collections.Generic.Enumerable<IBrowsableObjectInfo>.GetEnumerable(new ObjectModel.FileProtocolInfo(parent, clientVersion), new ObjectModel.ShellProtocolInfo(parent, clientVersion));

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

            protected override IEnumerable<IBrowsableObjectInfo> GetItemsOverride()
            {
                ShellObjectInfo getShellObjectInfo(in IKnownFolder knownFolder) => new ShellObjectInfo(knownFolder, ClientVersion);

                return Collections.Generic.Enumerable<ShellObjectInfo>.GetEnumerable(getShellObjectInfo(KnownFolders.Desktop), getShellObjectInfo(Computer), getShellObjectInfo(UsersFiles), getShellObjectInfo(UsersLibraries));
            }
        }

        public class ShellProtocolInfo : ProtocolInfo
        {
            public ShellProtocolInfo(in IBrowsableObjectInfo parent, in ClientVersion clientVersion) : base("shell", parent, clientVersion) { /* Left empty. */ }

            protected override IEnumerable<IBrowsableObjectInfo> GetItemsOverride()
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
    }
}
