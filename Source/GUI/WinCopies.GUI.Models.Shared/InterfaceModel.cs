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

using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using WinCopies.Commands;

namespace WinCopies.GUI.Controls.Models
{
    public interface IControlModel
    {
        bool IsEnabled { get; set; }
    }

    /// <summary>
    /// Represents a model that corresponds to a default view for <see cref="ContentControl"/>s.
    /// </summary>
    public interface IContentControlModel : IControlModel
    {
        /// <summary>
        /// Gets or sets the content of this <see cref="IContentControlModel"/>.
        /// </summary>
        object Content { get; set; }
    }

    public interface IContentDecorationControl
    {
        object ContentDecoration { get; set; }
    }

    public interface IToolTipControlModel
    {
        object ToolTip { get; set; }
    }

    /// <summary>
    /// Represents a model that corresponds to a default view for <see cref="ContentControl"/>s.
    /// </summary>
    public interface IContentControlModel<T> : IContentControlModel
    {
        /// <summary>
        /// Gets or sets the content of this <see cref="IContentControlModel{T}"/>.
        /// </summary>
        new T Content { get; set; }
    }

    /// <summary>
    /// Represents a model that corresponds to a default view for headered controls.
    /// </summary>
    public interface IHeaderedControlModel : IControlModel
    {
        /// <summary>
        /// Gets or sets the header of this <see cref="IHeaderedControlModel"/>.
        /// </summary>
        object Header { get; set; }
    }

    /// <summary>
    /// Represents a model that corresponds to a default view for <see cref="HeaderedContentControl"/>s.
    /// </summary>
    public interface IHeaderedContentControlModel : IContentControlModel, IHeaderedControlModel
    {
        // Left empty.
    }

    /// <summary>
    /// Represents a model that corresponds to a default view for headered controls.
    /// </summary>
    public interface IHeaderedControlModel<T> : IHeaderedControlModel
    {
        /// <summary>
        /// Gets or sets the header of this <see cref="IHeaderedControlModel{T}"/>.
        /// </summary>
        new T Header { get; set; }

#if CS8
        object IHeaderedControlModel.Header { get => Header; set => Header = (T)value; }
#endif
    }

    /// <summary>
    /// Represents a model that corresponds to a default view for <see cref="HeaderedContentControl"/>s.
    /// </summary>
    public interface IHeaderedContentControlModel<THeader, TContent> : IContentControlModel<TContent>, IHeaderedControlModel<THeader>, IHeaderedContentControlModel
    {
        // Left empty.
    }

    /// <summary>
    /// Represents a model that corresponds to a default view for <see cref="ItemsControl"/>s.
    /// </summary>
    public interface IItemsControlModel : IControlModel
    {
        /// <summary>
        /// Gets or sets the items of this <see cref="IItemsControlModel"/>.
        /// </summary>
        System.Collections.IEnumerable Items { get; set; }
    }

    /// <summary>
    /// Represents a model that corresponds to a default view for <see cref="ItemsControl"/>s.
    /// </summary>
    public interface IItemsControlModel<T> : IItemsControlModel
    {
        /// <summary>
        /// Gets or sets the items of this <see cref="IItemsControlModel{T}"/>.
        /// </summary>
        new System.Collections.Generic.IEnumerable<T> Items { get; set; }

#if CS8
        System.Collections.IEnumerable IItemsControlModel.Items { get => Items; set => Items = (System.Collections.Generic.IEnumerable<T>)value; }
#endif
    }

    /// <summary>
    /// Represents a model that corresponds to a default view for <see cref="HeaderedItemsControl"/>s.
    /// </summary>
    public interface IHeaderedItemsControlModel : IItemsControlModel, IHeaderedControlModel
    {
        // Left empty.
    }

    /// <summary>
    /// Represents a model that corresponds to a default view for <see cref="HeaderedItemsControl"/>s.
    /// </summary>
    public interface IHeaderedItemsControlModel<THeader, TItems> : IItemsControlModel<TItems>, IHeaderedControlModel<THeader>, IHeaderedItemsControlModel
    {
        // Left empty.
    }

