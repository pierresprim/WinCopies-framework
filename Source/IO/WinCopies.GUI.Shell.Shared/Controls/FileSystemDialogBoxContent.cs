using System;
using System.Windows;
using System.Windows.Controls;

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

    public class FileSystemDialogBoxContent : ContentControl
    {
        private static DependencyProperty Register<T>(in string propertyName) => Register<T, FileSystemDialogBoxContent>(propertyName);

        public static readonly DependencyProperty FilterProperty = Register<System.Collections.Generic.IEnumerable<string>>(nameof(Filter));

        public System.Collections.Generic.IEnumerable<string> Filter { get => (System.Collections.Generic.IEnumerable<string>)GetValue(FilterProperty); set => SetValue(FilterProperty, value); }

        public static readonly DependencyProperty PredicateProperty = Register<Predicate<IBrowsableObjectInfo>>(nameof(Filter));

        public Predicate<IBrowsableObjectInfo> Predicate { get => (Predicate<IBrowsableObjectInfo>)GetValue(PredicateProperty); set => SetValue(PredicateProperty, value); }

        public static readonly DependencyProperty PathProperty = Register<IBrowsableObjectInfo>(nameof(Path));

        public IBrowsableObjectInfo Path { get => (IBrowsableObjectInfo)GetValue(PathProperty); set => SetValue(PathProperty, value); }

        public static readonly DependencyProperty SelectedItemsProperty = Register<System.Collections.Generic.IReadOnlyList<IBrowsableObjectInfo>>(nameof(SelectedItems));

        public System.Collections.Generic.IReadOnlyList<IBrowsableObjectInfo> SelectedItems { get => (System.Collections.Generic.IReadOnlyList<IBrowsableObjectInfo>)GetValue(SelectedItemsProperty); set => SetValue(SelectedItemsProperty, value); }

        public static readonly DependencyProperty ModeProperty = Register<FileSystemDialogBoxMode>(nameof(Mode));

        public FileSystemDialogBoxMode Mode { get => (FileSystemDialogBoxMode)GetValue(ModeProperty); set => SetValue(ModeProperty, value); }

        static FileSystemDialogBoxContent() => DefaultStyleKeyProperty.OverrideMetadata(typeof(FileSystemDialogBoxContent), new FrameworkPropertyMetadata(typeof(FileSystemDialogBoxContent)));
    }
}
