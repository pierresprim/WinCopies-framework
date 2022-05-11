/*Copyright © Pierre Sprimont, 2021
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

#region Usings
#region System
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Imaging;
#endregion System

#region WinCopies
using WinCopies.Desktop;
using WinCopies.GUI.IO.Controls;
using WinCopies.GUI.IO.ObjectModel;
using WinCopies.IO;
using WinCopies.IO.Process;
using WinCopies.Linq;
using WinCopies.Util.Commands.Primitives;
using WinCopies.Util.Data;
#endregion WinCopies
#endregion Usings

namespace WinCopies.GUI.IO
{
    [ValueConversion(typeof(string), typeof(string))]
    public class PathConverter : AlwaysConvertibleOneWayConverter<string, object, string>
    {
        public override IReadOnlyConversionOptions ConvertOptions => ConverterHelper.ParameterCanBeNull;

        protected override string Convert(string value, object parameter, CultureInfo culture)
        {
            int length = value.Length - 1;

            for (int i = length; i >= 0; i--)

                if (value[i] == Path.PathSeparator && i < length)

                    return value.Remove(i);

            return value;
        }
    }

    [MultiValueConversion(typeof(IEnumerable<ICommand>))]
    public class ContextMenuConverter : MultiConverterBase
    {
        private class DelegateCommand : Util.Commands.Primitives.DelegateCommand
        {
            public BitmapSource Icon { get; }

            public System.Windows.Input.ICommand Command { get; }

            public IEnumerable<ICommand> Items { get; }

            public DelegateCommand(in string name, in string description, in Bitmap icon, IEnumerable<ICommand> items, in Predicate predicate, in Action<object> action) : base(name, description, predicate, action)
            {
                Icon = icon?.ToImageSource();

                Items = items;

                Command = new Commands.DelegateCommand(predicate, action);
            }
        }

        public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values[0] == null || values[0] == DependencyProperty.UnsetValue)

                return null;

            var value = (IBrowsableObjectInfoContextCommandEnumerable)values[0];

            if (value.BrowsableObjectInfo.IsBrowsable())
            {
                var _value = (IExplorerControlViewModel)((ContextMenu)values[1]).PlacementTarget.GetParent<ExplorerControl>(false).DataContext;

                return value.PrependValues(new DelegateCommand("Open in WinCopies", "Opens the selected items in WinCopies.", Icons.Properties.Resources.folder, null, Bool.True, param => _value.Path = _value.Factory.GetBrowsableObjectInfoViewModel(value.BrowsableObjectInfo)), new DelegateCommand("Open", null, null, new List<ICommand>() { new DelegateCommand("Open in new tab", null, Icons.Properties.Resources.tab_add, null, Bool.True, obj => _value.OpenInNewContextCommand?.TryExecute(value.BrowsableObjectInfo)) }, null, null), null);
            }

            else return value;
        }

        public override object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => throw new NotSupportedException();
    }

    namespace Process
    {
        [ValueConversion(typeof(ProcessTypes<IProcessErrorItem>.IProcessQueue), typeof(bool))]
        public class ErrorPathsToBooleanConverter : AlwaysConvertibleOneWayConverter<ProcessTypes<IProcessErrorItem>.IProcessQueue, object, bool>
        {
            public override IReadOnlyConversionOptions ConvertOptions => ConverterHelper.AllowNull;

            protected override bool Convert(ProcessTypes<IProcessErrorItem>.IProcessQueue value, object parameter, CultureInfo culture) => value?.HasItems == true;
        }

        [ValueConversion(typeof(object), typeof(Visibility))]
        public class ObjectToVisibilityConverter : ConverterBase
        {
            public override object Convert(object value, Type targetType, object parameter, CultureInfo culture) => value == null || ((string)value).Length == 0 ? Visibility.Collapsed : Visibility.Visible;

            public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotSupportedException();
        }
    }
}
