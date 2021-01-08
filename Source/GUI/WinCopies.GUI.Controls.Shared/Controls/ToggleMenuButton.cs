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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace WinCopies.GUI.Controls
{
    [TemplatePart(Name = PART_Popup, Type = typeof(Popup))]
    public class MenuButtonBase : MenuItem
    {
        public const string PART_Popup = "PART_Popup";
        private Popup _popup;



        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            AddHandler(ClickEvent, new RoutedEventHandler((object sender, RoutedEventArgs e) => OnSubmenuItemClick(e.Source is MenuItem menuItem && menuItem.StaysOpenOnClick, e)));
        }

        protected virtual void OnSubmenuItemClick(bool stayOpen, RoutedEventArgs e)
        {
            Mouse.Capture(null);

            if (!stayOpen)

                IsSubmenuOpen = false;
        }

        public override void OnApplyTemplate()
        {
            if (_popup != null)
            {
                _popup.Opened -= Submenu_Opened;
                _popup.Closed -= Submenu_Closed;

                _popup = null;
            }

            base.OnApplyTemplate();

            if ((_popup = GetTemplateChild(PART_Popup) as Popup) != null)
            {
                _popup.Opened += Submenu_Opened;
                _popup.Closed += Submenu_Closed;
            }
        }

        protected virtual void OnSubmenuOpened()
        {
            Mouse.Capture(this, CaptureMode.SubTree);

            RaiseEvent(new RoutedEventArgs(SubmenuOpenedEvent, this));
        }

        protected virtual void OnSubmenuClosed() => RaiseEvent(new RoutedEventArgs(SubmenuClosedEvent, this));

        private void Submenu_Opened(object sender, EventArgs e) => OnSubmenuOpened();

        private void Submenu_Closed(object sender, EventArgs e) => OnSubmenuClosed();

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);

            var itemBounds = new Rect(new Point(), RenderSize);

            if (!itemBounds.Contains(Mouse.GetPosition(this)))
            {
                Mouse.Capture(null);

                IsSubmenuOpen = false;
            }
        }
    }

    public class ToggleMenuButton : MenuButtonBase
    {



        static ToggleMenuButton() => DefaultStyleKeyProperty.OverrideMetadata(typeof(ToggleMenuButton), new FrameworkPropertyMetadata(typeof(ToggleMenuButton)));
    }
}
