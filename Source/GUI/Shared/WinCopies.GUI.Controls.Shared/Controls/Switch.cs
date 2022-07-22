using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

using WinCopies.Commands;
using WinCopies.Desktop;
using WinCopies.Util.Desktop;

using static WinCopies.Util.Desktop.UtilHelpers;

namespace WinCopies.GUI
{
    public enum SwitchStatus : byte
    {
        None = 0,

        Unchecked = 1,

        Checking = 2,

        Checked = 3,

        Unchecking = 4
    }

    public enum SwitcherShape : byte
    {
        Circle = 1,

        Ellipse = 2
    }

    namespace Controls
    {
        public interface ISwitch
        {
            SwitchStatus Status { get; }

            SwitcherShape SwitcherShape { get; set; }

            bool IsIndeterminateAState { get; set; }
        }

        public class SwitchBase : Control, ISwitch
        {
            private static DependencyProperty Register<T>(in string propertyName) => Register<T, SwitchBase>(propertyName);

            /// <summary>
            /// Identifies the <see cref="Status"/> dependency property.
            /// </summary>
            public static readonly DependencyProperty StatusProperty = Register<SwitchStatus>(nameof(Status));

            public SwitchStatus Status { get => (SwitchStatus)GetValue(StatusProperty); set => SetValue(StatusProperty, value); }

            /// <summary>
            /// Identifies the <see cref="SwitcherShape"/> dependency property.
            /// </summary>
            public static readonly DependencyProperty SwitcherShapeProperty = Register<SwitcherShape, SwitchBase>(nameof(SwitcherShape), new PropertyMetadata(SwitcherShape.Circle));

            public SwitcherShape SwitcherShape { get => (SwitcherShape)GetValue(SwitcherShapeProperty); set => SetValue(SwitcherShapeProperty, value); }

            /// <summary>
            /// Identifies the <see cref="IsHighlighted"/> dependency property.
            /// </summary>
            public static readonly DependencyProperty IsHighlightedProperty = Register<bool>(nameof(IsHighlighted));

            public bool IsHighlighted { get => (bool)GetValue(IsHighlightedProperty); set => SetValue(IsHighlightedProperty, value); }

            /// <summary>
            /// Identifies the <see cref="IsPressed"/> dependency property.
            /// </summary>
            public static readonly DependencyProperty IsPressedProperty = Register<bool, SwitchBase>(nameof(IsPressed), new PropertyMetadata(false, (DependencyObject d, DependencyPropertyChangedEventArgs e)=>
            {

            })
            );

            public bool IsPressed { get => (bool)GetValue(IsPressedProperty); set => SetValue(IsPressedProperty, value); }

            /// <summary>
            /// Identifies the <see cref="IsIndeterminateAState"/> dependency property.
            /// </summary>
            public static readonly DependencyProperty IsIndeterminateAStateProperty = Register<bool>(nameof(IsIndeterminateAState));

            public bool IsIndeterminateAState { get => (bool)GetValue(IsIndeterminateAStateProperty); set => SetValue(IsIndeterminateAStateProperty, value); }

            /// <summary>
            /// Identifies the <see cref="CornerRadius"/> dependency property.
            /// </summary>
            public static readonly DependencyProperty CornerRadiusProperty = Register<CornerRadius>(nameof(CornerRadius));

            public CornerRadius CornerRadius { get => (CornerRadius)GetValue(CornerRadiusProperty); set => SetValue(CornerRadiusProperty, value); }

            static SwitchBase() => DefaultStyleKeyProperty.OverrideDefaultStyleKey<SwitchBase>();
        }

        public class Switch : ToggleButton, IQueryCommandSource<bool>, ISwitch
        {
            private static DependencyProperty Register<T>(in string propertyName) => Register<T, Switch>(propertyName);

            private static readonly DependencyPropertyKey StatusPropertyKey = RegisterReadOnly<SwitchStatus, Switch>(nameof(Status), new PropertyMetadata(SwitchStatus.Unchecked));

            /// <summary>
            /// Identifies the <see cref="Status"/> dependency property.
            /// </summary>
            public static readonly DependencyProperty StatusProperty = StatusPropertyKey.DependencyProperty;

