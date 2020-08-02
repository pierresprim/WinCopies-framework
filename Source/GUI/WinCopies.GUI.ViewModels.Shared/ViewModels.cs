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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using WinCopies.GUI.Controls.Models;
using WinCopies.GUI.Windows.Dialogs;
using WinCopies.GUI.Windows.Dialogs.Models;
using WinCopies.GUI.Windows.Dialogs.ViewModels;
using WinCopies.Util;
using WinCopies.Util.Data;
using static WinCopies.Util.Util;

namespace WinCopies.GUI.Windows.Dialogs.ViewModels
{

    public class DialogViewModel<T> : ViewModel<T>, IDialogModel where T : IDialogModel
    {

        /// <summary>
        /// Gets or sets the title of this <see cref="DialogViewModel{T}"/>.
        /// </summary>
        public string Title { get => ModelGeneric.Title; set => Update(nameof(Title), value, typeof(IDialogModel)); }

        /// <summary>
        /// Gets or sets the <see cref="Dialogs.DialogButton"/> value of this <see cref="DialogViewModel{T}"/>.
        /// </summary>
        public DialogButton DialogButton { get => ModelGeneric.DialogButton; set => Update(nameof(DialogButton), value, typeof(IDialogModel)); }

        /// <summary>
        /// Gets or sets the <see cref="Dialogs.DefaultButton"/> value of this <see cref="DialogViewModel{T}"/>.
        /// </summary>
        public DefaultButton DefaultButton { get => ModelGeneric.DefaultButton; set => Update(nameof(DefaultButton), value, typeof(IDialogModel)); }

        /// <summary>
        /// Initializes a new instance of the <see cref="DialogViewModel{T}"/> class.
        /// </summary>
        /// <param name="model">The <typeparamref name="T"/> model to wrap in this <see cref="ViewModel{T}"/>.</param>
        public DialogViewModel(T model) : base(model) { }

    }

    //public class PropertyDialogViewModel<T> : DialogViewModel<T>, IPropertyDialogModel where T : IPropertyDialogModel

    //{

    //    /// <summary>
    //    /// Gets or sets the items of this <see cref="PropertyDialogViewModel{T}"/>.
    //    /// </summary>
    //    public IEnumerable<IPropertyTabItemModel> Items { get => ModelGeneric.Items; set => Update(nameof(Items), value, typeof(IPropertyDialogModel)); }

    //    /// <summary>
    //    /// Initializes a new instance of the <see cref="PropertyDialogViewModel{T}"/> class.
    //    /// </summary>
    //    /// <param name="model">The <typeparamref name="T"/> model to wrap in this <see cref="ViewModel{T}"/>.</param>
    //    public PropertyDialogViewModel(T model) : base(model) { }

    //}

}

namespace WinCopies.GUI.Controls.ViewModels

{

    [TypeForDataTemplate(typeof(IContentControlModel))]
    public class ContentControlViewModel<T> : ViewModel<T>, IContentControlModel, IDataTemplateSelectorsModel where T : IContentControlModel
    {

        private IModelDataTemplateSelectors _modelDataTemplateSelectors;

        private bool _autoAddDataTemplateSelectors;

        public object Content { get => ModelGeneric.Content; set { object oldValue = ModelGeneric.Content; ModelGeneric.Content = value; OnContentChanged(oldValue); } }

        public IModelDataTemplateSelectors ModelDataTemplateSelectors
        {

            get => (ModelGeneric as IDataTemplateSelectorsModel)?.ModelDataTemplateSelectors ?? _modelDataTemplateSelectors;

            set

            {

                if (ModelGeneric is IDataTemplateSelectorsModel dataTemplateSelectorsModel)

                    dataTemplateSelectorsModel.ModelDataTemplateSelectors = value;

                else

                    _modelDataTemplateSelectors = value;

                OnPropertyChanged(nameof(ModelDataTemplateSelectors));

            }

        }

        public bool AutoAddDataTemplateSelectors

