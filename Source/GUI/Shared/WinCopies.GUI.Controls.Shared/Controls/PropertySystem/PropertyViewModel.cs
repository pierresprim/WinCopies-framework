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

using WinCopies.PropertySystem;
using WinCopies.Util.Data;

namespace WinCopies.GUI.Controls.PropertySystem
{
    public class PropertyViewModel : ViewModel<IProperty>, IProperty
    {
        private object _value;

        public bool IsValueUpdated { get; private set; }

        internal new IProperty Model => ModelGeneric;

        public bool IsReadOnly => ModelGeneric.IsReadOnly;

        public bool IsEnabled { get => ModelGeneric.IsEnabled; set => throw new InvalidOperationException(); }

        public string Name => ModelGeneric.Name;

        public string DisplayName => ModelGeneric.DisplayName;

        public string Description => ModelGeneric.Description;

        public string EditInvitation => ModelGeneric.EditInvitation;

        public object PropertyGroup => ModelGeneric.PropertyGroup;

        public virtual object Value { get => IsValueUpdated ? _value : ModelGeneric.Value; set { _value = value; IsValueUpdated = true; } }

        public Type Type => ModelGeneric.Type;

        public PropertyViewModel(IProperty property) : base(property) { /* Left empty. */ }
    }
}
