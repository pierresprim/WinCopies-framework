using System.Windows.Input;

namespace WinCopies.GUI.Controls
{
    public class ListViewItem : System.Windows.Controls.ListViewItem
    {
        public MouseButtons ListenedButtons { get => ExtraProperties.GetListenedButtons(this); set => ExtraProperties.SetListenedButtons(this, value); }

        public event ClickEventHandler Click { add => ExtraEvents.AddClickHandler(this, value); remove => ExtraEvents.RemoveClickHandler(this, value); }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);

            using
#if !CS8
                (
#endif
                IValueObject<bool> isSelectedProperty = new PropertyValueObject<bool>(GetType().GetProperty(nameof(IsSelected)), this)
#if CS8
                ;
#else
                )
#endif

            Helpers.OnMouseDown(this, e, ListenedButtons, isSelectedProperty);
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);

            Helpers.OnMouseUp(this, e, ListenedButtons);
        }
    }
}