        {

            get => (ModelGeneric as IDataTemplateSelectorsModel)?.AutoAddDataTemplateSelectors ?? _autoAddDataTemplateSelectors;

            set

            {

                if (ModelGeneric is IDataTemplateSelectorsModel dataTemplateSelectorsModel)

                    dataTemplateSelectorsModel.AutoAddDataTemplateSelectors = value;

                else

                    _autoAddDataTemplateSelectors = value;

                OnPropertyChanged(nameof(AutoAddDataTemplateSelectors));

            }

        }

        public BindingDirection BindingDirection { get; } = BindingDirection.OneWay;

        public ContentControlViewModel(T model) : base(model) { }

        protected virtual void OnContentChanged(object oldValue)

        {

            if (!(ModelGeneric is IDataTemplateSelectorsModel))

            {

                this.TryReset(oldValue);

                this.TryUpdate(ModelGeneric.Content, BindingDirection);

            }

            OnPropertyChanged(nameof(Content));

        }

    }

    [TypeForDataTemplate(typeof(IContentControlModel))]
    public class ContentControlViewModel<TModel, TContent> : ViewModel<TModel>, IContentControlModel<TContent>, IDataTemplateSelectorsModel where TModel : IContentControlModel<TContent>
    {

        private IModelDataTemplateSelectors _modelDataTemplateSelectors;

        private bool _autoAddDataTemplateSelectors;

        public TContent Content
        {
            get => ModelGeneric.Content;

            set { object oldValue = ModelGeneric.Content; ModelGeneric.Content = value; OnContentChanged(oldValue); }
        }

        object IContentControlModel.Content { get => Content; set => Content = (value ?? throw GetArgumentNullException(nameof(value))) is TContent _value ? _value : throw GetExceptionForInvalidType<TContent>(value.GetType().ToString(), nameof(value)); }

        public IModelDataTemplateSelectors ModelDataTemplateSelectors
        {

            get => (ModelGeneric as IDataTemplateSelectorsModel)?.ModelDataTemplateSelectors ?? _modelDataTemplateSelectors;

            set

            {

                if (ModelGeneric is IDataTemplateSelectorsModel dataTemplateSelectorsModel)

                    dataTemplateSelectorsModel.ModelDataTemplateSelectors = value;

                else

                    _modelDataTemplateSelectors = value;

                OnPropertyChanged(nameof(ModelDataTemplateSelectors));

            }

        }

        public bool AutoAddDataTemplateSelectors

        {

            get => (ModelGeneric as IDataTemplateSelectorsModel)?.AutoAddDataTemplateSelectors ?? _autoAddDataTemplateSelectors;

            set

            {

                if (ModelGeneric is IDataTemplateSelectorsModel dataTemplateSelectorsModel)

                    dataTemplateSelectorsModel.AutoAddDataTemplateSelectors = value;

                else

                    _autoAddDataTemplateSelectors = value;

                OnPropertyChanged(nameof(AutoAddDataTemplateSelectors));

            }

        }

        public BindingDirection BindingDirection { get; } = BindingDirection.OneWay;

        public ContentControlViewModel(TModel model) : base(model) { }

        protected virtual void OnContentChanged(object oldValue)

        {

            if (!(ModelGeneric is IDataTemplateSelectorsModel))

            {

                this.TryReset(oldValue);

                this.TryUpdate(ModelGeneric.Content, BindingDirection);

            }

            OnPropertyChanged(nameof(Content));

        }

    }

    [TypeForDataTemplate(typeof(IHeaderedContentControlModel))]
    public class HeaderedContentControlViewModel<T> : ContentControlViewModel<T>, IHeaderedContentControlModel where T : IHeaderedContentControlModel

    {

        public object Header { get => ModelGeneric.Header; set { object oldValue = ModelGeneric.Header; ModelGeneric.Header = value; OnHeaderChanged(oldValue); } }

        public HeaderedContentControlViewModel(T model) : base(model) { }

        protected virtual void OnHeaderChanged(object oldValue)

        {

            if (!(ModelGeneric is IDataTemplateSelectorsModel))

            {

                this.TryReset(oldValue);

                this.TryUpdate(ModelGeneric.Header, BindingDirection);

            }

            OnPropertyChanged(nameof(Header));

        }

    }

