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
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with the WinCopies Framework. If not, see <https://www.gnu.org/licenses/>. */

#region Usings
using Microsoft.Win32;

#region WinCopies
using WinCopies.Collections.Generic;
using WinCopies.IO.AbstractionInterop;
using WinCopies.IO.ComponentSources.Item;
using WinCopies.IO.Process;
using WinCopies.IO.PropertySystem;
using WinCopies.IO.Selectors;
using WinCopies.PropertySystem;
#endregion WinCopies
#endregion Usings

namespace WinCopies.IO.ObjectModel
{
    public partial class RegistryItemInfo : RegistryItemInfo<IRegistryItemInfoProperties, RegistryItemInfoItemProvider, IEnumerableSelectorDictionary<RegistryItemInfoItemProvider, IBrowsableObjectInfo>, RegistryItemInfoItemProvider>, IRegistryItemInfo
    {
        #region Fields
        private ItemSource _itemSource;
        private IItemSourcesProvider<RegistryItemInfoItemProvider> _itemSources;
        private IRegistryItemInfoProperties _objectProperties;
        private static readonly BrowsabilityPathStack<IRegistryItemInfo> __browsabilityPathStack = new
#if !CS9
            BrowsabilityPathStack<IRegistryItemInfo>
#endif
            ();
        private static readonly WriteOnlyBrowsabilityPathStack<IRegistryItemInfo> _browsabilityPathStack = __browsabilityPathStack.AsWriteOnly();
        #endregion

        #region Properties
        public static IBrowsabilityPathStack<IRegistryItemInfo> BrowsabilityPathStack => _browsabilityPathStack;

        protected override IItemSourcesProvider<RegistryItemInfoItemProvider> ItemSourcesGenericOverride => _itemSources;

        public static IEnumerableSelectorDictionary<RegistryItemInfoItemProvider, IBrowsableObjectInfo> DefaultItemSelectorDictionary { get; } = new RegistryItemInfoSelectorDictionary();

        public static ISelectorDictionary<IRegistryItemInfoBase, System.Collections.Generic.IEnumerable<IProcessInfo>> DefaultCustomProcessesSelectorDictionary { get; } = new DefaultNullableValueSelectorDictionary<IRegistryItemInfoBase, System.Collections.Generic.IEnumerable<IProcessInfo>>();

        protected sealed override IRegistryItemInfoProperties ObjectPropertiesGenericOverride => _objectProperties;

        protected override IPropertySystemCollection<PropertyId, ShellPropertyGroup>
#if CS8
            ?
#endif
            ObjectPropertySystemOverride => null;

        protected override System.Collections.Generic.IEnumerable<IBrowsabilityPath> BrowsabilityPathsOverride => __browsabilityPathStack.GetBrowsabilityPaths(this);

        private RegistryItemType Properties
        {
#if CS9
            init
#else
            set
#endif
            {
                _objectProperties = new RegistryItemInfoProperties<IRegistryItemInfoBase>(this, value);
                SetItemSources();
            }
        }
        #endregion Properties

        #region Constructors
        public RegistryItemInfo() : this(GetDefaultClientVersion()) => SetItemSources();

        /// <summary>
        /// Initializes a new instance of the <see cref="RegistryItemInfo"/> class as the Registry root.
        /// </summary>
        public RegistryItemInfo(in ClientVersion clientVersion) : base(clientVersion) => Properties = RegistryItemType.Root;

        ///// <summary>
        ///// Initializes a new instance of the <see cref="RegistryItemInfo"/> class using a custom factory for <see cref="RegistryItemInfo"/>s.
        ///// </summary>
        ///// <param name="registryKey">The <see cref="Microsoft.Win32.RegistryKey"/> that the new <see cref="RegistryItemInfo"/> represents.</param>
        public RegistryItemInfo(in RegistryKey registryKey, in ClientVersion clientVersion) : base(registryKey, clientVersion) => Properties = RegistryItemType.Key;

