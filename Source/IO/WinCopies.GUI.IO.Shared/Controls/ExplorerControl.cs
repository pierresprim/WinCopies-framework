/* Copyright © Pierre Sprimont, 2020
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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WinCopies.Collections.Generic;
using WinCopies.IO.ObjectModel;
using WinCopies.Util.Data;

namespace WinCopies.GUI.IO.Controls
{
    public class ExplorerControl : Control
    {
        /// <summary>
        /// Identifies the <see cref="Path"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PathProperty = DependencyProperty.Register(nameof(Path), typeof(string), typeof(ExplorerControl), new PropertyMetadata((DependencyObject d, DependencyPropertyChangedEventArgs e) => ((ExplorerControl)d).OnPathChanged((string)e.OldValue, (string)e.NewValue)));

        public string Path { get => (string)GetValue(PathProperty); set => SetValue(PathProperty, value); }

        /// <summary>
        /// Identifies the <see cref="Text"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(nameof(Text), typeof(string), typeof(ExplorerControl));

        public string Text { get => (string)GetValue(TextProperty); set => SetValue(TextProperty, value); }

        public static readonly DependencyProperty HistoryProperty = DependencyProperty.Register(nameof(History), typeof(ObservableLinkedCollectionEnumerable<IBrowsableObjectInfo>), typeof(ExplorerControl));

        public ObservableLinkedCollectionEnumerable<IBrowsableObjectInfo> History { get => (ObservableLinkedCollectionEnumerable<IBrowsableObjectInfo>)GetValue(HistoryProperty); set => SetValue(HistoryProperty, value); }

        /// <summary>
        /// Identifies the <see cref="TreeViewStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TreeViewStyleProperty = DependencyProperty.Register(nameof(TreeViewStyle), typeof(Style), typeof(ExplorerControl));

        public Style TreeViewStyle { get => (Style)GetValue(TreeViewStyleProperty); set => SetValue(TreeViewStyleProperty, value); }

        /// <summary>
        /// Identifies the <see cref="ListViewStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ListViewStyleProperty = DependencyProperty.Register(nameof(ListViewStyle), typeof(Style), typeof(ExplorerControl));

        public Style ListViewStyle { get => (Style)GetValue(ListViewStyleProperty); set => SetValue(ListViewStyleProperty, value); }

        /// <summary>
        /// Identifies the <see cref="PropertyGridItemProperty"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PropertyGridItemProperty = DependencyProperty.Register(nameof(PropertyGridItem), typeof(IBrowsableObjectInfo), typeof(ExplorerControl));

        public IBrowsableObjectInfo PropertyGridItem { get => (IBrowsableObjectInfo)GetValue(PropertyGridItemProperty); set => SetValue(PropertyGridItemProperty, value); }

        /// <summary>
        /// Identifies the <see cref="PropertyGridDescriptionProperty"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PropertyGridDescriptionProperty = DependencyProperty.Register(nameof(PropertyGridDescription), typeof(string), typeof(ExplorerControl));

        public string PropertyGridDescription { get => (string)GetValue(PropertyGridDescriptionProperty); set => SetValue(PropertyGridDescriptionProperty, value); }

        /// <summary>
        /// Identifies the <see cref="IsCheckBoxVisible"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsCheckBoxVisibleProperty = DependencyProperty.Register(nameof(IsCheckBoxVisible), typeof(bool), typeof(ExplorerControl));

        public bool IsCheckBoxVisible { get => (bool)GetValue(IsCheckBoxVisibleProperty); set => SetValue(IsCheckBoxVisibleProperty, value); }

        /// <summary>
        /// Identifies the <see cref="SelectedIndexProperty"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedIndexProperty = DependencyProperty.Register(nameof(SelectedIndex), typeof(int), typeof(ExplorerControl), new PropertyMetadata(-1));

        public int SelectedIndex { get => (int)GetValue(SelectedIndexProperty); set => SetValue(SelectedIndexProperty, value); }

        /// <summary>
        /// Identifies the <see cref="SelectedItemProperty"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register(nameof(SelectedItem), typeof(object), typeof(ExplorerControl));

        public object SelectedItem { get => GetValue(SelectedItemProperty); set => SetValue(SelectedItemProperty, value); }

        private static readonly DependencyPropertyKey SelectedItemsPropertyKey = DependencyProperty.RegisterReadOnly(nameof(SelectedItems), typeof(IList), typeof(ExplorerControl), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="SelectedItemsProperty"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedItemsProperty = SelectedItemsPropertyKey.DependencyProperty;

        public IList SelectedItems { get => (IList)GetValue(SelectedItemsProperty); private set => SetValue(SelectedItemsPropertyKey, value); }

        /// <summary>
        /// Identifies the <see cref="GoButtonCommand"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty GoButtonCommandProperty = DependencyProperty.Register(nameof(GoButtonCommand), typeof(ICommand), typeof(ExplorerControl));

        public ICommand GoButtonCommand { get => (ICommand)GetValue(GoButtonCommandProperty); set => SetValue(GoButtonCommandProperty, value); }

        /// <summary>
        /// Identifies the <see cref="GoButtonCommandParameter"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty GoButtonCommandParameterProperty = DependencyProperty.Register(nameof(GoButtonCommandParameter), typeof(object), typeof(ExplorerControl));

        public object GoButtonCommandParameter { get => GetValue(GoButtonCommandParameterProperty); set => SetValue(GoButtonCommandParameterProperty, value); }

        /// <summary>
        /// Identifies the <see cref="PathChanged"/> routed event.
        /// </summary>
        public static readonly RoutedEvent PathChangedEvent = EventManager.RegisterRoutedEvent(nameof(PathChanged), RoutingStrategy.Bubble, typeof(RoutedEventHandler<ValueChangedEventArgs>), typeof(ExplorerControl));

        public event RoutedEventHandler<ValueChangedEventArgs> PathChanged
        {
            add => AddHandler(PathChangedEvent, value);

            remove => RemoveHandler(PathChangedEvent, value);
        }

        /// <summary>
        /// Identifies the <see cref="SelectionChangedEvent"/> routed event.
        /// </summary>
        public static readonly RoutedEvent SelectionChangedEvent = EventManager.RegisterRoutedEvent(nameof(SelectionChanged), RoutingStrategy.Bubble, typeof(SelectionChangedEventHandler), typeof(ExplorerControl));

        public event SelectionChangedEventHandler SelectionChanged
        {
            add => AddHandler(SelectionChangedEvent, value);

            remove => RemoveHandler(SelectionChangedEvent, value);
        }

        static ExplorerControl() => DefaultStyleKeyProperty.OverrideMetadata(typeof(ExplorerControl), new FrameworkPropertyMetadata(typeof(ExplorerControl)));

        public ExplorerControl() => OnInit();

        protected virtual void OnInit() => AddHandler(System.Windows.Controls.Primitives.Selector.SelectionChangedEvent, new SelectionChangedEventHandler(ListView_SelectionChanged));

        protected virtual void OnGoToPageCanExecute(CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;

            e.Handled = true;
        }

        protected virtual void OnListViewSelectionChanged(SelectionChangedEventArgs e)
        {
            if (e.Source is ListView listView)
            {
                e.RoutedEvent = SelectionChangedEvent;
                e.Source = this;

                SelectedItems = listView.SelectedItems;

                RaiseEvent(e);
            }
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e) => OnListViewSelectionChanged(e);

        protected virtual void OnPathChanged(string oldValue, string newValue) => RaiseEvent(new RoutedEventArgs<ValueChangedEventArgs>(PathChangedEvent, new ValueChangedEventArgs(oldValue, newValue)));
    }
}
