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

using System;
using System.Windows;
using System.Windows.Input;

using WinCopies.Collections.Generic;
using WinCopies.Commands;
using WinCopies.GUI.Controls.Models;
using WinCopies.GUI.Windows;
using WinCopies.PropertySystem;

namespace WinCopies.GUI.Controls.PropertySystem
{
    public class ArrayPropertyViewModel : PropertyViewModel, IButtonModel
    {
        private class SubProperty : IProperty
        {
            private readonly IProperty _innerProperty;
            private readonly int _index;
            private object _value;

            public bool IsReadOnly => _innerProperty.IsReadOnly;

            public object Value
            {
                get => _value
#if CS8
                    ??=
#else
                    ?? (_value =
#endif
                    ((Array)_innerProperty.Value).GetValue(_index)
#if !CS8
                    )
#endif
                    ; set => _value = value;
            }

            public bool IsEnabled => _innerProperty.IsEnabled;

            public string Name => throw new InvalidOperationException();

            public string DisplayName => throw new InvalidOperationException();

            public string Description => throw new InvalidOperationException();

            public string EditInvitation => _innerProperty.EditInvitation;

            public object PropertyGroup => throw new InvalidOperationException();

            public Type Type => _innerProperty.Type;

            object IReadOnlyProperty.Value => ((IReadOnlyProperty)_innerProperty).Value;

            internal SubProperty(in IProperty property, in int index)
            {
                _innerProperty = property;

                _index = index;
            }
        }

        #region Fields
        private Array _array;
        private PropertyViewModel[] _subProperties;
        private static ICommand _defaultCommand;
        private IInputElement _commandTarget;
        private static readonly string _content = "...";
        #endregion

        #region Properties
        private static ICommand DefaultCommand { get; } = _defaultCommand
#if CS8
            ??=
#else
            ?? (_defaultCommand =
#endif
            new DelegateCommand(value =>
            
            value == null ?  false : value is ArrayPropertyViewModel parameter ? !parameter.ModelGeneric.IsReadOnly : throw GetArgumentException(), _value =>
            {
                if (_value == null)

                    throw GetArgumentNullException();

                if (_value is ArrayPropertyViewModel _parameter)

                    if (new DialogWindow(_parameter.ModelGeneric.DisplayName) { Content = _parameter._subProperties }.ShowDialog() == true)
                    {
                        _parameter._array = Array.CreateInstance(_parameter._array.GetType().GetElementType(), _parameter._subProperties.Length);

                        for (int i = 0; i < _parameter._subProperties.Length; i++)

                            _parameter._array.SetValue(_parameter._subProperties[i].Value, i);
                    }
            })
#if !CS8
            )
#endif
            ;

        public PropertyViewModel[] SubProperties
        {
            get
            {
                if (_subProperties == null)
                {
                    Array array = _array;

                    _subProperties = new PropertyViewModel[array.Length];

                    for (int i = 0; i < _subProperties.Length; i++)

                        _subProperties[i] = PropertyConverter.TryConvert(new SubProperty(ModelGeneric, i));
                }

                return _subProperties;
            }
        }

        public override object Value { get => _array; set => throw new InvalidOperationException(); }

        public ICommand Command { get => DefaultCommand; set => throw new InvalidOperationException(); }

        public object CommandParameter { 
            
            get => this; set => throw new InvalidOperationException(); }

        public IInputElement CommandTarget { get => _commandTarget; set { _commandTarget = value; OnPropertyChanged(nameof(CommandTarget)); } }

        public object Content { get => _content; set => throw new InvalidOperationException(); }
        #endregion

        private ArrayPropertyViewModel(in IProperty property, in Array array) : base(property) => _array = array;

        #region Methods
        private static ArgumentException GetArgumentNullException() => new ArgumentException("The given parameter is null.");

        private static ArgumentException GetArgumentException() => new ArgumentException($"The given parameter must be an instance of {nameof(ArrayPropertyViewModel)}.");

        private static ArrayPropertyViewModel CreateFromNullValueProperty(in IProperty property) => new ArrayPropertyViewModel(property, Array.Empty<object>());

        public static ArrayPropertyViewModel CreateFromArrayProperty(in IProperty property) => property == null ? throw
#if WinCopies3
            ThrowHelper
#else
            Util
#endif
            .GetArgumentNullException(nameof(property)) : typeof(Array).IsAssignableFrom(property.Type) ? property.Value == null ? CreateFromNullValueProperty(property) : new ArrayPropertyViewModel(property, (Array)property.Value) : throw new ArgumentException($"{nameof(property)}'s property type is not assignable to {nameof(Array)}.");

        public static ArrayPropertyViewModel CreateFromEnumerableProperty(in IProperty property)
        {
            if (property == null) throw
#if WinCopies3
                    ThrowHelper
#else
                    Util
#endif
                    .GetArgumentNullException(nameof(property));

            if (property.Value == null) return CreateFromNullValueProperty(property);

            var arrayBuilder = new ArrayBuilder<object>();

            foreach (object value in (System.Collections.IEnumerable)property.Value)

                _ = arrayBuilder.AddLast(value);

            return new ArrayPropertyViewModel(property, arrayBuilder.ToArray());
        }
        #endregion
    }
}
