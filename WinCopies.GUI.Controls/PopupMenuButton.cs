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

using System.Windows;

namespace WinCopies.GUI.Controls
{
    public class PopupMenuButton : System.Windows.Controls.MenuItem
    {
        //public static readonly DependencyProperty IsMenuOpenProperty = DependencyProperty.Register("IsMenuOpen", typeof(bool), typeof(PopupMenuButton), new PropertyMetadata(false));

        //public bool IsMenuOpen

        //{

        //    get => (bool)GetValue(IsMenuOpenProperty);

        //    set => SetValue(IsMenuOpenProperty, value);

        //}

        static PopupMenuButton() => DefaultStyleKeyProperty.OverrideMetadata(typeof(PopupMenuButton), new FrameworkPropertyMetadata(typeof(PopupMenuButton)));

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);

            if (element is System.Windows.Controls.MenuItem)

                ((System.Windows.Controls.MenuItem)element).Click += PopupMenuButtonMenuItem_Click;
        }

        protected virtual void OnPopupMenuButtonMenuItemClick(RoutedEventArgs e)

        {

            if (!StaysOpenOnClick)

                IsSubmenuOpen = false;

            // todo: IsMouseCaptured etc.

        }

        private void PopupMenuButtonMenuItem_Click(object sender, RoutedEventArgs e) => OnPopupMenuButtonMenuItemClick(e);

        //public PopupMenuButton()

        //{

        //    ItemContainerGenerator.ItemsChanged += ItemContainerGenerator_ItemsChanged;

        //}

        //protected virtual void OnItemContainerGeneratorItemsChanged(System.Windows.Controls.Primitives.ItemsChangedEventArgs e)

        //{

        //    if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)

        //    {

        //        Debug.WriteLine("PopupMenuButton message: OnItemContainerGeneratorItemsChanged e.Action = Add");

        //        for (int i = e.Position.)

        //    }

        //}

        //private void ItemContainerGenerator_ItemsChanged(object sender, System.Windows.Controls.Primitives.ItemsChangedEventArgs e)
        //{

        //    OnItemContainerGeneratorItemsChanged(e);

        //}
    }
}
