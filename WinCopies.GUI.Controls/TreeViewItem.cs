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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using WinCopies.Util;

namespace WinCopies.GUI.Controls
{
    /// <summary>
    /// Implements a selectable item in a System.Windows.Controls.TreeView control.
    /// </summary>
    [StyleTypedProperty(Property = nameof(ItemContainerStyle), StyleTargetType = typeof(TreeViewItem))]
    public class TreeViewItem : System.Windows.Controls.TreeViewItem, ISelectable
    {

        //public static readonly DependencyProperty EnableClickEventProperty = DependencyProperty.Register(nameof(EnableClickEvent), typeof(bool), typeof(TreeViewItem));

        //public bool EnableClickEvent { get => (bool)GetValue(EnableClickEventProperty); set => SetValue(EnableClickEventProperty, value); }

        private static readonly DependencyPropertyKey IsFocusSelectionPropertyKey = DependencyProperty.RegisterReadOnly(nameof(IsFocusSelection), typeof(bool), typeof(TreeViewItem), new PropertyMetadata());

        /// <summary>
        /// Identifies the <see cref="IsFocusSelection"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsFocusSelectionProperty = IsFocusSelectionPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets a value that indicates whether this <see cref="TreeViewItem"/> is selected by focus.
        /// </summary>
        public bool IsFocusSelection => (bool)GetValue(IsFocusSelectionProperty);

        //private static readonly DependencyPropertyKey IsRealSelectionPropertyKey = DependencyProperty.RegisterReadOnly(nameof(IsRealSelection), typeof(bool), typeof(TreeViewItem), new PropertyMetadata());

        //public static readonly DependencyProperty IsRealSelectionProperty = IsRealSelectionPropertyKey.DependencyProperty;

        //public bool IsRealSelection => (bool)GetValue(IsRealSelectionProperty);

        internal static readonly DependencyPropertyKey IsPreviouslySelectedItemPropertyKey = DependencyProperty.RegisterReadOnly(nameof(IsPreviouslySelectedItem), typeof(bool), typeof(TreeViewItem), new PropertyMetadata());

        /// <summary>
        /// Identifies the <see cref="IsPreviouslySelectedItem"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsPreviouslySelectedItemProperty = IsPreviouslySelectedItemPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets a value that indicates whether this <see cref="TreeViewItem"/> 
        /// </summary>
        public bool IsPreviouslySelectedItem => (bool)GetValue(IsPreviouslySelectedItemProperty);

        /// <summary>
        /// Identifies the <see cref="Click"/> routed event.
        /// </summary>
        public static readonly RoutedEvent ClickEvent = EventManager.RegisterRoutedEvent(nameof(Click), RoutingStrategy.Bubble, typeof(RoutedEventArgs), typeof(TreeViewItem));

        /// <summary>
        /// Occurs when the user clicks or push the Enter key on this <see cref="TreeViewItem"/>.
        /// </summary>
        public event RoutedEventHandler Click { add => AddHandler(ClickEvent, value); remove => RemoveHandler(ClickEvent, value); }

        public TreeView ParentTreeView { get; internal set; } = null;

        static TreeViewItem() => DefaultStyleKeyProperty.OverrideMetadata(typeof(TreeViewItem), new FrameworkPropertyMetadata(typeof(TreeViewItem)));

        /// <summary>
        /// Initializes a new instance of the <see cref="TreeViewItem"/> class.
        /// </summary>
        public TreeViewItem() => Click += TreeViewItem_Click;

        /// <summary>
        /// Called when this <see cref="TreeViewItem"/> is clicked.
        /// </summary>
        /// <param name="e">The event parameters.</param>
        protected virtual void OnClick(RoutedEventArgs e) =>

            // this.GetParent<TreeView>(false)._isFocusSelection = false;

            SetValue(IsFocusSelectionPropertyKey, false);// SetValue(IsRealSelectionPropertyKey, true);

        private void TreeViewItem_Click(object sender, RoutedEventArgs e) => OnClick(e);

        /// <summary>
        /// Creates a new <see cref="TreeViewItem"/> to use to display the object.
        /// </summary>
        /// <returns>A new <see cref="TreeViewItem"/> to use to display the object.</returns>
        protected override DependencyObject GetContainerForItemOverride() => new TreeViewItem() { ParentTreeView = ParentTreeView };

