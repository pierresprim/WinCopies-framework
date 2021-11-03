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
        private Predicate<IBrowsableObjectInfo> _predicate;
        private SelectionMode _selectionMode;

        private static DependencyProperty Register<T>(in string propertyName) => Register<T, FileSystemDialogBoxContent>(propertyName);

        private static DependencyProperty Register<T>(in string propertyName, in PropertyMetadata metadata) => Register<T, FileSystemDialogBoxContent>(propertyName, metadata);

        public static readonly DependencyProperty FiltersProperty = Register<System.Collections.Generic.IEnumerable<INamedObject<string>>>(nameof(Filters)/*, new PropertyMetadata((DependencyObject d, DependencyPropertyChangedEventArgs e) => FileSystemDialog.ValidateFilters(((FileSystemDialogBoxContent)d).Mode, nameof(Mode)))*/);

        public System.Collections.Generic.IEnumerable<INamedObject<string>> Filters { get => (System.Collections.Generic.IEnumerable<INamedObject<string>>)GetValue(FiltersProperty); set => SetValue(FiltersProperty, value); }

        public static readonly DependencyProperty SelectedFilterProperty = Register<INamedObject<string>>(nameof(SelectedFilter));

        public INamedObject<string> SelectedFilter { get => (INamedObject<string>)GetValue(FiltersProperty); set => SetValue(FiltersProperty, value); }

        public static readonly DependencyProperty PredicateProperty = Register<Predicate<IBrowsableObjectInfo>>(nameof(Predicate));

        public Predicate<IBrowsableObjectInfo> Predicate { get => (Predicate<IBrowsableObjectInfo>)GetValue(PredicateProperty); set => SetValue(PredicateProperty, value); }

        public static readonly DependencyProperty SelectedPredicateProperty = Register<Predicate<IBrowsableObjectInfo>>(nameof(SelectedPredicate), new PropertyMetadata((DependencyObject d, DependencyPropertyChangedEventArgs e) => UpdatePath(((FileSystemDialogBoxContent)d).Path, e.NewValue as Predicate<IBrowsableObjectInfo>)));

        public Predicate<IBrowsableObjectInfo> SelectedPredicate { get => (Predicate<IBrowsableObjectInfo>)GetValue(SelectedPredicateProperty); set => SetValue(SelectedPredicateProperty, value); }

        public static readonly DependencyProperty PathProperty = Register<IExplorerControlViewModel>(nameof(Path), new PropertyMetadata((DependencyObject d, DependencyPropertyChangedEventArgs e) =>
        {
            var _d = (FileSystemDialogBoxContent)d;

            if (e.OldValue is IExplorerControlViewModel oldPath)
            {
                oldPath.Path.Filter = _d._predicate;

                oldPath.SelectionMode = _d._selectionMode;

                oldPath.PropertyChanged -= _d.Path_PropertyChanged;
                oldPath.Path.PropertyChanged -= _d.Path_PropertyChanged;
            }

            var newPath = e.NewValue as IExplorerControlViewModel;

            _d._predicate = newPath.Filter;

            _d._selectionMode = newPath.SelectionMode;

            newPath.PropertyChanged += _d.Path_PropertyChanged;
            newPath.Path.PropertyChanged += _d.Path_PropertyChanged;

            newPath.SelectionMode = SelectionMode.Single;

            UpdatePath(newPath, _d.SelectedPredicate);

            _d.SetText();
        }));

        public IExplorerControlViewModel Path { get => (IExplorerControlViewModel)GetValue(PathProperty); set => SetValue(PathProperty, value); }

        public static readonly DependencyProperty SelectedItemsProperty = Register<System.Collections.Generic.IReadOnlyList<IBrowsableObjectInfo>>(nameof(SelectedItems));

        public System.Collections.Generic.IReadOnlyList<IBrowsableObjectInfo> SelectedItems { get => (System.Collections.Generic.IReadOnlyList<IBrowsableObjectInfo>)GetValue(SelectedItemsProperty); set => SetValue(SelectedItemsProperty, value); }

        public static readonly DependencyProperty ModeProperty = Register<FileSystemDialogBoxMode>(nameof(Mode), new PropertyMetadata(FileSystemDialogBoxMode.SelectFolder, (DependencyObject d, DependencyPropertyChangedEventArgs e) => /*FileSystemDialog.ValidateMode((FileSystemDialogBoxMode)e.NewValue, nameof(Mode), ((FileSystemDialogBoxContent)d).Filters, nameof(Filters))*/
        {
            if ((FileSystemDialogBoxMode)e.NewValue != FileSystemDialogBoxMode.Save && d.GetValue(UserTextProperty) != null)

                throw new InvalidOperationException($"{nameof(UserText)} must be null when {nameof(Mode)} is not set to {nameof(FileSystemDialogBoxMode.Save)}.");
        }));

        public FileSystemDialogBoxMode Mode { get => (FileSystemDialogBoxMode)GetValue(ModeProperty); set => SetValue(ModeProperty, value); }

        private static readonly DependencyPropertyKey TextPropertyKey = RegisterReadOnly<string, FileSystemDialogBoxContent>(nameof(Text), new PropertyMetadata(null));

        public static readonly DependencyProperty TextProperty = TextPropertyKey.DependencyProperty;

        public string Text { get => (string)GetValue(TextProperty); private set => SetValue(TextPropertyKey, value); }

        public static readonly DependencyProperty UserTextProperty = Register<string>(nameof(UserText), new PropertyMetadata((DependencyObject d, DependencyPropertyChangedEventArgs e) =>
        {
            if ((FileSystemDialogBoxMode)d.GetValue(ModeProperty) != FileSystemDialogBoxMode.Save)

                throw new InvalidOperationException($"Can only set {nameof(UserText)} when {nameof(Mode)} is set to {nameof(FileSystemDialogBoxMode.Save)}.");
        }));

        public string UserText { get => (string)GetValue(UserTextProperty); set => SetValue(UserTextProperty, value); }

        static FileSystemDialogBoxContent() => DefaultStyleKeyProperty.OverrideMetadata(typeof(FileSystemDialogBoxContent), new FrameworkPropertyMetadata(typeof(FileSystemDialogBoxContent)));

        private static void UpdatePath(in IExplorerControlViewModel path, in Predicate<IBrowsableObjectInfo> predicate)
        {
            if (path != null)

                path.Filter = predicate;
        }

        private void SetText()
        {
            IBrowsableObjectInfoViewModel path = Path.Path;

            Text = (path.SelectedItem ?? path).Name;
        }

        protected virtual void OnPathSelectionChanged()
        {
            SetText();
        }

        protected virtual void OnInnerPathChanged()
        {
            if (Mode != FileSystemDialogBoxMode.Save)

                SetText();
        }

        protected virtual void OnPathPropertyChanged(System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(IExplorerControlViewModel.Path):

                    OnInnerPathChanged();

                    break;

                    case nameof(IBrowsableObjectInfoViewModel.SelectedItem):

                    OnPathSelectionChanged();

                    break;
            }
        }

        private void Path_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) => OnPathPropertyChanged(e);
    }
}
