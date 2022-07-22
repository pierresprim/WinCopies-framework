#region Usings
using System;

#region WinCopies
using WinCopies.Collections.Generic;
using WinCopies.Data.SQL;
using WinCopies.IO.ComponentSources.Bitmap;
using WinCopies.IO.ComponentSources.Item;
using WinCopies.IO.ObjectModel;
using WinCopies.IO.Process;
using WinCopies.IO.PropertySystem;
using WinCopies.PropertySystem;
#endregion WinCopies

using ISQLColumn = WinCopies.Data.SQL.ISQLColumnInfo;
#endregion Usings

namespace WinCopies.IO.SQL.ObjectModel
{
    public interface ISQLColumnInfo<TObjectProperties, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> : IBrowsableObjectInfo<ISQLTableInfo, TObjectProperties, ISQLColumn, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> where TObjectProperties : ISQLColumnInfoProperties where TSelectorDictionary : IEnumerableSelectorDictionary<TDictionaryItems, IBrowsableObjectInfo>
    {
        // Left empty.
    }

    public interface ISQLColumnInfoProperties : IFileSystemObjectInfoProperties
    {
        bool AllowNull { get; }

        bool AutoIncrement { get; }

        ISQLColumnType ColumnType { get; }

        object
#if CS8
            ?
#endif
            Default
        { get; }

        bool IsPrimaryKey { get; }
    }

    public class SQLColumnInfoProperties : SQLProperties<ISQLColumn>, ISQLColumnInfoProperties
    {
        public bool AllowNull => InnerObject.AllowNull;

        public bool AutoIncrement => InnerObject.AutoIncrement;

        public ISQLColumnType ColumnType => InnerObject.ColumnType;

        public object
#if CS8
            ?
#endif
            Default => InnerObject.Default;

        public bool IsPrimaryKey => InnerObject.IsPrimaryKey;

        public FileType FileType => FileType.File;

        public SQLColumnInfoProperties(in ISQLColumn innerObject) : base(innerObject) { /* Left empty. */ }
    }

    public interface ISQLColumnInfo : ISQLColumnInfo<ISQLColumnInfoProperties, object, IEnumerableSelectorDictionary<IBrowsableObjectInfo, IBrowsableObjectInfo>, IBrowsableObjectInfo>
    {
        // Left empty.
    }

    public abstract class SQLColumnInfo<TObjectProperties, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> : SQLItemInfo<ISQLTableInfo, TObjectProperties, ISQLColumn, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems>, ISQLColumnInfo<TObjectProperties, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> where TObjectProperties : ISQLColumnInfoProperties where TSelectorDictionary : IEnumerableSelectorDictionary<TDictionaryItems, IBrowsableObjectInfo>
    {
        public override string Name => InnerObjectGeneric.Name;

        public override string LocalizedName => Name;

        protected override bool IsLocalRootOverride => false;

        protected override IBitmapSourceProvider BitmapSourceProviderOverride => new Shell.ComponentSources.Bitmap.BitmapSourceProvider(this);

        protected override IBrowsabilityOptions BrowsabilityOverride => BrowsabilityOptions.BrowsableByDefault;

        protected override System.Collections.Generic.IEnumerable<IBrowsabilityPath>
#if CS8
            ?
#endif
            BrowsabilityPathsOverride => null;

        protected override string DescriptionOverride => $"{ParentGenericOverride.InnerObject.Connection.Normalization} Table";

        protected override bool IsRecursivelyBrowsableOverride => true;

        protected override bool IsSpecialItemOverride => false;

        protected override string ItemTypeNameOverride => DescriptionOverride;

        protected override IPropertySystemCollection<PropertyId, ShellPropertyGroup>
#if CS8
            ?
#endif
            ObjectPropertySystemOverride => null;

        protected override ISQLTableInfo ParentGenericOverride { get; }

        protected SQLColumnInfo(in ISQLTableInfo table, in ISQLColumn column) : base(column, $"{table.Path}{System.IO.Path.DirectorySeparatorChar}{column.Name}", table.ClientVersion) => ParentGenericOverride = table;

        protected override ArrayBuilder<IBrowsableObjectInfo>
#if CS8
            ?
#endif
            GetRootItemsOverride() => null;
        protected override System.Collections.Generic.IEnumerable<IBrowsableObjectInfo>
#if CS8
            ?
#endif
            GetSubRootItemsOverride() => null;
    }

    public class SQLColumnInfo : SQLColumnInfo<ISQLColumnInfoProperties, object, IEnumerableSelectorDictionary<IBrowsableObjectInfo, IBrowsableObjectInfo>, IBrowsableObjectInfo>, ISQLColumnInfo
    {
        public class ItemSource : ItemSourceBase4<ISQLColumnInfo, object, IEnumerableSelectorDictionary<IBrowsableObjectInfo, IBrowsableObjectInfo>, IBrowsableObjectInfo>
        {
            protected override IProcessSettings
#if CS8
                ?
#endif
                ProcessSettingsOverride => null;

            public override bool IsPaginationSupported => false;

            public ItemSource(in ISQLColumnInfo browsableObjectInfo) : base(browsableObjectInfo) { /* Left empty. */ }

            protected override System.Collections.Generic.IEnumerable<IBrowsableObjectInfo>
#if CS8
                    ?
#endif
                    GetItemProviders(Predicate<object> predicate) => null;

            protected override System.Collections.Generic.IEnumerable<IBrowsableObjectInfo>
#if CS8
                    ?
#endif
                    GetItemProviders() => GetItemProviders(Bool.True);
        }

        public static IEnumerableSelectorDictionary<IBrowsableObjectInfo, IBrowsableObjectInfo> DefaultItemSelectorDictionary { get; } = new EnumerableSelectorDictionary<IBrowsableObjectInfo>();

        protected override IItemSourcesProvider<object> ItemSourcesGenericOverride { get; }

        protected override ISQLColumnInfoProperties ObjectPropertiesGenericOverride { get; }

        public SQLColumnInfo(in ISQLTableInfo table, in ISQLColumn column) : base(table, column)
        {
            ObjectPropertiesGenericOverride = new SQLColumnInfoProperties(column);

            ItemSourcesGenericOverride = ItemSourcesProvider.Construct(new ItemSource(this));
        }

        protected override IEnumerableSelectorDictionary<IBrowsableObjectInfo, IBrowsableObjectInfo> GetSelectorDictionaryOverride() => DefaultItemSelectorDictionary;
    }
}
