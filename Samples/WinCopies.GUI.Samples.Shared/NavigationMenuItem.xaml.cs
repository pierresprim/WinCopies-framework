using System.Windows;
using System.Windows.Input;

namespace WinCopies.GUI.Samples
{
    public partial class NavigationMenuItem : IconControl, ICommandSource
    {
        private static DependencyProperty Register<T>(in string propertyName) => Util.Desktop.UtilHelpers.Register<T, NavigationMenuItem>(propertyName);

        public static readonly DependencyProperty HeaderProperty = Register<string>(nameof(Header));

        public string Header { get => (string)GetValue(HeaderProperty); set => SetValue(HeaderProperty, value); }

        public static readonly DependencyProperty CommandProperty = Register<ICommand>(nameof(Command));

        public ICommand Command { get => (ICommand)GetValue(CommandProperty); set => SetValue(CommandProperty, value); }

        public static readonly DependencyProperty CommandParameterProperty = Register<object>(nameof(CommandParameter));

        public object CommandParameter { get => GetValue(CommandParameterProperty); set => SetValue(CommandParameterProperty, value); }

        public static readonly DependencyProperty CommandTargetProperty = Register<IInputElement>(nameof(CommandTarget));

        public IInputElement CommandTarget { get => (IInputElement)GetValue(CommandTargetProperty); set => SetValue(CommandTargetProperty, value); }

        public static readonly DependencyProperty StaysOpenOnClickProperty = Register<bool>(nameof(StaysOpenOnClick));

        public bool StaysOpenOnClick { get => (bool)GetValue(StaysOpenOnClickProperty); set => SetValue(StaysOpenOnClickProperty, value); }

        public NavigationMenuItem() => InitializeComponent();
    }
}
