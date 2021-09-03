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

using Microsoft.WindowsAPICodePack.Shell;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WinCopies.Collections.Generic;
using WinCopies.IO.ObjectModel;

namespace WinCopies.GUI.IO.Controls
{
    //    public enum ViewStyle
    //    {

    //        SizeOne = 0,

    //        SizeTwo = 1,

    //        SizeThree = 2,

    //        SizeFour = 3,

    //        List = 4,

    //        Details = 5,

    //        Tiles = 6,

    //        Content = 7

    //    }

    public class ExplorerControlListView : ListView
    {
        private Point startPoint;
        //public static readonly DependencyProperty ViewStyleProperty = DependencyProperty.Register(nameof(ViewStyle), typeof(ViewStyle), typeof(ExplorerControlListView));

        //public ViewStyle ViewStyle { get => (ViewStyle)GetValue(ViewStyleProperty); set => SetValue(ViewStyleProperty, value); }

        protected override DependencyObject GetContainerForItemOverride() => new ExplorerControlListViewItem();

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            startPoint = e.GetPosition(null);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            Point point = e.GetPosition(null);

            Vector diff = startPoint - point;

            if (IsMouseCaptured && e.LeftButton == MouseButtonState.Pressed && (System.Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance || System.Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance) && SelectedItems.Count > 0)
            {
                var arrayBuilder = new ArrayBuilder<string>();

                foreach (var item in SelectedItems)
                {
                    if (((IBrowsableObjectInfo)item).InnerObject is ShellObject shellObject)

                        _ = arrayBuilder.AddLast(shellObject.ParsingName);
                }

                var sc = new StringCollection();

                sc.AddRange(arrayBuilder.ToArray());

                DataObject data = new DataObject();

                data.SetFileDropList(sc);

                _ = DragDrop.DoDragDrop(this, data, DragDropEffects.Copy);
            }
        }
    }
}
