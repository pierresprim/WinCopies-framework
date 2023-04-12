using System;
using System.Collections.Generic;
using System.Text;

namespace WinCopies.Installer.Online
{
    public abstract class ProcessWindow : GUI.IO.Process.ProcessWindow
    {
        protected override bool ValidateClosing() => true;
    }
}
