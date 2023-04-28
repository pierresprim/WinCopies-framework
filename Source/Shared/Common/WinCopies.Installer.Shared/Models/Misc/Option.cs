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

using WinCopies.Util.Data;

namespace WinCopies.Installer
{
    public interface IOption : ICheckableNamedObject
    {
        event Util.Data.EventHandler<bool> StatusChanged;

        bool Action(out string log);
    }

    public interface IOption2 : IOption
    {
        Action<bool> OnOptionChecked { get; }
    }

    public abstract class OptionBase
    {
        protected ICheckableNamedObject InnerObject { get; }

        public string Name { get => InnerObject.Name; set => InnerObject.Name = value; }

        public event Util.Data.EventHandler<bool> StatusChanged;

        protected OptionBase(in bool isChecked, in string name) => InnerObject = new CheckableNamedObject(isChecked, name);

        public abstract bool Action(out string
#if CS8
            ?
#endif
            log);

        protected virtual void OnPreChecked() { }

        protected virtual void OnStatusChanged(in bool value) => StatusChanged?.Invoke(this, new EventArgs<bool>(value));
    }

    public abstract class Option<T> : OptionBase, IOption where T : IOptionGroup
    {
        public T OptionGroup { get; }

        public bool IsChecked
        {
            get => InnerObject.IsChecked;

            set
            {
                if (value != IsChecked)
                {
                    InnerObject.IsChecked = value;

                    bool? _isChecked = OptionGroup.IsChecked;

                    if (_isChecked.HasValue)
                    {
                        bool isChecked = _isChecked.Value;

                        if (isChecked)

                            OnStatusChanged(value);

                        else

                            OnPreChecked();
                    }
                }
            }
        }

        protected Option(in T optionGroup, in bool isChecked, in string name) : base(isChecked, name) => OptionGroup = optionGroup;
    }

    public abstract class OptionBase2 : Option<IOptionGroup>
    {
        public OptionBase2(in IOptionGroup optionGroup, in bool isChecked, in string name) : base(optionGroup, isChecked, name) { }
    }


    public abstract class Option2<T> : Option<T> where T : OptionGroup2
    {
        protected Option2(in T optionGroup, in bool isChecked, in string name) : base(optionGroup, isChecked, name) { /* Left empty. */ }

        protected virtual void OnChecked() => OptionGroup.OnStatusChanged(true); // To be notified when the enum value has changed.

        protected override void OnStatusChanged(in bool value)
        {
            base.OnStatusChanged(value);

            if (value)

                OnChecked();
        }
    }

    public abstract class Option : Option2<OptionGroup3>, IOption2
    {
        public Action<bool> OnOptionChecked { get; }

        protected Option(in OptionGroup3 optionGroup, in bool isChecked, in string name, Action<bool> onOptionChecked) : base(optionGroup, isChecked, name)
        {
            OnOptionChecked = onOptionChecked;

            StatusChanged += (object sender, EventArgs<bool> e) => onOptionChecked(e.Value);
        }

        protected override void OnPreChecked()
        {
            base.OnPreChecked();

            OptionGroup.SelectedItem = this;
        }

        protected override void OnChecked()
        {
            OnPreChecked();

            base.OnChecked();
        }
    }

    public class Option2 : Option
    {
        protected FuncOut<string
#if CS8
            ?
#endif
            , bool> Delegate
        { get; }

        public Option2(in OptionGroup3 optionGroup, in bool isChecked, in string name, in Action<bool> onOptionChecked, in FuncOut<string
#if CS8
            ?
#endif
            , bool> func) : base(optionGroup, isChecked, name, onOptionChecked) => Delegate = func;

        public override bool Action(out string
#if CS8
            ?
#endif
            log) => Delegate(out log);
    }

    public enum ShortcutCreation : byte
    {
        NoShortcut = 0,

        ForCurrentUserOnly = 1,

        ForAllUsers
    }
}
