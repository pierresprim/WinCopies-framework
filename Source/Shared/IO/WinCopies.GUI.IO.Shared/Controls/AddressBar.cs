using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using WinCopies.IO.ObjectModel;

namespace WinCopies.GUI.IO.Controls
{
    public class AddressBar : Control
    {
        private static DependencyProperty Register<T>(in string propertyName) => Util.Desktop.UtilHelpers.Register<T, AddressBar>(propertyName);

        public static readonly DependencyProperty HistoryProperty = Register<ReadOnlyHistoryObservableCollection<IBrowsableObjectInfo>>(nameof(History));

        public ReadOnlyHistoryObservableCollection<IBrowsableObjectInfo> History { get => (ReadOnlyHistoryObservableCollection<IBrowsableObjectInfo>)GetValue(HistoryProperty); set => SetValue(HistoryProperty, value); }

        /// <summary>
        /// Identifies the <see cref="GoButtonCommand"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty GoButtonCommandProperty = Register<ICommand>(nameof(GoButtonCommand));

        public ICommand GoButtonCommand { get => (ICommand)GetValue(GoButtonCommandProperty); set => SetValue(GoButtonCommandProperty, value); }

        /// <summary>
        /// Identifies the <see cref="GoButtonCommandParameter"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty GoButtonCommandParameterProperty = Register<object>(nameof(GoButtonCommandParameter));

        public object GoButtonCommandParameter { get => GetValue(GoButtonCommandParameterProperty); set => SetValue(GoButtonCommandParameterProperty, value); }

        /// <summary>
        /// Identifies the <see cref="Path"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PathProperty = Register<string>(nameof(Path));

        public string Path { get => (string)GetValue(PathProperty); set => SetValue(PathProperty, value); }

        /// <summary>
        /// Identifies the <see cref="Text"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TextProperty = Register<string>(nameof(Text));

        public string Text { get => (string)GetValue(TextProperty); set => SetValue(TextProperty, value); }
    }
}
