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
using System.Globalization;
using WinCopies.Util.Data;

namespace WinCopies.GUI.Explorer.Data
{
    public class PathToItemsConverter : ConverterBase
    {

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture) => value == null ? null : ((Explorer.IBrowsableObjectInfo)value).Items;// ((ShellObjectInfo)value).PropertyChanged += PathToItemsConverter_PropertyChanged;// if (items != null) ((INotifyCollectionChanged)items).CollectionChanged += PathToItemsConverter_CollectionChanged;

        //private void PathToItemsConverter_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        //{
        //    Debug.WriteLine("PathToItemsConverter_PropertyChanged");
        //}
        //private void PathToItemsConverter_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        //{
        //    Debug.WriteLine("PathToItemsConverter_CollectionChanged");
        //}

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();

    }
}
