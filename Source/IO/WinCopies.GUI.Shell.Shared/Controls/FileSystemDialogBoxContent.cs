using System;
using System.Windows;
using System.Windows.Controls;

using WinCopies.GUI.IO.ObjectModel;
using WinCopies.IO.ObjectModel;

using static WinCopies.Util.Desktop.UtilHelpers;

namespace WinCopies.GUI.Shell
{
    public enum FileSystemDialogBoxMode
    {
        SelectFolder = 0,

        OpenFile = 1,

        Save = 2
    }

    public class FileSystemDialogBoxContent : Control
    {
        private static DependencyProperty Register<T>(in string propertyName) => Register<T, FileSystemDialogBoxContent>(propertyName);

        private static DependencyProperty Register<T>(in string propertyName, in PropertyMetadata metadata) => Register<T, FileSystemDialogBoxContent>(propertyName, metadata);

        public static readonly DependencyProperty FilterProperty = Register<System.Collections.Generic.IEnumerable<string>>(nameof(Filter), new PropertyMetadata((DependencyObject d, DependencyPropertyChangedEventArgs e) =>
        {
            if (((FileSystemDialogBoxContent)d).Mode == FileSystemDialogBoxMode.SelectFolder)

                throw new InvalidOperationException($"{nameof(Mode)} must not be set to {nameof(FileSystemDialogBoxMode.SelectFolder)} to modify {nameof(Filter)}.");
        }));

        public System.Collections.Generic.IEnumerable<string> Filter { get => (System.Collections.Generic.IEnumerable<string>)GetValue(FilterProperty); set => SetValue(FilterProperty, value); }

        public static readonly DependencyProperty PredicateProperty = Register<Predicate<IBrowsableObjectInfo>>(nameof(Predicate));

        public Predicate<IBrowsableObjectInfo> Predicate { get => (Predicate<IBrowsableObjectInfo>)GetValue(PredicateProperty); set => SetValue(PredicateProperty, value); }

        public static readonly DependencyProperty PathProperty = Register<IExplorerControlViewModel>(nameof(Path));

        public IExplorerControlViewModel Path { get => (IExplorerControlViewModel)GetValue(PathProperty); set => SetValue(PathProperty, value); }

        public static readonly DependencyProperty SelectedItemsProperty = Register<System.Collections.Generic.IReadOnlyList<IBrowsableObjectInfo>>(nameof(SelectedItems));

        public System.Collections.Generic.IReadOnlyList<IBrowsableObjectInfo> SelectedItems { get => (System.Collections.Generic.IReadOnlyList<IBrowsableObjectInfo>)GetValue(SelectedItemsProperty); set => SetValue(SelectedItemsProperty, value); }

        public static readonly DependencyProperty ModeProperty = Register<FileSystemDialogBoxMode>(nameof(Mode), new PropertyMetadata((DependencyObject d, DependencyPropertyChangedEventArgs e) =>
        {
            if ((FileSystemDialogBoxMode)e.NewValue == FileSystemDialogBoxMode.SelectFolder && ((FileSystemDialogBoxContent)d).Filter != null)

                throw new InvalidOperationException($"Cannot set {nameof(Mode)} to {nameof(FileSystemDialogBoxMode.SelectFolder)} when {nameof(Filter)} has value.");
        }));

        public FileSystemDialogBoxMode Mode { get => (FileSystemDialogBoxMode)GetValue(ModeProperty); set => SetValue(ModeProperty, value); }

        static FileSystemDialogBoxContent() => DefaultStyleKeyProperty.OverrideMetadata(typeof(FileSystemDialogBoxContent), new FrameworkPropertyMetadata(typeof(FileSystemDialogBoxContent)));
    }
}
