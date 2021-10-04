using System;
using System.Windows;
using System.Windows.Input;

namespace WinCopies.GUI.Shell
{
    public class FileSystemDialogBox : Windows.DialogWindowBase
    {
        protected override bool CanExecuteCommand => true;

        public static readonly DependencyProperty PathProperty = DependencyProperty.Register(nameof(Path), typeof(IFileSystemDialog), typeof(FileSystemDialogBox), new PropertyMetadata(null, (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
        {
            if (e.NewValue == null)

                throw new InvalidOperationException($"Cannot set {nameof(Path)} to null.");
        }));

        public IFileSystemDialog Path { get => (IFileSystemDialog)GetValue(PathProperty); set => SetValue(PathProperty, value); }

        public static readonly DependencyProperty DisposePathOnCloseProperty = DependencyProperty.Register(nameof(DisposePathOnClose), typeof(bool), typeof(FileSystemDialogBox));

        public bool DisposePathOnClose { get => (bool)GetValue(DisposePathOnCloseProperty); set => SetValue(DisposePathOnCloseProperty, value); }

        static FileSystemDialogBox() => DefaultStyleKeyProperty.OverrideMetadata(typeof(FileSystemDialogBox), new FrameworkPropertyMetadata(typeof(FileSystemDialogBox)));

        private FileSystemDialogBox(in bool disposePathOnClose)
        {
            DisposePathOnClose = disposePathOnClose;

            _ = CommandBindings.Add(new CommandBinding(ApplicationCommands.Close, (object sender, ExecutedRoutedEventArgs e) =>
            {
                if ((bool)e.Parameter)

                    CloseWindowWithDialogResult(true, Windows.MessageBoxResult.OK);

                else

                CloseWindowWithDialogResult(false, Windows.MessageBoxResult.Cancel);
            }, (object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = true));
        }

        public FileSystemDialogBox(in FileSystemDialogBoxMode mode, in bool disposePathOnClose) : this(disposePathOnClose) => Path = new FileSystemDialogBoxViewModel(mode);

        public FileSystemDialogBox(in IFileSystemDialog path, in bool disposePathOnClose) : this(disposePathOnClose) => Path = path ?? throw ThrowHelper.GetArgumentNullException(nameof(path));

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            if (DisposePathOnClose)

                Path?.Path?.Dispose();

            else

                Path?.Path?.StopMonitoring();
        }
    }
}
