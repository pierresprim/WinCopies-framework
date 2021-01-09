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

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WinCopies.Util;
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
        /// Identifies the <see cref="IsCheckBoxVisible"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsCheckBoxVisibleProperty = DependencyProperty.Register(nameof(IsCheckBoxVisible), typeof(bool), typeof(ExplorerControl));

        public bool IsCheckBoxVisible { get => (bool)GetValue(IsCheckBoxVisibleProperty); set => SetValue(IsCheckBoxVisibleProperty, value); }

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

        //public static readonly DependencyProperty TreeViewItemTemplateProperty = DependencyProperty.Register(nameof(TreeViewItemTemplate), typeof(DataTemplate), typeof(ExplorerControl));

        //public DataTemplate TreeViewItemTemplate { get => (DataTemplate)GetValue(TreeViewItemTemplateProperty); set => SetValue(TreeViewItemTemplateProperty, value); }

        //public static readonly DependencyProperty ListViewItemTemplateProperty = DependencyProperty.Register(nameof(ListViewItemTemplate), typeof(DataTemplate), typeof(ExplorerControl));

        //public DataTemplate ListViewItemTemplate { get => (DataTemplate)GetValue(ListViewItemTemplateProperty); set => SetValue(ListViewItemTemplateProperty, value); }

        //public static readonly DependencyProperty TreeViewItemsProperty = DependencyProperty.Register(nameof(TreeViewItems), typeof(IEnumerable), typeof(ExplorerControl));

        //public IEnumerable TreeViewItems { get => (IEnumerable)GetValue(TreeViewItemsProperty); set => SetValue(TreeViewItemsProperty, value); }

        //public static readonly DependencyProperty ListViewItemsProperty = DependencyProperty.Register(nameof(ListViewItems), typeof(IEnumerable), typeof(ExplorerControl));

        //public IEnumerable ListViewItems { get => (IEnumerable)GetValue(ListViewItemsProperty); set => SetValue(ListViewItemsProperty, value); }

        static ExplorerControl() => DefaultStyleKeyProperty.OverrideMetadata(typeof(ExplorerControl), new FrameworkPropertyMetadata(typeof(ExplorerControl)));

        protected virtual void OnPathChanged(string oldValue, string newValue) => RaiseEvent(new RoutedEventArgs<ValueChangedEventArgs>(PathChangedEvent, new ValueChangedEventArgs(oldValue, newValue)));

        //public ExplorerControl() => OnApplyCommandBindings();

        //protected virtual void OnApplyCommandBindings()
        //{
        //    CommandBindings.Add(new System.Windows.Input.CommandBinding(WinCopies.Util.Commands.Commands.CommonCommand, (object sender, ExecutedRoutedEventArgs e) => OnPathChange(e), (object sender, CanExecuteRoutedEventArgs e) => OnPathChange(e)));
        //}

        //protected virtual void OnPathChange(CanExecuteRoutedEventArgs e) => e.CanExecute = true;

        //protected virtual void OnPathChange(ExecutedRoutedEventArgs e) => path.Path = new BrowsableObjectInfoViewModel(ShellObjectInfo.From(ShellObject.FromParsingName(browsableObjectInfo.Text)));

        //protected virtual void OnPathChanged(IBrowsableObjectInfoViewModel oldPath, IBrowsableObjectInfoViewModel newPath ) => browsableObjectInfo.Text = Path; 
    }
}
