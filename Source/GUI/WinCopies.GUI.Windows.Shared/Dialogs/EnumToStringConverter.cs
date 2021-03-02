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
using System.Windows;
using WinCopies.Util.Data;

//namespace WinCopies.GUI.Windows.Dialogs
//{

//    public struct KeyValuePair
//    {

//        public Enum Key { get; set; }

//        public string Value { get; set; }

//        public KeyValuePair(Enum key, string value) { Key = key; Value = value; }

//        public override string ToString() => Value;

//    }

//    public class EnumToStringConverter : WinCopies.Util.Data.ConverterBase
//    {

//        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
//        {

//            if (parameter is string _parameter && _parameter == "SelectedItem")

//                return new KeyValuePair((Enum)value, (string)Themes.Generic.GetResource<ResourceDictionary>(string.Format("{0}s", value.GetType().Name))[value.ToString()]);

//            string[] enumNames;

//            if (parameter is Type t && t.IsEnum)

//            {

//                enumNames = t.GetEnumNames();

//                KeyValuePair[] values = new KeyValuePair[enumNames.Length];

//                for (int i = 0; i < enumNames.Length; i++)

//                {

//                    values[i] = new KeyValuePair((Enum)Enum.Parse(t, enumNames[i]), (string)Themes.Generic.GetResource<ResourceDictionary>(string.Format("{0}s", ((Type)parameter).Name))[enumNames[i].ToString()]);

//                }

//                return values;

//            }

//            return null;
//        }

//        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
//        {

//            //if (value == null)

//            //    return 0;

//            //else

//            //    // todo : to add a 'new' Enum.TryParse() method in order to avoid types compatibility problems.

//            //    try

//            //    {

//            //        return Enum.Parse(targetType, (string)value);

//            //    }

//            //    catch (Exception)

//            //    {

//            //        return 0;

//            //    }

//            if (parameter is string _parameter && _parameter == "SelectedItem")

//                return ((KeyValuePair)value).Key; // return new KeyValuePair((Enum)Enum.Parse(t, enumNames[i]), (string)Themes.Generic.GetResource<ResourceDictionary>(string.Format("{0}s", ((Type)parameter).Name))[enumNames[i].ToString()]);

//            else

//                throw new NotImplementedException();

//        }
//    }
//}