    /// <summary>
    /// Represents a model that corresponds to a default view for <see cref="GroupBox"/> controls.
    /// </summary>
    public interface IGroupBoxModel : IHeaderedContentControlModel
    {
        // Left empty.
    }

    /// <summary>
    /// Represents a model that corresponds to a default view for <see cref="GroupBox"/> controls.
    /// </summary>
    public interface IGroupBoxModel<THeader, TContent> : IGroupBoxModel, IHeaderedContentControlModel<THeader, TContent>
    {
        // Left empty.
    }

    /// <summary>
    /// Represents a model that corresponds to a default view for <see cref="TabItem"/> controls.
    /// </summary>
    public interface ITabItemModel : IHeaderedContentControlModel
    {
        // Left empty.
    }

    /// <summary>
    /// Represents a model that corresponds to a default view for <see cref="TabItem"/> controls.
    /// </summary>
    public interface ITabItemModel<THeader, TContent> : ITabItemModel, IHeaderedContentControlModel<THeader, TContent>
    {
        // Left empty.
    }

    /// <summary>
    /// Represents a model that corresponds to a default view for <see cref="Button"/>s.
    /// </summary>
    public interface IButtonModel : IContentControlModel, ICommandSource
    {
        new ICommand Command { get; set; }

        new object CommandParameter { get; set; }

        new IInputElement CommandTarget { get; set; }

#if CS8
        ICommand ICommandSource.Command => Command;

        object ICommandSource.CommandParameter => CommandParameter;

        IInputElement ICommandSource.CommandTarget => CommandTarget;
#endif
    }

    /// <summary>
    /// Represents a model that corresponds to a default view for <see cref="Button"/>s.
    /// </summary>
    public interface IButtonModel<TContent> : IButtonModel, ICommandSource, IContentControlModel<TContent>
    {
        // left empty.
    }

    /// <summary>
    /// Represents a model that corresponds to a default view for <see cref="Button"/>s.
    /// </summary>
    public interface IButtonModel<TContent, TCommandParameter> : IButtonModel, ICommandSource<TCommandParameter>, IContentControlModel<TContent>
    {
        new ICommand<TCommandParameter> Command { get; set; }

        new TCommandParameter CommandParameter { get; set; }

#if CS8
        ICommand IButtonModel.Command { get => Command; set => Command = (ICommand<TCommandParameter>)value; }
#endif
    }

    public interface IExtendedButtonModel : IButtonModel, ICommandSource, IContentControlModel, IContentDecorationControl, IToolTipControlModel
    {
        // Left empty.
    }

    public interface IExtendedButtonModel<TContent, TCommandParameter> : IButtonModel<TContent, TCommandParameter>, IExtendedButtonModel
    {
        // Left empty.
    }

    /// <summary>
    /// Represents a model that corresponds to a default view for <see cref="ToggleButton"/>s.
    /// </summary>
    public interface IToggleButtonModel<TContent, TCommandParameter> : IToggleButtonModel, IButtonModel<TContent, TCommandParameter>
    {
        // Left empty.
    }

    /// <summary>
    /// Represents a model that corresponds to a default view for <see cref="CheckBox"/>'s.
    /// </summary>
    public interface ICheckBoxModel : IToggleButtonModel
    {
        // Left empty.
    }

    /// <summary>
    /// Represents a model that corresponds to a default view for <see cref="CheckBox"/>'.
    /// </summary>
    public interface ICheckBoxModel<TContent, TCommandParameter> : IToggleButtonModel<TContent, TCommandParameter>, ICheckBoxModel
    {
        // Left empty.
    }

