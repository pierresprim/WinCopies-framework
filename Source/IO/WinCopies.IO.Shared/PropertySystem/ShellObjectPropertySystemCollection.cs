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

#if WinCopies3

using Microsoft.WindowsAPICodePack.Shell;
using Microsoft.WindowsAPICodePack.Shell.PropertySystem;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using WinCopies.Collections.Generic;
using WinCopies.IO.ObjectModel;

using static WinCopies.ThrowHelper;

namespace WinCopies.IO.PropertySystem
{
    public class ShellProperty : IProperty
    {
        private readonly IShellProperty _shellProperty;
        private PropertyGroup? _propertyGroup;

        public bool IsReadOnly => _shellProperty.Description.TypeFlags.HasFlag(Microsoft.WindowsAPICodePack.COMNative.Shell.PropertySystem.PropertyTypeOptions.IsInnate);

        // public bool IsEnabled => !IsReadOnly;

        public string Name => _shellProperty.CanonicalName;

        public string DisplayName => _shellProperty.Description.DisplayName;

        public string Description => _shellProperty.Description.DisplayType.ToString();

        public string EditInvitation => _shellProperty.Description.EditInvitation;

        public PropertyGroup PropertyGroup => _propertyGroup
#if CS8
            ??=
#else
            .HasValue ? _propertyGroup.Value : (_propertyGroup =
#endif
            GetPropertyGroup(_shellProperty)
#if !CS8
            ).Value
#endif
            ;

        object Temp.IReadOnlyProperty.PropertyGroup => PropertyGroup;

        public object Value => _shellProperty.ValueAsObject;

        public Type Type => _shellProperty.ValueType;

        public ShellProperty(in IShellProperty shellProperty)
        {
            _shellProperty = shellProperty;
        }

        public static PropertyGroup GetPropertyGroup(in IShellProperty shellProperty)
        {
            ThrowIfNull(shellProperty, nameof(shellProperty));

            string propertyNamespace = shellProperty.CanonicalName.Substring(shellProperty.CanonicalName.IndexOf('.'
#if CS8
                , StringComparison.OrdinalIgnoreCase
#endif
                ) + 1);

            if (propertyNamespace.Contains('.'
#if CS8
                , StringComparison.OrdinalIgnoreCase
#endif
                ))
            {
                propertyNamespace = propertyNamespace.Remove(propertyNamespace.LastIndexOf('.'));

                System.Collections.Generic.IEnumerable<FieldInfo> propertyGroups = typeof(PropertyGroup).GetTypeInfo().DeclaredFields;

                return (PropertyGroup?)propertyGroups.FirstOrDefault(fieldInfo => propertyNamespace.StartsWith(fieldInfo.Name, StringComparison.OrdinalIgnoreCase))?.GetValue(null) ?? PropertyGroup.Default;
            }

            return PropertyGroup.Default;
        }
    }

    public class ShellObjectPropertySystemCollection : PropertySystemCollection
    {
        private readonly ShellPropertyCollection _nativeProperties;
        private IReadOnlyList<KeyValuePair<PropertyId, IProperty>> _properties;
        private Temp.ReadOnlyList<KeyValuePair<PropertyId, IProperty>, PropertyId> _keys;
        private Temp.ReadOnlyList<KeyValuePair<PropertyId, IProperty>, IProperty> _values;

        private IReadOnlyList<KeyValuePair<PropertyId, IProperty>> _Properties
        {
            get
            {
                if (_properties == null)

                    PopulateDictionary();

                return _properties;
            }
        }

        public override IProperty this[int index] => _Properties[index].Value;

        public override IProperty this[PropertyId key] => _Properties.First(keyValuePair => keyValuePair.Key == key).Value;

        public override int Count => _nativeProperties.Count;

        private void PopulateDictionary()
        {
            var properties = new List<KeyValuePair<PropertyId, IProperty>>(Count);

            foreach (IShellProperty property in _nativeProperties)

                properties.Add(new KeyValuePair<PropertyId, IProperty>(new PropertyId(property.CanonicalName.Substring(property.CanonicalName.LastIndexOf('.') + 1), ShellProperty.GetPropertyGroup(property)), new ShellProperty(property)));

            _properties = properties.AsReadOnly();
        }

