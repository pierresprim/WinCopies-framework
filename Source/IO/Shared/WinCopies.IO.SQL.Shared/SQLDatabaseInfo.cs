using System;

using WinCopies.Collections.Generic;
using WinCopies.Data.SQL;
using WinCopies.IO.ComponentSources.Bitmap;
using WinCopies.IO.ComponentSources.Item;
using WinCopies.IO.ObjectModel;
using WinCopies.IO.Process;
using WinCopies.IO.PropertySystem;
using WinCopies.Linq;
using WinCopies.PropertySystem;

namespace WinCopies.IO.SQL.ObjectModel
{
    public interface ISQLDatabaseInfo<TObjectProperties, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> : IBrowsableObjectInfo<ISQLConnectionInfo, TObjectProperties, ISQLDatabase, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> where TObjectProperties : ISQLDatabaseInfoProperties where TSelectorDictionary : IEnumerableSelectorDictionary<TDictionaryItems, IBrowsableObjectInfo>
    {
        // Left empty.
    }

    public interface ISQLProperties
    {
        string Collation { get; }
    }

    public interface ISQLDatabaseInfoProperties : IFileSystemObjectInfoProperties, ISQLProperties
    {
        string CharacterSet { get; }
    }

    public class SQLProperties<T> : BrowsableObjectInfoProperties<T> where T : ISQLItemProperties
    {
        public string Collation => InnerObject.Collation;

        public Size? Size => null;

        public SQLProperties(in T innerObject) : base(innerObject) { /* Left empty. */ }
    }

    public class SQLDatabaseInfoProperties : SQLProperties<ISQLDatabase>, ISQLDatabaseInfoProperties
    {
        public string CharacterSet => InnerObject.CharacterSet;

        public FileType FileType => FileType.Folder;

        public Size? Size => null;

        public SQLDatabaseInfoProperties(in ISQLDatabase innerObject) : base(innerObject) { /* Left empty. */ }
    }

    public interface ISQLDatabaseInfo : ISQLDatabaseInfo<ISQLDatabaseInfoProperties, ISQLTable, IEnumerableSelectorDictionary<IBrowsableObjectInfo, IBrowsableObjectInfo>, IBrowsableObjectInfo>
    {
        // Left empty.
    }

    public abstract class SQLDatabaseInfo<TObjectProperties, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> : SQLItemInfo<ISQLConnectionInfo, TObjectProperties, ISQLDatabase, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems>, ISQLDatabaseInfo<TObjectProperties, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> where TObjectProperties : ISQLDatabaseInfoProperties where TSelectorDictionary : IEnumerableSelectorDictionary<TDictionaryItems, IBrowsableObjectInfo>
    {
        public override string Name => InnerObjectGeneric.Name;

        public override string LocalizedName => Name;

        protected override bool IsLocalRootOverride => false;

        protected override IBitmapSourceProvider BitmapSourceProviderOverride => Shell.ComponentSources.Bitmap.BitmapSourceProvider.Create(this);

        protected override IBrowsabilityOptions BrowsabilityOverride => BrowsabilityOptions.BrowsableByDefault;

        protected override System.Collections.Generic.IEnumerable<IBrowsabilityPath>
#if CS8
            ?
#endif
            BrowsabilityPathsOverride => null;

        protected override string DescriptionOverride => $"{ParentGenericOverride.InnerObject.Normalization} Database";

        protected override bool IsRecursivelyBrowsableOverride => true;

        protected override bool IsSpecialItemOverride => false;

        protected override string ItemTypeNameOverride => DescriptionOverride;

        protected override IPropertySystemCollection<PropertyId, ShellPropertyGroup> ObjectPropertySystemOverride => null;

        protected override ISQLConnectionInfo ParentGenericOverride { get; }

        protected SQLDatabaseInfo(in ISQLConnectionInfo connection, in ISQLDatabase database) : base(database, $"{connection.Name}{System.IO.Path.DirectorySeparatorChar}{database.Name}", connection.ClientVersion) => ParentGenericOverride = connection;

        protected override ArrayBuilder<IBrowsableObjectInfo> GetRootItemsOverride() => null;
        protected override System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> GetSubRootItemsOverride() => null;
    }

    public class SQLDatabaseInfo : SQLDatabaseInfo<ISQLDatabaseInfoProperties, ISQLTable, IEnumerableSelectorDictionary<IBrowsableObjectInfo, IBrowsableObjectInfo>, IBrowsableObjectInfo>, ISQLDatabaseInfo
    {
        public static IEnumerableSelectorDictionary<IBrowsableObjectInfo, IBrowsableObjectInfo> DefaultItemSelectorDictionary { get; } = new EnumerableSelectorDictionary<IBrowsableObjectInfo>();

        protected override IItemSourcesProvider<ISQLTable> ItemSourcesGenericOverride { get; }

        protected override ISQLDatabaseInfoProperties ObjectPropertiesGenericOverride { get; }

        public SQLDatabaseInfo(in ISQLConnectionInfo connection, in ISQLDatabase database) : base(connection, database)
        {
            ObjectPropertiesGenericOverride = new SQLDatabaseInfoProperties(database);

            ItemSourcesGenericOverride = ItemSourcesProvider.Construct(new ItemSource<ISQLDatabaseInfo, ISQLDatabase, ISQLTable, IEnumerableSelectorDictionary<IBrowsableObjectInfo, IBrowsableObjectInfo>, IBrowsableObjectInfo>(this, (b, t) => new SQLTableInfo(b, t)));
        }

        protected override IEnumerableSelectorDictionary<IBrowsableObjectInfo, IBrowsableObjectInfo> GetSelectorDictionaryOverride() => DefaultItemSelectorDictionary;
    }
}
