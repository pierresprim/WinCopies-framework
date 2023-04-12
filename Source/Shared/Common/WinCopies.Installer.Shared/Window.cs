using Microsoft.WindowsAPICodePack.Dialogs;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

using WinCopies.Desktop;

using static System.Environment;
using static System.Environment.SpecialFolder;
using static System.Windows.Input.NavigationCommands;

using static WinCopies.Desktop.Delegates;
using static WinCopies.Util.Desktop.UtilHelpers;
using Application = System.Windows.Application;

namespace WinCopies.Installer.GUI
{
    public class InstallerControl : ContentControl
    {
        public static readonly DependencyProperty NextStepNameProperty = Register<string, InstallerControl>(nameof(NextStepName));

        public string NextStepName { get => (string)GetValue(NextStepNameProperty); set => SetValue(NextStepNameProperty, value); }

        static InstallerControl() => DefaultStyleKeyProperty.OverrideDefaultStyleKey<InstallerControl>();
    }

    public class InstallerPage : ContentControl
    {
        private static DependencyProperty Register<T>(in string propertyName) => Register<T, InstallerPage>(propertyName);

        public static readonly DependencyProperty TitleProperty = Register<string>(nameof(Title));

        public string Title { get => (string)GetValue(TitleProperty); set => SetValue(TitleProperty, value); }

        public static readonly DependencyProperty ImageSourceProperty = Register<ImageSource>(nameof(ImageSource));

        public ImageSource ImageSource { get => (ImageSource)GetValue(ImageSourceProperty); set => SetValue(ImageSourceProperty, value); }
    }

    public class CommonPage : InstallerPage
    {
        private static DependencyProperty Register<T>(in string propertyName) => Register<T, CommonPage>(propertyName);

        public static readonly DependencyProperty DescriptionProperty = Register<string>(nameof(Description));

        public string Description { get => (string)GetValue(DescriptionProperty); set => SetValue(DescriptionProperty, value); }

        public static readonly DependencyProperty IconProperty = Register<ImageSource>(nameof(Icon));

        public Icon Icon { get => (Icon)GetValue(IconProperty); set => SetValue(IconProperty, value); }

        static CommonPage() => DefaultStyleKeyProperty.OverrideDefaultStyleKey<CommonPage>();
    }

    public class InstallerPageData : Control
    {

        public static readonly DependencyProperty InstallerProperty = Register<IInstaller, InstallerPageData>(nameof(Installer));

        public IInstaller Installer { get => (IInstaller)GetValue(InstallerProperty); set => SetValue(InstallerProperty, value); }
    }

    public class LicenseAgreement : InstallerPageData
    {
        public static readonly DependencyProperty DocumentProperty = Register<FlowDocument, LicenseAgreement>(nameof(Document));

        public FlowDocument Document { get => (FlowDocument)GetValue(DocumentProperty); set => SetValue(DocumentProperty, value); }

        static LicenseAgreement() => DefaultStyleKeyProperty.OverrideDefaultStyleKey<LicenseAgreement>();
    }

    public class UserGroup : InstallerPageData
    {
        public static readonly DependencyProperty InstallForCurrentUserOnlyProperty = Register<bool, UserGroup>(nameof(InstallForCurrentUserOnly));

        public bool InstallForCurrentUserOnly { get => (bool)GetValue(InstallForCurrentUserOnlyProperty); set => SetValue(InstallForCurrentUserOnlyProperty, value); }

        static UserGroup() => DefaultStyleKeyProperty.OverrideDefaultStyleKey<UserGroup>();
    }

    public class Destination : InstallerPageData
    {
        public static readonly DependencyProperty LocationProperty = Register<string, Destination>(nameof(Location));

        public string Location { get => (string)GetValue(LocationProperty); set => SetValue(LocationProperty, value); }

        static Destination()
        {
            DefaultStyleKeyProperty.OverrideDefaultStyleKey<Destination>();

            InstallerProperty.OverrideMetadata<Destination>(new PropertyMetadata(null, (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
            {
                var installer = (IInstaller)e.NewValue;

                if (installer != null)
                {
                    var _d = (Destination)d;

                    if (_d.IsInitialized)

                        _d.Reset(installer);
                }
            }));
        }

        public Destination() => AddCommandBindings();

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            var installer = Installer;

            if (installer != null)

                Reset(installer);
        }

        private void Reset(in IInstaller installer) => Location = System.IO.Path.Combine(installer.InstallForCurrentUserOnly ? System.IO.Path.Combine(GetFolderPath(LocalApplicationData), "Programs" + (installer.Is32Bit ? " (x86)" : null)) : GetFolderPath(installer.Is32Bit ? ProgramFilesX86 : ProgramFiles), installer.ProgramName);

        public void Reset() => Reset(Installer);

