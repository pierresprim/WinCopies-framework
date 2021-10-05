using System;
using System.Windows;
using System.Windows.Controls;

using WinCopies.GUI.IO.ObjectModel;
using WinCopies.IO.ObjectModel;
using WinCopies.Util.Data;
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

        public static readonly DependencyProperty FiltersProperty = Register<System.Collections.Generic.IEnumerable<INamedObject<string>>>(nameof(Filters), new PropertyMetadata((DependencyObject d, DependencyPropertyChangedEventArgs e) =>
        {
            if (((FileSystemDialogBoxContent)d).Mode == FileSystemDialogBoxMode.SelectFolder)

                throw new InvalidOperationException($"{nameof(Mode)} must not be set to {nameof(FileSystemDialogBoxMode.SelectFolder)} to modify {nameof(Filters)}.");
        }));

        public System.Collections.Generic.IEnumerable<INamedObject<string>> Filters { get => (System.Collections.Generic.IEnumerable<INamedObject<string>>)GetValue(FiltersProperty); set => SetValue(FiltersProperty, value); }

        public static readonly DependencyProperty SelectedFilterProperty = Register<INamedObject<string>>(nameof(SelectedFilter));

        public INamedObject<string> SelectedFilter { get => (INamedObject<string>)GetValue(FiltersProperty); set => SetValue(FiltersProperty, value); }

        public static readonly DependencyProperty PredicateProperty = Register<Predicate<IBrowsableObjectInfo>>(nameof(Predicate));

        public Predicate<IBrowsableObjectInfo> Predicate { get => (Predicate<IBrowsableObjectInfo>)GetValue(PredicateProperty); set => SetValue(PredicateProperty, value); }

        public static readonly DependencyProperty PathProperty = Register<IExplorerControlViewModel>(nameof(Path));

        public IExplorerControlViewModel Path { get => (IExplorerControlViewModel)GetValue(PathProperty); set => SetValue(PathProperty, value); }

        public static readonly DependencyProperty SelectedItemsProperty = Register<System.Collections.Generic.IReadOnlyList<IBrowsableObjectInfo>>(nameof(SelectedItems));

        public System.Collections.Generic.IReadOnlyList<IBrowsableObjectInfo> SelectedItems { get => (System.Collections.Generic.IReadOnlyList<IBrowsableObjectInfo>)GetValue(SelectedItemsProperty); set => SetValue(SelectedItemsProperty, value); }

        public static readonly DependencyProperty ModeProperty = Register<FileSystemDialogBoxMode>(nameof(Mode), new PropertyMetadata((DependencyObject d, DependencyPropertyChangedEventArgs e) =>
        {
            if ((FileSystemDialogBoxMode)e.NewValue == FileSystemDialogBoxMode.SelectFolder && ((FileSystemDialogBoxContent)d).Filters != null)

                throw new InvalidOperationException($"Cannot set {nameof(Mode)} to {nameof(FileSystemDialogBoxMode.SelectFolder)} when {nameof(Filters)} has value.");
        }));

        public FileSystemDialogBoxMode Mode { get => (FileSystemDialogBoxMode)GetValue(ModeProperty); set => SetValue(ModeProperty, value); }

        static FileSystemDialogBoxContent() => DefaultStyleKeyProperty.OverrideMetadata(typeof(FileSystemDialogBoxContent), new FrameworkPropertyMetadata(typeof(FileSystemDialogBoxContent)));
    }
}
