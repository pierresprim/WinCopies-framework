﻿/* Copyright © Pierre Sprimont, 2020
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

using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using WinCopies.Desktop;
using WinCopies.GUI.Controls.Models;
using WinCopies.IO.ComponentSources.Item;
using WinCopies.IO.ObjectModel;
using WinCopies.Util.Data;

using static System.Windows.RoutingStrategy;

using static WinCopies.Util.Desktop.UtilHelpers;

namespace WinCopies.GUI.IO.Controls
{
    public class ExplorerControl : ItemsControl
    {
        private static DependencyProperty Register<T>(in string propertyName) => Register<T, ExplorerControl>(propertyName);

        private static DependencyProperty Register<T>(in string propertyName, in PropertyMetadata propertyMetadata) => Register<T, ExplorerControl>(propertyName, propertyMetadata);

        private static DependencyPropertyKey RegisterReadOnly<T>(in string propertyName, in PropertyMetadata propertyMetadata) => RegisterReadOnly<T, ExplorerControl>(propertyName, propertyMetadata);

        private static RoutedEvent RegisterRoutedEvent<T>(in string eventName, in RoutingStrategy routingStrategy) => Register<T, ExplorerControl>(eventName, routingStrategy);

        public static readonly DependencyProperty SelectionModeProperty = Register<SelectionMode, ExplorerControl>(nameof(SelectionMode), SelectionMode.Extended);

        public SelectionMode SelectionMode { get => (SelectionMode)GetValue(SelectionModeProperty); set => SetValue(SelectionModeProperty, value); }

        public static readonly DependencyProperty BrowseToParentProperty = Register<ICommand>(nameof(BrowseToParent));

        public ICommand BrowseToParent { get => (ICommand)GetValue(BrowseToParentProperty); set => SetValue(BrowseToParentProperty, value); }

        /// <summary>
        /// Identifies the <see cref="Path"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PathProperty = Register<string>(nameof(Path), new PropertyMetadata((DependencyObject d, DependencyPropertyChangedEventArgs e) => ((ExplorerControl)d).OnPathChanged((string)e.OldValue, (string)e.NewValue)));

        public string Path { get => (string)GetValue(PathProperty); set => SetValue(PathProperty, value); }

        /// <summary>
        /// Identifies the <see cref="Text"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TextProperty = Register<string>(nameof(Text));

        public string Text { get => (string)GetValue(TextProperty); set => SetValue(TextProperty, value); }

        public static readonly DependencyProperty HistoryProperty = Register<ReadOnlyHistoryObservableCollection<IBrowsableObjectInfo>>(nameof(History));

        public ReadOnlyHistoryObservableCollection<IBrowsableObjectInfo> History { get => (ReadOnlyHistoryObservableCollection<IBrowsableObjectInfo>)GetValue(HistoryProperty); set => SetValue(HistoryProperty, value); }

        /// <summary>
        /// Identifies the <see cref="TreeViewStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TreeViewStyleProperty = Register<Style>(nameof(TreeViewStyle));

        public Style TreeViewStyle { get => (Style)GetValue(TreeViewStyleProperty); set => SetValue(TreeViewStyleProperty, value); }

        /// <summary>
        /// Identifies the <see cref="ListViewStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ListViewStyleProperty = Register<Style>(nameof(ListViewStyle));

        public Style ListViewStyle { get => (Style)GetValue(ListViewStyleProperty); set => SetValue(ListViewStyleProperty, value); }

        /// <summary>
        /// Identifies the <see cref="PropertyGridItemProperty"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PropertyGridItemProperty = Register<IBrowsableObjectInfo>(nameof(PropertyGridItem));

        public IBrowsableObjectInfo PropertyGridItem { get => (IBrowsableObjectInfo)GetValue(PropertyGridItemProperty); set => SetValue(PropertyGridItemProperty, value); }

        /// <summary>
        /// Identifies the <see cref="PropertyGridDescriptionProperty"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PropertyGridDescriptionProperty = Register<string>(nameof(PropertyGridDescription));

        public string PropertyGridDescription { get => (string)GetValue(PropertyGridDescriptionProperty); set => SetValue(PropertyGridDescriptionProperty, value); }

        /// <summary>
        /// Identifies the <see cref="IsCheckBoxVisible"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsCheckBoxVisibleProperty = Register<bool>(nameof(IsCheckBoxVisible));

        public bool IsCheckBoxVisible { get => (bool)GetValue(IsCheckBoxVisibleProperty); set => SetValue(IsCheckBoxVisibleProperty, value); }

        /// <summary>
        /// Identifies the <see cref="SelectedIndexProperty"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedIndexProperty = Register<int>(nameof(SelectedIndex), new PropertyMetadata(-1));

        public int SelectedIndex { get => (int)GetValue(SelectedIndexProperty); set => SetValue(SelectedIndexProperty, value); }

        /// <summary>
        /// Identifies the <see cref="SelectedItemProperty"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedItemProperty = Register<IItemSource>(nameof(SelectedItem));

        public IItemSource SelectedItem { get => (IItemSource)GetValue(SelectedItemProperty); set => SetValue(SelectedItemProperty, value); }

        private static readonly DependencyPropertyKey SelectedItemsPropertyKey = RegisterReadOnly<IList>(nameof(SelectedItems), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="SelectedItemsProperty"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedItemsProperty = SelectedItemsPropertyKey.DependencyProperty;

        public IList SelectedItems { get => (IList)GetValue(SelectedItemsProperty); private set => SetValue(SelectedItemsPropertyKey, value); }

        public static readonly DependencyProperty CommandsProperty = Register<System.Collections.Generic.IEnumerable<IButtonModel>>(nameof(Commands));

        public System.Collections.Generic.IEnumerable<IButtonModel> Commands { get => (System.Collections.Generic.IEnumerable<IButtonModel>)GetValue(CommandsProperty); set => SetValue(CommandsProperty, value); }

        /// <summary>
        /// Identifies the <see cref="GoButtonCommand"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty GoButtonCommandProperty = Register<ICommand>(nameof(GoButtonCommand));

        public ICommand GoButtonCommand { get => (ICommand)GetValue(GoButtonCommandProperty); set => SetValue(GoButtonCommandProperty, value); }

        /// <summary>
        /// Identifies the <see cref="GoButtonCommandParameter"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty GoButtonCommandParameterProperty = Register<object>(nameof(GoButtonCommandParameter));

        public object GoButtonCommandParameter { get => GetValue(GoButtonCommandParameterProperty); set => SetValue(GoButtonCommandParameterProperty, value); }

        /// <summary>
        /// Identifies the <see cref="PathChanged"/> routed event.
        /// </summary>
        public static readonly RoutedEvent PathChangedEvent = RegisterRoutedEvent<RoutedEventHandler<ValueChangedEventArgs>>(nameof(PathChanged), Bubble);

        public event RoutedEventHandler<ValueChangedEventArgs> PathChanged
        {
            add => AddHandler(PathChangedEvent, value);

            remove => RemoveHandler(PathChangedEvent, value);
        }

        /// <summary>
        /// Identifies the <see cref="SelectionChangedEvent"/> routed event.
        /// </summary>
        public static readonly RoutedEvent SelectionChangedEvent = RegisterRoutedEvent<SelectionChangedEventHandler>(nameof(SelectionChanged), Bubble);

        public event SelectionChangedEventHandler SelectionChanged
        {
            add => AddHandler(SelectionChangedEvent, value);

            remove => RemoveHandler(SelectionChangedEvent, value);
        }

        static ExplorerControl() => DefaultStyleKeyProperty.OverrideMetadata(typeof(ExplorerControl), new FrameworkPropertyMetadata(typeof(ExplorerControl)));

        public ExplorerControl() => OnInit();

        protected virtual void OnInit()
        {
            AddHandler(System.Windows.Controls.Primitives.Selector.SelectionChangedEvent, new SelectionChangedEventHandler(ListView_SelectionChanged));

            _ = CommandBindings.Add(new CommandBinding(WinCopies.Commands.NavigationCommands.BrowseToParent, (object sender, ExecutedRoutedEventArgs e) => { BrowseToParent?.Execute(null); e.Handled = true; }, (object sender, CanExecuteRoutedEventArgs e) => { e.CanExecute = BrowseToParent?.CanExecute(null) == true; e.Handled = true; }));
        }

        protected virtual void OnGoToPageCanExecute(CanExecuteRoutedEventArgs e)
        {
            if (e == null)

                return;

            e.CanExecute = true;

            e.Handled = true;
        }

        protected virtual void OnListViewSelectionChanged(SelectionChangedEventArgs e)
        {
            if (e != null && SelectedItems == null && e.Source is ExplorerControlListView listView && listView.GetParent<ExplorerControl>(false) == this)
                /*{
                    e.RoutedEvent = SelectionChangedEvent;
                    e.Source = this;*/

                SelectedItems = listView.SelectedItems;

            /*RaiseEvent(e);
        }*/
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e) => OnListViewSelectionChanged(e);

        protected virtual void OnPathChanged(string oldValue, string newValue) => RaiseEvent(new RoutedEventArgs<ValueChangedEventArgs>(PathChangedEvent, new ValueChangedEventArgs(oldValue, newValue)));
    }
}
