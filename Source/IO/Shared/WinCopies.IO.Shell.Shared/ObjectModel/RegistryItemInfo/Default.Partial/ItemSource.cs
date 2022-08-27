using Microsoft.Win32;

using System;
using System.Security;

using WinCopies.IO.AbstractionInterop;
using WinCopies.IO.ComponentSources.Item;
using WinCopies.IO.PropertySystem;
using WinCopies.Linq;
using WinCopies.Util;

namespace WinCopies.IO.ObjectModel
{
    public partial class RegistryItemInfo
    {
        public class ItemSource : ItemSourceBase4<IRegistryItemInfo, RegistryItemInfoItemProvider, IEnumerableSelectorDictionary<RegistryItemInfoItemProvider, IBrowsableObjectInfo>, RegistryItemInfoItemProvider>
        {
            private IProcessSettings _processSettings;

            public override bool IsPaginationSupported => false;

            protected override IProcessSettings ProcessSettingsOverride => _processSettings
#if CS8
                ??=
#else
                ?? (_processSettings =
#endif
                new ProcessSettings(new _ProcessFactory(BrowsableObjectInfo), DefaultCustomProcessesSelectorDictionary.Select(BrowsableObjectInfo))
#if !CS8
            )
#endif
                ;

            public ItemSource(in IRegistryItemInfo browsableObjectInfo) : base(browsableObjectInfo) { /* Left empty. */ }

            protected override System.Collections.Generic.IEnumerable<RegistryItemInfoItemProvider>
#if CS8
                ?
#endif
                GetItemProviders(Predicate<RegistryItemInfoItemProvider>
#if CS8
                    ?
#endif
                    predicate)
            {
                System.Collections.Generic.IEnumerable<RegistryItemInfoItemProvider> enumerable;

                switch (BrowsableObjectInfo.ObjectProperties.RegistryItemType)
                {
                    case RegistryItemType.Key:

                        //string[] items;

                        System.Collections.Generic.IEnumerable<RegistryItemInfoItemProvider>
#if CS8
                            ?
#endif
                            keys = null;
                        System.Collections.Generic.IEnumerable<RegistryItemInfoItemProvider>
#if CS8
                            ?
#endif
                            values = null;

                        RegistryKey
#if CS8
                            ?
#endif
                            registryKey;

                        if ((registryKey = BrowsableObjectInfo.TryOpenKey().Value) != null)

                            try
                            {
                                keys = registryKey.GetSubKeyNames().Select(item => new RegistryItemInfoItemProvider(item, BrowsableObjectInfo.ClientVersion));

                                values = registryKey.GetValueNames().Select(s => new RegistryItemInfoItemProvider(registryKey, s, BrowsableObjectInfo.ClientVersion) /*new RegistryItemInfo(Path, s)*/);

                                // foreach (string item in items)

                                // item.Substring(0, item.LastIndexOf(IO.Path.PathSeparator)), item.Substring(item.LastIndexOf(IO.Path.PathSeparator) + 1), false
                            }

                            catch (Exception ex) when (ex.Is(false, typeof(SecurityException), typeof(IOException), typeof(UnauthorizedAccessException)))
                            {
                                BrowsableObjectInfo.CloseKey();

                                return null;
                            }

                        //else

                        //    enumerate();

                        if (keys == null)

                            if (values == null)

                                return null;

                            else

                                enumerable = values;

                        else

                            enumerable = values == null ? keys : keys.AppendValues(values);

                        break;

                    case RegistryItemType.Root:

                        enumerable = typeof(Microsoft.Win32.Registry).GetFields().Select(f => new RegistryItemInfoItemProvider((RegistryKey)f.GetValue(null), BrowsableObjectInfo.ClientVersion));

                        break;

                    default:

                        return null;
                }

                return predicate == null ? enumerable : enumerable.WherePredicate(predicate);
            }

            protected override System.Collections.Generic.IEnumerable<RegistryItemInfoItemProvider>
#if CS8
                ?
#endif
                GetItemProviders() => GetItemProviders(null);

            internal new System.Collections.Generic.IEnumerable<IBrowsableObjectInfo>
#if CS8
                ?
#endif
                GetItems(System.Collections.Generic.IEnumerable<RegistryItemInfoItemProvider>
#if CS8
                ?
#endif
                items) => base.GetItems(items);

            internal System.Collections.Generic.IEnumerable<RegistryItemInfoItemProvider>
#if CS8
                ?
#endif
                _GetItemProviders(Predicate<RegistryItemInfoItemProvider>
#if CS8
                    ?
#endif
                    predicate) => GetItemProviders(predicate);

            protected override void DisposeManaged()
            {
                _processSettings = null;

                base.DisposeManaged();
            }
        }
    }
}
