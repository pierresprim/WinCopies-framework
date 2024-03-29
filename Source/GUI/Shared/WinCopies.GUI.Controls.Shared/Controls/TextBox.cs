﻿/* Copyright © Pierre Sprimont, 2020
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
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using WinCopies.Commands;
using WinCopies.GUI.Controls.Models;

using static System.Windows.RoutingStrategy;

using static WinCopies.Commands.TextCommands;
using static WinCopies.GUI.Controls.ButtonTextBoxModel;

using SystemApplicationCommands = System.Windows.Input.ApplicationCommands;

namespace WinCopies.GUI.Controls
{
    internal static class TextBoxHelper
    {
#if !WinCopies4
        public static string FirstCharOfEachWordToUpper(this string s, params char[] separators)
        {
            string[] text = s.Split(separators);

            char[] c = new char[s.Length];

            int _j;

            string _text;

            for (int i = 0, j = 0; i < text.Length; i++)
            {
                _text = text[i];

                c[j] = char.ToUpper(_text[0], CultureInfo.CurrentCulture);

                for (j++, _j = 1; _j < _text.Length; j++, _j++)

                    c[j] = _text[_j];
            }

            return new string(c);
        }

        public static string Reverse(this string s)
        {
            char[] c = new char[s.Length];

            for (int i = 0; i < s.Length; i++)

                c[i] = s[s.Length - i - 1];

            return new string(c);
        }
#endif
    }

    namespace DotNetFix
    {
        public class TextBox : System.Windows.Controls.TextBox
        {
            private static RoutedEvent Register<T>(in string eventName, in RoutingStrategy routingStrategy) => Util.Desktop.UtilHelpers.Register<RoutedEventHandler, TextBox>(eventName, routingStrategy);

            public static DependencyProperty CaretIndexProperty = Util.Desktop.UtilHelpers.Register<int, TextBox>(nameof(CaretIndex), new PropertyMetadata(0, (DependencyObject d, DependencyPropertyChangedEventArgs e) => ((System.Windows.Controls.TextBox)d).CaretIndex = (int)e.NewValue));

            public new int CaretIndex { get => (int)GetValue(CaretIndexProperty); set => SetValue(CaretIndexProperty, value); }

            public static readonly RoutedEvent TripleClickEvent = Register<RoutedEventHandler>(nameof(TripleClick), Bubble);

            public event RoutedEventHandler TripleClick { add => AddHandler(TripleClickEvent, value); remove => RemoveHandler(TripleClickEvent, value); }

            public static readonly RoutedEvent PreviewTripleClickEvent = Register<RoutedEventHandler>(nameof(PreviewTripleClick), Tunnel);

            public event RoutedEventHandler PreviewTripleClick { add => AddHandler(PreviewTripleClickEvent, value); remove => RemoveHandler(PreviewTripleClickEvent, value); }

            static TextBox() => DefaultStyleKeyProperty.OverrideMetadata(typeof(TextBox), new FrameworkPropertyMetadata(typeof(TextBox)));

            public TextBox() => RegisterCommandBindings();

            protected virtual void AddCommandBinding(ICommand command, Action<ExecutedRoutedEventArgs> action, Action<CanExecuteRoutedEventArgs> predicate) => CommandBindings.Add(new CommandBinding(command, (object sender, ExecutedRoutedEventArgs e) => action(e), (object _sender, CanExecuteRoutedEventArgs _e) => predicate(_e)));

            protected virtual void AddCaseCommandBinding(ICommand command, Action<ExecutedRoutedEventArgs> action) => AddCommandBinding(command, action, OnCaseCommandCanExecute);

            protected virtual void RegisterCommandBindings()
            {
                void setPredicateValue(in CanExecuteRoutedEventArgs e, in bool value) => e.CanExecute = value;

                void predicate(CanExecuteRoutedEventArgs e) => setPredicateValue(e, true);

                void setAction(in ICommand command, Action action) => AddCommandBinding(command, e => OnExecuteCommand(action, e), predicate);

                void setActionGeneric<T>(in ICommand command, Func<T> action, Func<bool> func) => AddCommandBinding(command, e => OnExecuteCommand(() => action(), e), e => setPredicateValue(e, func()));

                setActionGeneric(SystemApplicationCommands.Undo, Undo, () => CanUndo);
                setActionGeneric(SystemApplicationCommands.Redo, Redo, () => CanRedo);

                setAction(SystemApplicationCommands.Copy, Copy);
                setAction(SystemApplicationCommands.Cut, Cut);
                setAction(SystemApplicationCommands.Paste, Paste);
                setAction(SystemApplicationCommands.Delete, Delete);

                setAction(SystemApplicationCommands.SelectAll, SelectAll);

                AddCaseCommandBinding(Upper, OnUpperCommandExecuted);
                AddCaseCommandBinding(Lower, OnLowerCommandExecuted);
                AddCaseCommandBinding(FirstCharUpper, OnFirstCharUpperCommandExecuted);
                AddCaseCommandBinding(FirstCharOfEachWordUpper, OnFirstCharOfEachWordUpperCommandExecuted);
                AddCaseCommandBinding(Reverse, OnReverseExecuted);
            }

            protected virtual void OnDelete(ExecutedRoutedEventArgs e) => OnExecuteCommand(Delete, e);

            public void Delete() => UpdateText(Delegates.NullIn);

            protected virtual void OnExecuteCommand(Action action, ExecutedRoutedEventArgs e)
            {
                action();

                e.Handled = true;
            }

            protected virtual void OnUpperCommandExecuted(ExecutedRoutedEventArgs e) => UpdateText((in string s) => s.ToUpper(CultureInfo.CurrentCulture), e);

            private void UpdateText(in FuncIn<string, string> action)
            {
                if (SelectionLength == 0)

                    Text = action(Text);

                else
                {
                    var sb = new StringBuilder();

                    if (SelectionStart > 0)

                        _ = sb.Append(Text.
#if CS8
                            AsSpan
#else
                            Substring
#endif
                            (0, SelectionStart));

                    _ = sb.Append(action(Text.Substring(SelectionStart, SelectionLength)));

                    int l = SelectionStart + SelectionLength;

                    if (Text.Length >= l)

                        _ = sb.Append(Text.
#if CS8
                            AsSpan
#else
                            Substring
#endif
                            (l));

                    int selectionStart = SelectionStart;

                    int selectionLength = SelectionLength;

                    Text = sb.ToString();

                    Select(selectionStart, selectionLength);
                }
            }

            private void UpdateText(in FuncIn<string, string> action, in ExecutedRoutedEventArgs e)
            {
                UpdateText(action);

                e.Handled = true;
            }

            protected virtual void OnLowerCommandExecuted(ExecutedRoutedEventArgs e) => UpdateText((in string s) => s.ToLower(CultureInfo.CurrentCulture), e);

            private static string ToUpper(in char c) => char.ToUpper(c, CultureInfo.CurrentCulture).ToString();

            protected virtual void OnFirstCharUpperCommandExecuted(ExecutedRoutedEventArgs e) => UpdateText((in string s) =>
            {
                string toUpper() => ToUpper(Text[0]);

                return s.Length == 1 ? toUpper() : $"{toUpper()}{Text.Substring(1)}";
            }, e);

            protected virtual void OnFirstCharOfEachWordUpperCommandExecuted(ExecutedRoutedEventArgs e) => UpdateText((in string s) => s.FirstCharOfEachWordToUpper(' '), e);

            protected virtual void OnReverseExecuted(ExecutedRoutedEventArgs e) => UpdateText((in string s) => s.Reverse(), e);

            protected virtual void OnCaseCommandCanExecute(CanExecuteRoutedEventArgs e)
            {
                e.CanExecute = !UtilHelpers.IsNullEmptyOrWhiteSpace(Text);

                e.Handled = true;
            }

            protected virtual void OnTripleClick(MouseButtonEventArgs e) { /* Left empty. */ }

            protected virtual void OnPreviewTripleClick(MouseButtonEventArgs e)
            {
                SelectAll();

                e.RoutedEvent = PreviewTripleClickEvent;

                RaiseEvent(e);
            }

            protected override void OnMouseDown(MouseButtonEventArgs e)
            {
                base.OnMouseDown(e);

                if (e.ClickCount == 3)

                    OnTripleClick(e);
            }

            protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
            {
                base.OnPreviewMouseDown(e);

                if (e.ClickCount == 3)

                    OnPreviewTripleClick(e);
            }
        }
    }

    /// <summary>
    /// Represents a <see cref="System.Windows.Controls.TextBox"/> that can display items on the left and right of the text.
    /// </summary>
    public class TextBox : DotNetFix.TextBox
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

                    OnButtonsAdded(e.NewItems.Cast<IButtonModel>());

                    break;

                case NotifyCollectionChangedAction.Replace:

                    OnButtonsRemoved(e.OldItems.Cast<IButtonModel>());

                    OnButtonsAdded(e.NewItems.Cast<IButtonModel>());

                    break;

                case NotifyCollectionChangedAction.Remove:

                    OnButtonsRemoved(e.OldItems.Cast<IButtonModel>());

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

        public static ObservableCollection<IButtonModel> GetDefaultButtons() => new
#if !CS9
            ObservableCollection<IButtonModel>
#endif
            () { new ButtonModel<Bitmap>(Icons.Properties.Resources.cancel) { Command = DialogCommands.Cancel } };

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