        ///// <summary>
        ///// Initializes a new instance of the <see cref="RegistryItemInfo"/> class using a custom factory for <see cref="RegistryItemInfo"/>s.
        ///// </summary>
        ///// <param name="path">The path of the <see cref="Microsoft.Win32.RegistryKey"/> that the new <see cref="RegistryItemInfo"/> represents.</param>
        public RegistryItemInfo(in string path, in ClientVersion clientVersion) : base(path, clientVersion) => Properties = RegistryItemType.Key;

        ///// <summary>
        ///// Initializes a new instance of the <see cref="RegistryItemInfo"/> class using a custom factory for <see cref="RegistryItemInfo"/>s.
        ///// </summary>
        ///// <param name="registryKey">The <see cref="Microsoft.Win32.RegistryKey"/> that the new <see cref="RegistryItemInfo"/> represents.</param>
        ///// <param name="valueName">The name of the value that the new <see cref="RegistryItemInfo"/> represents.</param>
        public RegistryItemInfo(in RegistryKey registryKey, in string valueName, in ClientVersion clientVersion) : base(registryKey, valueName, clientVersion) => Properties = RegistryItemType.Value;

        ///// <summary>
        ///// Initializes a new instance of the <see cref="RegistryItemInfo"/> class using a custom factory for <see cref="RegistryItemInfo"/>s.
        ///// </summary>
        ///// <param name="registryKeyPath">The path of the <see cref="Microsoft.Win32.RegistryKey"/> that the new <see cref="RegistryItemInfo"/> represents.</param>
        ///// <param name="valueName">The name of the value that the new <see cref="RegistryItemInfo"/> represents.</param>
        public RegistryItemInfo(in string registryKeyPath, in string valueName, in ClientVersion clientVersion) : base(registryKeyPath, valueName, clientVersion) => Properties = RegistryItemType.Value;
        #endregion Constructors

        #region Methods
        public override IBrowsableObjectInfo Clone()
#if CS8
            =>
#else
        {
            switch (
#endif
            ObjectPropertiesGeneric.RegistryItemType
#if CS8
        switch
#else
        )
#endif
            {
#if !CS8
                case
#endif
                RegistryItemType.Key
#if CS8
            =>
#else
        :
                    return
#endif
                                    new RegistryItemInfo(InnerObjectGenericOverride, ClientVersion)
#if CS8
                ,
#else
                            ;
                case
#endif

                RegistryItemType.Root
#if CS8
            =>
#else
        :
                    return
#endif
                                    new RegistryItemInfo()
#if CS8
                ,
                _ =>
#else
                            ;
                default:
                    return
#endif
                    new RegistryItemInfo(InnerObjectGenericOverride, Name, ClientVersion)
#if CS8
            };
#else
                ;
            }
        }
#endif

        private void SetItemSources() => _itemSources = ItemSourcesProvider.Construct(_itemSource = new ItemSource(this));

        public static ArrayBuilder<IBrowsableObjectInfo> GetDefaultRootItems()
        {
            var arrayBuilder = new ArrayBuilder<IBrowsableObjectInfo>();

            _ = arrayBuilder.AddLast(new RegistryItemInfo(GetDefaultClientVersion()));

            return arrayBuilder;
        }

        protected override IEnumerableSelectorDictionary<RegistryItemInfoItemProvider, IBrowsableObjectInfo> GetSelectorDictionaryOverride() => DefaultItemSelectorDictionary;

        protected override System.Collections.Generic.IEnumerable<IBrowsableObjectInfo>
#if CS8
            ?
#endif
            GetSubRootItemsOverride() => _itemSource.GetItems(_itemSource._GetItemProviders(item => item.RegistryKey != null && item.ValueName == null));

        protected override void DisposeUnmanaged()
        {
            if (_objectProperties != null)
            {
                _objectProperties.Dispose();
                _objectProperties = null;
            }

            base.DisposeUnmanaged();
        }
        #endregion Methods
    }
}
