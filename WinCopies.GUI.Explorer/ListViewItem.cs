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
using System.Windows.Input;
using WinCopies.GUI.Controls;
using WinCopies.Util;

namespace WinCopies.GUI.Explorer
{
    public class ListViewItem : System.Windows.Controls.ListViewItem
    {

        public ExplorerControl ParentExplorerControl { get; } = null;

        public static readonly DependencyProperty IsCheckBoxEnabledProperty = DependencyProperty.Register(nameof(IsCheckBoxEnabled), typeof(bool), typeof(ListViewItem), new PropertyMetadata(false

            //    , (DependencyObject d, DependencyPropertyChangedEventArgs e) =>

            //{

            //    object a = e.NewValue;

            //}

            ));

        public bool IsCheckBoxEnabled { get => (bool)GetValue(IsCheckBoxEnabledProperty); set => SetValue(IsCheckBoxEnabledProperty, value); }

        public ListViewItem(ExplorerControl parentExplorerControl)
        {

            ParentExplorerControl = parentExplorerControl;//InputBindings.Add(new InputBinding(Commands.OpenOnLeftClick, new MouseGesture(MouseAction.LeftClick)));

            //CommandBindings.Add(new CommandBinding(parentExplorerControl.Command, Open_Executed, Open_CanExecute));

            //InputBindings.Add(new InputBinding(parentExplorerControl.Command, new KeyGesture(Key.Enter)));

        }

        static ListViewItem() => VisibilityProperty.OverrideMetadata(typeof(ListViewItem), new PropertyMetadata(VisibilityProperty.DefaultMetadata.DefaultValue, (DependencyObject d, DependencyPropertyChangedEventArgs e) =>

                                 {

                                     VisibilityProperty.DefaultMetadata.PropertyChangedCallback?.Invoke(d, e);

                                     ((ListViewItem)d).ParentExplorerControl.Value_IsVisibleChanged(d, e);

                                 }));
    }
}