            public SwitchStatus Status { get => (SwitchStatus)GetValue(StatusProperty); private set => SetValue(StatusPropertyKey, value); }

            /// <summary>
            /// Identifies the <see cref="SwitcherShape"/> dependency property.
            /// </summary>
            public static readonly DependencyProperty SwitcherShapeProperty = Register<SwitcherShape, Switch>(nameof(SwitcherShape), new PropertyMetadata(SwitcherShape.Circle));

            public SwitcherShape SwitcherShape { get => (SwitcherShape)GetValue(SwitcherShapeProperty); set => SetValue(SwitcherShapeProperty, value); }

            /// <summary>
            /// Identifies the <see cref="IsHighlighted"/> dependency property.
            /// </summary>
            public static readonly DependencyProperty IsHighlightedProperty = Register<bool>(nameof(IsHighlighted));

            public bool IsHighlighted { get => (bool)GetValue(IsHighlightedProperty); set => SetValue(IsHighlightedProperty, value); }

            /// <summary>
            /// Identifies the <see cref="IsIndeterminateAState"/> dependency property.
            /// </summary>
            public static readonly DependencyProperty IsIndeterminateAStateProperty = Register<bool>(nameof(IsIndeterminateAState));

            public bool IsIndeterminateAState { get => (bool)GetValue(IsIndeterminateAStateProperty); set => SetValue(IsIndeterminateAStateProperty, value); }

            /// <summary>
            /// Identifies the <see cref="CornerRadius"/> dependency property.
            /// </summary>
            public static readonly DependencyProperty CornerRadiusProperty = Register<CornerRadius>(nameof(CornerRadius));

            public CornerRadius CornerRadius { get => (CornerRadius)GetValue(CornerRadiusProperty); set => SetValue(CornerRadiusProperty, value); }

            /// <summary>
            /// Identifies the <see cref="Command"/> dependency property.
            /// </summary>
            public static new readonly DependencyProperty CommandProperty = Register<IQueryCommand<bool>>(nameof(Command));

            public new IQueryCommand<bool> Command { get => (IQueryCommand<bool>)GetValue(CommandProperty); set => SetValue(CommandProperty, value); }

            IQueryCommand<bool> IQueryCommandSource<bool>.Command => Command;

            static Switch() => DefaultStyleKeyProperty.OverrideDefaultStyleKey<Switch>();

            protected override void OnIndeterminate(RoutedEventArgs e)
            {
                base.OnIndeterminate(e);

                Status = SwitchStatus.None;
            }

            protected override void OnChecked(RoutedEventArgs e)
            {
                base.OnChecked(e);

                Status = SwitchStatus.Checked;
            }

            protected override void OnUnchecked(RoutedEventArgs e)
            {
                base.OnChecked(e);

                Status = SwitchStatus.Unchecked;
            }

            protected override void OnClick()
            {
                void action() => RaiseEvent(new RoutedEventArgs(ClickEvent, this));

                if (IsThreeState && !IsIndeterminateAState)
                {
                    action();

                    return;
                }

                bool? isChecked = IsChecked;

                if (!IsThreeState || !(isChecked.HasValue && isChecked.Value))

                    switch (Status) // for this code to take effect, the command should be able to execute in background an notify when the process has completed.
                    {
                        case SwitchStatus.Unchecked:

                            Status = SwitchStatus.Checking;

                            break;

                        case SwitchStatus.Checked:

                            Status = SwitchStatus.Unchecking;

                            break;

                        case SwitchStatus.None:

                            Status = SwitchStatus.Unchecking;

                            break;
                    }

                action();

                IQueryCommand<bool> command = Command;

                if (command == null || (command.TryExecute(this, out bool result) && result))

                    IsChecked = isChecked.HasValue ? isChecked.Value ? (IsThreeState ? null : new bool?(false)) : new bool?(true) : false;

                else

                    switch (Status)
                    {
                        case SwitchStatus.Checking:

                            IsChecked = false;

                            break;

                        case SwitchStatus.Unchecking:

                            IsChecked = true;

                            break;
                    }
            }
        }
    }
}
