using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace WinCopies.GUI
{
    public interface IHistoryCollection : IList
    {
        int CurrentIndex { get; set; }

        object Current { get; }

        bool CanMoveBack { get; }

        bool CanMoveForward { get; }

        bool NotifyOnPropertyChanged { get; }

        bool TryMoveBack();

        bool TryMoveForward();

        void MoveBack();

        void MoveForward();
    }

    public interface IHistoryCollection<T> : IHistoryCollection, IList<T>
    {
        T Previous { get; }

        T Next { get; }
    }

    public class HistoryObservableCollection<T> : ObservableCollection<T>, IHistoryCollection<T>
    {
        private int _currentIndex;
        private bool _notifyOnPropertyChanged = true;

        public int CurrentIndex { get => _currentIndex; set => UtilHelpers.UpdateValue(ref _currentIndex, value, OnPropertyChanged); }

        public T Current => this[CurrentIndex];

        object IHistoryCollection.Current => Current;

        public bool CanMoveBack => CurrentIndex < Count - 1;

        public bool CanMoveForward => CurrentIndex > 0;

        public T Previous => GetItem(TryGetPrevious, "back");

        public T Next => GetItem(TryGetNext, "forward");

        public bool NotifyOnPropertyChanged
        {
            get => _notifyOnPropertyChanged; set => UtilHelpers.UpdateValue(ref _notifyOnPropertyChanged, value, () =>
            {
                if (value)

                    OnPropertyChanged();
            });
        }

        private bool TryGetItem(bool canMove, int newIndex, out T result)
        {
            if (canMove)
            {
                result = this[newIndex];

                return true;
            }

            result = default;

            return false;
        }

        public bool TryGetPrevious(out T result) => TryGetItem(CanMoveBack, _currentIndex + 1, out result);

        public bool TryGetNext(out T result) => TryGetItem(CanMoveForward, _currentIndex - 1, out result);

        private T GetItem(in FuncOut<T, bool> func, in string param) => func(out T result) ? result : throw GetNavigationException(param);

        private bool TryMove(in bool canMove, in Action action)
        {
            if (canMove)
            {
                action();

                return true;
            }

            return false;
        }

        public bool TryMoveBack() => TryMove(CanMoveBack, () => CurrentIndex++);

        public bool TryMoveForward() => TryMove(CanMoveForward, () => CurrentIndex--);

        private void Move(in Func<bool> func, in string param)
        {
            if (!func())

                ThrowGetNavigationException(param);
        }

        public void MoveBack() => Move(TryMoveBack, "back");

        public void MoveForward() => Move(TryMoveForward, "forward");

        private InvalidOperationException GetNavigationException(in string param) => throw new InvalidOperationException($"Can not move {param}.");

        private void ThrowGetNavigationException(in string param) => throw GetNavigationException(param);

        protected virtual void OnPropertyChanged()
        {
            OnPropertyChanged(new PropertyChangedEventArgs(nameof(CurrentIndex)));
            OnPropertyChanged(new PropertyChangedEventArgs(nameof(Current)));
            OnPropertyChanged(new PropertyChangedEventArgs(nameof(CanMoveBack)));
            OnPropertyChanged(new PropertyChangedEventArgs(nameof(CanMoveForward)));
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (NotifyOnPropertyChanged)
            {
                base.OnPropertyChanged(e);

                if (e.PropertyName == nameof(Count))

                    OnPropertyChanged();
            }
        }
    }

    public class ReadOnlyHistoryObservableCollection<TItems, TCollection> : ReadOnlyObservableCollection<TItems>, IHistoryCollection where TCollection : HistoryObservableCollection<TItems>
    {
        protected TCollection InnerHistoryCollection { get; }

        public int CurrentIndex { get => InnerHistoryCollection.CurrentIndex; set => InnerHistoryCollection.CurrentIndex = value; }

        public TItems Current => InnerHistoryCollection.Current;

        public bool CanMoveBack => InnerHistoryCollection.CanMoveBack;

        public bool CanMoveForward => InnerHistoryCollection.CanMoveForward;

        public bool NotifyOnPropertyChanged => InnerHistoryCollection.NotifyOnPropertyChanged;

        object IHistoryCollection.Current => Current;

        public ReadOnlyHistoryObservableCollection(in TCollection collection) : base(collection) => InnerHistoryCollection = collection;

        public bool TryMoveBack() => InnerHistoryCollection.TryMoveBack();

        public bool TryMoveForward() => InnerHistoryCollection.TryMoveForward();

        public void MoveBack() => InnerHistoryCollection.MoveBack();

        public void MoveForward() => InnerHistoryCollection.MoveForward();
    }

    public class ReadOnlyHistoryObservableCollection<T> : ReadOnlyHistoryObservableCollection<T, HistoryObservableCollection<T>>
    {
        public ReadOnlyHistoryObservableCollection(in HistoryObservableCollection<T> collection) : base(collection) { /* Left empty. */ }
    }
}
