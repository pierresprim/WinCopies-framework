//* Copyright © Pierre Sprimont, 2020
// *
// * This file is part of the WinCopies Framework.
// *
// * The WinCopies Framework is free software: you can redistribute it and/or modify
// * it under the terms of the GNU General Public License as published by
// * the Free Software Foundation, either version 3 of the License, or
// * (at your option) any later version.
// *
// * The WinCopies Framework is distributed in the hope that it will be useful,
// * but WITHOUT ANY WARRANTY; without even the implied warranty of
// * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// * GNU General Public License for more details.
// *
// * You should have received a copy of the GNU General Public License
// * along with the WinCopies Framework.  If not, see <https://www.gnu.org/licenses/>. */

//using System;
//using System.Globalization;
//using System.Windows;
//using System.Windows.Controls;
//using System.Windows.Data;
//using System.Windows.Input;
//using System.Windows.Media;

//using WinCopies.GUI.Controls.Models;
//using WinCopies.Util.Data;

//using static WinCopies.
//#if WinCopies3
//    ThrowHelper
//#else
//    Util.Util
//#endif
//    ;

//namespace WinCopies.GUI.Controls
//{
//    public class PropertyViewModel : ViewModel<Temp.IProperty>, Temp.IProperty
//    {
//        internal new Temp.IProperty Model => ModelGeneric;

//        public bool IsEnabled => ModelGeneric.IsEnabled;

//        public string Name => ModelGeneric.Name;

//        public string DisplayName => ModelGeneric.DisplayName;

//        public string Description => ModelGeneric.Description;

//        public string EditInvitation => ModelGeneric.EditInvitation;

//        public object PropertyGroup => ModelGeneric.PropertyGroup;

//        public object Value => ModelGeneric.Value;

//        public Type Type => ModelGeneric.Type;

//        public PropertyViewModel(Temp.IProperty property) : base(property) { /* Left empty. */ }
//    }

//    [ValueConversion(typeof(Temp.IProperty), typeof(PropertyViewModel))]
//    public class PropertyConverter : Temp.ConverterBase<Temp.IProperty, object, PropertyViewModel>
//    {
//        public override PropertyViewModel Convert(Temp.IProperty value, object parameter, CultureInfo culture)
//        {
//            Type type = value.Type;

//            if (type == typeof(bool) || type == typeof(bool?))

//                return new CheckBoxProperty(value);
//        }

//        public override Temp.IProperty ConvertBack(PropertyViewModel value, object parameter, CultureInfo culture) => (value ?? throw GetArgumentNullException(nameof(value))).Model;
//    }

//    [TypeForDataTemplate(typeof(ICheckBoxModel))]
//    public class CheckBoxProperty : PropertyViewModel, ICheckBoxModel
//    {
//        public object Content { get => null; set => throw new InvalidOperationException(); }

//        public bool? IsChecked { get => IsThreeState ? (bool?)ModelGeneric.Value : (bool)ModelGeneric.Value; set => ModelGeneric; }

//        public bool IsThreeState { get => ModelGeneric.Type == typeof(bool?); set => throw new InvalidOperationException(); }

//        public ICommand Command { get => throw new InvalidOperationException(); set => throw new InvalidOperationException(); }

//        public object CommandParameter { get => throw new InvalidOperationException(); set => throw new InvalidOperationException(); }

//        public IInputElement CommandTarget { get => throw new InvalidOperationException(); set => throw new InvalidOperationException(); }

//        public CheckBoxProperty(Temp.IProperty property) : base(property) { /* Left empty. */ }
//    }

//    public class PropertyGrid : System.Windows.Controls.DataGrid
//    {
//        DataGridCheckBoxColumn
//        /// <summary>
//        /// Identifies the <see cref="Header1"/> dependency property.
//        /// </summary>
//        public static readonly DependencyProperty Header1Property = DependencyProperty.Register(nameof(Header1), typeof(object), typeof(PropertyGrid));

//        public object Header1 { get => GetValue(Header1Property); set => SetValue(Header1Property, value); }

//        /// <summary>
//        /// Identifies the <see cref="Header2"/> dependency property.
//        /// </summary>
//        public static readonly DependencyProperty Header2Property = DependencyProperty.Register(nameof(Header2), typeof(object), typeof(PropertyGrid));

//        public object Header2 { get => (ImageSource)GetValue(Header2Property); set => SetValue(Header2Property, value); }

//        /// <summary>
//        /// Identifies the <see cref="Icon"/> dependency property.
//        /// </summary>
//        public static readonly DependencyProperty IconProperty = DependencyProperty.Register(nameof(Icon), typeof(ImageSource), typeof(PropertyGrid));

//        public ImageSource Icon { get => (ImageSource)GetValue(IconProperty); set => SetValue(IconProperty, value); }

//        public static readonly DependencyProperty PropertiesProperty = DependencyProperty.Register(nameof(Properties), typeof(System.Collections.Generic.IEnumerable<Temp.IProperty>), typeof(PropertyGrid));

//        public System.Collections.Generic.IEnumerable<Temp.IProperty> Properties { get => (System.Collections.Generic.IEnumerable<Temp.IProperty>)GetValue(PropertiesProperty); set => SetValue(PropertiesProperty, value); }

//        static PropertyGrid() => DefaultStyleKeyProperty.OverrideMetadata(typeof(PropertyGrid), new FrameworkPropertyMetadata(typeof(PropertyGrid)));
//    }
//}