        /// <summary>
        /// Raises the <see cref="System.Windows.Controls.TreeViewItem.Selected"/> routed event when the
        ///     <see cref="System.Windows.Controls.TreeViewItem.IsSelected"/> property changes from <see langword="false"/> to
        ///     <see langword="true"/>.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        protected override void OnSelected(RoutedEventArgs e)
        {

            // ParentTreeView._selectedTreeViewItem = this;

            base.OnSelected(e);

            if (ParentTreeView != null)

                // {

                if (IsPreviouslySelectedItem)

                {

                    SetValue(IsPreviouslySelectedItemPropertyKey, false);

                    ParentTreeView.SetValue(TreeView.PreviouslySelectedItemPropertyKey, null);

                    SetValue(IsFocusSelectionPropertyKey, false);

                }

                else

                    SetValue(IsFocusSelectionPropertyKey, ParentTreeView._isFocusSelection);

            // SetValue(IsRealSelectionPropertyKey, !ParentTreeView._isFocusSelection);

            // }

            // ParentTreeView._isFocusSelection = false;

        }

        /// <summary>
        /// Raises the <see cref="System.Windows.Controls.TreeViewItem.Unselected"/> routed event when
        ///     the <see cref="System.Windows.Controls.TreeViewItem.IsSelected"/> property changes from <see langword="true"/>
        ///     to <see langword="false"/>.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        protected override void OnUnselected(RoutedEventArgs e)
        {

            base.OnUnselected(e);

            SetValue(IsFocusSelectionPropertyKey, false);

            // SetValue(IsRealSelectionPropertyKey, false);

        }

        /// <summary>
        /// Provides class handling for a <see cref="UIElement.KeyDown"/> event.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected override void OnKeyDown(KeyEventArgs e)
        {

            if (e.Key == Key.Enter)

                OnClick(e);

            else if (ParentTreeView != null && (e.Key == Key.Up || e.Key == Key.Down))

            {

                // var truc = this.SelectedValue;
                // var machin = this.SelectedValuePath;
                // if (treeViewItem != null)

                if (ParentTreeView.PreviouslySelectedItem == null)

                {

                    SetValue(TreeViewItem.IsPreviouslySelectedItemPropertyKey, true);

                    // TreeViewItem treeViewItem = null;

                    // if (ItemsSource == null)

                    // treeViewItem = e.OldValue as TreeViewItem;

                    // else

                    // {

                    // if (_selectedTreeViewItem != null)

                    // {

                    // ItemsControl parentItemsControl = ItemsControlFromItemContainer(this); // _selectedTreeViewItem.GetParent<TreeViewItem>(false) as ItemsControl;

                    //if (treeViewItemParent == null)

                    //{

                    //    ItemsControl _treeViewItemParent = _selectedTreeViewItem.GetParent<TreeView>(false) as ItemsControl;

                    //    if (_treeViewItemParent == this)

                    //        treeViewItemParent = _treeViewItemParent;

                    //}

                    // if (parentItemsControl != null && !(parentItemsControl is TreeView && parentItemsControl != ParentTreeView))

                    // treeViewItem = parentItemsControl.ItemContainerGenerator.ContainerFromItem(e.OldValue) as TreeViewItem;

                    // }

                    // if (parentItemsControl != null && !(parentItemsControl is TreeView && parentItemsControl != ParentTreeView))

                    ParentTreeView.SetValue(TreeView.PreviouslySelectedItemPropertyKey, this);

                    // }

                }

                // todo: really needed?

                ParentTreeView._isFocusSelection = true;

                // SetValue(IsFocusSelectionPropertyKey, true);

            }

            base.OnKeyDown(e);

        }

        /// <summary>
        /// Invoked when an unhandled System.Windows.Input.Mouse.MouseDown attached event
        ///     reaches an element in its route that is derived from this class. Implement this
        ///     method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="MouseButtonEventArgs"/> that contains the event data. This
        ///     event data reports details about the mouse button that was pressed and the handled
        ///     state.</param>
        protected override void OnMouseDown(MouseButtonEventArgs e)
        {

            base.OnMouseDown(e);

            if (ParentTreeView != null)

                ParentTreeView._isFocusSelection = true;

            CaptureMouse();

        }

        /// <summary>
        /// Invoked when an unhandled System.Windows.Input.Mouse.MouseUp routed event reaches
        ///     an element in its route that is derived from this class. Implement this method
        ///     to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="MouseButtonEventArgs"/> that contains the event data. The
        ///     event data reports that the mouse button was released.</param>
        protected override void OnMouseUp(MouseButtonEventArgs e)
        {

            base.OnMouseUp(e);

            if (IsMouseCaptured)

            {

                RaiseEvent(new RoutedEventArgs(ClickEvent));

                ReleaseMouseCapture();

            }
        }
    }
}
