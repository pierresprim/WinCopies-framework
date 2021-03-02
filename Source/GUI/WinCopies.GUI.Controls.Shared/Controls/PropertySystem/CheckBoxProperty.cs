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

using WinCopies.GUI.Controls.Models;
using WinCopies.PropertySystem;

namespace WinCopies.GUI.Controls.PropertySystem
{
    public class CheckBoxProperty : PropertyViewModel, ICheckBoxModel
    {
        public object Content { get => null; set => throw new InvalidOperationException(); }

        public bool? IsChecked { get => IsThreeState ? (bool?)Value : (bool)Value; set => Value = value; }

        public bool IsThreeState { get => ModelGeneric.Type == typeof(bool?); set => throw new InvalidOperationException(); }

        public ICommand Command { get => null; set => throw new InvalidOperationException(); }

        public object CommandParameter { get => null; set => throw new InvalidOperationException(); }

        public IInputElement CommandTarget { get => null; set => throw new InvalidOperationException(); }

        public CheckBoxProperty(IProperty property) : base(property) { /* Left empty. */ }
    }
}
