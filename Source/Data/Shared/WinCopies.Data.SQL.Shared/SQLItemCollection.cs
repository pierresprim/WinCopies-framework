using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using static WinCopies.ThrowHelper;

namespace WinCopies.Data.SQL
{
    public struct SQLColumn : ISQLColumn
    {
        public string
#if CS8
            ?
#endif
            TableName
        { get; set; }

        public string ColumnName { get; set; }

        public string
#if CS8
            ?
#endif
            Alias
        { get; set; }

        public string
#if CS8
            ?
#endif
            Decorator
        { get; set; }

        public SQLColumn(in string
#if CS8
            ?
#endif
            tableName, in string columnName, in string
#if CS8
            ?
#endif
            alias, in string
#if CS8
            ?
#endif
            decorator = null)
        {
            TableName = tableName;

            ColumnName = columnName;

            Alias = alias;

            Decorator = decorator;
        }

        public SQLColumn(in string column, in string
#if CS8
            ?
#endif
            decorator = null) : this(null, column, null, decorator) { /* Left empty. */ }

        public string GetDecoratedValue(in string value) => Decorator == null ? value : value.Surround(Decorator);

        public override string ToString()
        {
            StringBuilder sb = new
#if !CS9
                StringBuilder
#endif
                ();

            if (TableName != null)

                _ = sb.Append(GetDecoratedValue(TableName) + '.');

            _ = sb.Append(GetDecoratedValue(ColumnName));

            if (Alias != null)

                _ = sb.Append(GetDecoratedValue(Alias));

            return sb.ToString();
        }

        SQLColumn ISQLColumn.ToSQLColumn() => this;
    }

    public static class SQLItemCollection
    {
        public static SQLItemCollection<T> GetCollection<T>(in Collections.Generic.IExtensibleEnumerable<T> items) => new SQLItemCollection<T>(items);
        public static SQLItemCollection<T> GetCollection<T>(in IEnumerable<T> items) => new SQLItemCollection<T>(items);
        public static SQLItemCollection<T> GetCollection<T>(params T[] items) => new SQLItemCollection<T>(items);
    }

    public class SQLItemCollection<T> : Collections.Generic.IExtensibleEnumerable<T>
    {
        private Collections.Generic.IExtensibleEnumerable<T> _items;

        public Collections.Generic.IExtensibleEnumerable<T> Items { get => _items; set => _items = value ?? throw GetArgumentNullException(nameof(value)); }

#if CS8
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#endif
        public SQLItemCollection(in Collections.Generic.IExtensibleEnumerable<T> items) => Items = items; // The null-check is performed by the Items property.
#if CS8
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#endif
        public SQLItemCollection(in IEnumerable<T> items) : this(new Collections.DotNetFix.Generic.LinkedList<T>(items)) { /* Left empty. */ }
        public SQLItemCollection(params T[] items) : this(items.AsEnumerable()) { /* Left empty. */ }

        IEnumerator<T> IEnumerable<T>.GetEnumerator() => Items.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => Items.GetEnumerator();

        public string ToString(Converter<T, string
#if CS8
            ?
#endif
            > converter)
        {
            var sb = new StringBuilder();

            ActionIn<T> action = (in T value) =>
            {
                action = (in T _value) => sb.Append($", {converter(_value)}");

                _ = sb.Append(converter(value));
            };

            foreach (T item in Items)

                action(item);

            return sb.ToString();
        }

        public override string ToString() => ToString(Delegates.ToStringT);

        //public static SQLItemCollection<T> GetCollectionForItems(System.Collections.Generic.IEnumerable<T> items) => new(TO_BE_DELETED.LinkedList<T>.From(items));

        //public static SQLItemCollection<T> GetCollectionForItems(params T[] items) => GetCollectionForItems((System.Collections.Generic.IEnumerable<T>)items);

        void Collections.Generic.IPrependableExtensibleEnumerable<T>.Prepend(T item) => _items.Prepend(item);

        void Collections.Generic.IPrependableExtensibleEnumerable<T>.PrependRange(IEnumerable<T> items) => _items.PrependRange(items);

        void Collections.Generic.IAppendableExtensibleEnumerable<T>.Append(T item) => _items.Append(item);

        void Collections.Generic.IAppendableExtensibleEnumerable<T>.AppendRange(IEnumerable<T> items) => _items.AppendRange(items);
    }
}
