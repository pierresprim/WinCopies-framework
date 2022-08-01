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

//using System;
//using System.Collections.Generic;
//using System.Collections.Specialized;
//using System.ComponentModel;
//using System.Windows;
//using System.Windows.Controls;
//using System.Windows.Data;

//namespace WinCopies.GUI.Controls
//{

//    public class GridView : System.Windows.Controls.GridView
//    {

//        private static readonly DependencyPropertyKey ListViewPropertyKey = DependencyProperty.RegisterReadOnly(nameof(ListView), typeof(ListView), typeof(GridView), new PropertyMetadata(null));

//        public static readonly DependencyProperty ListViewProperty = ListViewPropertyKey.DependencyProperty;

//        public ListView ListView { get => (ListView)GetValue(ListViewProperty); internal set => SetValue(ListViewPropertyKey, value); }

//    }

//    public class GridViewColumns : DependencyObject
//    {

//        #region Consts

//        private const string ColumnCollectionBehaviour = "ColumnCollectionBehaviour";

//        private const string ColumnsSource = "ColumnsSource";

//        private const string CellTemplate = "CellTemplate";

//        private const string CellTemplateSelector = "CellTemplateSelector";

//        private const string DisplayMemberBinding = "DisplayMemberBinding";

//        private const string Header = "Header";

//        //private const string HeaderContainerStyle = "HeaderContainerStyle";

//        //private const string HeaderStringFormat = "HeaderStringFormat";

//        //private const string HeaderTemplate = "HeaderTemplate";

//        //private const string HeaderTemplateSelector = "HeaderTemplateSelector";

//        private const string Width = "Width";

//        #endregion

//        #region Dependency properties

//        public static readonly DependencyProperty ColumnCollectionBehaviourProperty =
//            DependencyProperty.RegisterAttached(ColumnCollectionBehaviour, typeof(GridViewColumnCollectionBehaviour), typeof(GridViewColumns), new UIPropertyMetadata(null));

//        public static readonly DependencyProperty ColumnsSourceProperty =
//            DependencyProperty.RegisterAttached(ColumnsSource, typeof(object), typeof(GridViewColumns), new UIPropertyMetadata(null, ColumnsSourceChanged));

//        public static readonly DependencyProperty CellTemplateProperty = DependencyProperty.Register(CellTemplate, typeof(DataTemplate), typeof(GridViewColumns), new UIPropertyMetadata(null, CellTemplateChanged));

//        public static readonly DependencyProperty CellTemplateSelectorProperty = DependencyProperty.Register(CellTemplateSelector, typeof(DataTemplateSelector), typeof(GridViewColumns), new UIPropertyMetadata(null, CellTemplateSelectorChanged));

//        //public static readonly DependencyProperty DisplayMemberFormatMemberProperty =
//        //    DependencyProperty.RegisterAttached("DisplayMemberFormatMember", typeof(string), typeof(GridViewColumns), new UIPropertyMetadata(null, DisplayMemberFormatMemberChanged));

//        public static readonly DependencyProperty DisplayMemberBindingProperty =
//            DependencyProperty.RegisterAttached(DisplayMemberBinding, typeof(BindingBase), typeof(GridViewColumns), new UIPropertyMetadata(null, DisplayMemberBindingChanged));

//        public static readonly DependencyProperty HeaderProperty =
//            DependencyProperty.RegisterAttached(Header, typeof(object), typeof(GridViewColumns), new UIPropertyMetadata(null, HeaderChanged));

//        //public static readonly DependencyProperty HeaderContainerStyleProperty =
//        //    DependencyProperty.Register(HeaderContainerStyle, typeof(Style), typeof(GridViewColumns), new UIPropertyMetadata(null, ));

//        //public static readonly DependencyProperty HeaderStringFormatProperty =
//        //    DependencyProperty.RegisterAttached(HeaderStringFormat, typeof(string), typeof(GridViewColumns), new UIPropertyMetadata(null, ));

