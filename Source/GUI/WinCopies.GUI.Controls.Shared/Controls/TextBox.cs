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

using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using WinCopies.Commands;
using WinCopies.GUI.Controls.Models;

using static WinCopies.GUI.Controls.ButtonTextBoxModel;

namespace WinCopies.GUI.Controls
{
    /// <summary>
    /// Represents a <see cref="System.Windows.Controls.TextBox"/> that can display items on the left and right of the text.
    /// </summary>
    public class TextBox : System.Windows.Controls.TextBox
    {
        /// <summary>
        /// Identifies the <see cref="LeftItems"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LeftItemsProperty = DependencyProperty.Register(nameof(LeftItems), typeof(IEnumerable), typeof(TextBox));

        public IEnumerable LeftItems { get => (IEnumerable)GetValue(LeftItemsProperty); set => SetValue(LeftItemsProperty, value); }

        /// <summary>
        /// Identifies the <see cref="RightItems"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty RightItemsProperty = DependencyProperty.Register(nameof(RightItems), typeof(IEnumerable), typeof(TextBox));

        public IEnumerable RightItems { get => (IEnumerable)GetValue(RightItemsProperty); set => SetValue(RightItemsProperty, value); }

        /// <summary>
        /// Identifies the <see cref="LeftItemsStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LeftItemsStyleProperty = DependencyProperty.Register(nameof(LeftItemsStyle), typeof(Style), typeof(TextBox));

        public Style LeftItemsStyle { get => (Style)GetValue(LeftItemsStyleProperty); set => SetValue(LeftItemsStyleProperty, value); }

        /// <summary>
        /// Identifies the <see cref="RightItemsStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty RightItemsStyleProperty = DependencyProperty.Register(nameof(RightItemsStyle), typeof(Style), typeof(TextBox));

        public Style RightItemsStyle { get => (Style)GetValue(RightItemsStyleProperty); set => SetValue(RightItemsStyleProperty, value); }

        /// <summary>
        /// Identifies the <see cref="LeftItemsTemplateSelector"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LeftItemsTemplateSelectorProperty = DependencyProperty.Register(nameof(LeftItemsTemplateSelector), typeof(DataTemplateSelector), typeof(TextBox));

        public DataTemplateSelector LeftItemsTemplateSelector { get => (DataTemplateSelector)GetValue(LeftItemsTemplateSelectorProperty); set => SetValue(LeftItemsTemplateSelectorProperty, value); }

        /// <summary>
        /// Identifies the <see cref="RightItemsTemplateSelector"/> dependency property.
        /// </summary>

        public static readonly DependencyProperty RightItemsTemplateSelectorProperty = DependencyProperty.Register(nameof(RightItemsTemplateSelector), typeof(DataTemplateSelector), typeof(TextBox));

        public DataTemplateSelector RightItemsTemplateSelector { get => (DataTemplateSelector)GetValue(RightItemsTemplateSelectorProperty); set => SetValue(RightItemsTemplateSelectorProperty, value); }

        static TextBox() => DefaultStyleKeyProperty.OverrideMetadata(typeof(TextBox), new FrameworkPropertyMetadata(typeof(TextBox)));
    }

    public class ButtonTextBox : TextBox
    {
        /// <summary>
        /// Identifies the <see cref="Buttons"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ButtonsProperty = DependencyProperty.Register(nameof(Buttons), typeof(System.Collections.Generic.IEnumerable<IButtonModel>), typeof(ButtonTextBox), new PropertyMetadata(GetDefaultButtons(), (DependencyObject d, DependencyPropertyChangedEventArgs e) => ((ButtonTextBox)d).OnButtonsChanged(e)));

        public System.Collections.Generic.IEnumerable<IButtonModel> Buttons { get => (System.Collections.Generic.IEnumerable<IButtonModel>)GetValue(ButtonsProperty); set => SetValue(ButtonsProperty, value); }

        /// <summary>
        /// Initializes a new instance of the <see cref="ButtonTextBox"/>.
        /// </summary>
        public ButtonTextBox() => AddButtons();

