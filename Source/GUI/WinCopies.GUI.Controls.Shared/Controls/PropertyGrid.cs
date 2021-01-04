/*Copyright © Pierre Sprimont, 2020
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
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using WinCopies.Collections.Generic;
using WinCopies.GUI.Controls.Models;
using WinCopies.Util.Data;

using static WinCopies.
#if WinCopies3
    ThrowHelper
#else
    Util.Util
#endif
    ;

namespace WinCopies.GUI.Controls
{
    public class PropertyViewModel : ViewModel<Temp.IProperty>, Temp.IProperty
    {
        private object _value;
        private bool _isValueUpdated;

        internal new Temp.IProperty Model => ModelGeneric;

        public bool IsEnabled => ModelGeneric.IsEnabled;

        public string Name => ModelGeneric.Name;

        public string DisplayName => ModelGeneric.DisplayName;

        public string Description => ModelGeneric.Description;

        public string EditInvitation => ModelGeneric.EditInvitation;

        public object PropertyGroup => ModelGeneric.PropertyGroup;

        public virtual object Value { get => _isValueUpdated ? _value : ModelGeneric.Value; set { _value = value; _isValueUpdated = true; } }

        public Type Type => ModelGeneric.Type;

        public PropertyViewModel(Temp.IProperty property) : base(property) { /* Left empty. */ }
    }

    public static class PropertyConverter
    {
        public static PropertyViewModel Convert(Temp.IProperty property)
        {
            ThrowIfNull(property, nameof(property));

            Type type = property.Type;

            if (typeof(Array).IsAssignableFrom(type))
            {
                var array = (Array)property.Value;

                if (array.Rank != 1)

                    return null;

                return ArrayPropertyViewModel.CreateFromArrayProperty(property);
            }

            if (typeof(System.Collections.IEnumerable).IsAssignableFrom(type))

                return ArrayPropertyViewModel.CreateFromEnumerableProperty(property);

            if (type == typeof(bool) || type == typeof(bool?))

                return new CheckBoxProperty(property);
        }
    }

    public class ArrayPropertyViewModel : PropertyViewModel
    {
        private class SubProperty : Temp.IProperty
        {
            private Temp.IProperty _innerProperty;
            private int _index;
            private object _value;

            public bool IsReadOnly => _innerProperty.IsReadOnly;

            public object Value { get => _value ??= ((Array)_innerProperty.Value).GetValue(_index); set => _value = value; }

            public bool IsEnabled => _innerProperty.IsEnabled;

            public string Name => throw new InvalidOperationException();

            public string DisplayName => throw new InvalidOperationException();

            public string Description => throw new InvalidOperationException();

            public string EditInvitation => _innerProperty.EditInvitation;

            public object PropertyGroup => throw new InvalidOperationException();

            public Type Type => _innerProperty.Type;

            object Temp.IReadOnlyProperty.Value => ((Temp.IReadOnlyProperty)_innerProperty).Value;

            internal SubProperty(in Temp.IProperty property, in int index)
            {
                _innerProperty = property;

                _index = index;
            }
        }

        private readonly Array _array;
        private PropertyViewModel[] _subProperties;

        public PropertyViewModel[] SubProperties
        {
            get
            {
                if (_subProperties == null)
                {
                    Array array = _array;

                    _subProperties = new PropertyViewModel[array.Length];

                    for (int i = 0; i < _subProperties.Length; i++)

                        _subProperties[i] = PropertyConverter.Convert(new SubProperty(ModelGeneric, i));
                }

                return _subProperties;
            }
        }

        private ArrayPropertyViewModel(in Temp.IProperty property, in Array array) : base(property) => _array = array;

        private static ArrayPropertyViewModel CreateFromNullValueProperty(in Temp.IProperty property) => new ArrayPropertyViewModel(property, Array.Empty<object>());

        public static ArrayPropertyViewModel CreateFromArrayProperty(in Temp.IProperty property) => property == null ? throw GetArgumentNullException(nameof(property)) : typeof(Array).IsAssignableFrom(property.Type) ? property.Value==null?CreateFromNullValueProperty(property): new ArrayPropertyViewModel(property, (Array)property.Value) : throw new ArgumentException($"{nameof(property)}'s property type is not assignable to {nameof(Array)}.");

        public static ArrayPropertyViewModel CreateFromEnumerableProperty(in Temp.IProperty property)
        {
            if (property == null) throw GetArgumentNullException(nameof(property));

            if (property.Value == null) return CreateFromNullValueProperty(property);

            var arrayBuilder = new ArrayBuilder<object>();

            foreach (object value in (System.Collections.IEnumerable)property.Value)

                arrayBuilder.AddLast(value);

            return new ArrayPropertyViewModel(property, arrayBuilder.ToArray());
        }

        public override object Value { get => _subProperties; set => throw new InvalidOperationException(); }
    }

    public class CheckBoxProperty : PropertyViewModel, ICheckBoxModel
    {
        public object Content { get => null; set => throw new InvalidOperationException(); }

        public bool? IsChecked { get => IsThreeState ? (bool?)ModelGeneric.Value : (bool?)ModelGeneric.Value; set => ModelGeneric.Value = value; }

        public bool IsThreeState { get => ModelGeneric.Type == typeof(bool?); set => throw new InvalidOperationException(); }

        public ICommand Command { get => throw new InvalidOperationException(); set => throw new InvalidOperationException(); }

        public object CommandParameter { get => throw new InvalidOperationException(); set => throw new InvalidOperationException(); }

        public IInputElement CommandTarget { get => throw new InvalidOperationException(); set => throw new InvalidOperationException(); }

        public CheckBoxProperty(Temp.IProperty property) : base(property) { /* Left empty. */ }
    }

    public class PropertyGrid : System.Windows.Controls.DataGrid
    {
        datagridcolumn
        /// <summary>
        /// Identifies the <see cref="Header1"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty Header1Property = DependencyProperty.Register(nameof(Header1), typeof(object), typeof(PropertyGrid));

        public object Header1 { get => GetValue(Header1Property); set => SetValue(Header1Property, value); }

        /// <summary>
        /// Identifies the <see cref="Header2"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty Header2Property = DependencyProperty.Register(nameof(Header2), typeof(object), typeof(PropertyGrid));

        public object Header2 { get => (ImageSource)GetValue(Header2Property); set => SetValue(Header2Property, value); }

        /// <summary>
        /// Identifies the <see cref="Icon"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IconProperty = DependencyProperty.Register(nameof(Icon), typeof(ImageSource), typeof(PropertyGrid));

        public ImageSource Icon { get => (ImageSource)GetValue(IconProperty); set => SetValue(IconProperty, value); }

        public static readonly DependencyProperty PropertiesProperty = DependencyProperty.Register(nameof(Properties), typeof(System.Collections.Generic.IEnumerable<Temp.IProperty>), typeof(PropertyGrid));

        public System.Collections.Generic.IEnumerable<Temp.IProperty> Properties { get => (System.Collections.Generic.IEnumerable<Temp.IProperty>)GetValue(PropertiesProperty); set => SetValue(PropertiesProperty, value); }

        static PropertyGrid() => DefaultStyleKeyProperty.OverrideMetadata(typeof(PropertyGrid), new FrameworkPropertyMetadata(typeof(PropertyGrid)));
    }
}
