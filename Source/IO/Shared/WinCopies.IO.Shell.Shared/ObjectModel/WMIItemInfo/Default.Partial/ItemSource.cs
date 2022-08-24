using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;

using WinCopies.IO.AbstractionInterop;
using WinCopies.IO.ComponentSources.Item;
using WinCopies.IO.PropertySystem;
using WinCopies.Linq;

namespace WinCopies.IO.ObjectModel
{
    public partial class WMIItemInfo
    {
        public class ItemSource : ItemSourceBase4<IWMIItemInfo, ManagementBaseObject, IEnumerableSelectorDictionary<WMIItemInfoItemProvider, IBrowsableObjectInfo>, WMIItemInfoItemProvider>
        {
            private const string EXCEPTION_MESSAGE = "No properties found.";

            public override bool IsPaginationSupported => false;

            protected override IProcessSettings
#if CS8
                    ?
#endif
                ProcessSettingsOverride
            { get; }

            public ItemSource(in IWMIItemInfo browsableObjectInfo) : base(browsableObjectInfo)
            {
                _ = ValidateProperties(() => new ArgumentException(EXCEPTION_MESSAGE, nameof(browsableObjectInfo)));

                ProcessSettingsOverride = new ProcessSettings(null, DefaultCustomProcessesSelectorDictionary.Select(BrowsableObjectInfo));
            }

            private IWMIItemInfoProperties ValidateProperties(in Func<Exception> func) => BrowsableObjectInfo.ObjectProperties ?? throw func();
            protected IWMIItemInfoProperties ValidateProperties() => ValidateProperties(() => new InvalidOperationException(EXCEPTION_MESSAGE));

            protected internal virtual IEnumerable<WMIItemInfoItemProvider>
#if CS8
                    ?
#endif
                GetItemProviders(Predicate<ManagementBaseObject> predicate, IWMIItemInfoOptions
#if CS8
                    ?
#endif
                options)
            {
                IWMIItemInfoProperties objectProperties = ValidateProperties();

                // var paths = new ArrayBuilder<PathInfo>();

                // string _path;

                bool dispose = false;

                IWMIItemInfo browsableObjectInfo = BrowsableObjectInfo;

                if (
#if !CS9
                    !(
#endif
                    browsableObjectInfo.InnerObject is
#if CS9
                        not
#endif
                    ManagementClass managementClass
#if !CS9
                    )
#endif
                    )
                {
                    dispose = true;

                    managementClass = GetManagementClassFromPath(browsableObjectInfo.Path, options);
                }

                managementClass.Get();

                try
                {
#if CS8
                        static
#endif
                    IEnumerable<ManagementBaseObject> _as(in ManagementObjectCollection collection) => collection.OfType<ManagementBaseObject>();

#if CS8
                        static
#endif
                    IEnumerable<ManagementBaseObject> enumerateInstances(in ManagementClass _managementClass, in IWMIItemInfoOptions
#if CS8
                        ?
#endif
                    _options) => _as(_options?.EnumerationOptions == null ? _managementClass.GetInstances() : _managementClass.GetInstances(_options?.EnumerationOptions));

                    IEnumerable<WMIItemInfoItemProvider> getEnumerable(in IEnumerable<ManagementBaseObject> enumerable, WMIItemType itemType) => enumerable.SelectConverter(item => new WMIItemInfoItemProvider(null, item, itemType, objectProperties.Options, browsableObjectInfo.ClientVersion));

                    if (objectProperties.ItemType == WMIItemType.Namespace)
                    {
                        IEnumerable<ManagementBaseObject> namespaces = enumerateInstances(managementClass, options);

                        IEnumerable<ManagementBaseObject> classes = _as(options?.EnumerationOptions == null ? managementClass.GetSubclasses() : managementClass.GetSubclasses(options?.EnumerationOptions));

                        if (predicate != null)
                        {
                            namespaces = namespaces.WherePredicate(predicate);

                            classes = classes.WherePredicate(predicate);
                        }

                        return getEnumerable(namespaces, WMIItemType.Namespace).AppendValues(getEnumerable(classes, WMIItemType.Class));
                    }

                    else if (objectProperties.ItemType == WMIItemType.Class /*&& WMIItemTypes.HasFlag(WMIItemTypes.Instance)*/)
                    {
                        managementClass.Get();

                        IEnumerable<ManagementBaseObject> items = enumerateInstances(managementClass, options);

                        if (predicate != null)

                            items = items.WherePredicate(predicate);

                        return getEnumerable(items, WMIItemType.Instance);
                    }

                    return null;
                }

                finally
                {
                    if (dispose)

                        managementClass.Dispose();
                }
            }

            protected override IEnumerable<WMIItemInfoItemProvider>
#if CS8
                    ?
#endif
                GetItemProviders(Predicate<ManagementBaseObject> predicate) => GetItemProviders(predicate, BrowsableObjectInfo.ObjectProperties?.Options);

            protected override IEnumerable<WMIItemInfoItemProvider>
#if CS8
                    ?
#endif
                GetItemProviders() => GetItemProviders(Bool.True, ValidateProperties().Options);

            internal new IEnumerable<IBrowsableObjectInfo>
#if CS8
                    ?
#endif
                GetItems(IEnumerable<WMIItemInfoItemProvider>
#if CS8
                    ?
#endif
                items) => base.GetItems(items);
        }
    }
}
