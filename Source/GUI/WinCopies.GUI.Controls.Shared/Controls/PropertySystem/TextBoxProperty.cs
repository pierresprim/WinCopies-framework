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
using System.Collections;

using WinCopies.GUI.Controls.Models;
using WinCopies.PropertySystem;

namespace WinCopies.GUI.Controls.PropertySystem
{
    public class TextBoxProperty : PropertyViewModel, IPlaceholderTextBoxModel, ITextBoxModelTextOriented
    {
        public new bool IsReadOnly { get => base.IsReadOnly; set => throw new InvalidOperationException(); }

        public string Text { get => Value?.ToString(); set => Value = value; }

        public string Placeholder { get => ModelGeneric.EditInvitation; set => throw new InvalidOperationException(); }

        public PlaceholderMode PlaceholderMode { get => PlaceholderMode.OnTextChanged; set => throw new InvalidOperationException(); }

        private System.Collections.Generic.IEnumerable<IButtonModel> _buttons;

        public System.Collections.Generic.IEnumerable<IButtonModel> Buttons
        {
            get => _buttons
#if CS8
                ??=
#else
                ?? (_buttons =
#endif
                ButtonTextBoxModel.GetDefaultButtons()
#if !CS8
                )
#endif
                ; set => throw new InvalidOperationException();
        }

        public IEnumerable LeftItems { get => throw new InvalidOperationException(); set => throw new InvalidOperationException(); }

        public IEnumerable RightItems { get => throw new InvalidOperationException(); set => throw new InvalidOperationException(); }

        public TextBoxProperty(IProperty property) : base(property) { /* Left empty. */ }

    }
}
