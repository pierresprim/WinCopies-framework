/* Copyright © Pierre Sprimont, 2021
 *
 * This file is part of the WinCopies Framework.
 *
 * The WinCopies Framework is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * The WinCopies Framework is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with the WinCopies Framework.  If not, see <https://www.gnu.org/licenses/>. */

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

using WinCopies.Util;
using WinCopies.Util.Data;

using static WinCopies.UtilHelpers;

namespace WinCopies.GUI.Controls
{
    public interface ISelector
    {
        bool IsChecked { get; set; }
    }

    public interface IFilter : ISelector
    {
        object Icon { get; }

        object Content { get; }

        Predicate Predicate { get; }
    }

    public interface IFilter<T> : IFilter
    {
        new Predicate<T> Predicate { get; }
    }

#if CS8
    public interface IFilterRef<T> : IFilter<T> where T : class
    {
        Predicate IFilter.Predicate => value => PredicateRef<T>(value, Predicate);
    }

    public interface IFilterVal<T> : IFilter<T> where T : struct
    {
        Predicate IFilter.Predicate => value => PredicateVal<T>(value, Predicate);
    }
#endif

    public abstract class Selector : ViewModelBase
    {
        private bool _isChecked;

        public bool IsChecked { get => _isChecked; set => UpdateValue(ref _isChecked, value, nameof(IsChecked)); }
    }

    public abstract class Filter<T> : Selector
    {
        protected abstract Predicate<T> PredicateOverride { get; }

        public Predicate<T> Predicate { get; }

        public abstract object Icon { get; }

        public abstract object Content { get; }

        public Filter() => Predicate = item => IsChecked && PredicateOverride(item);
    }

    public abstract class FilterRef<T> : Filter<T>,
#if CS8
        IFilterRef
#else
        IFilter
#endif
        <T> where T : class
    {
#if CS8
        // Left empty.
#else
        Predicate IFilter.Predicate => value => PredicateRef<T>(value, Predicate);
#endif
    }

    public abstract class FilterVal<T> : Filter<T>,
#if CS8
        IFilterVal
#else
        IFilter
#endif
        <T> where T : struct
    {
#if CS8
        // Left empty.
#else
        Predicate IFilter.Predicate => value => PredicateVal<T>(value, Predicate);
#endif
    }

    public interface IFilterEnumerable : ISelector, System.Collections.Generic.IEnumerable<IFilter>
    {
        object Header { get; }

        bool Filter(object value);
    }

    public interface IFilterEnumerable<T> : IFilterEnumerable, System.Collections.Generic.IEnumerable<IFilter<T>>
    {
        bool Filter(T item);

        new IEnumerator<IFilter<T>> GetEnumerator();

#if CS8
        IEnumerator<IFilter<T>> System.Collections.Generic.IEnumerable<IFilter<T>>.GetEnumerator() => GetEnumerator();

        IEnumerator<IFilter> System.Collections.Generic.IEnumerable<IFilter>.GetEnumerator() => GetEnumerator();
#endif
    }

#if CS8
    public interface IFilterEnumerableRef<T> : IFilterEnumerable<T> where T : class
    {
        bool IFilterEnumerable.Filter(object value) => PredicateRef<T>(value, Filter);
    }

    public interface IFilterEnumerableVal<T> : IFilterEnumerable<T> where T : struct
    {
        bool IFilterEnumerable.Filter(object value) => PredicateVal<T>(value, Filter);
    }
#endif

    public abstract class FilterEnumerable<T> : Selector, System.Collections.Generic.IEnumerable<IFilter<T>>
    {
        public abstract object Header { get; }

        public bool Filter(T item) => this.ForEachANDALSO(_item => _item.Predicate(item));

        public abstract IEnumerator<IFilter<T>> GetEnumerator();

#if !CS8
        IEnumerator<IFilter<T>> System.Collections.Generic.IEnumerable<IFilter<T>>.GetEnumerator() => GetEnumerator();
#endif

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public abstract class FilterEnumerableRef<T> : FilterEnumerable<T>,
#if CS8
        IFilterEnumerableRef
#else
        IFilterEnumerable
#endif
        <T> where T : class
    {
#if CS8
        // Left empty.
#else
        bool IFilterEnumerable.Filter(object value) => PredicateRef<T>(value, Filter);

        IEnumerator<IFilter> System.Collections.Generic.IEnumerable<IFilter>.GetEnumerator() => GetEnumerator();
#endif
    }

