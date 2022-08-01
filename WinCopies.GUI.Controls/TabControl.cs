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

using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace WinCopies.GUI.Controls
{
    /// <summary>
    /// Represents a control that contains multiple items that share the same space on the screen.
    /// </summary>
    public class TabControl : System.Windows.Controls.TabControl, ISelectionIndexableSettableSelector
    {

        private static readonly DependencyPropertyKey CanScrollToLeftPropertyKey = DependencyProperty.RegisterReadOnly(nameof(CanScrollToLeft), typeof(bool), typeof(TabControl), new PropertyMetadata(false));

        /// <summary>
        /// Identifies the <see cref="CanScrollToLeft"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CanScrollToLeftProperty = CanScrollToLeftPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets a value indicating if items can scroll to left. This is a dependency property.
        /// </summary>
        public bool CanScrollToLeft => (bool)GetValue(CanScrollToLeftProperty);

        private static readonly DependencyPropertyKey CanScrollToRightPropertyKey = DependencyProperty.RegisterReadOnly(nameof(CanScrollToRight), typeof(bool), typeof(TabControl), new PropertyMetadata(false));

        /// <summary>
        /// Identifies the <see cref="CanScrollToRight"/> dependencyc property.
        /// </summary>
        public static readonly DependencyProperty CanScrollToRightProperty = CanScrollToRightPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets a value indicating if items can scroll to right. This is a dependency property.
        /// </summary>
        public bool CanScrollToRight => (bool)GetValue(CanScrollToRightProperty);

        public static readonly DependencyProperty MovingStepOnScrollButtonUserDoubleClickProperty = DependencyProperty.Register(nameof(MovingStepOnScrollButtonUserDoubleClick), typeof(int), typeof(TabControl), new PropertyMetadata(10));

        public int MovingStepOnScrollButtonUserDoubleClick

        {

            get => (int)GetValue(MovingStepOnScrollButtonUserDoubleClickProperty);

            set => SetValue(MovingStepOnScrollButtonUserDoubleClickProperty, value);

        }

        // todo: do not reducing this enumerable to MenuItemData and perform a check for the type of item in xaml. Seeing also MenuItemData for the other use of this class in this project.

        /// <summary>
        /// Identifies the <see cref="TabMenu"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TabMenuProperty = DependencyProperty.Register(nameof(TabMenu), typeof(PopupMenuButton), typeof(TabControl), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the menu items for tab items gesture. This is a dependency property.
        /// </summary>
        public PopupMenuButton TabMenu

        {

            get => (PopupMenuButton)GetValue(TabMenuProperty);

            set => SetValue(TabMenuProperty, value);

        }

        static TabControl() => DefaultStyleKeyProperty.OverrideMetadata(typeof(TabControl), new FrameworkPropertyMetadata(typeof(TabControl)));

        /// <summary>
        /// Initializes a new instance of the <see cref="TabControl"/> class.
        /// </summary>
        public TabControl() =>

            // OnApplyTemplate();

            ItemContainerGenerator.ItemsChanged += ItemContainerGenerator_ItemsChanged;

        public override void OnApplyTemplate() => base.OnApplyTemplate();//ObservableCollection<MenuItemData> menuItems = new ObservableCollection<MenuItemData>();//menuItems.Add(new MenuItemData() { Header = ResourcesHelper.ResourceDictionary["RecentlyClosedTabs"], Items = new ObservableCollection<MenuItemData>() });//// todo: really needed? ://PART_ScrollViewer = (ScrollViewer)Template.FindName("PART_ScrollViewer", this);//PART_ScrollViewer_ItemsPresenter = PART_ScrollViewer.Content as ItemsPresenter;//if (PART_ScrollViewer != null && PART_ScrollViewer_ItemsPresenter != null)//    tabItems = new List<TabItem>();

        protected virtual void OnItemContainerGeneratorItemsChanged(System.Windows.Controls.Primitives.ItemsChangedEventArgs e)

        {

            switch (e.Action)

            {

                case NotifyCollectionChangedAction.Add:

                    Debug.WriteLine("OnItemsChangedFromItemContainerGenerator add");

                    bool foundVisibleItem = false;

                    bool checkIfScrollingIsAllowed(int index, DependencyPropertyKey dependencyProperty)

                    {

                        if (ItemContainerGenerator.ContainerFromIndex(index) is FrameworkElement oAsFrameworkElement)

                            if (oAsFrameworkElement.IsUserVisible(this))

                                foundVisibleItem = true;

                            else if (foundVisibleItem)

                            {

                                SetValue(dependencyProperty, true);

                                return true;

                            }

                        return false;

                    }

                    for (int i = ItemContainerGenerator.Items.Count - 1; i >= 0; i--)

                        if (checkIfScrollingIsAllowed(i, CanScrollToLeftPropertyKey))

                            break;

                    foundVisibleItem = false;

                    for (int i = 0; i <= ItemContainerGenerator.Items.Count - 1; i++)

                        if (checkIfScrollingIsAllowed(i, CanScrollToRightPropertyKey))

                            break;

                    break;

                case NotifyCollectionChangedAction.Remove:

                    // todo:

                    break;

                case NotifyCollectionChangedAction.Reset:

                    SetValue(CanScrollToLeftPropertyKey, false);

                    SetValue(CanScrollToRightPropertyKey, false);

                    break;

            }

        }

        private void ItemContainerGenerator_ItemsChanged(object sender, System.Windows.Controls.Primitives.ItemsChangedEventArgs e) => OnItemContainerGeneratorItemsChanged(e);

        //protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        //{
        //    base.OnItemsChanged(e);
        //}

        public void MoveToLeft(int step)

        {

            for (int i = 1; i <= ItemContainerGenerator.Items.Count - 1; i++)

                if (ItemContainerGenerator.ContainerFromIndex(i) is FrameworkElement oAsFrameworkElement && oAsFrameworkElement.IsUserVisible(this))

                {

                    // todo: try to reach the first non-visible FrameworkElement in order to bring it into view instead of this:

                    for (int _i = i < step ? 0 : i - step; _i <= i - 1; _i++)

                    {

                        if (ItemContainerGenerator.ContainerFromIndex(_i) is FrameworkElement o_previousAsFrameworkElement)

                        {

                            o_previousAsFrameworkElement.BringIntoView();

                            //if ( _i == 0) 

                            //{

                            //    SetValue(CanScrollToLeftPropertyKey, false);

                            //    return;

                            //}

                            //else

                            //{

                            for (int j = _i - 1; j >= 0; j--)

                                if (ItemContainerGenerator.ContainerFromIndex(j) is FrameworkElement /* && !((FrameworkElement)o).IsUserVisible(this) */ )

                                    return;

                            SetValue(CanScrollToLeftPropertyKey, false);

                            return;

                            //}

                        }

                        //else

                        //    return;

                    }

                    return;

                }

        }

        protected virtual void OnScrollToLeftButtonClick() => MoveToLeft(1);

        public void OnScrollToLeftButtonClickInternal() => OnScrollToLeftButtonClick();

        protected virtual void OnScrollToLeftButtonMouseDoubleClick() => MoveToLeft(MovingStepOnScrollButtonUserDoubleClick);

        public void OnScrollToLeftButtonMouseDoubleClickInternal() => OnScrollToLeftButtonMouseDoubleClick();

        public void MoveToRight(int step)

        {

            for (int i = ItemContainerGenerator.Items.Count - 2; i >= 0; i--)

                if (ItemContainerGenerator.ContainerFromIndex(i) is FrameworkElement oAsFrameworkElement && oAsFrameworkElement.IsUserVisible(this))

                {

                    // todo: try to reach the first non-visible FrameworkElement in order to bring it into view instead of this:

                    for (int _i = ItemContainerGenerator.Items.Count - 1 - i < step ? ItemContainerGenerator.Items.Count - 1 : i + step; _i >= i + 1; _i--)

                    {

                        if (ItemContainerGenerator.ContainerFromIndex(_i) is FrameworkElement o_nextAsFrameworkElement)

                        {

                            o_nextAsFrameworkElement.BringIntoView();

                            //if (_i == ItemContainerGenerator.Items.Count - 1)

                            //{

                            //    SetValue(CanScrollToRightPropertyKey, false);

                            //    return;

                            //}

                            //else

                            //{

                            for (int j = _i + 1; j <= ItemContainerGenerator.Items.Count - 1; j++)

                                if (ItemContainerGenerator.ContainerFromIndex(j) is FrameworkElement /* && !((FrameworkElement)o).IsUserVisible(this) */ )

                                    return;

                            SetValue(CanScrollToLeftPropertyKey, false);

                            return;

                            //}

                        }

                        //else

                        //    return;

                    }

                    return;

                }

        }

        protected virtual void OnScrollToRightButtonClick() => MoveToRight(1);

        public void OnScrollToRightButtonClickInternal() => OnScrollToRightButtonClick();

        protected void OnScrollToRightButtonMouseDoubleClick() => MoveToRight(MovingStepOnScrollButtonUserDoubleClick);

        public void OnScrollToRightButtonMouseDoubleClickInternal() => OnScrollToRightButtonMouseDoubleClick();

    }
}
