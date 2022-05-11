using System.Windows;

using WinCopies.GUI.Controls;

namespace WinCopies.GUI.IO.Controls
{
    public class ExplorerControlTabControl : TabControl
    {
        protected override DependencyObject GetContainerForItemOverride() => new TabItem();
    }
}
