using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

using WinCopies.Util;

namespace WinCopies.Collections.Generic
{
    public interface IHeaderedList<THeader, TItems> : IList<IHeaderedList<THeader, TItems>>
    {
        THeader Header { get; set; }
    }

    public class HeaderedList<THeader, TItems, TList> : List<TList>, IHeaderedList<THeader, TItems> where TList : IHeaderedList<THeader, TItems>
    {
        IHeaderedList<THeader, TItems> IList<IHeaderedList<THeader, TItems>>.this[int index] { get => this[index]; set => this[index] = (TList)value; }

        public THeader Header { get; set; }

        bool ICollection<IHeaderedList<THeader, TItems>>.IsReadOnly => false;

        void ICollection<IHeaderedList<THeader, TItems>>.Add(IHeaderedList<THeader, TItems> item) => Add((TList)item);

        bool ICollection<IHeaderedList<THeader, TItems>>.Contains(IHeaderedList<THeader, TItems> item) => item is TList list && Contains(list);

        void ICollection<IHeaderedList<THeader, TItems>>.CopyTo(IHeaderedList<THeader, TItems>[] array, int arrayIndex) => ((IList)this).CopyTo(array, arrayIndex);

        System.Collections.Generic.IEnumerator<IHeaderedList<THeader, TItems>> System.Collections.Generic.IEnumerable<IHeaderedList<THeader, TItems>>.GetEnumerator() => this.ToEnumerable<TList, IHeaderedList<THeader, TItems>>().GetEnumerator();

        int IList<IHeaderedList<THeader, TItems>>.IndexOf(IHeaderedList<THeader, TItems> item) => item is TList list ? IndexOf(list) : -1;

        void IList<IHeaderedList<THeader, TItems>>.Insert(int index, IHeaderedList<THeader, TItems> item) => Insert(index, (TList)item);

        bool ICollection<IHeaderedList<THeader, TItems>>.Remove(IHeaderedList<THeader, TItems> item) => item is TList list && Remove(list);
    }

    public class HeaderedList<THeader, TItems> : HeaderedList<THeader, TItems, HeaderedList<THeader, TItems>>
    {

    }

    public abstract class NavigableObservableHeaderedCollection<THeader, TParent, TItems> : System.Collections.ObjectModel.ObservableCollection<TItems>
    {
        public TParent Parent { get; protected internal set; }

        public THeader Header { get; set; }

        protected abstract object GetParent(TItems item);

        protected abstract void AddParent(TItems item);

        protected abstract void RemoveParent(TItems item);

        protected override void InsertItem(int index, TItems item)
        {
            if (GetParent(item) == null)
            {
                base.InsertItem(index, item);

                AddParent(item);
            }

            else

                throw new ArgumentException("The given list already has a parent.");
        }

        protected override void RemoveItem(int index)
        {
            TItems item = Items[index];

            base.RemoveItem(index);

            RemoveParent(item);
        }

        protected override void SetItem(int index, TItems item)
        {
            RemoveParent(Items[index]);

            base.SetItem(index, item);

            AddParent(item);
        }

        protected override void ClearItems()
        {
            foreach (TItems item in Items)

                RemoveParent(item);

            base.ClearItems();
        }
    }

    public abstract class NavigableObservableHeaderedCollection2<THeader, TParent, TItems> : NavigableObservableHeaderedCollection<THeader, TParent, TItems>
    {
        private ImageSource _icon;

        public ImageSource Icon { get => _icon; set => OnPropertyChanged(nameof(Icon), ref _icon, value); }

        protected void OnPropertyChanged<TValue>(in string propertyName, ref TValue value, in TValue newValue)
        {
            value = newValue;

            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }
    }

    public interface INavigableMenuItem : ICommandSource, IEnumerable
    {
        INavigableMenuItemGroup Group { get; }

        IEnumerable Parent => Group.Parent;

        bool StaysOpenOnClick { get; set; }
    }

    public class NavigableMenuItem<T> : NavigableObservableHeaderedCollection2<T, NavigableMenuItemGroup<T>, NavigableMenuItemGroup<T>>, INavigableMenuItem
    {
        private ICommand command;
        private object commandParameter;
        private IInputElement commandTarget;
        private bool staysOpenOnClick;

        public ICommand Command { get => command; set => OnPropertyChanged(nameof(Command), ref command, value); }

        public object CommandParameter { get => commandParameter; set => OnPropertyChanged(nameof(Command), ref commandParameter, value); }

        public IInputElement CommandTarget { get => commandTarget; set => OnPropertyChanged(nameof(Command), ref commandTarget, value); }

        public bool StaysOpenOnClick { get => staysOpenOnClick; set => OnPropertyChanged(nameof(StaysOpenOnClick), ref staysOpenOnClick, value); }

        INavigableMenuItemGroup INavigableMenuItem.Group => Parent;

        protected override object GetParent(NavigableMenuItemGroup<T> item) => item.Parent;

        protected override void AddParent(NavigableMenuItemGroup<T> item) => item.Parent = this;

        protected override void RemoveParent(NavigableMenuItemGroup<T> item) => item.Parent = null;
    }

    public interface INavigableMenuItemGroup
    {
        bool IsExpanded { get; set; }

        IEnumerable Parent { get; }
    }

    public class NavigableMenuItemGroup<T> : NavigableObservableHeaderedCollection2<T, IEnumerable, NavigableMenuItem<T>>, INavigableMenuItemGroup
    {
        private bool _isExpanded = true;

        public bool IsExpanded { get => _isExpanded; set => OnPropertyChanged(nameof(IsExpanded), ref _isExpanded, value); }

        protected override object GetParent(NavigableMenuItem<T> item) => item.Parent;

        protected override void AddParent(NavigableMenuItem<T> item) => item.Parent = this;

        protected override void RemoveParent(NavigableMenuItem<T> item) => item.Parent = null;
    }

    public interface INavigableMenu : IEnumerable
    {
        object Header { get; }

        bool IsOpen { get; set; }
    }

    public class NavigableMenu<T> : NavigableObservableHeaderedCollection2<T, object, NavigableMenuItemGroup<T>>, INavigableMenu
    {
        private bool _isOpen;

        public bool IsOpen { get => _isOpen; set => OnPropertyChanged(nameof(IsOpen), ref _isOpen, value); }

        object INavigableMenu.Header => Header;

        protected override object GetParent(NavigableMenuItemGroup<T> item) => item.Parent;

        protected override void AddParent(NavigableMenuItemGroup<T> item) => item.Parent = this;

        protected override void RemoveParent(NavigableMenuItemGroup<T> item) => item.Parent = null;
    }

    public class NavigationMenu : System.Collections.ObjectModel.ObservableCollection<INavigableMenu>
    {
        public void CloseAll()
        {
            foreach (INavigableMenu menu in this)

                menu.IsOpen = false;
        }
    }
}
