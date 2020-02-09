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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using WinCopies.Desktop;

using static WinCopies.Commands.DialogCommands;
using static WinCopies.Util.Desktop.UtilHelpers;

using static WinCopies.Diagnostics.IfHelpers;

using static WinCopies.Diagnostics.ComparisonType;
using static WinCopies.Diagnostics.ComparisonMode;
using static WinCopies.Diagnostics.Comparison;

namespace WinCopies.GUI.Windows
#if !WinCopies3
.Dialogs
#endif
{
    public abstract class DialogWindowBase : Window, ICommandSource
    {
        private static DependencyProperty Register<T>(in string propertyName) => Register<T, DialogWindowBase>(propertyName);

        /// <summary>
        /// Gets the <see cref="Windows.MessageBoxResult"/>.
        /// </summary>
        public MessageBoxResult MessageBoxResult { get; private set; } = MessageBoxResult.None;

        /// <summary>
        /// Identifies the <see cref="Command"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CommandProperty = Register<ICommand>(nameof(Command));

        /// <summary>
        /// Gets or sets the command that will be executed when the command source is invoked.
        /// </summary>
        public ICommand Command { get => (ICommand)GetValue(CommandProperty); set => SetValue(CommandProperty, value); }

        /// <summary>
        /// Identifies the <see cref="CommandParameter"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CommandParameterProperty = Register< object>(nameof(CommandParameter));

        /// <summary>
        /// Gets or sets a value that represents a user defined data value that can be passed to the command when it is executed.
        /// </summary>
        public object CommandParameter { get => GetValue(CommandParameterProperty); set => SetValue(CommandParameterProperty, value); }

        /// <summary>
        /// Identifies the <see cref="CommandTarget"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CommandTargetProperty = Register<IInputElement>(nameof(CommandTarget));

        /// <summary>
        /// Gets or sets a value that represents the object that the command is being executed on.
        /// </summary>
        public IInputElement CommandTarget { get => (IInputElement)GetValue(CommandTargetProperty); set => SetValue(CommandTargetProperty, value); }

        protected abstract bool CanExecuteCommand { get; }

        protected void CloseWindowWithDialogResult(bool dialogResult, MessageBoxResult messageBoxResult)
        {
            try
            {
                DialogResult = dialogResult;
            }

            catch (InvalidOperationException ex) when (ex.HResult == -2146233079) { }

            MessageBoxResult = messageBoxResult;

            if (dialogResult && Command != null && CanExecuteCommand)

                Command.Execute(CommandParameter, CommandTarget);

            Close();
        }

        protected virtual void OnCommandExecuted(ExecutedRoutedEventArgs e)
        {
            if (e.Command == Ok)

                CloseWindowWithDialogResult(true, MessageBoxResult.OK);

            else if (e.Command == Apply)

                Command.Execute(CommandParameter, CommandTarget);

            else if (e.Command == Yes)

                CloseWindowWithDialogResult(true, MessageBoxResult.Yes);

            else if (e.Command == No)

                CloseWindowWithDialogResult(false, MessageBoxResult.No);

            else if (e.Command == YesToAll)

                CloseWindowWithDialogResult(true, MessageBoxResult.YesToAll);

            else if (e.Command == NoToAll)

                CloseWindowWithDialogResult(false, MessageBoxResult.NoToAll);

            else if (e.Command == Cancel)

                CloseWindowWithDialogResult(false, MessageBoxResult.Cancel);

            else if (e.Command == Abort)

                CloseWindowWithDialogResult(false, MessageBoxResult.Abort);

            else if (e.Command == Ignore)

                CloseWindowWithDialogResult(false, MessageBoxResult.Ignore);

            else if (e.Command == Retry)

                CloseWindowWithDialogResult(true, MessageBoxResult.Retry);

            else if (e.Command == Continue)

                CloseWindowWithDialogResult(true, MessageBoxResult.Continue);
        }
    }

    /// <summary>
    /// Represents a common dialog window for WPF.
    /// </summary>
    public partial class DialogWindow : DialogWindowBase
    {
        /// <summary>
        /// Identifies the <see cref="DialogButton"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DialogButtonProperty = DependencyProperty.Register(nameof(DialogButton), typeof(DialogButton?), typeof(DialogWindow), new PropertyMetadata(
#if WinCopies3
            Windows.
#else
            WinCopies.GUI.Windows.Dialogs.
#endif
            DialogButton.OK, (DependencyObject sender, DependencyPropertyChangedEventArgs e) => ((DialogWindow)sender).OnDialogButtonChanged((DialogButton?)e.OldValue, (DialogButton?)e.NewValue)
        ));

        /// <summary>
        /// Gets or sets the <see cref="Windows.DialogButton"/>. <see cref="Command"/> will not be executed if this property is set to <see cref="DialogButton.OK"/>, or if the user clicks on one of the following buttons: Cancel, Ignore, Abort. This is a dependency property.
        /// </summary>
        public DialogButton? DialogButton { get => (DialogButton?)GetValue(DialogButtonProperty); set => SetValue(DialogButtonProperty, value); }

        /// <summary>
        /// Identifies the <see cref="ButtonAlignment"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ButtonAlignmentProperty = DependencyProperty.Register(nameof(ButtonAlignment), typeof(HorizontalAlignment), typeof(DialogWindow), new PropertyMetadata(Windows.HorizontalAlignment.Left));

        /// <summary>
        /// Gets or sets the <see cref="HorizontalAlignment"/> for the button alignment. This is a dependency property.
        /// </summary>
        public HorizontalAlignment ButtonAlignment { get => (HorizontalAlignment)GetValue(ButtonAlignmentProperty); set => SetValue(ButtonAlignmentProperty, value); }

        /// <summary>
        /// Identifies the <see cref="DefaultButton"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DefaultButtonProperty = DependencyProperty.Register(nameof(DefaultButton), typeof(DefaultButton), typeof(DialogWindow), new PropertyMetadata(DefaultButton.None, (DependencyObject d, DependencyPropertyChangedEventArgs e) => ((DialogWindow)d).OnDefaultButtonChanged((DefaultButton)e.OldValue, (DefaultButton)e.NewValue)));

        public DefaultButton DefaultButton { get => (DefaultButton)GetValue(DefaultButtonProperty); set => SetValue(DefaultButtonProperty, value); }

        /// <summary>
        /// Identifies the <see cref="CustomButtonsSource"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CustomButtonsSourceProperty = DependencyProperty.Register(nameof(CustomButtonsSource), typeof(IEnumerable), typeof(DialogWindow), new PropertyMetadata(null, (DependencyObject d, DependencyPropertyChangedEventArgs e) => ((DialogWindow)d).OnCustomButtonsSourceChanged((IEnumerable)e.OldValue, (IEnumerable)e.NewValue)));

        public IEnumerable CustomButtonsSource { get => (IEnumerable<Controls.Models.ButtonModel>)GetValue(CustomButtonsSourceProperty); set => SetValue(CustomButtonsSourceProperty, value); }

        public static readonly DependencyProperty ContentDecoratorStyleProperty = DependencyProperty.Register(nameof(ContentDecoratorStyle), typeof(Style), typeof(DialogWindow), new PropertyMetadata(new Style()));

        public Style ContentDecoratorStyle { get => (Style)GetValue(ContentDecoratorStyleProperty); set => SetValue(ContentDecoratorStyleProperty, value); }

        //public ItemCollection CustomButtons
        //{
        //    get
        //    {

        //        if (!(DialogButton is null) || !(CustomButtonsSource is null))

        //            return null;

        //        ApplyTemplate();

        //        OnApplyTemplate();

        //        return ((ItemsControl)Template?.FindName("PART_ItemsControl", this))?.Items;
        //    }
        //}

        /// <summary>
        /// Identifies the <see cref="CustomButtonTemplate"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CustomButtonTemplateProperty = DependencyProperty.Register(nameof(CustomButtonTemplate), typeof(DataTemplate), typeof(DialogWindow));

        public DataTemplate CustomButtonTemplate { get => (DataTemplate)GetValue(CustomButtonTemplateProperty); set => SetValue(CustomButtonTemplateProperty, value); }

        /// <summary>
        /// Identifies the <see cref="CustomButtonTemplateSelector"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CustomButtonTemplateSelectorProperty = DependencyProperty.Register(nameof(CustomButtonTemplateSelector), typeof(DataTemplateSelector), typeof(DialogWindow));

        public DataTemplateSelector CustomButtonTemplateSelector { get => (DataTemplateSelector)GetValue(CustomButtonTemplateSelectorProperty); set => SetValue(CustomButtonTemplateSelectorProperty, value); }

        /// <summary>
        /// Identifies the <see cref="ShowHelpButtonAsCommandButton"/> dependency property.
        /// </summary>
        public readonly static DependencyProperty ShowHelpButtonAsCommandButtonProperty = DependencyProperty.Register(nameof(ShowHelpButtonAsCommandButton), typeof(bool), typeof(DialogWindow));

        public bool ShowHelpButtonAsCommandButton { get => (bool)GetValue(ShowHelpButtonAsCommandButtonProperty); set => SetValue(ShowHelpButtonAsCommandButtonProperty, value); }

        protected override bool CanExecuteCommand => DialogButton != Windows.
