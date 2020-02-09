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
//using System.Diagnostics;
//using System.Linq;
//using System.Reflection;
//using System.Text;
//using System.Threading.Tasks;

//namespace WinCopies.IO.FilesProcesses
//{

//    public static class CommonHelper
//    {

//        public static void OnPropertyChangedHelper(INotifyPropertyChanged @object, string propertyName, string fieldName, object previousValue, object newValue)

//        {

//            BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic |
//                         BindingFlags.Static | BindingFlags.Instance |
//                         BindingFlags.DeclaredOnly;
//            @object.GetType().GetField(fieldName, flags).SetValue(@object, newValue);

//#if DEBUG

//            Debug.WriteLine(string.Format("Property name: {0}, previous value: {1}, new value: {2}, is read-only: false", propertyName, previousValue == null ? "null": previousValue.ToString(), newValue == null ? "null" :   newValue.ToString()));

//#endif

//        }

//#if DEBUG 

//        public static void OnPropertyChangedReadOnlyHelper(INotifyPropertyChanged @object, String propertyName, Object previousValue, Object newValue)

//        {

//            Debug.WriteLine(string.Format("Property name: {0}, previous value: {1}, new value: {2}, is read-only: true", propertyName, previousValue==null?"null": previousValue.ToString(), newValue==null?"null" : newValue.ToString()));

//        }

//#endif

//    }

//}