//        //public static readonly DependencyProperty HeaderTemplateProperty =
//        //    DependencyProperty.RegisterAttached(HeaderTemplate, typeof(DataTemplate), typeof(GridViewColumns), new UIPropertyMetadata(null, ));

//        //public static readonly DependencyProperty HeaderTemplateSelectorProperty =
//        //    DependencyProperty.RegisterAttached(HeaderTemplateSelector, typeof(DataTemplateSelector), typeof(GridViewColumns), new UIPropertyMetadata(null, ));

//        public static readonly DependencyProperty WidthProperty =
//            DependencyProperty.RegisterAttached(Width, typeof(double), typeof(GridViewColumns), new UIPropertyMetadata(100.0, WidthChanged));

//        #endregion

//        #region Dependency properties methods

//        [AttachedPropertyBrowsableForType(typeof(GridView))]
//        public static GridViewColumnCollectionBehaviour GetColumnCollectionBehaviour(DependencyObject obj) => (GridViewColumnCollectionBehaviour)obj.GetValue(ColumnCollectionBehaviourProperty);

//        public static void SetColumnCollectionBehaviour(DependencyObject obj, GridViewColumnCollectionBehaviour value) => obj.SetValue(ColumnCollectionBehaviourProperty, value);

//        [AttachedPropertyBrowsableForType(typeof(GridView))]
//        public static object GetColumnsSource(DependencyObject obj) => obj.GetValue(ColumnsSourceProperty);

//        public static void SetColumnsSource(DependencyObject obj, object value) => obj.SetValue(ColumnsSourceProperty, value);

//        [AttachedPropertyBrowsableForType(typeof(GridView))]
//        public static DataTemplate GetCellTemplate(DependencyObject obj) => (DataTemplate)obj.GetValue(CellTemplateProperty);

//        public static void SetCellTemplate(DependencyObject obj, DataTemplate value) => obj.SetValue(CellTemplateProperty, value);

//        [AttachedPropertyBrowsableForType(typeof(GridView))]
//        public static DataTemplateSelector GetCellTemplateSelector(DependencyObject obj) => (DataTemplateSelector)obj.GetValue(CellTemplateSelectorProperty);

//        public static void SetCellTemplateSelector(DependencyObject obj, DataTemplateSelector value) => obj.SetValue(CellTemplateSelectorProperty, value);

//        [AttachedPropertyBrowsableForType(typeof(GridView))]
//        public static BindingBase GetDisplayMemberBinding(DependencyObject obj) => (BindingBase)obj.GetValue(DisplayMemberBindingProperty);

//        public static void SetDisplayMemberBinding(DependencyObject obj, BindingBase value) => obj.SetValue(DisplayMemberBindingProperty, value);

//        //[AttachedPropertyBrowsableForType(typeof(GridView))]
//        //public static string GetDisplayMemberMember(DependencyObject obj) => (string)obj.GetValue(DisplayMemberMemberProperty);

//        //public static void SetDisplayMemberMember(DependencyObject obj, string value) => obj.SetValue(DisplayMemberMemberProperty, value);

//        [AttachedPropertyBrowsableForType(typeof(GridView))]
//        public static object GetHeader(DependencyObject obj) => obj.GetValue(HeaderProperty);

//        public static void SetHeader(DependencyObject obj, object value) => obj.SetValue(HeaderProperty, value);

//        //[AttachedPropertyBrowsableForType(typeof(GridView))]
//        //public static Style GetHeaderContainerStyle(DependencyObject obj) => (Style)obj.GetValue(HeaderContainerStyleProperty);

//        //public static void SetHeaderContainerStyle(DependencyObject obj, Style value) => obj.SetValue(HeaderContainerStyleProperty, value);

//        //[AttachedPropertyBrowsableForType(typeof(GridView))]
//        //public static string GetHeaderStringFormat(DependencyObject obj) => (string)obj.GetValue(HeaderStringFormatProperty);