    [TypeForDataTemplate(typeof(IHeaderedContentControlModel))]
    public class HeaderedContentControlViewModel<TModel, THeader, TContent> : ContentControlViewModel<TModel, TContent>, IHeaderedContentControlModel<THeader, TContent> where TModel : IHeaderedContentControlModel<THeader, TContent>

    {

        public THeader Header { get => ModelGeneric.Header; set { object oldValue = ModelGeneric.Header; ModelGeneric.Header = value; OnHeaderChanged(oldValue); } }

        object IHeaderedControlModel.Header { get => Header; set => Header = (value ?? throw GetArgumentNullException(nameof(value))) is THeader _value ? _value : throw GetExceptionForInvalidType<THeader>(value.GetType().ToString(), nameof(value)); }

        public HeaderedContentControlViewModel(TModel model) : base(model) { }

        protected virtual void OnHeaderChanged(object oldValue)

        {

            if (!(ModelGeneric is IDataTemplateSelectorsModel))

            {

                this.TryReset(oldValue);

                this.TryUpdate(ModelGeneric.Header, BindingDirection);

            }

            OnPropertyChanged(nameof(Header));

        }

    }

    [TypeForDataTemplate(typeof(IItemsControlModel))]
    public class ItemsControlViewModel<T> : ViewModel<T>, IItemsControlModel, IDataTemplateSelectorsModel where T : IItemsControlModel
    {

        private IModelDataTemplateSelectors _modelDataTemplateSelectors;

        private bool _autoAddDataTemplateSelectors;

        public IEnumerable Items { get => ModelGeneric.Items; set { IEnumerable oldItems = ModelGeneric.Items; ModelGeneric.Items = value; OnItemsChanged(oldItems); } }

        public IModelDataTemplateSelectors ModelDataTemplateSelectors
        {

            get => (ModelGeneric as IDataTemplateSelectorsModel)?.ModelDataTemplateSelectors ?? _modelDataTemplateSelectors;

            set

            {

                if (ModelGeneric is IDataTemplateSelectorsModel dataTemplateSelectorsModel)

                    dataTemplateSelectorsModel.ModelDataTemplateSelectors = value;

                else

                    _modelDataTemplateSelectors = value;

                OnPropertyChanged(nameof(ModelDataTemplateSelectors));

            }

        }

        public bool AutoAddDataTemplateSelectors

        {

            get => (ModelGeneric as IDataTemplateSelectorsModel)?.AutoAddDataTemplateSelectors ?? _autoAddDataTemplateSelectors;

            set

            {

                if (ModelGeneric is IDataTemplateSelectorsModel dataTemplateSelectorsModel)

                    dataTemplateSelectorsModel.AutoAddDataTemplateSelectors = value;

                else

                    _autoAddDataTemplateSelectors = value;

                OnPropertyChanged(nameof(AutoAddDataTemplateSelectors));

            }

        }

        public BindingDirection BindingDirection { get; } = BindingDirection.OneWay;

        public ItemsControlViewModel(T model) : base(model) { }

        protected virtual void OnItemsChanged(IEnumerable oldItems)

        {

            if (!(ModelGeneric is IDataTemplateSelectorsModel))

            {

                this.TryReset(oldItems);

                this.TryUpdate(ModelGeneric.Items, BindingDirection);

            }

            OnPropertyChanged(nameof(Items));

        }

    }

    [TypeForDataTemplate(typeof(IItemsControlModel))]
    public class ItemsControlViewModel<TModel, TItems> : ViewModel<TModel>, IItemsControlModel<TItems>, IDataTemplateSelectorsModel where TModel : IItemsControlModel<TItems>

    {

        private IModelDataTemplateSelectors _modelDataTemplateSelectors;

        private bool _autoAddDataTemplateSelectors;

        public IEnumerable<TItems> Items { get => ModelGeneric.Items; set { IEnumerable oldItems = ModelGeneric.Items; ModelGeneric.Items = value; OnItemsChanged(oldItems); } }

