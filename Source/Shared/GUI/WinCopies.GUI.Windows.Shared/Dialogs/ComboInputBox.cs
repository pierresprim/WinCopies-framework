/* Copyright © Pierre Sprimont, 2019
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
using WinCopies.Util;

//namespace WinCopies.GUI.Windows.Dialogs
//{

//    public class ComboInputBox : InputBox
//    {

//        ///// <summary>
//        ///// Identifies the <see cref="Orientation"/> dependency property.
//        ///// </summary>
//        //public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(nameof(Orientation), typeof(System.Windows.Controls.Orientation), typeof(ComboInputBox), new PropertyMetadata(System.Windows.Controls.Orientation.Horizontal));

//        ///// <summary>
//        ///// Gets or sets the <see cref="System.Windows.Controls.Orientation"/> of the label and the text box. This is a dependency property.
//        ///// </summary>
//        //public System.Windows.Controls.Orientation Orientation { get => (System.Windows.Controls.Orientation)GetValue(OrientationProperty); set => SetValue(OrientationProperty, value); }

//        ///// <summary>
//        ///// Identifies the <see cref="Label"/> dependency property.
//        ///// </summary>
//        //public static readonly DependencyProperty LabelProperty = DependencyProperty.Register(nameof(Label), typeof(object), typeof(ComboInputBox));

//        ///// <summary>
//        ///// Gets or sets the label text for the user. This is a dependency property.
//        ///// </summary>
//        //public object Label { get => GetValue(LabelProperty); set => SetValue(LabelProperty, value); }

//        #region ComboBox properties implementation

//        public static readonly DependencyProperty AlternationCountProperty = DependencyProperty.Register(nameof(AlternationCount), typeof(int), typeof(ComboInputBox), new PropertyMetadata(0));

//        /// <summary>
//        /// Gets or sets the number of alternating item containers in the <see cref="System.Windows.Controls.ItemsControl" />,
//        ///     which enables alternating containers to have a unique appearance.
//        /// </summary>
//        /// <value>The number of alternating item containers in the <see cref="System.Windows.Controls.ItemsControl" />.</value>
//        public int AlternationCount { get => (int)GetValue(AlternationCountProperty); set => SetValue(AlternationCountProperty, value); }

//        public static readonly DependencyProperty GroupStyleProperty = DependencyProperty.Register(nameof(GroupStyle), typeof(System.Collections.ObjectModel.ObservableCollection<GroupStyle>), typeof(ComboInputBox), new PropertyMetadata(null));

//        public System.Collections.ObjectModel.ObservableCollection<GroupStyle> GroupStyle { get => (System.Collections.ObjectModel.ObservableCollection<GroupStyle>)GetValue(GroupStyleProperty); set => SetValue(GroupStyleProperty, value); }

//        public static readonly DependencyProperty GroupStyleSelectorProperty = DependencyProperty.Register(nameof(GroupStyleSelector), typeof(GroupStyleSelector), typeof(ComboInputBox), new PropertyMetadata(null));

//        /// <summary>
//        /// Gets or sets a method that enables you to provide custom selection logic for
//        ///     a <see cref="System.Windows.Controls.GroupStyle" /> to apply to each group in a collection.
//        /// </summary>
//        /// <value>A method that enables you to provide custom selection logic for a <see cref="System.Windows.Controls.GroupStyle" />
//        ///     to apply to each group in a collection.</value>
//        public GroupStyleSelector GroupStyleSelector { get => (GroupStyleSelector)GetValue(GroupStyleSelectorProperty); set => SetValue(GroupStyleSelectorProperty, value); }

//        public static readonly DependencyProperty IsDropDownOpenProperty = DependencyProperty.Register(nameof(IsDropDownOpen), typeof(bool), typeof(ComboInputBox), new PropertyMetadata(false));

//        public bool IsDropDownOpen { get => (bool)GetValue(IsDropDownOpenProperty); set => SetValue(IsDropDownOpenProperty, value); }

//        public static readonly DependencyProperty IsEditableProperty = DependencyProperty.Register(nameof(IsEditable), typeof(bool), typeof(ComboInputBox), new PropertyMetadata(false));

//        public bool IsEditable { get => (bool)GetValue(IsEditableProperty); set => SetValue(IsEditableProperty, value); }

//        public static readonly DependencyProperty IsReadOnlyProperty = DependencyProperty.Register(nameof(IsReadOnly), typeof(bool), typeof(ComboInputBox), new PropertyMetadata(false));

//        public bool IsReadOnly { get => (bool)GetValue(IsReadOnlyProperty); set => SetValue(IsReadOnlyProperty, value); }

//        public static readonly DependencyProperty IsTextSearchCaseSensitiveProperty = DependencyProperty.Register(nameof(IsTextSearchCaseSensitive), typeof(bool), typeof(ComboInputBox), new PropertyMetadata(false));

//        /// <summary>
//        /// Gets or sets a value that indicates whether case is a condition when searching
//        ///     for items.
//        /// </summary>
//        /// <value><see langword="true" /> if text searches are case-sensitive; otherwise, <see langword="false" />.</value>
//        public bool IsTextSearchCaseSensitive { get => (bool)GetValue(IsTextSearchCaseSensitiveProperty); set => SetValue(IsTextSearchCaseSensitiveProperty, value); }

//        public static readonly DependencyProperty IsTextSearchEnabledProperty = DependencyProperty.Register(nameof(IsTextSearchEnabled), typeof(bool), typeof(ComboInputBox), new PropertyMetadata(true));

//        public bool IsTextSearchEnabled { get => (bool)GetValue(IsTextSearchEnabledProperty); set => SetValue(IsTextSearchEnabledProperty, value); }

//        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(nameof(ItemsSource), typeof(IEnumerable), typeof(ComboInputBox), new PropertyMetadata(null));

//        /// <summary>
//        /// Gets or sets a collection used to generate the content of the <see cref="System.Windows.Controls.ItemsControl" />.
//        /// </summary>
//        /// <value>A collection that is used to generate the content of the <see cref="System.Windows.Controls.ItemsControl" />.
//        ///     The default is null.</value>
//        public IEnumerable ItemsSource { get => (IEnumerable)GetValue(ItemsSourceProperty); set => SetValue(ItemsSourceProperty, value); }

//        public static readonly DependencyProperty ShouldPreserveUserEnteredPrefixProperty = DependencyProperty.Register(nameof(ShouldPreserveUserEnteredPrefix), typeof(bool), typeof(ComboInputBox), new PropertyMetadata(false));

//        public bool ShouldPreserveUserEnteredPrefix { get => (bool)GetValue(ShouldPreserveUserEnteredPrefixProperty); set => SetValue(ShouldPreserveUserEnteredPrefixProperty, value); }

//        public static readonly DependencyProperty StaysOpenOnEditProperty = DependencyProperty.Register(nameof(StaysOpenOnEdit), typeof(bool), typeof(ComboInputBox), new PropertyMetadata(false));

//        public bool StaysOpenOnEdit { get => (bool)GetValue(StaysOpenOnEditProperty); set => SetValue(StaysOpenOnEditProperty, value); }

//        //public static readonly DependencyProperty TextProperty = DependencyProperty.Register(nameof(Text), typeof(string), typeof(ComboInputBox), new PropertyMetadata(null));

//        //public string Text { get => (string)GetValue(TextProperty); set => SetValue(TextProperty, value); }

//        #endregion 

//        //public static readonly RoutedEvent TextChangedEvent = EventManager.RegisterRoutedEvent(nameof(TextChanged), RoutingStrategy.Bubble, typeof(TextChangedEventHandler), typeof(ComboInputBox));

//        //public event TextChangedEventHandler TextChanged

//        //{

//        //    add => AddHandler(TextChangedEvent, value);

//        //    remove => RemoveHandler(TextChangedEvent, value);

//        //}

//        public static readonly RoutedEvent SelectionChangedEvent = EventManager.RegisterRoutedEvent(nameof(SelectionChanged), RoutingStrategy.Bubble, typeof(SelectionChangedEventHandler), typeof(ComboInputBox));

//        public event SelectionChangedEventHandler SelectionChanged

//        {

//            add => AddHandler(SelectionChangedEvent, value);

//            remove => RemoveHandler(SelectionChangedEvent, value);

//        }

//        static ComboInputBox() => DefaultStyleKeyProperty.OverrideMetadata(typeof(ComboInputBox), new FrameworkPropertyMetadata(typeof(ComboInputBox)));

//        public ComboInputBox() => CommandBindings.Add(new CommandBinding(Util.Commands.Commands.CommonCommand, OnEvent));

//        protected virtual void OnTextChanged(TextChangedEventArgs e) => Command?.TryExecute(CommandParameter, CommandTarget);

//        protected virtual void OnSelectionChanged(SelectionChangedEventArgs e) => Command?.TryExecute(CommandParameter, CommandTarget);

//        public void OnEvent(object sender, ExecutedRoutedEventArgs e)

//        {

//            if (e.Parameter is TextChangedEventArgs _e)

//            {

//                e.RoutedEvent = TextChangedEvent;

//                RaiseEvent(e);

//                OnTextChanged(_e);

//            }

//            else if (e.Parameter is SelectionChangedEventArgs __e)

//            {

//                e.RoutedEvent = SelectionChangedEvent;

//                RaiseEvent(e);

//                OnSelectionChanged(__e);

//            }

//        }

//    }
//}