//        //public static void SetHeaderStringFormat(DependencyObject obj, string value) => obj.SetValue(HeaderStringFormatProperty, value);

//        //[AttachedPropertyBrowsableForType(typeof(GridView))]
//        //public static DataTemplate GetHeaderTemplate(DependencyObject obj) => (DataTemplate)obj.GetValue(HeaderTemplateProperty);

//        //public static void SetHeaderTemplate(DependencyObject obj, DataTemplate value) => obj.SetValue(HeaderTemplateProperty, value);

//        //[AttachedPropertyBrowsableForType(typeof(GridView))]
//        //public static DataTemplateSelector GetHeaderTemplateSelector(DependencyObject obj) => (DataTemplateSelector)obj.GetValue(HeaderTemplateSelectorProperty);

//        //public static void SetHeaderTemplateSelector(DependencyObject obj, DataTemplateSelector value) => obj.SetValue(HeaderTemplateSelectorProperty, value);

//        [AttachedPropertyBrowsableForType(typeof(GridView))]
//        public static double GetWidthMember(DependencyObject obj) => (double)obj.GetValue(WidthProperty);

//        public static void SetWidthMember(DependencyObject obj, double value) => obj.SetValue(WidthProperty, value);

//        #endregion

//        private static void ColumnsSourceChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) => GetOrCreateColumnCollectionBehaviour(sender).ColumnsSource = e.NewValue;

//        private static void CellTemplateChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) => GetOrCreateColumnCollectionBehaviour(sender).CellTemplate = (DataTemplate)e.NewValue;

//        private static void CellTemplateSelectorChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) => GetOrCreateColumnCollectionBehaviour(sender).CellTemplateSelector = (DataTemplateSelector)e.NewValue;

//        private static void DisplayMemberBindingChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) => GetOrCreateColumnCollectionBehaviour(sender).DisplayMemberBinding = (BindingBase)e.NewValue;

//        private static void HeaderChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) => GetOrCreateColumnCollectionBehaviour(sender).Header = e.NewValue;

//        //private static void HeaderContainerStyleChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) => GetOrCreateColumnCollectionBehaviour(sender).HeaderContainerStyle = (Style)e.NewValue;

//        //private static void HeaderStringFormatChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) => GetOrCreateColumnCollectionBehaviour(sender).HeaderStringFormat = (string)e.NewValue;

//        //private static void HeaderTemplateChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) => GetOrCreateColumnCollectionBehaviour(sender).HeaderTemplate = (DataTemplate)e.NewValue;

//        //private static void HeaderTemplateSelectorChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) => GetOrCreateColumnCollectionBehaviour(sender).HeaderTemplateSelector = (DataTemplateSelector)e.NewValue;

//        private static void WidthChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) => GetOrCreateColumnCollectionBehaviour(sender).Width = (double)e.NewValue;

//        private static GridViewColumnCollectionBehaviour GetOrCreateColumnCollectionBehaviour(DependencyObject source)
//        {

//            GridViewColumnCollectionBehaviour behaviour = GetColumnCollectionBehaviour(source);

//            if (behaviour == null)
//            {

//                GridView typedSource = source as GridView;

//                if (typedSource == null)

//                    // todo:

//                    throw new Exception("This property can only be set on controls deriving GridView");

//                behaviour = new GridViewColumnCollectionBehaviour(typedSource);

//                SetColumnCollectionBehaviour(typedSource, behaviour);

//            }

//            return behaviour;

//        }

//    }

//    public class GridViewColumnCollectionBehaviour
//    {

//        private object columnsSource;

//        private GridView gridView;

//        public GridViewColumnCollectionBehaviour(GridView gridView) => this.gridView = gridView;

//        public object ColumnsSource
//        {
//            get => columnsSource;

//            set
//            {
//                // todo:

