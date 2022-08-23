using System;
using System.Windows;
using System.Windows.Input;

using WinCopies.Desktop;

using static WinCopies.Util.Desktop.UtilHelpers;

namespace WinCopies.Installer.GUI
{
    public class Process : InstallerPageData
    {
        private static DependencyProperty Register<T>(in string propertyName) => Register<T, Process>(propertyName);

        public static readonly DependencyProperty OverallProgressProperty = Register<double>(nameof(OverallProgress));

        public double OverallProgress { get => (double)GetValue(OverallProgressProperty); set => SetValue(OverallProgressProperty, value); }

        public static readonly DependencyProperty CurrentItemProgressProperty = Register<double>(nameof(CurrentItemProgress));

        public double CurrentItemProgress { get => (double)GetValue(CurrentItemProgressProperty); set => SetValue(CurrentItemProgressProperty, value); }

        public static readonly DependencyProperty StepNameProperty = Register<string>(nameof(StepName));

        public string StepName { get => (string)GetValue(StepNameProperty); set => SetValue(StepNameProperty, value); }

        public static readonly DependencyProperty StepDataProperty = Register<byte>(nameof(StepData));

        public byte StepData { get => (byte)GetValue(StepDataProperty); set => SetValue(StepDataProperty, value); }

        public static readonly DependencyProperty CommandProperty = Register<ICommand>(nameof(Command));

        public ICommand Command { get => (ICommand)GetValue(CommandProperty); set => SetValue(CommandProperty, value); }

        public static readonly DependencyProperty LogProperty = Register<string>(nameof(Log));

        public string Log { get => (string)GetValue(LogProperty); set => SetValue(LogProperty, value); }

        static Process() => DefaultStyleKeyProperty.OverrideDefaultStyleKey<Process>();

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            _ = Command.TryExecute(null);
        }
    }
}
