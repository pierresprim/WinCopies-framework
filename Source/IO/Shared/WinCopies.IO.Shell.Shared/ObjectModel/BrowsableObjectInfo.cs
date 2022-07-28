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
#region Microsoft
using Microsoft.WindowsAPICodePack.Shell;
using Microsoft.WindowsAPICodePack.Win32Native.Shell;
#endregion Microsoft

#region System
using System;
using System.Drawing;
using System.Windows.Media.Imaging;
#endregion System

#region WinCopies
using WinCopies.IO.ComponentSources.Bitmap;
using WinCopies.IO.ComponentSources.Item;
using WinCopies.IO.ObjectModel;
#endregion WinCopies

#region Static Usings
using static Microsoft.WindowsAPICodePack.Shell.KnownFolders;

using static WinCopies.Collections.Enumerable;
using static WinCopies.IO.ObjectModel.BrowsableObjectInfo;
#endregion Static Usings
#endregion Usings

namespace WinCopies.IO.Shell
{
    namespace ComponentSources.Bitmap
    {
        public class BrowsableObjectInfoIconBitmapSources : BitmapSources<Icon>
        {
            protected override BitmapSource
#if CS8
                ?
#endif
                SmallOverride => TryGetBitmapSource(InnerObject);

            protected override BitmapSource
#if CS8
                ?
#endif
                MediumOverride => TryGetBitmapSource(InnerObject);

            protected override BitmapSource
#if CS8
                ?
#endif
                LargeOverride => TryGetBitmapSource(InnerObject);

            protected override BitmapSource
#if CS8
                ?
#endif
                ExtraLargeOverride => TryGetBitmapSource(InnerObject);

            public BrowsableObjectInfoIconBitmapSources(in Icon icon) : base(icon) { /* Left empty. */ }
        }
    }

    public class BrowsableObjectInfoPlugin : IO.BrowsableObjectInfoPlugin
    {
        public override IBitmapSourceProvider BitmapSourceProvider => null;

        public override System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> GetStartPages(ClientVersion clientVersion) => GetEnumerable<IBrowsableObjectInfo>(ShellObjectInfo.GetDefault(clientVersion), new RegistryItemInfo(), new WMIItemInfo());

        public override System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> GetProtocols(IBrowsableObjectInfo parent, ClientVersion clientVersion) => GetEnumerable<IBrowsableObjectInfo>(new ObjectModel.FileProtocolInfo(parent, clientVersion), new ObjectModel.ShellProtocolInfo(parent, clientVersion));

        public BrowsableObjectInfoPlugin()
        {
            RegisterBrowsableObjectInfoSelectorsStack.Push(ObjectModel.BrowsableObjectInfo.RegisterDefaultBrowsableObjectInfoSelector);
            RegisterProcessSelectorsStack.Push(ShellObjectInfo.RegisterDefaultProcessSelectors);
        }
    }

    namespace ObjectModel
    {
        public class FileProtocolInfo : ProtocolInfo
        {
            protected override IItemSourcesProvider ItemSourcesOverride { get; }

            public FileProtocolInfo(in IBrowsableObjectInfo parent, in ClientVersion clientVersion) : base("file", parent, clientVersion) => ItemSourcesOverride = new ItemSourcesProvider(() =>
            {
                ShellObjectInfo getShellObjectInfo(in IKnownFolder knownFolder) => new
#if !CS9
                    ShellObjectInfo
#endif
                    (knownFolder, ClientVersion);

                return GetEnumerable(getShellObjectInfo(KnownFolders.Desktop), getShellObjectInfo(Computer), getShellObjectInfo(UsersFiles), getShellObjectInfo(UsersLibraries));
            });
        }

        public class ShellProtocolInfo : ProtocolInfo
        {
            protected override IItemSourcesProvider ItemSourcesOverride { get; }

            public ShellProtocolInfo(in IBrowsableObjectInfo parent, in ClientVersion clientVersion) : base("shell", parent, clientVersion) => ItemSourcesOverride = new ItemSourcesProvider(GetItems);

            private System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> GetItems()
            {
                ShellObjectInfo
#if CS8
                    ?
#endif
                    shellObjectInfo;

                void trySetShellObjectInfo(in IKnownFolder _knownFolder)
                {
                    try
                    {
                        shellObjectInfo = new ShellObjectInfo(_knownFolder, ClientVersion);
                    }

                    catch (ShellException)
                    {
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
                DefaultBrowsableObjectInfoSelectorDictionary.Push(item => Predicate(item, typeof(Consts.Shell.Protocols)), TryGetBrowsableObjectInfo

                // System.Reflection.Assembly.GetExecutingAssembly().DefinedTypes.FirstOrDefault(t => t.Namespace.StartsWith(typeof(Process.ObjectModel.IProcess).Namespace) && t.GetCustomAttribute<ProcessGuidAttribute>().Guid == guid);
                );

                RegisterDefaultBrowsableObjectInfoSelector = Delegates.EmptyVoid;
            };

            public static IBrowsableObjectInfo GetBrowsableObjectInfo(string path) => ShellObjectInfo.From(ShellObjectFactory.Create(path));

            public static IBrowsableObjectInfo
#if CS8
                ?
#endif
                TryGetBrowsableObjectInfo(BrowsableObjectInfoURL3 url)
            {
                BrowsableObjectInfoURL2 _url = url.URL;
#if CS9
                return
#else
                switch (
#endif
                    _url.Protocol
#if CS9
                    switch
#else
                    )
#endif
                    {
#if !CS9
                    case
#endif
                        Consts.Shell.Protocols.SHELL
#if CS9
                        or
#else
                        :
                    case
#endif
                        Consts.Shell.Protocols.FILE
#if CS9
                        =>
#else
                        :
                        return
#endif
                        GetBrowsableObjectInfo(_url.URL.URI)
#if CS9
                        ,
#else
                        ;
                    case
#endif
                        Consts.Shell.Protocols.REGISTRY
#if CS9
                        =>
#else
                        :
                        return
#endif
                        new RegistryItemInfo(_url.URL.URI, url.ClientVersion)
#if CS9
                        ,
#else
                        ;
                    case
#endif
                        Consts.Shell.Protocols.WMI
#if CS9
                        =>
#else
                        :
                        return
#endif
                        WMIItemInfo.GetWMIItemInfo(WMIItemInfo.RootPath, _url.URL.URI, null, url.ClientVersion)
#if CS9
                        ,
                        _ =>
#else
                        ;
                    default:
                        return
#endif
                        null
#if CS9
                    };
#else
                        ;
                }
#endif
            }

            public static IBrowsableObjectInfo GetBrowsableObjectInfo(BrowsableObjectInfoURL3 url) => TryGetBrowsableObjectInfo(url) ?? throw new InvalidOperationException(Resources.ExceptionMessages.NoBrowsableObjectInfoCouldBeGenerated);

            public static IBrowsableObjectInfoPlugin GetPlugInParameters() => new BrowsableObjectInfoPlugin();
        }
    }
}