//                object oldValue = columnsSource;
//                columnsSource = value;
//                ColumnsSourceChanged(oldValue, columnsSource);
//            }
//        }

//        public DataTemplate CellTemplate { get; set; }

//        public DataTemplateSelector CellTemplateSelector { get; set; }

//        public BindingBase DisplayMemberBinding { get; set; }

//        public object Header { get; set; }

//        //public Style HeaderContainerStyle { get; set; }

//        //public string HeaderStringFormat { get; set; }

//        //public DataTemplate HeaderTemplate { get; set; }

//        //public DataTemplateSelector HeaderTemplateSelector { get; set; }

//        public double Width { get; set; }

//        private void AddHandlers(ICollectionView collectionView) => collectionView.CollectionChanged += ColumnsSource_CollectionChanged;

//        private void ColumnsSource_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
//        {
//            // ICollectionView view = sender as ICollectionView;

//            if (gridView == null)

//                return;

//            switch (e.Action)
//            {

//                case NotifyCollectionChangedAction.Add:

//                    for (int i = 0; i < e.NewItems.Count; i++)

//                        CreateColumn(e.NewStartingIndex + i, false, e.NewItems[i]);

//                    break;

//                case NotifyCollectionChangedAction.Move:

//                    List<GridViewColumn> columns = new List<GridViewColumn>();

//                    for (int i = 0; i < e.OldItems.Count; i++)

//                        columns.Add(gridView.Columns[e.OldStartingIndex + i]);

//                    for (int i = 0; i < e.NewItems.Count; i++)

//                        gridView.Columns.Insert(e.NewStartingIndex + i, columns[i]);

//                    break;

//                case NotifyCollectionChangedAction.Remove:

//                    for (int i = 0; i < e.OldItems.Count; i++)

//                        gridView.Columns.RemoveAt(e.OldStartingIndex);

//                    break;

//                case NotifyCollectionChangedAction.Replace:

//                    for (int i = 0; i < e.NewItems.Count; i++)

//                        CreateColumn(e.NewStartingIndex + i, true, e.NewItems[i]);

//                    break;

//                case NotifyCollectionChangedAction.Reset:

//                    gridView.Columns.Clear();

//                    CreateColumns(sender as ICollectionView);

//                    break;

//                default:

//                    break;

//            }

//        }

//        private void ColumnsSourceChanged(object oldValue, object newValue)
//        {
//            if (gridView != null)
//            {

//                gridView.Columns.Clear();

//                if (oldValue != null)
//                {

//                    ICollectionView view = CollectionViewSource.GetDefaultView(oldValue);

//                    if (view != null)

//                        RemoveHandlers(view);

//                }

//                if (newValue != null)
//                {

//                    ICollectionView view = CollectionViewSource.GetDefaultView(newValue);

//                    if (view != null)
//                    {

//                        AddHandlers(view);

//                        CreateColumns(view);

//                    }

//                }

//            }

//        }

//        private void CreateColumn(int index, bool deletePrevious, object columnSource)
//        {

//            GridViewColumn column = new GridViewColumn();

//            if (deletePrevious)

//                gridView.Columns.RemoveAt(index);

//            gridView.Columns.Insert(index, column);

//            gridView.ListView.ApplyTemplate();

//            #region Column properties assignments

//            column.CellTemplate = CellTemplate;

//            column.CellTemplateSelector = CellTemplateSelector;

//            column.DisplayMemberBinding = DisplayMemberBinding;

//            column.HeaderTemplate = gridView.ColumnHeaderTemplate;

//            column.HeaderTemplate.LoadContent();

//            Control control = (Control)column.HeaderTemplate.na.FindName("Azerty");
//            gridView.ListView.View
//            control.DataContext = columnSource;

//            column.Header = Header;

//            column.Width = Width;

//            #endregion

//            // return column;
//        }

//        private void CreateColumns(ICollectionView collectionView)
//        {

//            foreach (object item in collectionView)

