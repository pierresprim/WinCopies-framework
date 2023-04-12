using System;
using System.Collections.Generic;
using System.Text;
using WinCopies.Collections.Generic;

namespace WinCopies.Console
{
    public interface ILabel : IControl
    {
        new string Text { get; set; }
    }

    public class Label : ControlElement, ILabel
    {
        public new string Text { get => base.Text; set => base.Text = value; }

        protected override string RenderOverride2() => Text;
    }

    internal interface _ILabeledControl
    {
        Label Label { get; }
    }

    public class LabeledControl : Control
    {
        public Label Label => (Label)((ReadOnlyArray<ControlElement>)Controls)[0];

        public ControlElement Control => ((ReadOnlyArray<ControlElement>)Controls)[1];

        public LabeledControl(in Label label, in ControlElement control) : base(new ReadOnlyArray<ControlElement>(new ControlElement[] { label, control }))
        {

        }
    }
}
