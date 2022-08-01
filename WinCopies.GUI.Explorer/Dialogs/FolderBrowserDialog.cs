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
using System.Windows.Input;
using WinCopies.GUI.Explorer;
using WinCopies.IO;
using WinCopies.Util;
using static WinCopies.Util.Util;

namespace WinCopies.GUI.Windows.Dialogs
{

    public enum FolderBrowserDialogMode
    {

        OpenFiles, OpenFolder, Save

    }

    public struct FolderBrowserDialogFilter
    {

        public string FilterName { get; set; }

        public string Filter { get; set; }

        public override string ToString() => string.Format("{0} ({1})", FilterName, Filter);

        public override bool Equals(object obj) => obj is FolderBrowserDialogFilter _obj ? _obj.Filter == Filter : false;

    }

    public class FolderBrowserDialog : Window, ICommandSource
    {

        // todo: to replace with data binding

        public ExplorerControl ExplorerControl { get; private set; }

        /// <summary>
        /// Identifies the <see cref="Text"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(nameof(Text), typeof(string), typeof(FolderBrowserDialog));

        public string Text { get => (string)GetValue(TextProperty); set => SetValue(TextProperty, value); }

        /// <summary>
        /// Identifies the <see cref="BottomContent"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty BottomContentProperty = DependencyProperty.Register(nameof(BottomContent), typeof(object), typeof(FolderBrowserDialog));

        public object BottomContent { get => GetValue(BottomContentProperty); set => SetValue(BottomContentProperty, value); }

        /// <summary>
        /// Identifies the <see cref="Mode"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ModeProperty = DependencyProperty.Register(nameof(Mode), typeof(FolderBrowserDialogMode), typeof(FolderBrowserDialog));

        public FolderBrowserDialogMode Mode { get => (FolderBrowserDialogMode)GetValue(ModeProperty); set => SetValue(ModeProperty, value); }

        /// <summary>
        /// Identifies the <see cref="Command"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(nameof(Command), typeof(ICommand), typeof(FolderBrowserDialog), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the command that will be executed when the command source is invoked.
        /// </summary>
        public ICommand Command { get => (ICommand)GetValue(CommandProperty); set => SetValue(CommandProperty, value); }

        /// <summary>
        /// Identifies the <see cref="CommandParameter"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register(nameof(CommandParameter), typeof(object), typeof(FolderBrowserDialog), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets a value that represents a user defined data value that can be passed to the command when it is executed.
        /// </summary>
        public object CommandParameter { get => GetValue(CommandParameterProperty); set => SetValue(CommandParameterProperty, value); }

        /// <summary>
        /// Identifies the <see cref="CommandTarget"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CommandTargetProperty = DependencyProperty.Register(nameof(CommandTarget), typeof(IInputElement), typeof(FolderBrowserDialog), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets a value that represents the object that the command is being executed on.
        /// </summary>
        public IInputElement CommandTarget { get => (IInputElement)GetValue(CommandTargetProperty); set => SetValue(CommandTargetProperty, value); }



        static FolderBrowserDialog() => DefaultStyleKeyProperty.OverrideMetadata(typeof(FolderBrowserDialog), new FrameworkPropertyMetadata(typeof(FolderBrowserDialog)));



        public FolderBrowserDialog() => Init(FolderBrowserDialogMode.OpenFiles, /*SelectionMode.Single,*/ System.Reflection.Assembly.GetEntryAssembly().GetName().Name);

        public FolderBrowserDialog(FolderBrowserDialogMode mode, SelectionMode selectionMode) => Init(mode, /*selectionMode,*/ mode == FolderBrowserDialogMode.OpenFiles ? selectionMode != SelectionMode.Single ? Explorer.Themes.Generic.OpenFiles : Explorer.Themes.Generic.OpenFile : mode == FolderBrowserDialogMode.OpenFolder ? Explorer.Themes.Generic.OpenFolder : mode == FolderBrowserDialogMode.Save ? Explorer.Themes.Generic.Save : System.Reflection.Assembly.GetEntryAssembly().GetName().Name);

        public FolderBrowserDialog(string title) => Init(FolderBrowserDialogMode.OpenFiles, /*SelectionMode.Single,*/ title);

        public FolderBrowserDialog(FolderBrowserDialogMode mode, /*SelectionMode selectionMode,*/ string title) => Init(mode, /*selectionMode,*/ title);

        private void Init(FolderBrowserDialogMode mode, /*SelectionMode selectionMode,*/ string title)

        {

            Mode = mode;

            Title = title;

            //CommandBindings.Add(new CommandBinding(CommonCommand, OnCommandExecuted, OnCommandCanExecute));

        }

        public override void OnApplyTemplate()
        {

            base.OnApplyTemplate();

            ExplorerControl = (ExplorerControl)Template.FindName($"PART_{nameof(ExplorerControl)}", this);

        }



        //protected void OnCommandCanExecute(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = e.CanExecute = e.Parameter is bool result ? !result || Command == null ? true : Command.CanExecute(CommandParameter) : true;

        //protected void OnCommandExecuted(object sender, ExecutedRoutedEventArgs e)

        //{

        //    if (e.Parameter is bool result)

        //    {

        //        Explorer.Explorer.IBrowsableObjectInfo selectedItem = ExplorerControl.Path.SelectedItem;

        //        if (result && selectedItem != null && (selectedItem.FileType == FileType.Folder || (selectedItem.FileType == FileType.SpecialFolder && selectedItem is Explorer.ShellObjectInfo so && so.ShellObject.IsFileSystemObject) || selectedItem.FileType == FileType.Drive))

        //        {

        //            ExplorerControl.Open(selectedItem);

        //            return;

        //        }

        //        CloseWindowWithDialogResult(result);

        //    }

        //}

        protected void CloseWindowWithDialogResult(bool dialogResult)

        {

            DialogResult = dialogResult;

            if (dialogResult)

                Command.Execute(CommandParameter, CommandTarget);

            Close();

        }

        //private void SelectFileCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = true;

        //private void SelectFileCommand_Executed(object sender, ExecutedRoutedEventArgs e)

        //{

        //    if ((If(ComparisonType.Or, ComparisonMode.Logical, Comparison.Equals, Mode, FolderBrowserDialogMode.OpenFiles, FolderBrowserDialogMode.Save) && If(ComparisonType.Or, ComparisonMode.Logical, Comparison.Equals, ExplorerControl.Path.SelectedItem.FileType, FileType.Folder, FileType.SpecialFolder, FileType.Link)) || Mode == FolderBrowserDialogMode.OpenFolder)

        //        ExplorerControl.Open();

        //}

    }
}
