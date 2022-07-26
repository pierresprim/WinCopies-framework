using System;

#region WinCopies
using WinCopies.Collections.Generic;
using WinCopies.Data.SQL;
using WinCopies.IO.ComponentSources.Bitmap;
using WinCopies.IO.ComponentSources.Item;
using WinCopies.IO.ObjectModel;
using WinCopies.IO.PropertySystem;
using WinCopies.Linq;
using WinCopies.PropertySystem;
#endregion WinCopies

namespace WinCopies.IO.SQL
{
    public class ItemSource<TBrowsableObjectInfo, TInnerObject, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> : ItemSourceBase4<TBrowsableObjectInfo, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> where TBrowsableObjectInfo : IBrowsableObjectInfo<TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems>, IEncapsulatorBrowsableObjectInfo<TInnerObject> where TSelectorDictionary : IEnumerableSelectorDictionary<TDictionaryItems, IBrowsableObjectInfo> where TInnerObject : IAsEnumerable<TPredicateTypeParameter>
    {
        private readonly Func<TBrowsableObjectInfo, TPredicateTypeParameter, TDictionaryItems> _func;

        public override bool IsPaginationSupported => false;

        protected override IProcessSettings
#if CS8
            ?
#endif
            ProcessSettingsOverride => null;

        public ItemSource(in TBrowsableObjectInfo browsableObjectInfo, in Func<TBrowsableObjectInfo, TPredicateTypeParameter, TDictionaryItems> func) : base(browsableObjectInfo) => _func = func;

        protected override System.Collections.Generic.IEnumerable<TDictionaryItems>
#if CS8
            ?
#endif
            GetItemProviders(Predicate<TPredicateTypeParameter> predicate) => BrowsableObjectInfo.InnerObject.AsEnumerable().WhereSelectPredicateConverter(predicate, db => _func(BrowsableObjectInfo, db));
    }

    public interface ISQLConnectionInfoProperties : IFileSystemObjectInfoProperties
    {
        bool IsOpen { get; }

        #region Granted Actions
        bool CanUseDB { get; }
        bool CanCreateDB { get; }
        bool CanSelect { get; }
        bool CanCount { get; }
        bool CanInsert { get; }
        bool CanUpdate { get; }
        bool CanDelete { get; }
        #endregion
    }

    public class SQLConnectionInfoProperties : BrowsableObjectInfoProperties<ISQLConnection>, ISQLConnectionInfoProperties
    {
        public bool IsOpen => !InnerObject.IsClosed;

        public FileType FileType => FileType.Folder;

        public Size? Size => null;

        #region Granted Actions
        public bool CanUseDB => InnerObject.CanUseDB;
        public bool CanCreateDB => InnerObject.CanCreateDB;
        public bool CanSelect => InnerObject.CanSelect;
        public bool CanCount => InnerObject.CanCount;
        public bool CanInsert => InnerObject.CanInsert;
        public bool CanUpdate => InnerObject.CanUpdate;
        public bool CanDelete => InnerObject.CanDelete;
        #endregion

        public SQLConnectionInfoProperties(in ISQLConnection connection) : base(connection) { /* Left empty. */ }
    }

    namespace ObjectModel
    {
        public interface ISQLConnectionInfo<TObjectProperties, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> : IBrowsableObjectInfo<IBrowsableObjectInfo, TObjectProperties, ISQLConnection, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> where TObjectProperties : ISQLConnectionInfoProperties where TSelectorDictionary : IEnumerableSelectorDictionary<TDictionaryItems, IBrowsableObjectInfo>
        {
            // Left empty.
        }

        public interface ISQLConnectionInfo : ISQLConnectionInfo<ISQLConnectionInfoProperties, ISQLDatabase, IEnumerableSelectorDictionary<IBrowsableObjectInfo, IBrowsableObjectInfo>, IBrowsableObjectInfo>
        {
            // Left empty.
        }

