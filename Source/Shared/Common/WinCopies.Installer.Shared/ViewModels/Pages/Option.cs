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

using System.Collections;
using System.Collections.Generic;
using System.Linq;

using WinCopies.Util;
using WinCopies.Util.Data;

namespace WinCopies.Installer
{
    public class OptionViewModel : CheckableNamedObjectViewModel<IOption>, IOption
    {
        public event Util.Data.EventHandler<bool> StatusChanged { add => ModelGeneric.StatusChanged += value; remove => ModelGeneric.StatusChanged -= value; }

        public OptionViewModel(IOption option) : base(option) { /* Left empty. */ }

        public bool Action(out string
#if CS8
            ?
#endif
            log) => ModelGeneric.Action(out log);
    }

    public class OptionsPageViewModel : CommonPageViewModel3<IDestinationPage, IOptionsPage, IProcessPage, IOptionsData>, IOptionsPage
    {
        public class GroupBoxViewModel : ViewModel<IOptionGroup>, IOptionGroup
        {
            public string Name { get => ModelGeneric.Name; set { ModelGeneric.Name = value; OnPropertyChanged(nameof(Name)); } }
            public bool? IsChecked { get => ModelGeneric.IsChecked; set { ModelGeneric.IsChecked = value; OnPropertyChanged(nameof(IsChecked)); } }
            public bool IsMultipleChoice => ModelGeneric.IsMultipleChoice;

            public GroupBoxViewModel(in IOptionGroup optionGroup, in bool isChecked = false) : base(GetOptionGroup(optionGroup, isChecked)) { /* Left empty. */ }

            private static IOptionGroup GetOptionGroup(in IOptionGroup optionGroup, in bool isChecked)
            {
                optionGroup.IsChecked = isChecked;

                return optionGroup;
            }

            public IEnumerator<IOption> GetEnumerator() => ModelGeneric.GetEnumerator();

#if !CS8
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
#endif
        }

        public class CheckBoxListViewModel : GroupBoxViewModel
        {
            public CheckBoxListViewModel(in IOptionGroup model, in bool isChecked = false) : base(model, isChecked) { /* Left empty. */ }
        }

        public class RadioButtonListViewModel : GroupBoxViewModel
        {
            public RadioButtonListViewModel(in IOptionGroup model, in bool isChecked = false) : base(model, isChecked) { /* Left empty. */ }
        }

        protected class OptionsDataViewModel : InstallerPageDataViewModel<IOptionsData>, IOptionsData
        {
            protected IEnumerator<IOptionGroup> OptionGroups { get; private set; }

            public ShortcutCreation ShortcutCreation
            {
                get => ModelGeneric.ShortcutCreation;

                set
                {
                    ModelGeneric.ShortcutCreation = value;

                    OnPropertyChanged(nameof(ShortcutCreation));
                }
            }

            public OptionsDataViewModel(in OptionsPageViewModel optionsPage) : base(optionsPage.ModelGeneric.Data, optionsPage.Installer) { /* Left empty. */ }

            private void OptionGroup_StatusChanged(IOptionGroup2 sender, System.EventArgs e)
            {
                if (sender is IOptionGroup3 optionGroup)

                    OnPropertyChanged(optionGroup.PropertyName);
            }

            public IEnumerator<IOptionGroup> GetEnumerator() => OptionGroups
#if CS8
                ??=
#else
                ?? (OptionGroups =
#endif
                ModelGeneric.Select(optionGroup =>
                {
                    IOptionGroup _optionGroup = optionGroup.IsMultipleChoice ? new CheckBoxListViewModel(optionGroup) : new RadioButtonListViewModel(optionGroup)
#if !CS9
                        .AsFromType<IOptionGroup>();
#endif
                    ;

                    if (optionGroup is IOptionGroup3 __optionGroup)

                        __optionGroup.StatusChanged += OptionGroup_StatusChanged;

                    return _optionGroup;
                }
                ).GetEnumerator()
#if !CS8
                )
#endif
                ;
#if !CS8
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
#endif
        }

        internal OptionsPageViewModel(in IOptionsPage optionsPage, in InstallerViewModel installer) : base(optionsPage, installer) { /* Left empty. */ }

        public override IOptionsData GetData() => new OptionsDataViewModel(this);

        public override IInstallerPageViewModel GetPrevious() => new DestinationPageViewModel(ModelGeneric.PreviousPage, Installer);
        public override IInstallerPageViewModel GetNext() => new ProcessPageViewModel(ModelGeneric.NextPage, Installer);
    }
}
