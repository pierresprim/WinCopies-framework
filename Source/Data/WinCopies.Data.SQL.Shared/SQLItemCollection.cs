using System.Collections;
using System.Text;

using WinCopies.Temp;

using static WinCopies.ThrowHelper;

namespace WinCopies.Data.SQL
{
    public struct SQLColumn : ISQLColumn
    {
        public string? TableName { get; set; }

        public string ColumnName { get; set; }

        public string? Alias { get; set; }

        public string? Decorator { get; set; }

        public SQLColumn(in string? tableName, in string columnName, in string? alias, in string? decorator = null)
        {
            TableName = tableName;

            ColumnName = columnName;

            Alias = alias;

            Decorator = decorator;
        }

        public SQLColumn(in string column, in string? decorator = null) : this(null, column, null, decorator) { /* Left empty. */ }

        public string GetDecoratedValue(in string value) => Decorator == null ? value : value.Surround(Decorator);

        public override string ToString()
        {
            StringBuilder sb = new();

            if (TableName != null)

                _ = sb.Append(GetDecoratedValue(TableName) + '.');

            _ = sb.Append(GetDecoratedValue(ColumnName));

            if (Alias != null)

                _ = sb.Append(GetDecoratedValue(Alias));

            return sb.ToString();
        }

        SQLColumn ISQLColumn.ToSQLColumn() => this;
    }

    public class SQLItemCollection<T> : IExtensibleEnumerable<T>
    {
        private IExtensibleEnumerable<T> _items;

        public IExtensibleEnumerable<T> Items { get => _items; set => _items = value ?? throw GetArgumentNullException(nameof(value)); }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public SQLItemCollection(in IExtensibleEnumerable<T> items) => Items = items; // The null-check is performed by the Items property.
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public SQLItemCollection(in IEnumerable<T> items) : this(new TO_BE_DELETED.LinkedList<T>(items)) { /* Left empty. */ }

        public SQLItemCollection(params T[] items) : this(new TO_BE_DELETED.LinkedList<T>(items)) { /* Left empty. */ }

        IEnumerator<T> IEnumerable<T>.GetEnumerator() => Items.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => Items.GetEnumerator();

        public string ToString(Converter<T, string?> converter)
        {
            var sb = new StringBuilder();

            ActionIn<T> action = (in T value) =>
            {
                action = (in T value) => sb.Append($", {converter(value)}");

                _ = sb.Append(converter(value));
            };

            foreach (T item in Items)

                action(item);

            return sb.ToString();
        }

        public override string ToString() => ToString(Temp.Delegates.ToStringT);

        //public static SQLItemCollection<T> GetCollectionForItems(System.Collections.Generic.IEnumerable<T> items) => new(TO_BE_DELETED.LinkedList<T>.From(items));

        //public static SQLItemCollection<T> GetCollectionForItems(params T[] items) => GetCollectionForItems((System.Collections.Generic.IEnumerable<T>)items);

        void IPrependableExtensibleEnumerable<T>.Prepend(T item) => _items.Prepend(item);

        void IPrependableExtensibleEnumerable<T>.PrependRange(IEnumerable<T> items) => _items.PrependRange(items);

        void IAppendableExtensibleEnumerable<T>.Append(T item) => _items.Append(item);

        void IAppendableExtensibleEnumerable<T>.AppendRange(IEnumerable<T> items) => _items.AppendRange(items);
    }
}