        IEnumerable IItemsControlModel.Items { get => Items; set => Items = (value ?? throw GetArgumentNullException(nameof(value))) is IEnumerable<TItems> _value ? _value : throw GetExceptionForInvalidType<IEnumerable<TItems>>(value.GetType().ToString(), nameof(value)); }

        public IModelDataTemplateSelectors ModelDataTemplateSelectors
        {

            get => (ModelGeneric as IDataTemplateSelectorsModel)?.ModelDataTemplateSelectors ?? _modelDataTemplateSelectors;

            set

            {

                if (ModelGeneric is IDataTemplateSelectorsModel dataTemplateSelectorsModel)

                    dataTemplateSelectorsModel.ModelDataTemplateSelectors = value;

                else

                    _modelDataTemplateSelectors = value;

                OnPropertyChanged(nameof(ModelDataTemplateSelectors));

            }

        }

        public bool AutoAddDataTemplateSelectors

        {

            get => (ModelGeneric as IDataTemplateSelectorsModel)?.AutoAddDataTemplateSelectors ?? _autoAddDataTemplateSelectors;

            set

            {

                if (ModelGeneric is IDataTemplateSelectorsModel dataTemplateSelectorsModel)

                    dataTemplateSelectorsModel.AutoAddDataTemplateSelectors = value;

                else

                    _autoAddDataTemplateSelectors = value;

                OnPropertyChanged(nameof(AutoAddDataTemplateSelectors));

            }

        }

        public BindingDirection BindingDirection { get; } = BindingDirection.OneWay;

        public ItemsControlViewModel(TModel model) : base(model) { }

        protected virtual void OnItemsChanged(IEnumerable oldItems)

        {

            if (!(ModelGeneric is IDataTemplateSelectorsModel))

            {

                this.TryReset(oldItems);

                this.TryUpdate(ModelGeneric.Items, BindingDirection);

            }

            OnPropertyChanged(nameof(Items));

        }

    }

    [TypeForDataTemplate(typeof(IHeaderedItemsControlModel))]
    public class HeaderedItemsControlViewModel<T> : ItemsControlViewModel<T>, IHeaderedItemsControlModel where T : IHeaderedItemsControlModel

    {

        public object Header { get => ModelGeneric.Header; set { object oldValue = ModelGeneric.Header; ModelGeneric.Header = value; OnHeaderChanged(oldValue); } }

        public HeaderedItemsControlViewModel(T model) : base(model) { }

        protected virtual void OnHeaderChanged(object oldValue)

        {

            if (!(ModelGeneric is IDataTemplateSelectorsModel))

            {

                this.TryReset(oldValue);

                this.TryUpdate(ModelGeneric.Header, BindingDirection);

            }

            OnPropertyChanged(nameof(Header));

        }

    }

    [TypeForDataTemplate(typeof(IHeaderedItemsControlModel))]
    public class HeaderedItemsControlViewModel<TModel, THeader, TItems> : ItemsControlViewModel<TModel, TItems>, IHeaderedItemsControlModel<THeader, TItems> where TModel : IHeaderedItemsControlModel<THeader, TItems>

    {

        public THeader Header { get => ModelGeneric.Header; set { object oldValue = ModelGeneric.Header; ModelGeneric.Header = value; OnHeaderChanged(oldValue); } }

        object IHeaderedControlModel.Header { get => Header; set => Header = (value ?? GetArgumentNullException(nameof(value))) is THeader _value ? _value : throw GetExceptionForInvalidType<THeader>(value.GetType().ToString(), nameof(value)); }

        public HeaderedItemsControlViewModel(TModel model) : base(model) { }

        protected virtual void OnHeaderChanged(object oldValue)

        {

            if (!(ModelGeneric is IDataTemplateSelectorsModel))

            {

                this.TryReset(oldValue);

                this.TryUpdate(ModelGeneric.Header, BindingDirection);

            }

            OnPropertyChanged(nameof(Header));

        }

    }

    [TypeForDataTemplate(typeof(IGroupBoxModel))]
    public class GroupBoxViewModel<T> : HeaderedContentControlViewModel<T>, IGroupBoxModel where T : IGroupBoxModel
    {

