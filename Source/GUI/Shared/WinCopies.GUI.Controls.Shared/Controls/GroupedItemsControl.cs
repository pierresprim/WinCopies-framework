using System.Windows;
using System.Windows.Controls;

using WinCopies.Desktop;
using WinCopies.Util;

using static System.Windows.RoutingStrategy;

using static WinCopies.Util.Desktop.UtilHelpers;

using RoutedEventHandler = WinCopies.DotNetFix.RoutedEventHandler<WinCopies.DotNetFix.RoutedEventArgs<bool?>>;
using RoutedEventArgs = WinCopies.DotNetFix.RoutedEventArgs<bool?>;

namespace WinCopies.GUI.Controls
{
    public class GroupedItemsControl : HeaderedItemsControl
    {
        public static readonly DependencyProperty AreItemsEnabledProperty = Register<bool, GroupedItemsControl>(nameof(AreItemsEnabled), new PropertyMetadata(true));

        public bool AreItemsEnabled { get => (bool)GetValue(AreItemsEnabledProperty); set => SetValue(AreItemsEnabledProperty, value); }

        static GroupedItemsControl() => DefaultStyleKeyProperty.OverrideDefaultStyleKey<GroupedItemsControl>();
    }

    public class CheckableGroupedItemsControl : GroupedItemsControl
    {
        private static RoutedEvent Register(in string propertyName, in RoutingStrategy routingStrategy) => Register<RoutedEventHandler, CheckableGroupedItemsControl>(propertyName, routingStrategy);



        public static readonly DependencyProperty IsCheckedProperty = Register<bool?, GroupedItemsControl>(nameof(IsChecked), new FrameworkPropertyMetadata(false, (DependencyObject d, DependencyPropertyChangedEventArgs e) => ((CheckableGroupedItemsControl)d).OnCheckStatusChanged((bool?)e.NewValue, (bool?)e.NewValue)));

        public bool? IsChecked { get => (bool?)GetValue(IsCheckedProperty); set => SetValue(IsCheckedProperty, value); }



        public static readonly RoutedEvent CheckStatusChangedEvent = Register(nameof(CheckStatusChanged), Bubble);

        public event RoutedEventHandler CheckStatusChanged { add => AddHandler(CheckStatusChangedEvent, value); remove => RemoveHandler(CheckStatusChangedEvent, value); }

        public static readonly RoutedEvent PreviewCheckStatusChangedEvent = Register(nameof(PreviewCheckStatusChanged), Tunnel);

        public event RoutedEventHandler PreviewCheckStatusChanged { add => AddHandler(PreviewCheckStatusChangedEvent, value); remove => RemoveHandler(PreviewCheckStatusChangedEvent, value); }



        static CheckableGroupedItemsControl() => DefaultStyleKeyProperty.OverrideDefaultStyleKey<CheckableGroupedItemsControl>();



        private void RaiseEvent(in bool? oldValue, in bool? newValue, in RoutedEvent routedEvent) => RaiseEvent(new RoutedEventArgs(oldValue, newValue, routedEvent, this));

        protected virtual void OnCheckStatusChanged(bool? oldValue, bool? newValue)
        {
            void raiseEvent(in RoutedEvent routedEvent) => RaiseEvent(oldValue, newValue, routedEvent);

            raiseEvent(PreviewCheckStatusChangedEvent);
            raiseEvent(CheckStatusChangedEvent);
        }
    }

    public class GroupedCheckBoxesControl : CheckableGroupedItemsControl
    {
        private byte _bools = 0b11;

        private bool ProcessItemEvents { get => GetBit(0); set => SetBit(0, value); }
        internal bool ProcessSubItemEvents { get => GetBit(1); private set => SetBit(1, value); }



        private bool GetBit(in byte pos) => _bools.GetBit(pos);
        private void SetBit(in byte pos, in bool value) => UtilHelpers.SetBit(ref _bools, pos, value);



        protected override void OnCheckStatusChanged(bool? oldValue, bool? newValue)
        {
            ProcessItemEvents = false;

            base.OnCheckStatusChanged(oldValue, newValue);

            ProcessItemEvents = true;
        }

        protected override DependencyObject GetContainerForItemOverride() => new GroupedCheckBox(this);

        private bool? OnItemCheckStatusChanged(in bool value)
        {
            if (ProcessItemEvents)
            {
                void update(in bool? _value)
                {
                    ProcessSubItemEvents = false;

                    IsChecked = _value;

                    ProcessSubItemEvents = true;
                }

                ItemContainerGenerator itemContainerGenerator = ItemContainerGenerator;
                int count = itemContainerGenerator.Items.Count;

                for (int i = 0; i < count; i++)

                    if (itemContainerGenerator.ContainerFromIndex(i) is CheckBox checkBox && checkBox.IsChecked != value)
                    {
                        update(null);

                        return false;
                    }

                update(value);

                return true;
            }

            return null;
        }

        protected internal virtual bool? OnItemChecked() => OnItemCheckStatusChanged(true);

        protected internal virtual bool? OnItemUnchecked() => OnItemCheckStatusChanged(false);
    }

    public class GroupedCheckBox : CheckBox
    {
        public GroupedCheckBoxesControl GroupBox { get; }

        public GroupedCheckBox(in GroupedCheckBoxesControl groupBox) => (GroupBox = groupBox).PreviewCheckStatusChanged += (object sender, RoutedEventArgs e) => OnGroupBoxCheckStatusChanged(e);

        private void OnGroupBoxCheckStatusChanged(in bool value)
        {
            if (GroupBox.ProcessSubItemEvents)

                IsChecked = value;
        }

        protected virtual void OnGroupBoxCheckStatusChanged(RoutedEventArgs e) => UtilHelpers.PerformActionIn(e.NewValue, OnGroupBoxCheckStatusChanged);

        protected override void OnChecked(System.Windows.RoutedEventArgs e)
        {
            base.OnChecked(e);

            _ = GroupBox.OnItemChecked();
        }

        protected override void OnUnchecked(System.Windows.RoutedEventArgs e)
        {
            base.OnUnchecked(e);

            _ = GroupBox.OnItemUnchecked();
        }
    }

    public class GroupedRadioButtonsControl : CheckableGroupedItemsControl
    {
        static GroupedRadioButtonsControl() => DefaultStyleKeyProperty.OverrideDefaultStyleKey<GroupedRadioButtonsControl>();

        protected override DependencyObject GetContainerForItemOverride() => new RadioButton();
    }
}
