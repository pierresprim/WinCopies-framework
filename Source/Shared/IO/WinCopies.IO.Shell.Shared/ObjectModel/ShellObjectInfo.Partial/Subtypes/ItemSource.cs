using System;
using System.Collections.Generic;
using System.Linq;

using WinCopies.IO.AbstractionInterop;
using WinCopies.IO.ComponentSources.Item;
using WinCopies.IO.Enumeration;
using WinCopies.IO.Process;
using WinCopies.IO.PropertySystem;
using WinCopies.PropertySystem;
using static WinCopies.IO.FileType;

namespace WinCopies.IO.ObjectModel
{
    public partial class ShellObjectInfo
    {
        public class ItemSource : ItemSourceBase3<IShellObjectInfo, ShellObjectInfoEnumeratorStruct, IEnumerableSelectorDictionary<ShellObjectInfoItemProvider, IBrowsableObjectInfo>, ShellObjectInfoItemProvider>
        {
            private IProcessSettings _processSettings;

            public override bool IsPaginationSupported => false;

            public override bool IsDisposed => _processSettings == null;

            protected sealed override IProcessSettings ProcessSettingsOverride => _processSettings;

            public ItemSource(in IShellObjectInfo shellObjectInfo) : base(MAIN, DEFAULT_DATA_SOURCE, shellObjectInfo) => _processSettings = new ProcessSettings(new ShellObjectInfoProcessFactory(shellObjectInfo), DefaultCustomProcessesSelectorDictionary.Select(shellObjectInfo));

            protected internal IEnumerable<ShellObjectInfoItemProvider>
#if CS8
                ?
#endif
                GetItemProviders(Predicate<ArchiveFileInfoEnumeratorStruct> func)
#if CS8
             =>
#else
            {
                switch (
#endif
                BrowsableObjectInfo.ObjectProperties.FileType
#if CS8
                switch
#else
                    )
#endif
                {
#if !CS8
                    case
#endif
                    Archive
#if CS8
                    =>
#else
                    :
                        return
#endif
                    ArchiveItemInfo.GetArchiveItemInfoItems(BrowsableObjectInfo, func).Select(ShellObjectInfoItemProvider.ToShellObjectInfoItemProvider)
#if CS8
                    ,
                    _ =>
#else
                    ;
                    default:
                        return
#endif
                    null
#if CS8
                };
#else
                    ;
                }
            }
#endif

            protected override IEnumerable<ShellObjectInfoItemProvider>
#if CS8
                ?
#endif
                GetItemProviders(Predicate<ShellObjectInfoEnumeratorStruct> predicate) => BrowsableObjectInfo.IsBrowsable()
                ? BrowsableObjectInfo.ObjectProperties?.FileType == Archive
                    ? GetItemProviders(item => predicate(new ShellObjectInfoEnumeratorStruct(item)))
                    : ShellObjectInfoEnumeration.From(BrowsableObjectInfo, predicate)
                : null;

            internal new IEnumerable<ShellObjectInfoItemProvider>
#if CS8
                ?
#endif
                GetItemProviders() => base.GetItemProviders();

            internal IEnumerable<IBrowsableObjectInfo>
#if CS8
                    ?
#endif
                GetItems(in IEnumerable<ShellObjectInfoItemProvider>
#if CS8
                    ?
#endif
                items) => base.GetItems(items);

            protected override void DisposeManaged() => _processSettings = null;
        }
    }
}