//                CreateColumn(gridView.Columns.Count, false, item);

//        }

//        // private object GetPropertyValue(object obj, string propertyName) => obj?.GetType().GetProperty(propertyName)?.GetValue(obj, null);

//        private void RemoveHandlers(ICollectionView collectionView) => collectionView.CollectionChanged -= ColumnsSource_CollectionChanged;

//    }

//    //public static class GridViewColumns
//    //{

//    //    [AttachedPropertyBrowsableForType(typeof(GridView))]
//    //    public static object GetColumnsSource(DependencyObject obj) => obj.GetValue(ColumnsSourceProperty);

//    //    public static void SetColumnsSource(DependencyObject obj, object value) => obj.SetValue(ColumnsSourceProperty, value);

//    //    // Using a DependencyProperty as the backing store for ColumnsSource.  This enables animation, styling, binding, etc...
//    //    public static readonly DependencyProperty ColumnsSourceProperty =
//    //            DependencyProperty.RegisterAttached(
//    //                "ColumnsSource",
//    //                typeof(object),
//    //                typeof(GridViewColumns),
//    //                new UIPropertyMetadata(
//    //                    null,
//    //                    ColumnsSourceChanged));


//    //    [AttachedPropertyBrowsableForType(typeof(GridView))]
//    //    public static string GetHeaderTextMember(DependencyObject obj) => (string)obj.GetValue(HeaderTextMemberProperty);

//    //    public static void SetHeaderTextMember(DependencyObject obj, string value) => obj.SetValue(HeaderTextMemberProperty, value);

//    //    // Using a DependencyProperty as the backing store for HeaderTextMember.  This enables animation, styling, binding, etc...
//    //    public static readonly DependencyProperty HeaderTextMemberProperty =
//    //        DependencyProperty.RegisterAttached("HeaderTextMember", typeof(string), typeof(GridViewColumns), new UIPropertyMetadata(null));


//    //    [AttachedPropertyBrowsableForType(typeof(GridView))]
//    //    public static string GetDisplayMemberMember(DependencyObject obj) => (string)obj.GetValue(DisplayMemberMemberProperty);

//    //    public static void SetDisplayMemberMember(DependencyObject obj, string value) => obj.SetValue(DisplayMemberMemberProperty, value);

//    //    // Using a DependencyProperty as the backing store for DisplayMember.  This enables animation, styling, binding, etc...
//    //    public static readonly DependencyProperty DisplayMemberMemberProperty =
//    //        DependencyProperty.RegisterAttached("DisplayMemberMember", typeof(string), typeof(GridViewColumns), new UIPropertyMetadata(null));

//    //    private static void ColumnsSourceChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
//    //    {

//    //        GridView gridView = obj as GridView;

//    //        if (gridView != null)
//    //        {

//    //            gridView.Columns.Clear();

//    //            if (e.OldValue != null)
//    //            {

//    //                ICollectionView view = CollectionViewSource.GetDefaultView(e.OldValue);

//    //                if (view != null)

//    //                    RemoveHandlers(gridView, view);

//    //            }

//    //            if (e.NewValue != null)
//    //            {

//    //                ICollectionView view = CollectionViewSource.GetDefaultView(e.NewValue);

//    //                if (view != null)
//    //                {

//    //                    AddHandlers(gridView, view);

//    //                    CreateColumns(gridView, view);

//    //                }

//    //            }

//    //        }

//    //    }

//    //    private static IDictionary<ICollectionView, List<GridView>> _gridViewsByColumnsSource =
//    //        new Dictionary<ICollectionView, List<GridView>>();

//    //    private static List<GridView> GetGridViewsForColumnSource(ICollectionView columnSource)
//    //    {

//    //        List<GridView> gridViews;

//    //        if (!_gridViewsByColumnsSource.TryGetValue(columnSource, out gridViews))
//    //        {

//    //            gridViews = new List<GridView>();

