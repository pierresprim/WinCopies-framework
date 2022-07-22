using System;
using System.Windows;
using System.Windows.Input;

using WinCopies.GUI.IO.ObjectModel;
using WinCopies.IO.ObjectModel;

namespace WinCopies.GUI.IO
{
    public class FileSystemDialogBox : Windows.DialogWindowBase
    {
        protected override bool CanExecuteCommand => true;

        public static readonly DependencyProperty DialogProperty = DependencyProperty.Register(nameof(Dialog), typeof(IFileSystemDialog), typeof(FileSystemDialogBox), new PropertyMetadata(null, (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
        {
            if (e.NewValue == null)

                throw new InvalidOperationException($"Cannot set {nameof(Dialog)} to null.");
        }));

        public IFileSystemDialog Dialog { get => (IFileSystemDialog)GetValue(DialogProperty); set => SetValue(DialogProperty, value); }

        static FileSystemDialogBox() => DefaultStyleKeyProperty.OverrideMetadata(typeof(FileSystemDialogBox), new FrameworkPropertyMetadata(typeof(FileSystemDialogBox)));

        public FileSystemDialogBox(in IFileSystemDialog dialog)
        {
            Dialog = dialog ?? throw ThrowHelper.GetArgumentNullException(nameof(dialog));

            _ = CommandBindings.Add(new CommandBinding(ApplicationCommands.Close, (object sender, ExecutedRoutedEventArgs e) =>
                                                      {
                                                          if ((bool)e.Parameter)

                                                              CloseWindowWithDialogResult(true, Windows.MessageBoxResult.OK);

                                                          else

                                                              CloseWindowWithDialogResult(false, Windows.MessageBoxResult.Cancel);
                                                      }, (object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = true));
        }

        public FileSystemDialogBox(in FileSystemDialogBoxMode mode, in IExplorerControlViewModel path) : this(new FileSystemDialog(mode, path)) { /* Left empty. */ }

        public FileSystemDialogBox(in FileSystemDialogBoxMode mode, IBrowsableObjectInfoViewModel path) : this(new FileSystemDialog(mode, path)) { /* Left empty. */ }

        public FileSystemDialogBox(in FileSystemDialogBoxMode mode, IBrowsableObjectInfo path) : this(new FileSystemDialog(mode, path)) { /* Left empty. */ }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            Dialog?.Path?.StopMonitoring();
        }
    }
}
