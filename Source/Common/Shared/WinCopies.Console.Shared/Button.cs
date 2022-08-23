using System;

using WinCopies.Collections.Generic;

using static System.Console;

using static WinCopies.Console.Console;
using static WinCopies.UtilHelpers;

namespace WinCopies.Console
{
    public abstract class Button : SelectableControl
    {
        private int _i = 2;
        private FuncIn<string, string> _getText;
        private bool _printTags;

        public new string Text
        {
            get => base.Text;

            set
            {
                InitGetTextFunc();

                base.Text = value;
            }
        }

        public Button(in bool printTags)
        {
            _printTags = printTags;

            InitGetTextFunc();
        }

        private void InitGetTextFunc()
        {
            if (_printTags)

                _getText = GetTextFunc;

            else

                _getText = Delegates.SelfIn;
        }

        private string GetTextFunc(in string s)
        {
            _getText = Delegates.SelfIn;

            return $"<{s}>";
        }

        protected override string RenderOverride2()
        {
            string result = Text;

            if (result != null)

                _ = UtilHelpers.TruncateIfLonger(ref result, GetMaxLength() - _i);

            _i = 0;

            return _getText(result);
        }

        protected override void OnSelect()
        {
            Reverse(Foreground, Background, y => Foreground = y, x => Background = x);

            Render();
        }

        protected internal override bool OnSelect(ConsoleKeyInfo key)
        {
            SetCursorPosition(CursorPosition);

#if CS9
return
#else
            switch (
#endif
            key.Key
#if CS9
            is
#else
            )
            {
                case
#endif
            ConsoleKey.Enter
#if CS9
            or
#else
            :
                case
#endif
                    ConsoleKey.Spacebar
#if CS9
                    ?
#else
                    :

                    return
#endif
                    OnClick()
#if CS9
                    :
#else
                    ;
            }

            return
#endif
                        true;
        }

        protected abstract bool OnClick();

        protected internal override void OnDeselect() => OnSelect();
    }

    public class ActionButton : Button
    {
        public Func<bool> Action { get; set; }

        public ActionButton() : base(true) { }

        public ActionButton(in Func<bool> action) : base(true) => Action = action;

        protected override bool OnClick()
        {
            bool? result = Action?.Invoke();

            return result ?? true;
        }
    }

    public abstract class CheckableButtonBase : Button
    {
        private bool _isChecked;

        public bool IsChecked
        {
            get => _isChecked;

            set
            {
                if (value != _isChecked)

                    _ = OnSelectionChanging();
            }
        }

        public abstract bool IsMultiCheckAvailable { get; }

        public CheckableButtonBase() : base(false) { }

        protected virtual bool OnSelectionChanging() => _isChecked ? OnUnchecked() : OnChecked();

        protected override bool OnClick() => OnSelectionChanging();

        protected virtual bool OnChecked()
        {
            _isChecked = true;

            return true;
        }

        protected virtual bool OnUnchecked()
        {
            _isChecked = false;

            return true;
        }
    }

    public abstract class CheckableButtonBase2 : CheckableButtonBase
    {
        public abstract string Label { get; }

        public abstract string SelectedLabel { get; }

        public abstract string DeselectedLabel { get; }

        protected virtual void OnSelectionChanged() => ((_ILabeledControl)Parent).Label.Text = IsSelected ? SelectedLabel : DeselectedLabel;

        protected override bool OnSelectionChanging()
        {
            bool result = base.OnSelectionChanging();

            OnSelectionChanged();

            return result;
        }
    }

    public class CheckBoxButton : CheckableButtonBase2
    {
        public override bool IsMultiCheckAvailable => true;

        public override string Label => $"[{(IsChecked ? 'x' : ' ')}] ";

        public override string SelectedLabel => Label;

        public override string DeselectedLabel => Label;
    }

    public class RadioButton : CheckBoxButton
    {
        public override bool IsMultiCheckAvailable => false;

        public RadioButtonGroup Group
        {
            get; internal
#if CS9
            init;
#else
            set;
#endif
        }

        public override string Label => $"({(IsChecked ? 'o' : ' ')}) ";

        internal RadioButton() { /* Left empty. */ }

        protected override bool OnClick() => IsChecked || base.OnClick();

        protected override bool OnChecked()
        {
            bool result = base.OnChecked();

            if (Group != null)
            {
                if (Group.SelectedItem != null)

                    Group.SelectedItem.IsChecked = false;

                Group.SelectedItem = this;
            }

            return result;
        }

        protected override bool OnUnchecked()
        {
            bool result = base.OnUnchecked();

            if (Group != null)

                Group.SelectedItem = null;

            return result;
        }
    }

    public class RadioButtonGroup
    {
        public RadioButton
#if CS8
            ?
#endif
            SelectedItem
        { get; internal set; }

        public virtual CheckBox GetNewRadioButton() => CheckBox.GetNewRadioButton(this);

        public virtual RadioButton GetNewRadioButtonButton() => new
#if !CS9
            RadioButton
#endif
            ()
        { Group = this };
    }

    public class CheckBox : Control, _ILabeledControl
    {
        Label _ILabeledControl.Label => Label;

        protected Label Label => (Label)((ReadOnlyArray<ControlElement>)Controls)[0];

        protected CheckableButtonBase2 Button => (CheckableButtonBase2)((ReadOnlyArray<ControlElement>)Controls)[1];

        public string Text { get => Button.Text; set => Button.Text = value; }

        private static Label GetLabel(in Label label, in CheckableButtonBase2 button)
        {
            label.Text = button.Label;

            return label;
        }

        public CheckBox(in Label label, in CheckableButtonBase2 button) : base(new ReadOnlyArray<ControlElement>(new ControlElement[] { GetLabel(label, button), button })) { /* Left empty. */ }

        public CheckBox(in CheckableButtonBase2 button) : this(new Label(), button) { /* Left empty. */ }

        public void Register() => _ = Screen == null ? throw new InvalidOperationException("This control was not added to any screen.") : Screen.AddSelectable(Button);

        public void AddToScreen(in Screen screen)
        {
            _ = screen.Add(this);

            Register();
        }

        public static CheckBox GetNewCheckBox() => new
#if !CS9
            CheckBox
#endif
            (new CheckBoxButton());

        public static CheckBox GetNewRadioButton(in RadioButtonGroup group) => new
#if !CS9
            CheckBox
#endif
            (group == null ? new RadioButton() : group.GetNewRadioButtonButton());

        public static CheckBox GetNewRadioButton() => new
#if !CS9
            CheckBox
#endif
            (new RadioButton());
    }
}
