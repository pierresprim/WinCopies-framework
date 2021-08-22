using SevenZip;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;

using WinCopies.Collections.Generic;

namespace WinCopies.Console.Samples
{
    public class Setup : WinCopies.Console.Setup
    {
        public override System.Collections.Generic.IEnumerable<string> Welcome => new string[] { "Welcome on the WinCopies Setup Wizard Sample!", "This program will guide you through the installation of WinCopies" };

        public override string DefaultPath { get; } = Microsoft.WindowsAPICodePack.Shell.KnownFolders.Desktop.Path + "\\WinCopies";

        public override string SoftwareFileName => nameof(WinCopies) + ".exe";

        public override InArchiveFormat InArchiveFormat => InArchiveFormat.Zip;

        public override System.IO.Stream GetStream() => Assembly.GetExecutingAssembly().GetManifestResourceStream($"{nameof(WinCopies)}.{nameof(Console)}.{nameof(Samples)}.{nameof(Resources)}.wincopies-2.4-a.zip");
    }

    public class LabeledControl2 : Control
    {
        public Label Label => (Label)((ReadOnlyArray<ControlElement>)Controls)[0];

        public ControlElement Control => ((ReadOnlyArray<ControlElement>)Controls)[1];

        public Label Label2 => (Label)((ReadOnlyArray<ControlElement>)Controls)[2];

        public ControlElement Control2 => ((ReadOnlyArray<ControlElement>)Controls)[3];

        public LabeledControl2(in Label label, in ControlElement control, in Label label2, in ControlElement control2) : base(new ReadOnlyArray<ControlElement>(new ControlElement[] { label, control, label2, control2 }))
        {

        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            new Setup().Main();

            //return;

            //WinCopies.Console.Console console = WinCopies.Console.Console.Instance;

            //Screen screen = console.GetScreen();

            //console.Screen = screen;

            //screen.Add(new Label()).Text = "Label text";

            //var selector = screen.AddSelectable(new ScrollableSelect());

            //ListLoopEnumerator<string> getItems() => new ListLoopEnumerator<string>(new string[] { "Yes", "No" });

            //selector.Items = getItems();

            //screen.Select(selector);

            //var _selector = screen.Add(new LabeledControl(new Label() { Text = "Select a value: " }, new ScrollableSelect()));

            //((ScrollableSelect)_selector.Control).Items = getItems();

            //screen.AddSelectable((SelectableControl)_selector.Control);

            //screen.Select((SelectableControl)_selector.Control);



            //var select = new ScrollableSelect() { Items = getItems() };

            //var select2 = new ScrollableSelect() { Items = getItems() };

            //var _selector2 = screen.Add(new LabeledControl2(new Label() { Text = "Select a value: " }, select, new Label() { Text = " " }, select2));

            //screen.AddSelectable(select);

            //screen.AddSelectable(select2);

            //screen.Select(select);



            //screen.Add(new Label()).Text = new string('a', 120);

            //screen.Add(new Label()).Text = new string('a', 121);



            //select = new ScrollableSelect() { Items = getItems() };

            //select2 = new ScrollableSelect() { Items = getItems() };

            //_selector2 = screen.Add(new LabeledControl2(new Label() { Text = "Select a value: " }, select, new Label() { Text = " " }, select2));

            //screen.AddSelectable(select);

            //screen.AddSelectable(select2);

            //screen.Select(select);

            //CheckBox __selector = screen.Add(CheckBox.GetNewCheckBox());

            //__selector.Text = "Azerty";

            //__selector.Register();

            //__selector = screen.Add(CheckBox.GetNewRadioButton());

            //__selector.Text = "Qwerty";

            //__selector.Register();

            //var radioButtonGroup = new RadioButtonGroup();

            //__selector = screen.Add(CheckBox.GetNewRadioButton(radioButtonGroup));

            //__selector.Text = "Qwerty1";

            //__selector.Register();

            //__selector = screen.Add(CheckBox.GetNewRadioButton(radioButtonGroup));

            //__selector.Text = "Qwerty2";

            //__selector.Register();

            //var progressBar = screen.Add(new ProgressBar());

            //progressBar.Minimum = 0u;

            //progressBar.Maximum = 50u;

            //progressBar.Size = 100u;

            //for (uint i = 0u; i <= progressBar.Maximum; i++)
            //{
            //    Thread.Sleep(5);

            //    progressBar.Value = i;
            //}

            //progressBar = screen.Add(new ProgressBar());

            //progressBar.Minimum = 0u;

            //progressBar.Maximum = 100u;

            //progressBar.Size = 100u;

            //for (uint i = 0u; i <= progressBar.Maximum; i++)
            //{
            //    Thread.Sleep(5);

            //    progressBar.Value = i;
            //}

            //progressBar = screen.Add(new ProgressBar());

            //progressBar.Minimum = 0u;

            //progressBar.Maximum = 200u;

            //progressBar.Size = 100u;

            //for (uint i = 0u; i <= progressBar.Maximum; i++)
            //{
            //    Thread.Sleep(5);

            //    progressBar.Value = i;
            //}

            //progressBar = screen.Add(new ProgressBar());

            //progressBar.Minimum = 0u;

            //progressBar.Maximum = 100u;

            //progressBar.Size = 50u;

            //for (uint i = 0u; i <= progressBar.Maximum; i++)
            //{
            //    Thread.Sleep(5);

            //    progressBar.Value = i;
            //}

            //progressBar = screen.Add(new ProgressBar());

            //progressBar.Minimum = 0u;

            //progressBar.Maximum = 100u;

            //progressBar.Size = 200u;

            //for (uint i = 0u; i <= progressBar.Maximum; i++)
            //{
            //    Thread.Sleep(5);

            //    progressBar.Value = i;
            //}

            //progressBar = screen.Add(new ProgressBar());

            //progressBar.Minimum = 0u;

            //progressBar.Maximum = 50u;

            //progressBar.Size = 50u;

            //for (uint i = 0u; i <= progressBar.Maximum; i++)
            //{
            //    Thread.Sleep(5);

            //    progressBar.Value = i;
            //}

            //progressBar = screen.Add(new ProgressBar());

            //progressBar.Minimum = 0u;

            //progressBar.Maximum = 200u;

            //progressBar.Size = 200u;

            //for (uint i = 0u; i <= progressBar.Maximum; i++)
            //{
            //    Thread.Sleep(5);

            //    progressBar.Value = i;
            //}

            //console.Loop();
        }
    }
}