#if !WinCopies3
            Dialogs.
#endif
         DialogButton.OK;



        static DialogWindow() => DefaultStyleKeyProperty.OverrideMetadata(typeof(DialogWindow), new FrameworkPropertyMetadata(typeof(DialogWindow)));



        public DialogWindow() : this(System.Reflection.Assembly.GetEntryAssembly().GetName().Name) { }

        // InitializeComponent();

        // ActionCommand = new WinCopies.Util.DelegateCommand(ActionCommandMethod);

        public DialogWindow(string title)
        {
            Title = title;

            OnDialogButtonChanged(null, DialogButton);
        }

        //public override void OnApplyTemplate()
        //{

        //    base.OnApplyTemplate();

        //    //switch (DialogButton)

        //    //{

        //    //    case DialogButton.OK:
        //    //        var b = (Template.FindName(PART_OkButton, this) as Button);
        //    //        _ = b.CommandBindings.Add(new CommandBinding(Commands.CommonCommand, Command_Executed, Command_CanExecute));
        //    //        MessageBox.Show(b.Content.ToString());
        //    //        break;

        //    //    case DialogButton.OKCancel:

        //    //        _ = (GetTemplateChild(PART_OkButton) as Button)?.CommandBindings.Add(new CommandBinding(Commands.CommonCommand, Command_Executed, Command_CanExecute));

        //    //        _ = (GetTemplateChild(PART_CancelButton) as Button)?.CommandBindings.Add(new CommandBinding(Commands.CommonCommand, Command_Executed, Command_CanExecute));

        //    //        break;

        //    //    case DialogButton.OKApplyCancel:

        //    //        _ = (GetTemplateChild(PART_OkButton) as Button).CommandBindings.Add(new CommandBinding(Commands.CommonCommand, Command_Executed, Command_CanExecute));

        //    //        _ = (GetTemplateChild(PART_ApplyButton) as Button)?.CommandBindings.Add(new CommandBinding(Commands.CommonCommand, Command_Executed, Command_CanExecute));

        //    //        _ = (GetTemplateChild(PART_CancelButton) as Button)?.CommandBindings.Add(new CommandBinding(Commands.CommonCommand, Command_Executed, Command_CanExecute));

        //    //        break;

        //    //    case DialogButton.YesNo:

        //    //        _ = (GetTemplateChild(PART_YesButton) as Button)?.CommandBindings.Add(new CommandBinding(Commands.CommonCommand, Command_Executed, Command_CanExecute));

        //    //        _ = (GetTemplateChild(PART_NoButton) as Button)?.CommandBindings.Add(new CommandBinding(Commands.CommonCommand, Command_Executed, Command_CanExecute));

        //    //        break;

        //    //    case DialogButton.YesNoCancel:

        //    //        _ = (GetTemplateChild(PART_YesButton) as Button)?.CommandBindings.Add(new CommandBinding(Commands.CommonCommand, Command_Executed, Command_CanExecute));

        //    //        _ = (GetTemplateChild(PART_NoButton) as Button)?.CommandBindings.Add(new CommandBinding(Commands.CommonCommand, Command_Executed, Command_CanExecute));

        //    //        _ = (GetTemplateChild(PART_CancelButton) as Button)?.CommandBindings.Add(new CommandBinding(Commands.CommonCommand, Command_Executed, Command_CanExecute));

        //    //        break;

        //    //}

        //}

        protected virtual void OnDialogButtonChanged(DialogButton? oldValue, DialogButton? newValue)
        {
            // MessageBox.Show(e.NewValue.ToString());

            if (DefaultButton != DefaultButton.None) throw new InvalidOperationException($"{nameof(DefaultButton)} must be set to {nameof(DefaultButton.None)} in order to perform this action.");

            if (!(CustomButtonsSource is null)) throw new InvalidOperationException("CustomButtonsSource is not null.");

            CommandBindings.Clear();

            if (!(newValue is null))
            {
                void addCommandBinding(in ICommand command, in ExecutedRoutedEventHandler executed, in CanExecuteRoutedEventHandler canExecute) => CommandBindings.Add(new CommandBinding(command, executed, canExecute));

                switch (newValue)
                {
                    case Windows.
#if !WinCopies3
            Dialogs.
#endif
          DialogButton.OK:

                        addCommandBinding(Ok, Command_Executed, Command_CanExecute);

                        break;

                    case Windows.
#if !WinCopies3
            Dialogs.
#endif
          DialogButton.OKCancel:

                        addCommandBinding(Ok, Command_Executed, Command_CanExecute);

                        addCommandBinding(Cancel, Command_Executed, Command_CanExecute);

                        break;

                    case Windows.
#if !WinCopies3
            Dialogs.
#endif
          DialogButton.AbortRetryIgnore:

                        addCommandBinding(Abort, Command_Executed, Command_CanExecute);

                        addCommandBinding(Retry, Command_Executed, Command_CanExecute);

                        addCommandBinding(Ignore, Command_Executed, Command_CanExecute);

                        break;

                    case Windows.
#if !WinCopies3
            Dialogs.
#endif
          DialogButton.YesNoCancel:

                        addCommandBinding(Yes, Command_Executed, Command_CanExecute);

                        addCommandBinding(No, Command_Executed, Command_CanExecute);

                        addCommandBinding(Cancel, Command_Executed, Command_CanExecute);

                        break;

                    case Windows.
#if !WinCopies3
            Dialogs.
#endif
          DialogButton.YesNo:

                        addCommandBinding(Yes, Command_Executed, Command_CanExecute);

                        addCommandBinding(No, Command_Executed, Command_CanExecute);

                        break;

                    case Windows.
#if !WinCopies3
            Dialogs.
#endif
          DialogButton.RetryCancel:

                        addCommandBinding(Retry, Command_Executed, Command_CanExecute);

                        addCommandBinding(Cancel, Command_Executed, Command_CanExecute);

                        break;

                    case Windows.
#if !WinCopies3
            Dialogs.
#endif
          DialogButton.CancelTryContinue:

                        addCommandBinding(Cancel, Command_Executed, Command_CanExecute);

                        addCommandBinding(Retry, Command_Executed, Command_CanExecute);

                        addCommandBinding(Continue, Command_Executed, Command_CanExecute);

                        break;

                    case Windows.
#if !WinCopies3
            Dialogs.
#endif
          DialogButton.ContinueIgnoreCancel:

                        addCommandBinding(Continue, Command_Executed, Command_CanExecute);

                        addCommandBinding(Ignore, Command_Executed, Command_CanExecute);

                        addCommandBinding(Cancel, Command_Executed, Command_CanExecute);

                        break;

                    case Windows.
#if !WinCopies3
            Dialogs.
#endif
          DialogButton.OKApplyCancel:

                        addCommandBinding(Ok, Command_Executed, Command_CanExecute);

                        addCommandBinding(Apply, Command_Executed, Command_CanExecute);

                        addCommandBinding(Cancel, Command_Executed, Command_CanExecute);

                        break;

                    case Windows.
#if !WinCopies3
            Dialogs.
#endif
          DialogButton.RetryIgnoreCancel:

                        addCommandBinding(Retry, Command_Executed, Command_CanExecute);

                        addCommandBinding(Ignore, Command_Executed, Command_CanExecute);

                        addCommandBinding(Cancel, Command_Executed, Command_CanExecute);

                        break;

                    case Windows.
#if !WinCopies3
            Dialogs.
#endif
          DialogButton.IgnoreCancel:

                        addCommandBinding(Ignore, Command_Executed, Command_CanExecute);

                        addCommandBinding(Cancel, Command_Executed, Command_CanExecute);

                        break;

                    case Windows.
#if !WinCopies3
            Dialogs.
#endif
          DialogButton.YesToAllNoToAllCancel:

                        addCommandBinding(YesToAll, Command_Executed, Command_CanExecute);

                        addCommandBinding(NoToAll, Command_Executed, Command_CanExecute);

                        addCommandBinding(Cancel, Command_Executed, Command_CanExecute);

                        break;

                    case Windows.
#if !WinCopies3
            Dialogs.
#endif
          DialogButton.YesToAllNoToAll:

                        addCommandBinding(YesToAll, Command_Executed, Command_CanExecute);

                        addCommandBinding(NoToAll, Command_Executed, Command_CanExecute);

                        break;
                }
            }

            // ((DialogWindow)d).DefaultButton = DefaultButton.None;
        }

        protected virtual void OnDefaultButtonChanged(DefaultButton oldValue, DefaultButton newValue)
        {
            if (newValue == DefaultButton.None) return;

#if !NETFRAMEWORK

            static

#endif

                void throwArgumentException() => throw new ArgumentException("DefaultButton must be included in DialogButton value.");

            switch (DialogButton)
            {
                case Windows.
#if !WinCopies3
            Dialogs.
#endif
          DialogButton.OK:

                    if (newValue != DefaultButton.OK) throwArgumentException();

                    break;

                case Windows.
#if !WinCopies3
            Dialogs.
#endif
          DialogButton.OKCancel:

                    if (If(And, Logical, NotEqual, newValue, DefaultButton.OK, DefaultButton.Cancel)) throwArgumentException();

                    break;

                case Windows.
#if !WinCopies3
            Dialogs.
#endif
          DialogButton.AbortRetryIgnore:

                    if (If(And, Binary, Equal, newValue, DefaultButton.Abort, DefaultButton.Retry, DefaultButton.Ignore)) throwArgumentException();

                    break;

                case Windows.
#if !WinCopies3
            Dialogs.
#endif
          DialogButton.YesNoCancel:

                    if (If(And, Logical, NotEqual, newValue, DefaultButton.Yes, DefaultButton.No, DefaultButton.Cancel)) throwArgumentException();

                    break;

                case Windows.
#if !WinCopies3
            Dialogs.
#endif
          DialogButton.YesNo:

                    if (If(And, Logical, NotEqual, newValue, DefaultButton.Yes, DefaultButton.No)) throwArgumentException();

                    break;

                case Windows.
#if !WinCopies3
            Dialogs.
#endif
          DialogButton.RetryCancel:

                    if (If(And, Logical, NotEqual, newValue, DefaultButton.Retry, DefaultButton.Cancel)) throwArgumentException();

                    break;

                case Windows.
#if !WinCopies3
            Dialogs.
#endif
          DialogButton.CancelTryContinue:

                    if (If(And, Logical, NotEqual, newValue, DefaultButton.Cancel, DefaultButton.Retry, DefaultButton.Continue)) throwArgumentException();

                    break;

                case Windows.
#if !WinCopies3
            Dialogs.
#endif
          DialogButton.ContinueIgnoreCancel:

                    if (If(And, Logical, NotEqual, newValue, DefaultButton.Continue, DefaultButton.Ignore, DefaultButton.Cancel)) throwArgumentException();

                    break;

                case Windows.
#if !WinCopies3
            Dialogs.
#endif
          DialogButton.OKApplyCancel:

                    if (If(And, Logical, NotEqual, newValue, DefaultButton.OK, DefaultButton.Apply, DefaultButton.Cancel)) throwArgumentException();

                    break;

                case Windows.
#if !WinCopies3
            Dialogs.
#endif
          DialogButton.RetryIgnoreCancel:

                    if (If(And, Logical, NotEqual, newValue, DefaultButton.Retry, DefaultButton.Ignore, DefaultButton.Cancel)) throwArgumentException();

                    break;

                case Windows.
#if !WinCopies3
            Dialogs.
#endif
          DialogButton.IgnoreCancel:

                    if (If(And, Logical, NotEqual, newValue, DefaultButton.Ignore, DefaultButton.Cancel)) throwArgumentException();

                    break;

                case Windows.
#if !WinCopies3
            Dialogs.
#endif
          DialogButton.YesToAllNoToAllCancel:

                    if (If(And, Logical, NotEqual, newValue, DefaultButton.YesToAll, DefaultButton.NoToAll, DefaultButton.Cancel)) throwArgumentException();

                    break;

                case Windows.
#if !WinCopies3
            Dialogs.
#endif
          DialogButton.YesToAllNoToAll:

                    if (If(And, Logical, NotEqual, newValue, DefaultButton.YesToAll, DefaultButton.NoToAll)) throwArgumentException();

                    break;
            }
        }

        protected virtual void OnCustomButtonsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            if (!(DialogButton is null)) throw new InvalidOperationException("DialogButton is not null.");
        }

        private void Command_CanExecute(object sender, CanExecuteRoutedEventArgs e) => OnCommandCanExecute(e);

        protected virtual void OnCommandCanExecute(CanExecuteRoutedEventArgs e)
        {
            if (e.Command == Ok)

                e.CanExecute = DialogButton == Windows.
#if !WinCopies3
            Dialogs.
#endif
          DialogButton.OK || Command == null || Command.CanExecute(CommandParameter, CommandTarget);

            else if (e.Command == Apply)

                e.CanExecute = Command?.CanExecute(CommandParameter, CommandTarget) == true;

            else if (If(Or, Logical, Equal, e.Command, Yes, Retry, Continue))

                e.CanExecute = Command == null || Command.CanExecute(CommandParameter, CommandTarget);

            else if (If(Or, Logical, Equal, e.Command, No, YesToAll, NoToAll, Cancel, Abort, Ignore))

                e.CanExecute = true;
        }

        private void Command_Executed(object sender, ExecutedRoutedEventArgs e) => OnCommandExecuted(e);
    }
}