        public GroupBoxViewModel(T model) : base(model) { }

    }

    [TypeForDataTemplate(typeof(IGroupBoxModel))]
    public class GroupBoxViewModel<TModel, THeader, TContent> : HeaderedContentControlViewModel<TModel, THeader, TContent>, IGroupBoxModel<THeader, TContent> where TModel : IGroupBoxModel<THeader, TContent>
    {

        public GroupBoxViewModel(TModel model) : base(model) { }

    }

    [TypeForDataTemplate(typeof(ITabItemModel))]
    public class TabItemViewModel<T> : HeaderedContentControlViewModel<T>, ITabItemModel where T : ITabItemModel

    {

        public TabItemViewModel(T model) : base(model) { }

    }

    [TypeForDataTemplate(typeof(ITabItemModel))]
    public class TabItemViewModel<TModel, THeader, TContent> : HeaderedContentControlViewModel<TModel, THeader, TContent>, ITabItemModel<THeader, TContent> where TModel : ITabItemModel<THeader, TContent>

    {

        public TabItemViewModel(TModel model) : base(model) { }

    }

    //[TypeForDataTemplate(typeof(IPropertyTabItemModel))]
    //public class PropertyTabItemViewModel<T> : HeaderedItemsControlViewModel<T, object, IGroupBoxModel>, IPropertyTabItemModel where T : IPropertyTabItemModel

    //{

    //    public PropertyTabItemViewModel(T model) : base(model) { }

    //}

    //[TypeForDataTemplate(typeof(IPropertyTabItemModel))]
    //public class PropertyTabItemViewModel<TModel, TItemHeader, TGroupBoxHeader, TGroupBoxContent> : HeaderedItemsControlViewModel<TModel, TItemHeader, IGroupBoxModel<TGroupBoxHeader, TGroupBoxContent>>, IPropertyTabItemModel<TItemHeader, TGroupBoxHeader, TGroupBoxContent> where TModel : IPropertyTabItemModel<TItemHeader, TGroupBoxHeader, TGroupBoxContent>

    //{

    //    IEnumerable<IGroupBoxModel> IPropertyTabItemModel.Items { get => Items; set => Items = GetOrThrowIfNotType<IEnumerable<IGroupBoxModel<TGroupBoxHeader, TGroupBoxContent>>>(value, nameof(value)); }

    //    public PropertyTabItemViewModel(TModel model) : base(model) { }

    //}

    [TypeForDataTemplate(typeof(IButtonModel))]
    public class ButtonViewModel<T> : ContentControlViewModel<T>, IButtonModel where T : IButtonModel

    {

        public ICommand Command { get => ModelGeneric.Command; set => Update(nameof(Command), value, typeof(IButtonModel)); }

        public object CommandParameter { get => ModelGeneric.CommandParameter; set => Update(nameof(CommandParameter), value, typeof(IButtonModel)); }

        public IInputElement CommandTarget { get => ModelGeneric.CommandTarget; set => Update(nameof(CommandTarget), value, typeof(IButtonModel)); }

        public ButtonViewModel(T model) : base(model) { }

    }

    [TypeForDataTemplate(typeof(IButtonModel))]
    public class ButtonViewModel<TModel, TContent> : ContentControlViewModel<TModel, TContent>, IButtonModel<TContent> where TModel : IButtonModel<TContent>

    {

        public ICommand Command { get => ModelGeneric.Command; set => Update(nameof(Command), value, typeof(IButtonModel)); }

        public object CommandParameter { get => ModelGeneric.CommandParameter; set => Update(nameof(CommandParameter), value, typeof(IButtonModel)); }

        public IInputElement CommandTarget { get => ModelGeneric.CommandTarget; set => Update(nameof(CommandTarget), value, typeof(IButtonModel)); }

        public ButtonViewModel(TModel model) : base(model) { }

    }

    [TypeForDataTemplate(typeof(IToggleButtonModel))]
    public class ToggleButtonViewModel<T> : ButtonViewModel<T>, IToggleButtonModel where T : IToggleButtonModel

    {

