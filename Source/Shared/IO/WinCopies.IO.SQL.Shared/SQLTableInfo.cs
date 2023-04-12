using System;
using System.Collections.Generic;
using System.Linq;

#region WinCopies
using WinCopies.Collections.Generic;
using WinCopies.Data.SQL;
using WinCopies.IO.ComponentSources.Bitmap;
using WinCopies.IO.ComponentSources.Item;
using WinCopies.IO.ObjectModel;
using WinCopies.IO.PropertySystem;
using WinCopies.PropertySystem;
using WinCopies.Util;
#endregion WinCopies

using ISQLColumn = WinCopies.Data.SQL.ISQLColumnInfo;

namespace WinCopies.IO.SQL.ObjectModel
{
    public interface ISQLTableInfo<TObjectProperties, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> : IBrowsableObjectInfo<ISQLDatabaseInfo, TObjectProperties, ISQLTable, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> where TObjectProperties : ISQLTableInfoProperties where TSelectorDictionary : IEnumerableSelectorDictionary<TDictionaryItems, IBrowsableObjectInfo>
    {
        // Left empty.
    }

    public interface ISQLTableInfoProperties : IFileSystemObjectInfoProperties
    {
        // Left empty.
    }

    public class SQLTableInfoProperties : SQLProperties<ISQLTable>, ISQLTableInfoProperties
    {
        public FileType FileType => FileType.Folder;

        public SQLTableInfoProperties(in ISQLTable innerObject) : base(innerObject) { /* Left empty. */ }
    }

    public interface ISQLTableInfo : ISQLTableInfo<ISQLTableInfoProperties, ISQLColumn, IEnumerableSelectorDictionary<IBrowsableObjectInfo, IBrowsableObjectInfo>, IBrowsableObjectInfo>
    {
        // Left empty.
    }

    public abstract class SQLTableInfo<TObjectProperties, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> : SQLItemInfo<ISQLDatabaseInfo, TObjectProperties, ISQLTable, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems>, ISQLTableInfo<TObjectProperties, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> where TObjectProperties : ISQLTableInfoProperties where TSelectorDictionary : IEnumerableSelectorDictionary<TDictionaryItems, IBrowsableObjectInfo>
    {
        public override string Name => InnerObjectGeneric.Name;

        public override string LocalizedName => Name;

        protected override bool IsLocalRootOverride => false;

        protected override IBitmapSourceProvider BitmapSourceProviderOverride => Shell.ComponentSources.Bitmap.BitmapSourceProvider.Create(this);

        protected override IBrowsabilityOptions BrowsabilityOverride => BrowsabilityOptions.BrowsableByDefault;

        protected override IEnumerable<IBrowsabilityPath>
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

        protected override ISQLDatabaseInfo ParentGenericOverride { get; }

        protected SQLTableInfo(in ISQLDatabaseInfo database, in ISQLTable table) : base(table, $"{database.Path}{System.IO.Path.DirectorySeparatorChar}{table.Name}", database.ClientVersion) => ParentGenericOverride = database;

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

    public class SQLRowInfo : BrowsableObjectInfo5<ISQLTableInfo>
    {
        public override string LocalizedName => Name;

        public override string Name { get; }

        protected override IItemSourcesProvider
#if CS8
            ?
#endif
            ItemSourcesOverride => null;

        protected override ISQLTableInfo ParentGenericOverride { get; }

        protected override bool IsLocalRootOverride => false;

        protected override IBitmapSourceProvider BitmapSourceProviderOverride =>  Shell.ComponentSources.Bitmap.BitmapSourceProvider.Create(this);

        protected override IBrowsabilityOptions BrowsabilityOverride => BrowsabilityOptions.NotBrowsable;

        protected override IEnumerable<IBrowsabilityPath>
#if CS8
            ?
#endif
            BrowsabilityPathsOverride => null;

        protected override string DescriptionOverride => $"{ParentGenericOverride.InnerObject.Connection.Normalization} Table";

        protected override object
#if CS8
            ?
#endif
            InnerObjectOverride => null;

        protected override bool IsRecursivelyBrowsableOverride => false;

        protected override bool IsSpecialItemOverride => false;

        protected override string ItemTypeNameOverride => DescriptionOverride;

        protected override object
#if CS8
            ?
#endif
            ObjectPropertiesOverride => null;

        protected override IPropertySystemCollection<PropertyId, ShellPropertyGroup>
#if CS8
            ?
#endif
            ObjectPropertySystemOverride
        { get; }

        private SQLRowInfo(in ISQLTableInfo table, in string name, in IPropertySystemCollection<PropertyId, ShellPropertyGroup> properties) : base($"{(table ?? throw new ArgumentNullException(nameof(table))).Path}{System.IO.Path.DirectorySeparatorChar}{name}", table.ClientVersion)
        {
            ParentGenericOverride = table;
            Name = name;
            ObjectPropertySystemOverride = properties;
        }

