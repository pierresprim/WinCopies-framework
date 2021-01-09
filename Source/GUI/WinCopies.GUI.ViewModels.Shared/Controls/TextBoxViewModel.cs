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

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using WinCopies.GUI.Controls.Models;
using WinCopies.Util.Data;

#if !WinCopies3
using WinCopies.Util;
#endif

namespace WinCopies.GUI.Controls.ViewModels
{
    [TypeForDataTemplate(typeof(ITextBoxModelTextOriented))]
    public class TextBoxViewModelTextOriented<T> : ViewModel<T>, ITextBoxModelTextOriented where T : ITextBoxModelTextOriented
    {
        public bool IsEnabled { get => ModelGeneric.IsEnabled; set { ModelGeneric.IsEnabled = value; OnPropertyChanged(nameof(IsEnabled)); } }

        public string Text { get => ModelGeneric.Text; set => Update(nameof(Text), value, typeof(ITextBoxModelTextOriented)); }

        public bool IsReadOnly { get => ModelGeneric.IsReadOnly; set => Update(nameof(IsReadOnly), value, typeof(ITextBoxModelTextOriented)); }

        public TextBoxViewModelTextOriented(T model) : base(model) { }
    }

    [TypeForDataTemplate(typeof(ITextBoxModelSelectionOriented))]
    public class TextBoxViewModelSelectionOriented<T> : TextBoxViewModelTextOriented<T>, ITextBoxModelSelectionOriented where T : ITextBoxModelSelectionOriented
    {
        public bool IsReadOnlyCaretVisible { get => ModelGeneric.IsReadOnlyCaretVisible; set => Update(nameof(IsReadOnlyCaretVisible), value, typeof(ITextBoxModelSelectionOriented)); }

        public bool AutoWordSelection { get => ModelGeneric.AutoWordSelection; set => Update(nameof(AutoWordSelection), value, typeof(ITextBoxModelSelectionOriented)); }

        public Brush SelectionBrush { get => ModelGeneric.SelectionBrush; set => Update(nameof(SelectionBrush), value, typeof(ITextBoxModelSelectionOriented)); }

        public double SelectionOpacity { get => ModelGeneric.SelectionOpacity; set => Update(nameof(SelectionOpacity), value, typeof(ITextBoxModelSelectionOriented)); }

        public Brush SelectionTextBrush { get => ModelGeneric.SelectionTextBrush; set => Update(nameof(SelectionTextBrush), value, typeof(ITextBoxModelSelectionOriented)); }

        public Brush CaretBrush { get => ModelGeneric.CaretBrush; set => Update(nameof(CaretBrush), value, typeof(ITextBoxModelSelectionOriented)); }

        public bool IsInactiveSelectionHighlightEnabled { get => ModelGeneric.IsInactiveSelectionHighlightEnabled; set => Update(nameof(IsInactiveSelectionHighlightEnabled), value, typeof(ITextBoxModelSelectionOriented)); }

        public TextBoxViewModelSelectionOriented(T model) : base(model) { }
    }

    [TypeForDataTemplate(typeof(ITextBoxModelTextEditingOriented))]
    public class TextBoxViewModelTextEditingOriented<T> : TextBoxViewModelTextOriented<T>, ITextBoxModelTextEditingOriented where T : ITextBoxModelTextEditingOriented
    {
        public int MinLines { get => ModelGeneric.MinLines; set => Update(nameof(MinLines), value, typeof(ITextBoxModelTextEditingOriented)); }

        public int MaxLines { get => ModelGeneric.MaxLines; set => Update(nameof(MaxLines), value, typeof(ITextBoxModelTextEditingOriented)); }

        public CharacterCasing CharacterCasing { get => ModelGeneric.CharacterCasing; set => Update(nameof(CharacterCasing), value, typeof(ITextBoxModelTextEditingOriented)); }

        public int MaxLength { get => ModelGeneric.MaxLength; set => Update(nameof(MaxLength), value, typeof(ITextBoxModelTextEditingOriented)); }

        public TextAlignment TextAlignment { get => ModelGeneric.TextAlignment; set => Update(nameof(TextAlignment), value, typeof(ITextBoxModelTextEditingOriented)); }

        public TextDecorationCollection TextDecorations { get => ModelGeneric.TextDecorations; set => Update(nameof(TextDecorations), value, typeof(ITextBoxModelTextEditingOriented)); }

