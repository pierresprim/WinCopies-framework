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
using System.Windows;
using System.Windows.Input;
using WinCopies.Util;
using WinCopies.Util.Commands;
using static WinCopies.Util.Util;
using IfCT = WinCopies.Util.Util.ComparisonType;
using IfCM = WinCopies.Util.Util.ComparisonMode;
using IfComp = WinCopies.Util.Util.Comparison;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Collections;

namespace WinCopies.GUI.Windows.Dialogs
{
    /// <summary>
    /// Represents a common dialog window for WPF.
    /// </summary>
    public partial class DialogWindow : Window, ICommandSource
    {

        /// <summary>
        /// Identifies the <see cref="DialogButton"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DialogButtonProperty = DependencyProperty.Register(nameof(DialogButton), typeof(DialogButton?), typeof(DialogWindow), new PropertyMetadata(WinCopies.GUI.Windows.Dialogs.DialogButton.OK, (DependencyObject sender, DependencyPropertyChangedEventArgs e) => ((DialogWindow)sender).OnDialogButtonChanged((DialogButton?)e.OldValue, (DialogButton?)e.NewValue)
        ));

        /// <summary>
        /// Gets or sets the <see cref="Dialogs.DialogButton"/>. <see cref="Command"/> will not be executed if this property is set to <see cref="DialogButton.OK"/>, or if the user clicks on one of the following buttons: Cancel, Ignore, Abort. This is a dependency property.
        /// </summary>
        public DialogButton? DialogButton { get => (DialogButton?)GetValue(DialogButtonProperty); set => SetValue(DialogButtonProperty, value); }

        /// <summary>
        /// Identifies the <see cref="ButtonAlignment"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ButtonAlignmentProperty = DependencyProperty.Register(nameof(ButtonAlignment), typeof(HorizontalAlignment), typeof(DialogWindow), new PropertyMetadata(Dialogs.HorizontalAlignment.Left));

        /// <summary>
        /// Gets or sets the <see cref="HorizontalAlignment"/> for the button alignment. This is a dependency property.
        /// </summary>
        public HorizontalAlignment ButtonAlignment { get => (HorizontalAlignment)GetValue(ButtonAlignmentProperty); set => SetValue(ButtonAlignmentProperty, value); }

        /// <summary>
        /// Indentifies the <see cref="DefaultButton"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DefaultButtonProperty = DependencyProperty.Register(nameof(DefaultButton), typeof(DefaultButton), typeof(DialogWindow), new PropertyMetadata(DefaultButton.None, (DependencyObject d, DependencyPropertyChangedEventArgs e) => ((DialogWindow)d).OnDefaultButtonChanged((DefaultButton)e.OldValue, (DefaultButton)e.NewValue)));

        public DefaultButton DefaultButton { get => (DefaultButton)GetValue(DefaultButtonProperty); set => SetValue(DefaultButtonProperty, value); }

        /// <summary>
        /// Indentifies the <see cref="CustomButtonsSource"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CustomButtonsSourceProperty = DependencyProperty.Register(nameof(CustomButtonsSource), typeof(IEnumerable), typeof(DialogWindow), new PropertyMetadata(null, (DependencyObject d, DependencyPropertyChangedEventArgs e) => ((DialogWindow)d).OnCustomButtonsSourceChanged((IEnumerable)e.OldValue, (IEnumerable)e.NewValue)));

        public IEnumerable CustomButtonsSource { get => (IEnumerable)GetValue(CustomButtonsSourceProperty); set => SetValue(CustomButtonsSourceProperty, value); }

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
        /// Indentifies the <see cref="CustomButtonTemplate"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CustomButtonTemplateProperty = DependencyProperty.Register(nameof(CustomButtonTemplate), typeof(DataTemplate), typeof(DialogWindow));

        public DataTemplate CustomButtonTemplate { get => (DataTemplate)GetValue(CustomButtonTemplateProperty); set => SetValue(CustomButtonTemplateProperty, value); }

        /// <summary>
        /// Indentifies the <see cref="CustomButtonTemplateSelector"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CustomButtonTemplateSelectorProperty = DependencyProperty.Register(nameof(CustomButtonTemplateSelector), typeof(DataTemplateSelector), typeof(DialogWindow));

        public DataTemplateSelector CustomButtonTemplateSelector { get => (DataTemplateSelector)GetValue(CustomButtonTemplateSelectorProperty); set => SetValue(CustomButtonTemplateSelectorProperty, value); }

        /// <summary>
        /// Indentifies the <see cref="ShowHelpButtonAsCommandButton"/> dependency property.
        /// </summary>
        public readonly static DependencyProperty ShowHelpButtonAsCommandButtonProperty = DependencyProperty.Register(nameof(ShowHelpButtonAsCommandButton), typeof(bool), typeof(DialogWindow));

        public bool ShowHelpButtonAsCommandButton { get => (bool)GetValue(ShowHelpButtonAsCommandButtonProperty); set => SetValue(ShowHelpButtonAsCommandButtonProperty, value); }

        /// <summary>
        /// Gets the <see cref="WinCopies.GUI.Windows.Dialogs.MessageBoxResult"/>.
        /// </summary>
        public MessageBoxResult MessageBoxResult { get; private set; } = MessageBoxResult.None;

        /// <summary>
        /// Identifies the <see cref="Command"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(nameof(Command), typeof(ICommand), typeof(DialogWindow), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the command that will be executed when the command source is invoked.
        /// </summary>
        public ICommand Command { get => (ICommand)GetValue(CommandProperty); set => SetValue(CommandProperty, value); }

        /// <summary>
        /// Identifies the <see cref="CommandParameter"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register(nameof(CommandParameter), typeof(object), typeof(DialogWindow), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets a value that represents a user defined data value that can be passed to the command when it is executed.
        /// </summary>
        public object CommandParameter { get => GetValue(CommandParameterProperty); set => SetValue(CommandParameterProperty, value); }

        /// <summary>
        /// Identifies the <see cref="CommandTarget"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CommandTargetProperty = DependencyProperty.Register(nameof(CommandTarget), typeof(IInputElement), typeof(DialogWindow), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets a value that represents the object that the command is being executed on.
        /// </summary>
        public IInputElement CommandTarget { get => (IInputElement)GetValue(CommandTargetProperty); set => SetValue(CommandTargetProperty, value); }



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

                switch (newValue)

                {

                    case Dialogs.DialogButton.OK:

                        _ = CommandBindings.Add(new CommandBinding(DialogCommands.Ok, Command_Executed, Command_CanExecute));

                        break;

                    case Dialogs.DialogButton.OKCancel:

                        _ = CommandBindings.Add(new CommandBinding(DialogCommands.Ok, Command_Executed, Command_CanExecute));

                        _ = CommandBindings.Add(new CommandBinding(DialogCommands.Cancel, Command_Executed, Command_CanExecute));

                        break;

                    case Dialogs.DialogButton.AbortRetryIgnore:

                        _ = CommandBindings.Add(new CommandBinding(DialogCommands.Abort, Command_Executed, Command_CanExecute));

                        _ = CommandBindings.Add(new CommandBinding(DialogCommands.Retry, Command_Executed, Command_CanExecute));

                        _ = CommandBindings.Add(new CommandBinding(DialogCommands.Ignore, Command_Executed, Command_CanExecute));

                        break;

                    case Dialogs.DialogButton.YesNoCancel:

                        _ = CommandBindings.Add(new CommandBinding(DialogCommands.Yes, Command_Executed, Command_CanExecute));

                        _ = CommandBindings.Add(new CommandBinding(DialogCommands.No, Command_Executed, Command_CanExecute));

                        _ = CommandBindings.Add(new CommandBinding(DialogCommands.Cancel, Command_Executed, Command_CanExecute));

                        break;

                    case Dialogs.DialogButton.YesNo:

                        _ = CommandBindings.Add(new CommandBinding(DialogCommands.Yes, Command_Executed, Command_CanExecute));

                        _ = CommandBindings.Add(new CommandBinding(DialogCommands.No, Command_Executed, Command_CanExecute));

                        break;

                    case Dialogs.DialogButton.RetryCancel:

                        _ = CommandBindings.Add(new CommandBinding(DialogCommands.Retry, Command_Executed, Command_CanExecute));

                        _ = CommandBindings.Add(new CommandBinding(DialogCommands.Cancel, Command_Executed, Command_CanExecute));

                        break;

                    case Dialogs.DialogButton.CancelTryContinue:

                        _ = CommandBindings.Add(new CommandBinding(DialogCommands.Cancel, Command_Executed, Command_CanExecute));

                        _ = CommandBindings.Add(new CommandBinding(DialogCommands.Retry, Command_Executed, Command_CanExecute));

                        _ = CommandBindings.Add(new CommandBinding(DialogCommands.Continue, Command_Executed, Command_CanExecute));

                        break;

                    case Dialogs.DialogButton.ContinueIgnoreCancel:

                        _ = CommandBindings.Add(new CommandBinding(DialogCommands.Continue, Command_Executed, Command_CanExecute));

                        _ = CommandBindings.Add(new CommandBinding(DialogCommands.Ignore, Command_Executed, Command_CanExecute));

                        _ = CommandBindings.Add(new CommandBinding(DialogCommands.Cancel, Command_Executed, Command_CanExecute));

                        break;

                    case Dialogs.DialogButton.OKApplyCancel:

                        _ = CommandBindings.Add(new CommandBinding(DialogCommands.Ok, Command_Executed, Command_CanExecute));

                        _ = CommandBindings.Add(new CommandBinding(DialogCommands.Apply, Command_Executed, Command_CanExecute));

                        _ = CommandBindings.Add(new CommandBinding(DialogCommands.Cancel, Command_Executed, Command_CanExecute));

                        break;

                    case Dialogs.DialogButton.RetryIgnoreCancel:

                        _ = CommandBindings.Add(new CommandBinding(DialogCommands.Retry, Command_Executed, Command_CanExecute));

                        _ = CommandBindings.Add(new CommandBinding(DialogCommands.Ignore, Command_Executed, Command_CanExecute));

                        _ = CommandBindings.Add(new CommandBinding(DialogCommands.Cancel, Command_Executed, Command_CanExecute));

                        break;

                    case Dialogs.DialogButton.IgnoreCancel:

                        _ = CommandBindings.Add(new CommandBinding(DialogCommands.Ignore, Command_Executed, Command_CanExecute));

                        _ = CommandBindings.Add(new CommandBinding(DialogCommands.Cancel, Command_Executed, Command_CanExecute));

                        break;

                    case Dialogs.DialogButton.YesToAllNoToAllCancel:

                        _ = CommandBindings.Add(new CommandBinding(DialogCommands.YesToAll, Command_Executed, Command_CanExecute));

                        _ = CommandBindings.Add(new CommandBinding(DialogCommands.NoToAll, Command_Executed, Command_CanExecute));

                        _ = CommandBindings.Add(new CommandBinding(DialogCommands.Cancel, Command_Executed, Command_CanExecute));

                        break;

                    case Dialogs.DialogButton.YesToAllNoToAll:

                        _ = CommandBindings.Add(new CommandBinding(DialogCommands.YesToAll, Command_Executed, Command_CanExecute));

                        _ = CommandBindings.Add(new CommandBinding(DialogCommands.NoToAll, Command_Executed, Command_CanExecute));

                        break;

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

                case Dialogs.DialogButton.OK:

                    if (newValue != DefaultButton.OK) throwArgumentException();

                    break;

                case Dialogs.DialogButton.OKCancel:

                    if (If(IfCT.And, IfCM.Logical, IfComp.NotEqual, newValue, DefaultButton.OK, DefaultButton.Cancel)) throwArgumentException();

                    break;

                case Dialogs.DialogButton.AbortRetryIgnore:

                    if (If(IfCT.And, IfCM.Binary, IfComp.Equal, newValue, DefaultButton.Abort, DefaultButton.Retry, DefaultButton.Ignore)) throwArgumentException();

                    break;

                case Dialogs.DialogButton.YesNoCancel:

                    if (If(IfCT.And, IfCM.Logical, IfComp.NotEqual, newValue, DefaultButton.Yes, DefaultButton.No, DefaultButton.Cancel)) throwArgumentException();

                    break;

                case Dialogs.DialogButton.YesNo:

                    if (If(IfCT.And, IfCM.Logical, IfComp.NotEqual, newValue, DefaultButton.Yes, DefaultButton.No)) throwArgumentException();

                    break;

                case Dialogs.DialogButton.RetryCancel:

                    if (If(IfCT.And, IfCM.Logical, IfComp.NotEqual, newValue, DefaultButton.Retry, DefaultButton.Cancel)) throwArgumentException();

                    break;

                case Dialogs.DialogButton.CancelTryContinue:

                    if (If(IfCT.And, IfCM.Logical, IfComp.NotEqual, newValue, DefaultButton.Cancel, DefaultButton.Retry, DefaultButton.Continue)) throwArgumentException();

                    break;

                case Dialogs.DialogButton.ContinueIgnoreCancel:

                    if (If(IfCT.And, IfCM.Logical, IfComp.NotEqual, newValue, DefaultButton.Continue, DefaultButton.Ignore, DefaultButton.Cancel)) throwArgumentException();

                    break;

                case Dialogs.DialogButton.OKApplyCancel:

                    if (If(IfCT.And, IfCM.Logical, IfComp.NotEqual, newValue, DefaultButton.OK, DefaultButton.Apply, DefaultButton.Cancel)) throwArgumentException();

                    break;

                case Dialogs.DialogButton.RetryIgnoreCancel:

                    if (If(IfCT.And, IfCM.Logical, IfComp.NotEqual, newValue, DefaultButton.Retry, DefaultButton.Ignore, DefaultButton.Cancel)) throwArgumentException();

                    break;

                case Dialogs.DialogButton.IgnoreCancel:

                    if (If(IfCT.And, IfCM.Logical, IfComp.NotEqual, newValue, DefaultButton.Ignore, DefaultButton.Cancel)) throwArgumentException();

                    break;

                case Dialogs.DialogButton.YesToAllNoToAllCancel:

                    if (If(IfCT.And, IfCM.Logical, IfComp.NotEqual, newValue, DefaultButton.YesToAll, DefaultButton.NoToAll, DefaultButton.Cancel)) throwArgumentException();

                    break;

                case Dialogs.DialogButton.YesToAllNoToAll:

                    if (If(IfCT.And, IfCM.Logical, IfComp.NotEqual, newValue, DefaultButton.YesToAll, DefaultButton.NoToAll)) throwArgumentException();

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

            if (e.Command == DialogCommands.Ok)

                e.CanExecute = DialogButton == Dialogs.DialogButton.OK || Command == null || Command.CanExecute(CommandParameter, CommandTarget);

            else if (e.Command == DialogCommands.Apply)

                e.CanExecute = Command?.CanExecute(CommandParameter, CommandTarget) == true;

            else if (If(IfCT.Or, IfCM.Logical, IfComp.Equal, e.Command, DialogCommands.Yes, DialogCommands.Retry, DialogCommands.Continue))

                e.CanExecute = Command == null || Command.CanExecute(CommandParameter, CommandTarget);

            else if (If(IfCT.Or, IfCM.Logical, IfComp.Equal, e.Command, DialogCommands.No, DialogCommands.YesToAll, DialogCommands.NoToAll, DialogCommands.Cancel, DialogCommands.Abort, DialogCommands.Ignore))

                e.CanExecute = true;

        }

        private void Command_Executed(object sender, ExecutedRoutedEventArgs e) => OnCommandExecuted(e);

        protected virtual void OnCommandExecuted(ExecutedRoutedEventArgs e)

        {

            if (e.Command == DialogCommands.Ok)

                CloseWindowWithDialogResult(true, MessageBoxResult.OK);

            else if (e.Command == DialogCommands.Apply)

                Command.Execute(CommandParameter, CommandTarget);

            else if (e.Command == DialogCommands.Yes)

                CloseWindowWithDialogResult(true, MessageBoxResult.Yes);

            else if (e.Command == DialogCommands.No)

                CloseWindowWithDialogResult(false, MessageBoxResult.No);

            else if (e.Command == DialogCommands.YesToAll)

                CloseWindowWithDialogResult(true, MessageBoxResult.YesToAll);

            else if (e.Command == DialogCommands.NoToAll)

                CloseWindowWithDialogResult(false, MessageBoxResult.NoToAll);

            else if (e.Command == DialogCommands.Cancel)

                CloseWindowWithDialogResult(false, MessageBoxResult.Cancel);

            else if (e.Command == DialogCommands.Abort)

                CloseWindowWithDialogResult(false, MessageBoxResult.Abort);

            else if (e.Command == DialogCommands.Ignore)

                CloseWindowWithDialogResult(false, MessageBoxResult.Ignore);

            else if (e.Command == DialogCommands.Retry)

                CloseWindowWithDialogResult(true, MessageBoxResult.Retry);

            else if (e.Command == DialogCommands.Continue)

                CloseWindowWithDialogResult(true, MessageBoxResult.Continue);

        }

        protected void CloseWindowWithDialogResult(bool dialogResult, MessageBoxResult messageBoxResult)

        {

            try

            {

                DialogResult = dialogResult;

            }
            catch (InvalidOperationException ex) when (ex.HResult == -2146233079) { }

            MessageBoxResult = messageBoxResult;

            if (dialogResult && Command != null && DialogButton != Dialogs.DialogButton.OK)

                Command.Execute(CommandParameter, CommandTarget);

            Close();

        }

    }
}
