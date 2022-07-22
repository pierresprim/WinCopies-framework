using System.Windows;
using System.Windows.Documents;

namespace WinCopies.GUI.Controls
{
    public class RichTextBox : System.Windows.Controls.RichTextBox
    {
        public static readonly DependencyProperty DocumentProperty = Util.Desktop.UtilHelpers.Register<FlowDocument, RichTextBox>(nameof(Document), new PropertyMetadata(null, new PropertyChangedCallback((DependencyObject d, DependencyPropertyChangedEventArgs e) => ((System.Windows.Controls.RichTextBox)d).Document = (FlowDocument)e.NewValue ?? new FlowDocument())));

        public new FlowDocument Document { get => (FlowDocument)GetValue(DocumentProperty); set => SetValue(DocumentProperty, value); }
    }
}