        public TextWrapping TextWrapping { get => ModelGeneric.TextWrapping; set => Update(nameof(TextWrapping), value, typeof(ITextBoxModelTextEditingOriented)); }

        public bool AcceptsReturn { get => ModelGeneric.AcceptsReturn; set => Update(nameof(AcceptsReturn), value, typeof(ITextBoxModelTextEditingOriented)); }

        public bool AcceptsTab { get => ModelGeneric.AcceptsTab; set => Update(nameof(AcceptsTab), value, typeof(ITextBoxModelTextEditingOriented)); }

        public bool IsUndoEnabled { get => ModelGeneric.IsUndoEnabled; set => Update(nameof(IsUndoEnabled), value, typeof(ITextBoxModelTextEditingOriented)); }

        public int UndoLimit { get => ModelGeneric.UndoLimit; set => Update(nameof(UndoLimit), value, typeof(ITextBoxModelTextEditingOriented)); }

        public TextBoxViewModelTextEditingOriented(T model) : base(model) { }
    }

    [TypeForDataTemplate(typeof(ITextBoxModel))]
    public class TextBoxViewModel<T> : TextBoxViewModelTextOriented<T>, ITextBoxModel where T : ITextBoxModel
    {
        public int MinLines { get => ModelGeneric.MinLines; set => Update(nameof(MinLines), value, typeof(ITextBoxModel)); }

        public int MaxLines { get => ModelGeneric.MaxLines; set => Update(nameof(MaxLines), value, typeof(ITextBoxModel)); }

        public CharacterCasing CharacterCasing { get => ModelGeneric.CharacterCasing; set => Update(nameof(CharacterCasing), value, typeof(ITextBoxModel)); }

        public int MaxLength { get => ModelGeneric.MaxLength; set => Update(nameof(MaxLength), value, typeof(ITextBoxModel)); }

        public TextAlignment TextAlignment { get => ModelGeneric.TextAlignment; set => Update(nameof(TextAlignment), value, typeof(ITextBoxModel)); }

        public TextDecorationCollection TextDecorations { get => ModelGeneric.TextDecorations; set => Update(nameof(TextDecorations), value, typeof(ITextBoxModel)); }

        public TextWrapping TextWrapping { get => ModelGeneric.TextWrapping; set => Update(nameof(TextWrapping), value, typeof(ITextBoxModel)); }

        public bool AcceptsReturn { get => ModelGeneric.AcceptsReturn; set => Update(nameof(AcceptsReturn), value, typeof(ITextBoxModel)); }

        public bool AcceptsTab { get => ModelGeneric.AcceptsTab; set => Update(nameof(AcceptsTab), value, typeof(ITextBoxModel)); }

        public double SelectionOpacity { get => ModelGeneric.SelectionOpacity; set => Update(nameof(SelectionOpacity), value, typeof(ITextBoxModel)); }

        public bool IsUndoEnabled { get => ModelGeneric.IsUndoEnabled; set => Update(nameof(IsUndoEnabled), value, typeof(ITextBoxModel)); }

        public int UndoLimit { get => ModelGeneric.UndoLimit; set => Update(nameof(UndoLimit), value, typeof(ITextBoxModel)); }

        public bool IsReadOnlyCaretVisible { get => ModelGeneric.IsReadOnlyCaretVisible; set => Update(nameof(IsReadOnlyCaretVisible), value, typeof(ITextBoxModel)); }

        public bool AutoWordSelection { get => ModelGeneric.AutoWordSelection; set => Update(nameof(AutoWordSelection), value, typeof(ITextBoxModel)); }

        public Brush SelectionBrush { get => ModelGeneric.SelectionBrush; set => Update(nameof(SelectionBrush), value, typeof(ITextBoxModel)); }

        public Brush SelectionTextBrush { get => ModelGeneric.SelectionTextBrush; set => Update(nameof(SelectionTextBrush), value, typeof(ITextBoxModel)); }

        public Brush CaretBrush { get => ModelGeneric.CaretBrush; set => Update(nameof(CaretBrush), value, typeof(ITextBoxModel)); }

        public bool IsInactiveSelectionHighlightEnabled { get => ModelGeneric.IsInactiveSelectionHighlightEnabled; set => Update(nameof(IsInactiveSelectionHighlightEnabled), value, typeof(ITextBoxModel)); }

        public TextBoxViewModel(T model) : base(model) { }
    }
}
