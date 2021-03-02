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
using System.Diagnostics;
using System.Linq;
using System.Reflection;

using WinCopies.PropertySystem;

using static WinCopies.ThrowHelper;

namespace WinCopies.IO.PropertySystem
{
    [DebuggerDisplay("{" + nameof(ToString) + "(),nq}")]
    public class ShellProperty : IShellProperty
    {
        private readonly Microsoft.WindowsAPICodePack.Shell.PropertySystem.IShellProperty _shellProperty;
        private ShellPropertyGroup? _propertyGroup;

        public bool IsReadOnly => _shellProperty.Description.TypeFlags.HasFlag(Microsoft.WindowsAPICodePack.COMNative.Shell.PropertySystem.PropertyTypeOptions.IsInnate);

        public bool IsEnabled => !IsReadOnly;

        public string Name => _shellProperty.CanonicalName;

        public string DisplayName => _shellProperty.Description.DisplayName;

        public string Description => _shellProperty.Description.DisplayType.ToString();

        public string EditInvitation => _shellProperty.Description.EditInvitation;

        public ShellPropertyGroup PropertyGroup => _propertyGroup
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

        object IReadOnlyProperty.PropertyGroup => PropertyGroup;

        public object Value => _shellProperty.ValueAsObject;

        public Type Type => _shellProperty.ValueType;

        public ShellProperty(in Microsoft.WindowsAPICodePack.Shell.PropertySystem.IShellProperty shellProperty) => _shellProperty = shellProperty;

        public static ShellPropertyGroup GetPropertyGroup(in Microsoft.WindowsAPICodePack.Shell.PropertySystem.IShellProperty shellProperty)
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

                System.Collections.Generic.IEnumerable<FieldInfo> propertyGroups = typeof(ShellPropertyGroup).GetTypeInfo().DeclaredFields;

                return (ShellPropertyGroup?)propertyGroups.FirstOrDefault(fieldInfo => propertyNamespace.StartsWith(fieldInfo.Name, StringComparison.OrdinalIgnoreCase))?.GetValue(null) ?? ShellPropertyGroup.Default;
            }

            return ShellPropertyGroup.Default;
        }

        public override string ToString() => $"{PropertyGroup}.{Name}";
    }
}
