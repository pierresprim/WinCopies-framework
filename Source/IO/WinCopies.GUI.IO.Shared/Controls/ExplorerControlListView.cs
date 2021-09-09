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

using System.Collections.Generic;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using WinCopies.Collections.Generic;
using WinCopies.GUI.IO.ObjectModel;
using WinCopies.IO.ObjectModel;
using WinCopies.IO.Process;
using WinCopies.Linq;

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

    public class DroppedEventArgs : RoutedEventArgs
    {
        public IProcessParameters ProcessParameters { get; }

        public DroppedEventArgs(in IProcessParameters processParameters) => ProcessParameters = processParameters;
    }

    public class ExplorerControlListViewContextMenuRequestedEventArgs : RoutedEventArgs
    {
        public MouseButtonEventArgs MouseButtonEventArgs { get; }

        public ExplorerControlListViewContextMenuRequestedEventArgs(in MouseButtonEventArgs mouseButtonEventArgs) : base(ExplorerControlListView.ContextMenuRequestedEvent) => MouseButtonEventArgs = mouseButtonEventArgs;

        public ExplorerControlListViewContextMenuRequestedEventArgs(in MouseButtonEventArgs mouseButtonEventArgs, in RoutedEvent routedEvent) : base(routedEvent) => MouseButtonEventArgs = mouseButtonEventArgs;

        public ExplorerControlListViewContextMenuRequestedEventArgs(in MouseButtonEventArgs mouseButtonEventArgs, in RoutedEvent routedEvent, in object source) : base(routedEvent, source) => MouseButtonEventArgs = mouseButtonEventArgs;
    }

    public class ExplorerControlListView : ListView
    {
        private Point _startPoint;

        //public static readonly DependencyProperty ViewStyleProperty = DependencyProperty.Register(nameof(ViewStyle), typeof(ViewStyle), typeof(ExplorerControlListView));

        //public ViewStyle ViewStyle { get => (ViewStyle)GetValue(ViewStyleProperty); set => SetValue(ViewStyleProperty, value); }

        public static readonly RoutedEvent DroppedEvent = EventManager.RegisterRoutedEvent(nameof(Dropped), RoutingStrategy.Bubble, typeof(RoutedEventHandler<DroppedEventArgs>), typeof(ExplorerControlListView));

        public event RoutedEventHandler<DroppedEventArgs> Dropped
        {
            add => AddHandler(DroppedEvent, value);

            remove => RemoveHandler(DroppedEvent, value);
        }

        public static readonly RoutedEvent ContextMenuRequestedEvent = EventManager.RegisterRoutedEvent(nameof(ContextMenuRequested), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ExplorerControlListView));

        public event RoutedEventHandler<DroppedEventArgs> ContextMenuRequested
        {
            add => AddHandler(ContextMenuRequestedEvent, value);

            remove => RemoveHandler(ContextMenuRequestedEvent, value);
        }

        public ExplorerControlListView() => AllowDrop = true;

        protected override DependencyObject GetContainerForItemOverride() => new ExplorerControlListViewItem();

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            _startPoint = e.GetPosition(null);
        }

        protected override void OnMouseRightButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseRightButtonUp(e);

            RaiseEvent(new ExplorerControlListViewContextMenuRequestedEventArgs(e) );
        }

        public IDragDropProcessInfo GetDragDropProcessInfo() => ((IExplorerControlBrowsableObjectInfoViewModel)DataContext).Path.ProcessFactory.DragDrop;

        protected virtual DataObject GetDragDropData()
        {
            DataObject data = new DataObject();

            System.Collections.Generic.IDictionary<string, object> result = GetDragDropProcessInfo().TryGetData(GetSelectedPaths(), out DragDropEffects dragDropEffects);

            if (result.Count == 1 && result.ContainsKey(DataFormats.FileDrop))
            {
                object obj = result[DataFormats.FileDrop];

                if (obj is StringCollection sc)
                {
                    data.SetFileDropList(sc);

                    return data;
                }

                else if (obj is System.Collections.Generic.IEnumerable<string> paths)
                {
                    data.SetFileDropList(WinCopies.IO.Path.GetStringCollection(paths));

                    return data;
                }
            }

            foreach (var d in result)

                data.SetData(d.Key, d.Value);

            return data;
        }

        protected System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> GetSelectedPaths() => SelectedItems.To<IBrowsableObjectInfo>();

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (IsMouseCaptured && e.LeftButton == MouseButtonState.Pressed&& SelectedItems.Count > 0 &&GetDragDropProcessInfo().CanRun(GetSelectedPaths()))
            {
                Vector diff = _startPoint - e.GetPosition(null);

                if (System.Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance || System.Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance)

                    _ = DragDrop.DoDragDrop(this, GetDragDropData(), DragDropEffects.Copy | DragDropEffects.Move | DragDropEffects.Link);
            }
        }

        protected override void OnDragEnter(DragEventArgs e)
        {
            base.OnDragEnter(e);

            if (!e.Data.GetDataPresent(DataFormats.FileDrop) || e.Source == this)

                e.Effects = DragDropEffects.None;
        }

        protected override void OnDrop(DragEventArgs e)
        {
            base.OnDrop(e);

            string[] formats = e.Data.GetFormats();

            var data = new ArrayBuilder<string>();

            foreach (string item in formats)

                _ = data.AddLast(item);

            var dic = new Dictionary<string, object>((int)data.Count);

            foreach (string item in data)

                dic.Add(item, e.Data.GetData(item));

            var processParameters = GetDragDropProcessInfo().TryGetProcessParameters(dic);

            if (processParameters != null)

                RaiseEvent(new DroppedEventArgs(processParameters) { RoutedEvent = DroppedEvent });
        }
    }
}