    public abstract class FilterEnumerableVal<T> : FilterEnumerable<T>,
#if CS8
        IFilterEnumerableVal
#else
        IFilterEnumerable
#endif
        <T> where T : struct
    {
#if CS8
        // Left empty.
#else
        bool IFilterEnumerable.Filter(object value) => PredicateVal<T>(value, Filter);

        IEnumerator<IFilter> System.Collections.Generic.IEnumerable<IFilter>.GetEnumerator() => GetEnumerator();
#endif
    }

    public class ListView : System.Windows.Controls.ListView
    {
        public static readonly DependencyProperty FiltersProperty = DependencyProperty.Register(nameof(Filters), typeof(IEnumerable<IFilterEnumerable>), typeof(ListView), new PropertyMetadata(null, (DependencyObject d, DependencyPropertyChangedEventArgs e) => ((ListView)d).OnFilterChanged((IEnumerable<IFilterEnumerable>)e.OldValue, (IEnumerable<IFilterEnumerable>)e.NewValue)));

        public IEnumerable<IFilterEnumerable> Filters { get => (IEnumerable<IFilterEnumerable>)GetValue(FiltersProperty); set => SetValue(FiltersProperty, value); }

        public static readonly DependencyProperty ContentProperty = DependencyProperty.Register(nameof(Content), typeof(object), typeof(ListView));

        public object Content { get => GetValue(ContentProperty); set => SetValue(ContentProperty, value); }

        static ListView() => DefaultStyleKeyProperty.OverrideMetadata(typeof(ListView), new FrameworkPropertyMetadata(typeof(ListView)));

        public ListView() => OnAddCommandBindings();

        protected virtual void OnAddCommandBindings()
        {
            _ = CommandBindings.Add(new CommandBinding(Commands.DialogCommands.Apply, (object sender, ExecutedRoutedEventArgs e) => OnApplyFiltersCommand(e), (object sender, CanExecuteRoutedEventArgs e) => OnCanApplyFiltersCommand(e)));

            _ = CommandBindings.Add(new CommandBinding(Commands.ApplicationCommands.Reset, (object sender, ExecutedRoutedEventArgs e) => OnResetFiltersCommand(e), (object sender, CanExecuteRoutedEventArgs e) => OnCanApplyFiltersCommand(e)));
        }

        protected virtual void OnCanResetFiltersCommand(CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = CanFilter();

            e.Handled = true;
        }

        private bool CanFilter() => !(ItemsSource == null || Filters == null) && CollectionViewSource.GetDefaultView(ItemsSource).CanFilter;

        public void ResetFilters()
        {
            if (CanFilter())

                foreach (IFilterEnumerable filterEnumerable in Filters)

                    foreach (IFilter filter in filterEnumerable)

                        filter.IsChecked = false;

            OnApplyFilters();
        }

        protected virtual void OnResetFiltersCommand(ExecutedRoutedEventArgs e)
        {
            OnApplyFilters();

            e.Handled = true;
        }

        protected virtual void OnCanApplyFiltersCommand(CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = CanFilter();

            e.Handled = true;
        }

        protected virtual void OnApplyFilters() => CollectionViewSource.GetDefaultView(ItemsSource).Refresh();

        protected virtual void OnApplyFiltersCommand(ExecutedRoutedEventArgs e)
        {
            OnApplyFilters();

            e.Handled = true;
        }

        protected virtual void OnFilterChanged(System.Collections.Generic.IEnumerable<IFilterEnumerable> oldValue, System.Collections.Generic.IEnumerable<IFilterEnumerable> newValue)
        {
            ICollectionView view;

            if (ItemsSource != null && (view = CollectionViewSource.GetDefaultView(ItemsSource)).CanFilter)

                if (newValue == null)

                    view.Filter = null;

                else
                {
                    if (oldValue == null)

                        view.Filter = Filter;

                    view.Refresh();
                }
        }

        protected virtual bool Filter(object item) => Filters.ForEachANDALSO(_item => _item.Filter(item));

        protected override void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            base.OnItemsSourceChanged(oldValue, newValue);

            ICollectionView view;

            if (oldValue != null && (view = CollectionViewSource.GetDefaultView(oldValue)).CanFilter)

                view.Filter = null;

            if (newValue != null && (view = CollectionViewSource.GetDefaultView(newValue)).CanFilter)

                if (Filters != null)

                    view.Filter = Filter;
        }
    }
}
