using Microsoft.WindowsAPICodePack.Dialogs;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

using WinCopies.Desktop;

using static System.Environment;
using static System.Environment.SpecialFolder;

using static WinCopies.Desktop.Delegates;
using static WinCopies.Util.Desktop.UtilHelpers;

namespace WinCopies.Installer.GUI
{
    public abstract class BrowseTextBoxBase : GroupBox
    {
        private static DependencyProperty Register<T>(in string propertyName) => Register<T, BrowseTextBoxBase>(propertyName);

        public static readonly DependencyProperty InstallerProperty = Register<IInstaller>(nameof(Installer));

        public IInstaller Installer { get => (IInstaller)GetValue(InstallerProperty); set => SetValue(InstallerProperty, value); }

        public static readonly DependencyProperty TextProperty = Register<string>(nameof(Text));

        public string Text { get => (string)GetValue(TextProperty); set => SetValue(TextProperty, value); }

        public static readonly DependencyProperty LocationProperty = Register<string>(nameof(Location));

        public string Location { get => (string)GetValue(LocationProperty); set => SetValue(LocationProperty, value); }

        static BrowseTextBoxBase()
        {
            DefaultStyleKeyProperty.OverrideDefaultStyleKey<BrowseTextBoxBase>();

            InstallerProperty.OverrideMetadata<BrowseTextBoxBase>(new PropertyMetadata(null, (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
            {
                var installer = (IInstaller)e.NewValue;

                if (installer != null)
                {
                    var _d = (BrowseTextBoxBase)d;

                    if (_d.IsInitialized)

                        _d.ResetLocation(installer);
                }
            }));
        }

        public BrowseTextBoxBase()
        {
            SetResourceReference(StyleProperty, typeof(BrowseTextBoxBase));

            AddCommandBindings();
        }

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

            add(Commands.ApplicationCommands.Reset, (object sender, ExecutedRoutedEventArgs e) => ResetLocation(Installer), CanExecute);
        }

        protected abstract string GetDefaultLocation(in IInstaller installer);

        private void ResetLocation(in IInstaller installer) => Location = GetDefaultLocation(installer);

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            var installer = Installer;

            if (installer != null)

                ResetLocation(installer);
        }
    }

    public class BrowseTextBox : BrowseTextBoxBase
    {
        protected override string GetDefaultLocation(in IInstaller installer) => System.IO.Path.Combine(installer.InstallForCurrentUserOnly ? System.IO.Path.Combine(GetFolderPath(LocalApplicationData), "Programs" + (installer.Is32Bit ? " (x86)" : null)) : GetFolderPath(installer.Is32Bit ? ProgramFilesX86 : ProgramFiles), installer.ProgramName);
    }

    public class Destination : InstallerPageData
    {
        private static DependencyProperty Register<T>(in string propertyName) => Register<T, Destination>(propertyName);

        public static readonly DependencyProperty LocationProperty = Register<string>(nameof(Location));

        public string Location { get => (string)GetValue(LocationProperty); set => SetValue(LocationProperty, value); }

        public static readonly DependencyProperty ExtraDataProperty = Register<object>(nameof(ExtraData));

        public object ExtraData { get => GetValue(ExtraDataProperty); set => SetValue(ExtraDataProperty, value); }

        static Destination() => DefaultStyleKeyProperty.OverrideDefaultStyleKey<Destination>();
    }

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
        private static DependencyProperty Register<T>(in string propertyName) => Register<T, UserGroup>(propertyName);

        public static readonly DependencyProperty InstallForCurrentUserOnlyProperty = Register<bool>(nameof(InstallForCurrentUserOnly));

        public bool InstallForCurrentUserOnly { get => (bool)GetValue(InstallForCurrentUserOnlyProperty); set => SetValue(InstallForCurrentUserOnlyProperty, value); }

        public static readonly DependencyProperty ExtraDataProperty = Register<object>(nameof(ExtraData));

        public object ExtraData { get => GetValue(ExtraDataProperty); set => SetValue(ExtraDataProperty, value); }

        static UserGroup() => DefaultStyleKeyProperty.OverrideDefaultStyleKey<UserGroup>();
    }

    public class Options : InstallerPageData
    {
        public static readonly DependencyProperty OptionCollectionProperty = Register<IEnumerable<ICheckableNamedEnumerable2<ICheckableObject>>, Options>(nameof(OptionCollection));

        public IEnumerable<ICheckableNamedEnumerable2<ICheckableObject>> OptionCollection { get => (IEnumerable<ICheckableNamedEnumerable2<ICheckableObject>>)GetValue(OptionCollectionProperty); set => SetValue(OptionCollectionProperty, value); }

        static Options() => DefaultStyleKeyProperty.OverrideDefaultStyleKey<Options>();
    }

    public class ActionsControl : Control
    {
        public static readonly DependencyProperty ActionsProperty = Register<Actions, ActionsControl>(nameof(Actions), Actions.Both);

        public Actions Actions { get => (Actions)GetValue(ActionsProperty); set => SetValue(ActionsProperty, value); }

        static ActionsControl() => DefaultStyleKeyProperty.OverrideDefaultStyleKey<ActionsControl>();
    }

    public class TemporaryDestination : BrowseTextBoxBase
    {
        public static readonly DependencyProperty TemporaryDirectoryProperty = Register<string, TemporaryDestination>(nameof(TemporaryDirectory));

        public string TemporaryDirectory { get => (string)GetValue(TemporaryDirectoryProperty); set => SetValue(TemporaryDirectoryProperty, value); }

        static TemporaryDestination() => DefaultStyleKeyProperty.OverrideDefaultStyleKey<TemporaryDestination>();

        public TemporaryDestination() => SetResourceReference(StyleProperty, typeof(TemporaryDestination));

        protected override string GetDefaultLocation(in IInstaller installer) => System.IO.Path.Combine(GetFolderPath(Installer.InstallForCurrentUserOnly ? LocalApplicationData : CommonApplicationData), "Temp", Assembly.GetEntryAssembly().GetName().Name, Installer.ProgramName);
    }
}