        public bool? IsChecked { get => ModelGeneric.IsChecked; set => Update(nameof(IsChecked), value, typeof(IToggleButtonModel)); }

        public bool IsThreeState { get => ModelGeneric.IsThreeState; set => Update(nameof(IsThreeState), value, typeof(IToggleButtonModel)); }

        public ToggleButtonViewModel(T model) : base(model) { }

    }

    [TypeForDataTemplate(typeof(IToggleButtonModel))]
    public class ToggleButtonViewModel<TModel, TContent> : ButtonViewModel<TModel, TContent>, IToggleButtonModel<TContent> where TModel : IToggleButtonModel<TContent>

    {

        public bool? IsChecked { get => ModelGeneric.IsChecked; set => Update(nameof(IsChecked), value, typeof(IToggleButtonModel)); }

        public bool IsThreeState { get => ModelGeneric.IsThreeState; set => Update(nameof(IsThreeState), value, typeof(IToggleButtonModel)); }

        public ToggleButtonViewModel(TModel model) : base(model) { }

    }

    [TypeForDataTemplate(typeof(ICheckBoxModel))]
    public class CheckBoxViewModel<T> : ToggleButtonViewModel<T>, ICheckBoxModel where T : ICheckBoxModel

    {

        public CheckBoxViewModel(T model) : base(model) { }

    }

    [TypeForDataTemplate(typeof(ICheckBoxModel))]
    public class CheckBoxViewModel<TModel, TContent> : ToggleButtonViewModel<TModel, TContent>, ICheckBoxModel<TContent> where TModel : ICheckBoxModel<TContent>

    {

        public CheckBoxViewModel(TModel model) : base(model) { }

    }

    [TypeForDataTemplate(typeof(ITextBoxModelTextOriented))]
    public class TextBoxViewModelTextOriented<T> : ViewModel<T>, ITextBoxModelTextOriented where T : ITextBoxModelTextOriented

    {

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

    [TypeForDataTemplate(typeof(IRadioButtonCollection))]
    public class ObservableRadioButtonCollection : ObservableCollection<IRadioButtonModel>, IRadioButtonCollection

    {

        public string GroupName
        {

            get => ((RadioButtonCollection)Items).GroupName; set

            {

                ((RadioButtonCollection)Items).GroupName = value;

                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs(nameof(GroupName)));

            }

        }

        public ObservableRadioButtonCollection(RadioButtonCollection items) : base(items) { }

    }

    [TypeForDataTemplate(typeof(IRadioButtonCollection))]
    public class ObservableRadioButtonCollection<T> : ObservableCollection<IRadioButtonModel<T>>, IRadioButtonCollection<T>

    {

        public string GroupName
        {

            get => ((RadioButtonCollection)Items).GroupName; set

            {

                ((RadioButtonCollection)Items).GroupName = value;

                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs(nameof(GroupName)));

            }

        }

        IEnumerator<IRadioButtonModel> IEnumerable<IRadioButtonModel>.GetEnumerator() => GetEnumerator();

        public ObservableRadioButtonCollection(RadioButtonCollection<T> items) : base(items) { }

    }

    [TypeForDataTemplate(typeof(IGroupingRadioButtonModel))]
    public class GroupingRadioButtonViewModel<T> : ToggleButtonViewModel<T>, IGroupingRadioButtonModel where T : IGroupingRadioButtonModel

    {

        public string GroupName { get => ModelGeneric.GroupName; set => Update(nameof(GroupName), value, typeof(IGroupingRadioButtonModel)); }

        public GroupingRadioButtonViewModel(T model) : base(model) { }

    }

    [TypeForDataTemplate(typeof(IGroupingRadioButtonModel))]
    public class GroupingRadioButtonViewModel<TModel, TContent> : ToggleButtonViewModel<TModel, TContent>, IGroupingRadioButtonModel where TModel : IGroupingRadioButtonModel<TContent>

    {

        public string GroupName { get => ModelGeneric.GroupName; set => Update(nameof(GroupName), value, typeof(IGroupingRadioButtonModel)); }

        public GroupingRadioButtonViewModel(TModel model) : base(model) { }

    }

}