        public override IReadOnlyList<PropertyId> Keys => _keys
#if CS8
            ??=
#else
            ?? (_keys =
#endif
            new Temp.ReadOnlyList<KeyValuePair<PropertyId, IProperty>, PropertyId>(_Properties, keyValuePair => keyValuePair.Key)
#if !CS8
            )
#endif
            ;

        public override IReadOnlyList<IProperty> Values => _values
#if CS8
            ??=
#else
            ?? (_values =
#endif
            new Temp.ReadOnlyList<KeyValuePair<PropertyId, IProperty>, IProperty>(_Properties, keyValuePair => keyValuePair.Value)
#if !CS8
            )
#endif
            ;

        private ShellObjectPropertySystemCollection(in ShellPropertyCollection shellProperties) => _nativeProperties = shellProperties;

        public static ShellObjectPropertySystemCollection GetShellObjectPropertySystemCollection(in ShellPropertyCollection shellProperties) => new ShellObjectPropertySystemCollection(shellProperties ?? throw GetArgumentNullException(nameof(shellProperties)));

        private ShellObjectPropertySystemCollection(in ShellObject shellObject) : this(shellObject.Properties.DefaultPropertyCollection) { /* Left empty. */ }

        public static ShellObjectPropertySystemCollection GetShellObjectPropertySystemCollection(in ShellObject shellObject) => new ShellObjectPropertySystemCollection(shellObject ?? throw GetArgumentNullException(nameof(shellObject)));

        internal static ShellObjectPropertySystemCollection _GetShellObjectPropertySystemCollection<T>(in IEncapsulatorBrowsableObjectInfo<T> browsableObjectInfo) where T : ShellObject => new ShellObjectPropertySystemCollection(browsableObjectInfo.EncapsulatedObject);

        public static ShellObjectPropertySystemCollection GetShellObjectPropertySystemCollection<T>(in IEncapsulatorBrowsableObjectInfo<T> browsableObjectInfo) where T : ShellObject => _GetShellObjectPropertySystemCollection(browsableObjectInfo ?? throw GetArgumentNullException(nameof(ShellObjectInfo)));

        public override IEnumeratorInfo2<KeyValuePair<PropertyId, IProperty>> GetKeyValuePairEnumerator() => new Enumerator(this);

        public override IEnumeratorInfo2<KeyValuePair<PropertyId, IProperty>> GetReversedKeyValuePairEnumerator() => new ReversedEnumerator(this);

        public class Enumerator : EnumeratorInfo<KeyValuePair<PropertyId, IProperty>>
        {
            public override bool? IsResetSupported => true;

            internal Enumerator(in ShellObjectPropertySystemCollection properties) : base(properties._properties) { /* Left empty. */ }

            public static Enumerator GetNewEnumerator(in ShellObjectPropertySystemCollection properties) => new Enumerator(properties ?? throw GetArgumentNullException(nameof(properties)));
        }

        public class ReversedEnumerator : Enumerator<KeyValuePair<PropertyId, IProperty>>
        {
            private IReadOnlyList<KeyValuePair<PropertyId, IProperty>> _properties;
            private int _index = -1;
            private Func<bool> _moveNext;

            public override bool? IsResetSupported => true;

            protected override KeyValuePair<PropertyId, IProperty> CurrentOverride => _properties[_index];

            internal ReversedEnumerator(in ShellObjectPropertySystemCollection properties)
            {
                _properties = properties._properties;

                ResetMoveNext();
            }

            public static ReversedEnumerator GetNewReversedEnumerator(in ShellObjectPropertySystemCollection properties) => new ReversedEnumerator(properties ?? throw GetArgumentNullException(nameof(properties)));

            private void ResetMoveNext() => _moveNext = () =>
                {
                    _index = _properties.Count - 1;

                    if (_index < 0)

                        return false;

                    _moveNext = () =>
                      {
                          _index--;

                          return _index >= 0;
                      };

                    return true;
                };

            protected override bool MoveNextOverride() => _moveNext();

            protected override void ResetOverride()
            {
                base.ResetOverride();

                ResetMoveNext();
            }

            protected override void ResetCurrent() => _index = -1;
        }
    }
}

#endif
