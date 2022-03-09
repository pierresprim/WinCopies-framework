using WinCopies.Collections.DotNetFix.Generic;
using WinCopies.Collections.Generic;
using WinCopies.Temp;

namespace WinCopies.TO_BE_DELETED
{
    public static class Path
    {
        public const char PathSeparator = '\\';
    }

    public class LinkedList<T> : Collections.DotNetFix.Generic.LinkedList<T>, IExtensibleEnumerable<T>
    {
        public LinkedList() : base() { /* Left empty. */ }

        public LinkedList(System.Collections.Generic.IEnumerable<T> collection) : base(collection) { /* Left empty. */ }

        public LinkedList(params T[] collection) : this((System.Collections.Generic.IEnumerable<T>)collection) { /* Left empty. */ }

        //public static LinkedList<T> From(in System.Collections.Generic.IEnumerable<T> values)
        //{
        //    LinkedList<T> result = new();

        //    _ = result.AddRangeLast(values);

        //    return result;
        //}

        //public static LinkedList<T> From(params T[] values) => From(values);

        protected ArrayBuilder<LinkedListNode> AddRange(System.Collections.Generic.IEnumerable<T> values, FuncIn<T, LinkedListNode> action)
        {
            ArrayBuilder<LinkedListNode> builder = new();

            foreach (T item in values)

                _ = builder.AddLast(action(item));

            return builder;
        }

        protected void AddRange(in System.Collections.Generic.IEnumerable<LinkedListNode> values, in ActionIn<LinkedListNode> action)
        {
            foreach (LinkedListNode item in values)

                action(item);
        }


        protected ArrayBuilder<LinkedListNode> AddRange(LinkedListNode node, in System.Collections.Generic.IEnumerable<T> values, Func<LinkedListNode, T, LinkedListNode> action) => AddRange(values, (in T value) => action(node, value));

        protected void AddRange(LinkedListNode node, in System.Collections.Generic.IEnumerable<LinkedListNode> values, Action<LinkedListNode, LinkedListNode> action) => AddRange(values, (in LinkedListNode newNode) => action(node, newNode));



        public ArrayBuilder<LinkedListNode> AddRangeFirst(in System.Collections.Generic.IEnumerable<T> values) => AddRange(values, AddFirst);

        public void AddRangeFirst(in System.Collections.Generic.IEnumerable<LinkedListNode> values) => AddRange(values, AddFirst);

        public ArrayBuilder<LinkedListNode> AddRangeLast(in System.Collections.Generic.IEnumerable<T> values) => AddRange(values, AddLast);

        public void AddRangeLast(in System.Collections.Generic.IEnumerable<LinkedListNode> values) => AddRange(values, AddLast);



        public System.Collections.Generic.IEnumerable<LinkedListNode> AddRangeBefore(in LinkedListNode node, in System.Collections.Generic.IEnumerable<T> values) => AddRange(node, values, AddBefore);

        public void AddRangeBefore(in LinkedListNode node, in System.Collections.Generic.IEnumerable<LinkedListNode> values) => AddRange(node, values, AddBefore);

        public System.Collections.Generic.IEnumerable<LinkedListNode> AddRangeAfter(in LinkedListNode node, in System.Collections.Generic.IEnumerable<T> values) => AddRange(node, values, AddAfter);

        public void AddRangeAfter(in LinkedListNode node, in System.Collections.Generic.IEnumerable<LinkedListNode> values) => AddRange(node, values, AddAfter);



        void IAppendableExtensibleEnumerable<T>.Append(T item) => AddLast(item);

        void IAppendableExtensibleEnumerable<T>.AppendRange(System.Collections.Generic.IEnumerable<T> items) => AddRangeLast(items);



        void IPrependableExtensibleEnumerable<T>.Prepend(T item) => AddFirst(item);

        void IPrependableExtensibleEnumerable<T>.PrependRange(System.Collections.Generic.IEnumerable<T> items) => AddRangeFirst(items);
    }

    public class LinkedListTEMP<T> : IUIntCountableEnumerable<T>
    {
        public  readonly ILinkedList<T> List;

        public uint Count => List.Count;

        public LinkedListTEMP(in ILinkedList<T> list) => List = list;

        public IUIntCountableEnumerator<T> GetEnumerator() => List.GetEnumerator();
    }
}