        protected virtual void OnButtonsChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != null)

                OnButtonCollectionRemoved((System.Collections.Generic.IEnumerable<IButtonModel>)e.OldValue);

            if (e.NewValue != null)

                OnButtonCollectionAdded((System.Collections.Generic.IEnumerable<IButtonModel>)e.NewValue);
        }

        protected virtual void OnButtonCollectionAdded(System.Collections.Generic.IEnumerable<IButtonModel> buttons)
        {
            OnButtonsAdded(buttons);

            if (buttons is INotifyCollectionChanged collection)

                collection.CollectionChanged += Buttons_CollectionChanged;
        }

        protected virtual void OnButtonCollectionRemoved(System.Collections.Generic.IEnumerable<IButtonModel> buttons)
        {
            OnButtonsRemoved(buttons);

            if (buttons is INotifyCollectionChanged collection)

                collection.CollectionChanged -= Buttons_CollectionChanged;
        }

        protected virtual void OnButtonsAdded(System.Collections.Generic.IEnumerable<IButtonModel> buttons)
        {
            foreach (IButtonModel button in buttons)

                OnButtonAdded(button);
        }

        protected virtual void OnButtonsRemoved(System.Collections.Generic.IEnumerable<IButtonModel> buttons)
        {
            foreach (IButtonModel button in buttons)

                OnButtonRemoved(button);
        }

        private void Buttons_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) => OnButtonsCollectionChanged(e);

        protected virtual void OnButtonsCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:

                    OnButtonsAdded(e.NewItems.To<IButtonModel>());

                    break;

                case NotifyCollectionChangedAction.Replace:

                    OnButtonsRemoved(e.OldItems.To<IButtonModel>());

                    OnButtonsAdded(e.NewItems.To<IButtonModel>());

                    break;

                case NotifyCollectionChangedAction.Remove:

                    OnButtonsRemoved(e.OldItems.To<IButtonModel>());

                    break;
            }
        }

        /// <summary>
        /// Adds the default buttons to the <see cref="Buttons"/> property.
        /// </summary>
        protected virtual void AddButtons()
        {
            if (Buttons != null)

                OnButtonCollectionAdded(Buttons);

            _ = CommandBindings.Add(new CommandBinding(DialogCommands.Cancel, (object sender, ExecutedRoutedEventArgs e) => OnCancel(e), (object sender, CanExecuteRoutedEventArgs e) => OnCanCancel(e)));
        }

        protected virtual void OnButtonAdded(IButtonModel button)
        {
            if (button.CommandTarget == null)

                button.CommandTarget = this;
        }

        protected virtual void OnButtonRemoved(IButtonModel button)
        {
            if (button.CommandTarget == this)

                button.CommandTarget = null;
        }

        /// <summary>
        /// Determines whether the <see cref="DialogCommands.Cancel"/> command can be executed. This method handles the command.
        /// </summary>
        /// <param name="e">The event args of the command.</param>
        protected virtual void OnCanCancel(CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !(IsReadOnly || string.IsNullOrEmpty(Text));

            e.Handled = true;
        }

        /// <summary>
        /// Clears <see cref="System.Windows.Controls.TextBox.Text"/>. This method is called on <see cref="DialogCommands.Cancel"/>. This method handles the command.
        /// </summary>
        /// <param name="e">The event args of the command.</param>
        protected virtual void OnCancel(ExecutedRoutedEventArgs e)
        {
            Clear();

            e.Handled = true;
        }
    }

    #region (View)Models
    public interface ITextBoxModel : IControlModel
    {
        IEnumerable LeftItems { get; set; }

        IEnumerable RightItems { get; set; }
    }

    public class TextBoxModelTextOriented : Models.TextBoxModelTextOriented, ITextBoxModel
    {
        public IEnumerable LeftItems { get; set; }

        public IEnumerable RightItems { get; set; }
    }

    public class TextBoxViewModelTextOriented<T> : ViewModels.TextBoxViewModelTextOriented<T>, ITextBoxModel where T : ITextBoxModel, ITextBoxModelTextOriented
    {
        public IEnumerable LeftItems { get => ModelGeneric.LeftItems; set { ModelGeneric.LeftItems = value; OnPropertyChanged(nameof(LeftItems)); } }

        public IEnumerable RightItems { get => ModelGeneric.RightItems; set { ModelGeneric.RightItems = value; OnPropertyChanged(nameof(RightItems)); } }

        public TextBoxViewModelTextOriented(T model) : base(model) { /* Left empty. */ }
    }

    public class TextBoxModelSelectionOriented : Models.TextBoxModelSelectionOriented, ITextBoxModel
    {
        public IEnumerable LeftItems { get; set; }

        public IEnumerable RightItems { get; set; }
    }

    public class TextBoxViewModelSelectionOriented<T> : ViewModels.TextBoxViewModelSelectionOriented<T>, ITextBoxModel where T : ITextBoxModel, ITextBoxModelSelectionOriented
    {
        public IEnumerable LeftItems { get => ModelGeneric.LeftItems; set { ModelGeneric.LeftItems = value; OnPropertyChanged(nameof(LeftItems)); } }

        public IEnumerable RightItems { get => ModelGeneric.RightItems; set { ModelGeneric.RightItems = value; OnPropertyChanged(nameof(RightItems)); } }

        public TextBoxViewModelSelectionOriented(T model) : base(model) { /* Left empty. */ }
    }

    public class TextBoxModelTextEditingOriented : Models.TextBoxModelTextEditingOriented, ITextBoxModel
    {
        public IEnumerable LeftItems { get; set; }

        public IEnumerable RightItems { get; set; }
    }

    public class TextBoxViewModelTextEditingOriented<T> : ViewModels.TextBoxViewModelTextEditingOriented<T>, ITextBoxModel where T : ITextBoxModel, ITextBoxModelTextEditingOriented
    {
        public IEnumerable LeftItems { get => ModelGeneric.LeftItems; set { ModelGeneric.LeftItems = value; OnPropertyChanged(nameof(LeftItems)); } }

        public IEnumerable RightItems { get => ModelGeneric.RightItems; set { ModelGeneric.RightItems = value; OnPropertyChanged(nameof(RightItems)); } }

        public TextBoxViewModelTextEditingOriented(T model) : base(model) { /* Left empty. */ }
    }

    public class TextBoxModel : Models.TextBoxModel, ITextBoxModel
    {
        public IEnumerable LeftItems { get; set; }

        public IEnumerable RightItems { get; set; }
    }

    public class TextBoxViewModel<T> : ViewModels.TextBoxViewModel<T>, ITextBoxModel where T : ITextBoxModel, Models.ITextBoxModel
    {
        public IEnumerable LeftItems { get => ModelGeneric.LeftItems; set { ModelGeneric.LeftItems = value; OnPropertyChanged(nameof(LeftItems)); } }

        public IEnumerable RightItems { get => ModelGeneric.RightItems; set { ModelGeneric.RightItems = value; OnPropertyChanged(nameof(RightItems)); } }

        public TextBoxViewModel(T model) : base(model) { /* Left empty. */ }
    }

    public interface ITextBoxModel2 : ITextBoxModel, Models.ITextBoxModel
    {
        // Left empty.
    }

    public interface IButtonTextBoxModel : ITextBoxModel
    {
        System.Collections.Generic.IEnumerable<IButtonModel> Buttons { get; set; }
    }

    public class ButtonTextBoxModelTextOriented : TextBoxModelTextOriented, IButtonTextBoxModel
    {
        public System.Collections.Generic.IEnumerable<IButtonModel> Buttons { get; set; }

        public ButtonTextBoxModelTextOriented() => Buttons = GetDefaultButtons();

        public ButtonTextBoxModelTextOriented(System.Collections.Generic.IEnumerable<IButtonModel> buttons) => Buttons = buttons;
    }

    public class ButtonTextBoxViewModelTextOriented<T> : TextBoxViewModelTextOriented<T>, IButtonTextBoxModel where T : IButtonTextBoxModel, ITextBoxModelTextOriented
    {
        public System.Collections.Generic.IEnumerable<IButtonModel> Buttons { get => ModelGeneric.Buttons; set { ModelGeneric.Buttons = value; OnPropertyChanged(nameof(Buttons)); } }

        public ButtonTextBoxViewModelTextOriented(T model) : base(model) { /* Left empty. */ }
    }

    public class ButtonTextBoxModelSelectionOriented : TextBoxModelSelectionOriented, IButtonTextBoxModel
    {
        public System.Collections.Generic.IEnumerable<IButtonModel> Buttons { get; set; }

        public ButtonTextBoxModelSelectionOriented() => Buttons = GetDefaultButtons();

        public ButtonTextBoxModelSelectionOriented(System.Collections.Generic.IEnumerable<IButtonModel> buttons) => Buttons = buttons;
    }

    public class ButtonTextBoxViewModelSelectionOriented<T> : TextBoxViewModelSelectionOriented<T>, IButtonTextBoxModel where T : IButtonTextBoxModel, ITextBoxModelSelectionOriented
    {
        public System.Collections.Generic.IEnumerable<IButtonModel> Buttons { get => ModelGeneric.Buttons; set { ModelGeneric.Buttons = value; OnPropertyChanged(nameof(Buttons)); } }

        public ButtonTextBoxViewModelSelectionOriented(T model) : base(model) { /* Left empty. */ }
    }

    public class ButtonTextBoxModelTextEditingOriented : TextBoxModelTextEditingOriented, IButtonTextBoxModel
    {
        public System.Collections.Generic.IEnumerable<IButtonModel> Buttons { get; set; }

        public ButtonTextBoxModelTextEditingOriented() => Buttons = GetDefaultButtons();

        public ButtonTextBoxModelTextEditingOriented(System.Collections.Generic.IEnumerable<IButtonModel> buttons) => Buttons = buttons;
    }

    public class ButtonTextBoxViewModelTextEditingOriented<T> : TextBoxViewModelTextEditingOriented<T>, IButtonTextBoxModel where T : IButtonTextBoxModel, ITextBoxModelTextEditingOriented
    {
        public System.Collections.Generic.IEnumerable<IButtonModel> Buttons { get => ModelGeneric.Buttons; set { ModelGeneric.Buttons = value; OnPropertyChanged(nameof(Buttons)); } }

        public ButtonTextBoxViewModelTextEditingOriented(T model) : base(model) { /* Left empty. */ }
    }

    public class ButtonTextBoxModel : TextBoxModel, IButtonTextBoxModel
    {
        public System.Collections.Generic.IEnumerable<IButtonModel> Buttons { get; set; }

        public static ObservableCollection<IButtonModel> GetDefaultButtons() => new ObservableCollection<IButtonModel>() { new ButtonModel<Bitmap>(Icons.Properties.Resources.cancel) { Command = DialogCommands.Cancel } };

        public ButtonTextBoxModel() => Buttons = GetDefaultButtons();

        public ButtonTextBoxModel(System.Collections.Generic.IEnumerable<IButtonModel> buttons) => Buttons = buttons;
    }

    public class ButtonTextBoxViewModel<T> : TextBoxViewModel<T>, IButtonTextBoxModel where T : ITextBoxModel, IButtonTextBoxModel, Models.ITextBoxModel
    {
        public System.Collections.Generic.IEnumerable<IButtonModel> Buttons { get => ModelGeneric.Buttons; set { ModelGeneric.Buttons = value; OnPropertyChanged(nameof(Buttons)); } }

        public ButtonTextBoxViewModel(T model) : base(model) { /* Left empty. */ }
    }

    public interface IButtonTextBoxModel2 : IButtonTextBoxModel, ITextBoxModel2
    {
        // Left empty.
    }
    #endregion
}