        public static string GetName(in IPropertySystemCollection<PropertyId, ShellPropertyGroup> properties)
        {
            var _properties = (properties ?? throw new ArgumentNullException(nameof(properties))).AsFromType<IEnumerable<IProperty>>();

            return ((_properties.FirstOrDefault(_property => typeof(string).IsAssignableFrom(_property.Type)) ?? _properties.FirstOrDefault()) is IProperty property ? property.Value?.ToString() ?? "<Null value>" : null) ?? "<Empty item>";
        }

        internal static SQLRowInfo Create(in ISQLTableInfo table, in IPropertySystemCollection<PropertyId, ShellPropertyGroup> properties) => new SQLRowInfo(table, GetName(properties), properties);

        protected override ArrayBuilder<IBrowsableObjectInfo>
#if CS8
            ?
#endif
            GetRootItemsOverride() => null;
        protected override IEnumerable<IBrowsableObjectInfo>
#if CS8
            ?
#endif
            GetSubRootItemsOverride() => null;
    }

    public class SQLTableInfo : SQLTableInfo<ISQLTableInfoProperties, ISQLColumn, IEnumerableSelectorDictionary<IBrowsableObjectInfo, IBrowsableObjectInfo>, IBrowsableObjectInfo>, ISQLTableInfo
    {
        public class ItemSource : ItemSourceBase3<ISQLTableInfo, ISQLColumn, IEnumerableSelectorDictionary<IBrowsableObjectInfo, IBrowsableObjectInfo>, IBrowsableObjectInfo>
        {
            public override bool IsPaginationSupported => true;

            protected override IProcessSettings
#if CS8
                ?
#endif
                ProcessSettingsOverride => null;

            public override bool IsDisposed => false;

            public ItemSource(in ISQLTableInfo browsableObjectInfo) : base(browsableObjectInfo) { /* Left empty. */ }

            protected override System.Collections.Generic.IEnumerable<IBrowsableObjectInfo>
#if CS8
                ?
#endif
                GetItemProviders(Predicate<ISQLColumn> predicate)
            {
                ISQLTable table = BrowsableObjectInfo.InnerObject;

                table.Connection.UseDB(table.Database.Name);

                string tableName = table.Name;
                int itemCount;

                using (ISQLGetter getter = table.Connection.GetCountSelect(tableName).ExecuteQuery(false).First())

                    itemCount = (int)getter[0];

                PropertyId[] getColumns()
                {
                    var _columns = new ArrayBuilder<PropertyId>();

                    foreach (ISQLColumn column in table.AsEnumerable())

                        _ = _columns.AddLast(new PropertyId(column.Name, ShellPropertyGroup.Default));

                    return _columns.ToArray();
                }

                PropertyId[] columns = getColumns();

                ISelect select = table.Connection.GetSelect(SQLItemCollection.GetCollection(tableName), null);

                select.Interval = Interval;

                var dic = new MultiValueDictionary<PropertyId, IProperty>(columns, itemCount);
                MultiValueDictionary<PropertyId, IProperty>.Dictionary _dic;

                int i;
                string name;
                ISQLGetter _getter;

                void add(in FuncIn<int, string, object> func)
                {
                    for (i = 0; i < columns.Length; i++)

                        _dic[i] = new PropertyInfo<ShellPropertyGroup>((name = dic.Keys[i].Name), name, null, ShellPropertyGroup.Default, func(i, name)).GetProperty();
                }

                void addI() => add((in int _i, in string key) => _getter[_i]);
                void addS() => add((in int _i, in string key) => _getter[key]);

                int j = 0;

                foreach (ISQLGetter __getter in select.ExecuteQuery(true))
                {
                    _dic = dic[j++];

                    if ((_getter = __getter).IsIntIndexable)

                        addI();

                    else if (_getter.IsStringIndexable)

                        addS();

                    yield return SQLRowInfo.Create(BrowsableObjectInfo, new Dictionary(_dic));
                }
            }

            protected override void DisposeManaged() { /* Left empty. */ }
        }

        public static IEnumerableSelectorDictionary<IBrowsableObjectInfo, IBrowsableObjectInfo> DefaultItemSelectorDictionary { get; } = new EnumerableSelectorDictionary<IBrowsableObjectInfo>();

        protected override IItemSourcesProvider<ISQLColumn> ItemSourcesGenericOverride { get; }

        protected override ISQLTableInfoProperties ObjectPropertiesGenericOverride { get; }

        public SQLTableInfo(in ISQLDatabaseInfo database, in ISQLTable table) : base(database, table)
        {
            ObjectPropertiesGenericOverride = new SQLTableInfoProperties(table);

            ItemSourcesGenericOverride = new ItemSourcesProvider<ISQLColumn, IEnumerableSelectorDictionary<IBrowsableObjectInfo, IBrowsableObjectInfo>, IBrowsableObjectInfo>(new ItemSource(this), new ItemSource<ISQLTableInfo, ISQLTable, ISQLColumn, IEnumerableSelectorDictionary<IBrowsableObjectInfo, IBrowsableObjectInfo>, IBrowsableObjectInfo>(this, (b, c) => new SQLColumnInfo(b, c)));
        }

        protected override IEnumerableSelectorDictionary<IBrowsableObjectInfo, IBrowsableObjectInfo> GetSelectorDictionaryOverride() => DefaultItemSelectorDictionary;
    }
}