//    //            _gridViewsByColumnsSource.Add(columnSource, gridViews);

//    //        }

//    //        return gridViews;
//    //    }

//    //    private static void AddHandlers(GridView gridView, ICollectionView view)
//    //    {

//    //        GetGridViewsForColumnSource(view).Add(gridView);

//    //        view.CollectionChanged += ColumnsSource_CollectionChanged;

//    //    }

//    //    private static void CreateColumns(GridView gridView, ICollectionView view)
//    //    {

//    //        foreach (object item in view)

//    //            gridView.Columns.Add(CreateColumn(gridView, item));

//    //    }

//    //    private static void RemoveHandlers(GridView gridView, ICollectionView view)
//    //    {

//    //        view.CollectionChanged -= ColumnsSource_CollectionChanged;

//    //        GetGridViewsForColumnSource(view).Remove(gridView);

//    //    }

//    //    private static void ColumnsSource_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
//    //    {

//    //        ICollectionView view = sender as ICollectionView;

//    //        List<GridView> gridViews = GetGridViewsForColumnSource(view);

//    //        if (gridViews == null || gridViews.Count == 0) return;

//    //        switch (e.Action)
//    //        {

//    //            case NotifyCollectionChangedAction.Add:

//    //                foreach (GridView gridView in gridViews)

//    //                    for (int i = 0; i < e.NewItems.Count; i++)

//    //                        gridView.Columns.Insert(e.NewStartingIndex + i, CreateColumn(gridView, e.NewItems[i]));

//    //                break;

//    //            case NotifyCollectionChangedAction.Move:

//    //                List<GridViewColumn> columns;

//    //                foreach (GridView gridView in gridViews)
//    //                {

//    //                    columns = new List<GridViewColumn>();

//    //                    for (int i = 0; i < e.OldItems.Count; i++)

//    //                        columns.Add(gridView.Columns[e.OldStartingIndex + i]);

//    //                    for (int i = 0; i < e.NewItems.Count; i++)

//    //                        gridView.Columns.Insert(e.NewStartingIndex + i, columns[i]);

//    //                }

//    //                break;

//    //            case NotifyCollectionChangedAction.Remove:

//    //                foreach (GridView gridView in gridViews)

//    //                    for (int i = 0; i < e.OldItems.Count; i++)

//    //                        gridView.Columns.RemoveAt(e.OldStartingIndex);

//    //                break;

//    //            case NotifyCollectionChangedAction.Replace:

//    //                foreach (GridView gridView in gridViews)

//    //                    for (int i = 0; i < e.NewItems.Count; i++)

//    //                        gridView.Columns[e.NewStartingIndex + i] = CreateColumn(gridView, e.NewItems[i]);

//    //                break;

//    //            case NotifyCollectionChangedAction.Reset:

//    //                foreach (GridView gridView in gridViews)
//    //                {

//    //                    gridView.Columns.Clear();

//    //                    CreateColumns(gridView, sender as ICollectionView);

//    //                }

//    //                break;

//    //            default:

//    //                break;

//    //        }

//    //    }

//    //    private static GridViewColumn CreateColumn(GridView gridView, object columnSource)
//    //    {

//    //        GridViewColumn column = new GridViewColumn();

//    //        string headerTextMember = GetHeaderTextMember(gridView);

//    //        string displayMemberMember = GetDisplayMemberMember(gridView);

//    //        if (!string.IsNullOrEmpty(headerTextMember))

//    //            column.Header = GetPropertyValue(columnSource, headerTextMember);

//    //        if (!string.IsNullOrEmpty(displayMemberMember))

//    //            column.DisplayMemberBinding = new Binding(GetPropertyValue(columnSource, displayMemberMember) as string);

//    //        return column;

//    //    }

//    //    private static object GetPropertyValue(object obj, string propertyName) => obj?.GetType().GetProperty(propertyName)?.GetValue(obj, null);

//    //}

//}
