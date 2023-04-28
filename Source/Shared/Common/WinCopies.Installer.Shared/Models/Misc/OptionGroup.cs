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

using WinCopies.Util;

namespace WinCopies.Installer
{
    public interface IOptionGroup : ICheckableNamedEnumerable2<IOption>
#if CS8
        , Collections.DotNetFix.Generic.IEnumerable<IOption>
#endif
    {
        bool IsMultipleChoice { get; }
    }

    public interface IOptionGroup2 : IOptionGroup
    {
        IEnumerable<IOption>
#if CS9
            ?
#endif
            Items
        { get; set; }

        event EventHandler<IOptionGroup2> StatusChanged;
    }

    public interface IOptionGroup3 : IOptionGroup2
    {
        string PropertyName { get; }
    }

    public interface IOptionGroup4 : IOptionGroup3
    {
        IOption2 SelectedItem { get; }
    }

    public class OptionGroup : IOptionGroup2
    {
        private byte _bools;

        protected CheckableNamedEnumerable2<IOption> InnerObject { get; } = new CheckableNamedEnumerable2<IOption>();

        protected bool Parsed => GetBit(0);

        public bool IsMultipleChoice
        {
            get => GetBit(1);

            private
#if CS9
            init
#else
                set
#endif
                => UtilHelpers.SetBit(ref _bools, 1, value);
        }

        public IEnumerable<IOption>
#if CS9
            ?
#endif
            Items
        { get => InnerObject.Items; set => InnerObject.Items = value; }

        public string Name { get => InnerObject.Name; set => InnerObject.Name = value; }

        string INamedObjectBase.Name => InnerObject.Name;

        public bool? IsChecked
        {
            get => InnerObject.IsChecked;

            set
            {
                if (IsChecked != value)
                {
                    InnerObject.IsChecked = value;

                    OnStatusChanged(false);
                }
            }
        }

        public event EventHandler<IOptionGroup2> StatusChanged;

        public OptionGroup(in bool isMultipleChoice) => IsMultipleChoice = isMultipleChoice;

        private bool GetBit(in byte pos) => _bools.GetBit(pos);

        protected internal virtual void OnStatusChanged(in bool fromItem) => StatusChanged?.Invoke(this, System.EventArgs.Empty);

        public IEnumerator<IOption> GetEnumerator()
        {
            CheckableNamedEnumerable2<IOption> innerObject = InnerObject;

            if (_bools == 0)
            {
                void action()
                {
                    ICheckableNamedObject
#if CS8
                        ?
#endif
                        first = null;

                    PredicateIn<ICheckableNamedObject> predicate = (in ICheckableNamedObject _obj) =>
                    {
                        first = _obj;

                        return (predicate = (in ICheckableNamedObject __obj) => innerObject.IsChecked == true)(_obj);
                    };

                    foreach (ICheckableNamedObject obj in innerObject)

                        if (predicate(obj))

                            return;

                    if (first != null)

                        first.IsChecked = true;
                }

                action();
            }

            return innerObject.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public class OptionGroup2 : OptionGroup, IOptionGroup3
    {
        public string PropertyName { get; }

        public OptionGroup2(in string propertyName, in bool isMultipleChoice) : base(isMultipleChoice) => PropertyName = propertyName;
    }

    public class OptionGroup3 : OptionGroup2, IOptionGroup4
    {
        public IOption2 SelectedItem { get; internal set; }

        public OptionGroup3(in string propertyName) : base(propertyName, false) { }

        protected internal override void OnStatusChanged(in bool fromItem)
        {
            if (!fromItem && IsChecked == true)

                SelectedItem.OnOptionChecked(true);

            base.OnStatusChanged(fromItem);
        }
    }
}