        protected virtual void AddCommandBindings()
        {
            void add(in ICommand command, ExecutedRoutedEventHandler executedRoutedEventHandler, CanExecuteRoutedEventHandler canExecuteRoutedEventHandler) => CommandBindings.Add(command, executedRoutedEventHandler, canExecuteRoutedEventHandler);

            add(Commands.NavigationCommands.Browse, (object sender, ExecutedRoutedEventArgs e) =>
            {
                var dialog = new CommonOpenFileDialog { IsFolderPicker = true, InitialDirectory = Location };

                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)

                    Location = dialog.FileNames[0];
            },
            CanExecute);

            add(Commands.ApplicationCommands.Reset, (object sender, ExecutedRoutedEventArgs e) => Reset(), CanExecute);
        }
    }

    public class Options : InstallerPageData
    {
        public static readonly DependencyProperty OptionCollectionProperty = Register<IEnumerable<ICheckableNamedEnumerable2<ICheckableObject>>, Options>(nameof(OptionCollection));

        public IEnumerable<ICheckableNamedEnumerable2<ICheckableObject>> OptionCollection { get => (IEnumerable<ICheckableNamedEnumerable2<ICheckableObject>>)GetValue(OptionCollectionProperty); set => SetValue(OptionCollectionProperty, value); }

        static Options() => DefaultStyleKeyProperty.OverrideDefaultStyleKey<Options>();
    }

    public class Process : InstallerPageData
    {
        private static DependencyProperty Register<T>(in string propertyName) => Register<T, Process>(propertyName);

        public static readonly DependencyProperty OverallProgressProperty = Register<double>(nameof(OverallProgress));

        public double OverallProgress { get => (double)GetValue(OverallProgressProperty); set => SetValue(OverallProgressProperty, value); }

        public static readonly DependencyProperty CurrentItemProgressProperty = Register<double>(nameof(CurrentItemProgress));

        public double CurrentItemProgress { get => (double)GetValue(CurrentItemProgressProperty); set => SetValue(CurrentItemProgressProperty, value); }

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

    public class InstallerWindow : Window
    {
        private static DependencyPropertyKey RegisterReadOnly<T>(in string propertyName, in T defaultValue) => RegisterReadOnly<T, InstallerWindow>(propertyName, defaultValue);

        private static readonly DependencyPropertyKey InstallerPropertyKey = RegisterReadOnly<IInstallerModel
#if CS8
            ?
#endif
            >(nameof(Installer), null);

        public static readonly DependencyProperty InstallerProperty = InstallerPropertyKey.DependencyProperty;

        public IInstallerModel Installer { get => (IInstallerModel)GetValue(InstallerProperty); private set => SetValue(InstallerPropertyKey, value); }

        static InstallerWindow() => DefaultStyleKeyProperty.OverrideDefaultStyleKey<InstallerWindow>();

        public InstallerWindow(in IInstallerModel installer)
        {
            SetResourceReference(StyleProperty, typeof(InstallerWindow));

            Installer = installer;

            AddCommandBindings();
        }

        protected virtual void AddCommandBindings()
        {
            void add(in ICommand command, Action action, Func<bool> condition) => CommandBindings.Add(command, (object sender, ExecutedRoutedEventArgs e) =>
                {
                    action?.Invoke();
                    e.Handled = true;
                }, (object sender, CanExecuteRoutedEventArgs e) =>
                {
                    e.CanExecute = condition();
                    e.Handled = true;
                });

            void updateInstaller(in ICommand command, Func<Action
#if CS8
                ?
#endif
                > func, in Func<bool> condition) => add(command, () => func()?.Invoke(), condition);

            updateInstaller(BrowseBack, () => Installer.Current.MovePrevious, () => Installer.Current.CanBrowseBack);
            updateInstaller(BrowseForward, () =>
            {
                IInstallerModel installer = Installer;
                IInstallerPageViewModel page = installer.Current;

                if (installer.Completed && !page.CanBrowseForward)
                {
                    Application.Current.Shutdown(0);

                    return null;
                }

                if (page is ICommonPage commonPage)
                {
                    string
#if CS8
                        ?
#endif
                        error = commonPage.Data.Error;

                    if (error != null)
                    {
                        _ = MessageBox.Show(error, "Data Error", MessageBoxButton.OK, MessageBoxImage.Error);

                        return null;
                    }
                }

                return page.MoveNext;
            }, () =>
            {
                IInstallerModel installer = Installer;

                return installer.Current.CanBrowseForward || installer.Completed;
            });

            add(Commands.DialogCommands.Cancel, Close, () => Installer.Current.CanCancel);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            IInstallerModel installer = Installer;
            IInstallerPageViewModel current = installer.Current;

            if (installer.Completed)
            {
                if (current.CanBrowseForward)

                    e.Cancel = true;

                return;
            }

            if ((!current.CanBrowseForward || MessageBox.Show($"Are you sure you want to cancel the installation of {installer.ProgramName}?", "Installation Cancellation - Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes) == MessageBoxResult.No))

                e.Cancel = true;
        }
    }
}