    /// <summary>
    /// Represents a model that corresponds to a default view for <see cref="TextBox"/>'s.
    /// </summary>
    public interface ITextBoxModelTextOriented
#if WinCopies3
        : IControlModel
#endif
    {
        /// <summary>
        /// Gets or sets the text of this <see cref="ITextBoxModelTextOriented"/>.
        /// </summary>
        string Text { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether this <see cref="ITextBoxModelTextOriented"/> is read-only.
        /// </summary>
        bool IsReadOnly { get; set; }
    }

    /// <summary>
    /// Represents a model that corresponds to a default view for <see cref="TextBox"/>'s.
    /// </summary>
    public interface ITextBoxModelSelectionOriented : ITextBoxModelTextOriented
    {
        bool IsReadOnlyCaretVisible { get; set; }

        bool AutoWordSelection { get; set; }

        Brush SelectionBrush { get; set; }

        double SelectionOpacity { get; set; }

        Brush SelectionTextBrush { get; set; }

        Brush CaretBrush { get; set; }

        bool IsInactiveSelectionHighlightEnabled { get; set; }
    }

    /// <summary>
    /// Represents a model that corresponds to a default view for <see cref="TextBox"/>'s.
    /// </summary>
    public interface ITextBoxModelTextEditingOriented : ITextBoxModelTextOriented
    {
        int MinLines { get; set; }

        int MaxLines { get; set; }

        CharacterCasing CharacterCasing { get; set; }

        int MaxLength { get; set; }

        TextAlignment TextAlignment { get; set; }

        TextDecorationCollection TextDecorations { get; set; }

        TextWrapping TextWrapping { get; set; }

        bool AcceptsReturn { get; set; }

        bool AcceptsTab { get; set; }

        bool IsUndoEnabled { get; set; }

        int UndoLimit { get; set; }
    }

    /// <summary>
    /// Represents a model that corresponds to a default view for <see cref="TextBox"/>'s.
    /// </summary>
    public interface ITextBoxModel : ITextBoxModelTextOriented, ITextBoxModelTextEditingOriented, ITextBoxModelSelectionOriented
    {
        // Left empty.
    }

    /// <summary>
    /// Represents a model that corresponds to a default view for <see cref="RadioButton"/> collection.
    /// </summary>
    public interface IRadioButtonCollection : IEnumerable<IRadioButtonModel>
    {
        string GroupName { get; set; }
    }

    /// <summary>
    /// Represents a model that corresponds to a default view for <see cref="RadioButton"/> collection.
    /// </summary>
    public interface IRadioButtonCollection<TContent, TCommandParameter> : IRadioButtonCollection, IEnumerable<IRadioButtonModel<TContent, TCommandParameter>>
    {
        // Left empty.
    }

    /// <summary>
    /// Represents a model that corresponds to a default view for <see cref="RadioButton"/>s.
    /// </summary>
    public interface IRadioButtonModel : IToggleButtonModel
    {
        // Left empty.
    }

    /// <summary>
    /// Represents a model that corresponds to a default view for <see cref="RadioButton"/>s.
    /// </summary>
    public interface IRadioButtonModel<TContent, TCommandParameter> : IRadioButtonModel, IToggleButtonModel<TContent, TCommandParameter>
    {
        // Left empty.
    }

    /// <summary>
    /// Represents a model that corresponds to a default view for <see cref="RadioButton"/>s.
    /// </summary>
    public interface IGroupingRadioButtonModel : IRadioButtonModel
    {
        string GroupName { get; set; }
    }

    /// <summary>
    /// Represents a model that corresponds to a default view for <see cref="RadioButton"/>s.
    /// </summary>
    public interface IGroupingRadioButtonModel<TContent, TCommandParameter> : IGroupingRadioButtonModel, IRadioButtonModel<TContent, TCommandParameter>
    {
        // Left empty.
    }

    public interface IMenuItemModel : IHeaderedItemsControlModel, ICommandSource
    {
        new ICommand Command { get; set; }

        new object CommandParameter { get; set; }

        new IInputElement CommandTarget { get; set; }
    }

    public interface IMenuItemModel<T> : IMenuItemModel, IHeaderedControlModel<T>
    {
        // Left empty.
    }

    public interface IMenuItemModel<THeader, TItems, TCommandParameter> : IMenuItemModel<THeader>, IHeaderedItemsControlModel<THeader, TItems>, ICommandSource<TCommandParameter>
    {
        new ICommand<TCommandParameter> Command { get; set; }

        new TCommandParameter CommandParameter { get; set; }

#if CS8
        ICommand ICommandSource.Command => Command;

        object ICommandSource.CommandParameter => CommandParameter;

        ICommand IMenuItemModel.Command { get => Command; set => Command = (ICommand<TCommandParameter>)value; }

        object IMenuItemModel.CommandParameter { get => CommandParameter; set => CommandParameter = (TCommandParameter)value; }
#endif
    }
}