        public abstract class SQLItemInfo<TParent, TObjectProperties, TInnerObject, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> : BrowsableObjectInfo2<TParent, TObjectProperties, TInnerObject, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> where TParent : IBrowsableObjectInfo where TSelectorDictionary : IEnumerableSelectorDictionary<TDictionaryItems, IBrowsableObjectInfo>
        {
            public override string Protocol => "sql";

            protected SQLItemInfo(in TInnerObject innerObject, in string path, in ClientVersion clientVersion) : base(innerObject, path, clientVersion) { /* Left empty. */ }
        }

        public abstract class SQLConnectionInfo<TObjectProperties, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> : SQLItemInfo<IBrowsableObjectInfo, TObjectProperties, ISQLConnection, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems>, ISQLConnectionInfo<TObjectProperties, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> where TObjectProperties : ISQLConnectionInfoProperties where TSelectorDictionary : IEnumerableSelectorDictionary<TDictionaryItems, IBrowsableObjectInfo>
        {
            public override string Name => InnerObjectGeneric.ServerName;

            public override string LocalizedName => Name;

            protected override bool IsLocalRootOverride => true;

            protected override IBitmapSourceProvider BitmapSourceProviderOverride => Shell.ComponentSources.Bitmap.BitmapSourceProvider.Create(this);

            protected override IBrowsabilityOptions BrowsabilityOverride => BrowsabilityOptions.BrowsableByDefault;

            protected override System.Collections.Generic.IEnumerable<IBrowsabilityPath>
#if CS8
                ?
#endif
                BrowsabilityPathsOverride => null;

            protected override string DescriptionOverride => $"{ItemTypeNameOverride}/{InnerObjectGenericOverride.ServerName}";

            protected override bool IsRecursivelyBrowsableOverride => true;

            protected override bool IsSpecialItemOverride => false;

            protected override string ItemTypeNameOverride => $"SQL/{InnerObjectGenericOverride.Normalization} DB";

            protected override IPropertySystemCollection<PropertyId, ShellPropertyGroup>
#if CS8
                ?
#endif
                ObjectPropertySystemOverride => null;

            protected override IBrowsableObjectInfo
#if CS8
                ?
#endif
                ParentGenericOverride => null;

            protected SQLConnectionInfo(in ISQLConnection innerObject, in ClientVersion clientVersion) : base(innerObject, innerObject.ServerName, clientVersion) { /* Left empty. */ }

            protected override ArrayBuilder<IBrowsableObjectInfo> GetRootItemsOverride() => null;
            protected override System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> GetSubRootItemsOverride() => null;
        }

        public class SQLConnectionInfo : SQLConnectionInfo<ISQLConnectionInfoProperties, ISQLDatabase, IEnumerableSelectorDictionary<IBrowsableObjectInfo, IBrowsableObjectInfo>, IBrowsableObjectInfo>, ISQLConnectionInfo
        {
            public static IEnumerableSelectorDictionary<IBrowsableObjectInfo, IBrowsableObjectInfo> DefaultItemSelectorDictionary { get; } = new EnumerableSelectorDictionary<IBrowsableObjectInfo>();

            protected override IItemSourcesProvider<ISQLDatabase> ItemSourcesGenericOverride { get; }

            protected override ISQLConnectionInfoProperties ObjectPropertiesGenericOverride { get; }

            public SQLConnectionInfo(in ISQLConnection innerObject, in ClientVersion clientVersion) : base(innerObject, clientVersion)
            {
                ObjectPropertiesGenericOverride = new SQLConnectionInfoProperties(innerObject);

                ItemSourcesGenericOverride = ItemSourcesProvider.Construct(new ItemSource<ISQLConnectionInfo, ISQLConnection, ISQLDatabase, IEnumerableSelectorDictionary<IBrowsableObjectInfo, IBrowsableObjectInfo>, IBrowsableObjectInfo>(this, (b, db) => new SQLDatabaseInfo(b, db)));
            }

            protected override IEnumerableSelectorDictionary<IBrowsableObjectInfo, IBrowsableObjectInfo> GetSelectorDictionaryOverride() => DefaultItemSelectorDictionary;
        }
    }
}
