using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinCopies.IO.FilesProcesses
{
   public class FileSystemInfoWithExceptionsCollection : IList<FileSystemInfo>, ICollection<FileSystemInfo>, IEnumerable<FileSystemInfo>, IEnumerable, IList, ICollection, IReadOnlyList<FileSystemInfo>, IReadOnlyCollection<FileSystemInfo>, INotifyCollectionChanged, INotifyPropertyChanged
    {
        public FileSystemInfo this[int index] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        object IList.this[int index] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public int Count => throw new NotImplementedException();

        public bool IsReadOnly => throw new NotImplementedException();

        public bool IsFixedSize => throw new NotImplementedException();

        public object SyncRoot => throw new NotImplementedException();

        public bool IsSynchronized => throw new NotImplementedException();

        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public event PropertyChangedEventHandler PropertyChanged;

       public void Add(FileSystemInfo item)
        {
            for
        }

        int IList. Add(object value)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(FileSystemInfo item)
        {
            throw new NotImplementedException();
        }

        public bool Contains(object value)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(FileSystemInfo[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<FileSystemInfo> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public int IndexOf(FileSystemInfo item)
        {
            throw new NotImplementedException();
        }

        public int IndexOf(object value)
        {
            throw new NotImplementedException();
        }

        public void Insert(int index, FileSystemInfo item)
        {
            throw new NotImplementedException();
        }

        public void Insert(int index, object value)
        {
            throw new NotImplementedException();
        }

        public void OnPropertyChanged(string propertyName)
        {
            throw new NotImplementedException();
        }

        public void OnPropertyChanged(string propertyName, string fieldName, object previousValue, object newValue)
        {
            throw new NotImplementedException();
        }

        public void OnPropertyChangedReadOnly(string propertyName, object previousValue, object newValue)
        {
            throw new NotImplementedException();
        }

        public bool Remove(FileSystemInfo item)
        {
            throw new NotImplementedException();
        }

        public void Remove(object value)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        protected override void InsertItem(int index, FileSystemInfo item)
        {
            base.InsertItem(index, item);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
