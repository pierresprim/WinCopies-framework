using System.Windows;
using System.Windows.Controls;

using WinCopies.Desktop;
using WinCopies.Util;

using static WinCopies.Util.Desktop.UtilHelpers;

namespace WinCopies.GUI.Controls
{
    public class ComboListBox : System.Windows.Controls.Primitives.Selector
    {
        private byte _bools;

        protected bool AutoTextChange { get => GetBit(0); private set => SetBit(0, value); }
        protected bool AutoSelectionChange { get => GetBit(1); private set => SetBit(1, value); }

        public static readonly DependencyProperty TextProperty = Register<string, ComboListBox>(nameof(Text), new PropertyMetadata(null, (DependencyObject d, DependencyPropertyChangedEventArgs e) => ((ComboListBox)d).OnTextChanged((string)e.OldValue)));

        public string Text { get => (string)GetValue(TextProperty); set => SetValue(TextProperty, value); }

        public static readonly DependencyProperty ForceUpdateProperty = Register<bool, ComboListBox>(nameof(ForceUpdate));

        public bool ForceUpdate { get => (bool)GetValue(ForceUpdateProperty); set => SetValue(ForceUpdateProperty, value); }

        static ComboListBox() => DefaultStyleKeyProperty.OverrideDefaultStyleKey<ComboListBox>();

        private bool GetBit(in byte pos) => _bools.GetBit(pos);
        private void SetBit(in byte pos, in bool value) => UtilHelpers.SetBit(ref _bools, pos, value);

        protected virtual void OnTextChanged(string oldValue)
        {
            if (AutoTextChange)

                return;

            string text = Text;

            void update(in object _item)
            {
                AutoSelectionChange = true;

                SelectedItem = _item;

                AutoSelectionChange = false;
            }

            if (TryGetValue(ItemContainerGenerator.Items, text?.ToLower(), true, out object
#if CS8
                ?
#endif
                item))

                update(item);

            else if (ForceUpdate)

                update(text);
        }

        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            base.OnSelectionChanged(e);

            if (AutoSelectionChange)

                return;

            AutoTextChange = true;

            Text = TryGetValue(SelectedItem, Text?.ToLower(), true);

            AutoTextChange = false;
        }
    }
}
