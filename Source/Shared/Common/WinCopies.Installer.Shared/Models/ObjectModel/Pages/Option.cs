/* Copyright © Pierre Sprimont, 2022
 *
 * This file is part of the WinCopies Framework.
 *
 * The WinCopies Framework is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * The WinCopies Framework is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with the WinCopies Framework.  If not, see <https://www.gnu.org/licenses/>. */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using WinCopies.Linq;

using static System.Environment;
using static System.Environment.SpecialFolder;

using static WinCopies.Collections.Enumerable;
using static WinCopies.Installer.ShortcutCreation;

namespace WinCopies.Installer
{
    public interface IOptionsData : IInstallerPageData,
#if CS8
        Collections.DotNetFix.Generic.
#endif
        IEnumerable<IOptionGroup>
    {
        ShortcutCreation ShortcutCreation { get; set; }
    }

    public interface IOptionsPage : IBrowsableCommonPage<IDestinationPage, IProcessPage, IOptionsData>
    {
        // Left empty.
    }

    public abstract class OptionsPage : CommonInstallerPage<DestinationPage, IProcessPage, IOptionsData>, IOptionsPage
    {
        protected abstract class OptionsData : InstallerPageData, IOptionsData
        {
            protected IEnumerable<IOptionGroup> OptionGroups { get; private set; }

            public ShortcutCreation ShortcutCreation { get; set; }

            public IEnumerable<IOptionGroup>
#if CS8
                ?
#endif
                Options
            { get; set; }

            public override string
#if CS8
                ?
#endif
                Error => ShortcutCreation == ForAllUsers && Installer.InstallForCurrentUserOnly ? "The installer cannot create shortcuts for all users because you have selected the option for installing for the current user only." : null;

            public OptionsData(in Installer installer) : base(installer) => installer.Options = this;

            private void Enumerable_StatusChanged(IOptionGroup2 sender, EventArgs e)
            {
                if (sender.IsChecked == false)

                    ShortcutCreation = NoShortcut;
            }

            protected abstract void CreateShortcut(in string source, in string destination);

            public IEnumerator<IOptionGroup> GetEnumerator()
            {
                IEnumerable<IOptionGroup> getOptionGroups()
                {
                    var enumerable = new OptionGroup3(nameof(ShortcutCreation)) { Name = "Shortcut Creation" };

                    IOption getOption(in string name, in Action<bool> action, in FuncOut<string
#if CS8
            ?
#endif
            , bool> func) => new Option2(enumerable, false, name, action, func);

                    Action<bool> getAction(ShortcutCreation value) => isChecked =>
                    {
                        if (isChecked)

                            ShortcutCreation = value;
                    };

                    enumerable.StatusChanged += Enumerable_StatusChanged;

                    FuncOut<string
#if CS8
                        ?
#endif
                        , bool> getFunc(SpecialFolder desktop, SpecialFolder startMenu) => (out string
#if CS8
                        ?
#endif
                        log) =>
                        {
                            log = null;

                            string source;
                            string destination;
                            string name;

                            void createShortcut() => CreateShortcut(source, destination);

                            foreach (string relativePath in Installer.Files.WhereSelect(file => file.Value.IsMainApp, file => file.Key))
                            {
                                name = System.IO.Path.GetFileNameWithoutExtension(relativePath) + ".lnk";
                                source = System.IO.Path.Combine(Installer.Location, relativePath);
                                destination = System.IO.Path.Combine(GetFolderPath(desktop), name);
#if DEBUG
                                log = $"Shortcut creation for {source}. Desktop Link Destination: {destination}.\n";
#endif
                                createShortcut();

                                destination = System.IO.Path.Combine(GetFolderPath(startMenu), "Programs", name);
#if DEBUG
                                log += $"Start Menu Link Destination: {destination}.";
#endif
                                createShortcut();
                            }

                            return true;
                        };

                    enumerable.Items = GetEnumerable(getOption("For all users", getAction(ForAllUsers), getFunc(CommonDesktopDirectory, CommonStartMenu)), getOption("For the current user only", getAction(ForCurrentUserOnly), getFunc(DesktopDirectory, StartMenu)));

                    return Options?.Prepend(enumerable) ?? GetEnumerable(enumerable);
                }

                return (OptionGroups
#if CS8
                    ??=
#else
                    ?? (OptionGroups =
#endif
                    getOptionGroups()
#if !CS8
                    )
#endif
                    ).GetEnumerator();
            }
#if !CS8
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
#endif
        }

        protected class DefaultOptionsData : OptionsData
        {
            public DefaultOptionsData(in Installer installer) : base(installer) { }

            protected override void CreateShortcut(in string source, in string destination) => Microsoft.WindowsAPICodePack.Shell.ShellLink.Create(source, destination, false);
        }

        public override string Title => "Options";

        public override string Description => "Please choose the options you want to activate.";

        public override string NextStepName => "_Install";

        IDestinationPage IBrowsableInstallerPage<IDestinationPage, IProcessPage>.PreviousPage => PreviousPage;

        protected OptionsPage(in DestinationPage previousPage) : base(previousPage) { /* Left empty. */ }

        protected override IOptionsData GetData() => new DefaultOptionsData(Installer);
    }
}
