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

using WinCopies.Collections.Abstraction.Generic;
using WinCopies.Collections.Generic;
using WinCopies.IO.ObjectModel;
using WinCopies.PropertySystem;

using static WinCopies.ThrowHelper;

namespace WinCopies.IO.PropertySystem
{

    public class ShellObjectPropertySystemCollection : PropertySystemCollection<PropertyId, ShellPropertyGroup>
    {
        #region Fields
        private readonly ShellPropertyCollection _nativeProperties;
        private System.Collections.Generic.IReadOnlyList<KeyValuePair<PropertyId, IProperty>> _properties;
        private ReadOnlyList<KeyValuePair<PropertyId, IProperty>, PropertyId> _keys;
        private ReadOnlyList<KeyValuePair<PropertyId, IProperty>, IProperty> _values;
        #endregion

        #region Properties
        private System.Collections.Generic.IReadOnlyList<KeyValuePair<PropertyId, IProperty>> _Properties
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

        public override System.Collections.Generic.IReadOnlyList<PropertyId> Keys => _keys
#if CS8
            ??=
#else
            ?? (_keys =
#endif
            new ReadOnlyList<KeyValuePair<PropertyId, IProperty>, PropertyId>(_Properties, keyValuePair => keyValuePair.Key)
#if !CS8
            )
#endif
            ;

        public override System.Collections.Generic.IReadOnlyList<IProperty> Values => _values
#if CS8
            ??=
#else
            ?? (_values =
#endif
            new ReadOnlyList<KeyValuePair<PropertyId, IProperty>, IProperty>(_Properties, keyValuePair => keyValuePair.Value)
#if !CS8
            )
#endif
            ;
        #endregion

        private ShellObjectPropertySystemCollection(in ShellObject shellObject) : this(shellObject.Properties.DefaultPropertyCollection) { /* Left empty. */ }

        private ShellObjectPropertySystemCollection(in ShellPropertyCollection shellProperties) => _nativeProperties = shellProperties;

        #region Methods
        private void PopulateDictionary()
        {
            var properties = new List<KeyValuePair<PropertyId, IProperty>>(Count);

            foreach (Microsoft.WindowsAPICodePack.Shell.PropertySystem.IShellProperty property in _nativeProperties.Where(p=>!string.IsNullOrEmpty(p.CanonicalName)))

                properties.Add(new KeyValuePair<PropertyId, IProperty>(new PropertyId(property.CanonicalName.Substring(property.CanonicalName.LastIndexOf('.') + 1), ShellProperty.GetPropertyGroup(property)), new ShellProperty(property)));

            _properties = properties.AsReadOnly();
        }

        public static ShellObjectPropertySystemCollection GetShellObjectPropertySystemCollection(in ShellPropertyCollection shellProperties) => new ShellObjectPropertySystemCollection(shellProperties ?? throw GetArgumentNullException(nameof(shellProperties)));

        public static ShellObjectPropertySystemCollection GetShellObjectPropertySystemCollection(in ShellObject shellObject) => new ShellObjectPropertySystemCollection(shellObject ?? throw GetArgumentNullException(nameof(shellObject)));

        internal static ShellObjectPropertySystemCollection _GetShellObjectPropertySystemCollection<T>(in IEncapsulatorBrowsableObjectInfo<T> browsableObjectInfo) where T : ShellObject => new ShellObjectPropertySystemCollection(browsableObjectInfo.InnerObject);

        public static ShellObjectPropertySystemCollection GetShellObjectPropertySystemCollection<T>(in IEncapsulatorBrowsableObjectInfo<T> browsableObjectInfo) where T : ShellObject => _GetShellObjectPropertySystemCollection(browsableObjectInfo ?? throw GetArgumentNullException(nameof(ShellObjectInfo)));

        public override IEnumeratorInfo2<KeyValuePair<PropertyId, IProperty>> GetKeyValuePairEnumerator() => new Enumerator(_Properties);

        public override IEnumeratorInfo2<KeyValuePair<PropertyId, IProperty>> GetReversedKeyValuePairEnumerator() => new ReversedEnumerator(_Properties);
        #endregion
    }
}

#endif
