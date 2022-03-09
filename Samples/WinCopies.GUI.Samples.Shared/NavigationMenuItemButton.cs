using System;
using System.Collections;
using System.Windows.Controls;
using System.Windows.Input;
using WinCopies.Collections.Generic;
using WinCopies.Commands;
using WinCopies.Desktop;

namespace WinCopies.GUI.Samples
{
    public abstract class ButtonCommand<T> : ICommand<T>
    {
        protected Button Button { get; }

        public event EventHandler CanExecuteChanged { add => CommandManager.RequerySuggested += value; remove => CommandManager.RequerySuggested -= value; }

        public ButtonCommand(in Button button) => Button = button;

        public abstract bool CanExecute(T parameter);

        public abstract void Execute(T parameter);
    }

    public class PreviousCommand : ButtonCommand<INavigableMenuItem>
    {
        public PreviousCommand(in Button button) : base(button) { }

        public override bool CanExecute(INavigableMenuItem parameter) => parameter.Parent != null;

        public override void Execute(INavigableMenuItem parameter) => Button.GetParent<NavigationMenu>(false).Items = parameter.Parent;
    }

    public class NextCommand : ButtonCommand<IList>
    {
        public NextCommand(in Button button) : base(button) { }

        public override bool CanExecute(IList parameter) => parameter.Count > 0;

        public override void Execute(IList parameter) => Button.GetParent<NavigationMenu>(false).Items = parameter;
    }

    public enum NavigationMenuItemButtonDirection
    {
        Previous,

        Next
    }

    public class NavigationMenuItemButton : Button
    {
        public NavigationMenuItemButton(in NavigationMenuItemButtonDirection direction)
        {
            switch (direction)
            {
                case NavigationMenuItemButtonDirection.Previous:

                    Content = '<';

                    Command = new PreviousCommand(this);

                    break;

                case NavigationMenuItemButtonDirection.Next:

                    Content = '>';

                    Command = new NextCommand(this);

                    break;
            }
        }
    }

    public class NavigationPreviousMenuItemButton : NavigationMenuItemButton
    {
        public NavigationPreviousMenuItemButton() : base(NavigationMenuItemButtonDirection.Previous) { }
    }

    public class NavigationNextMenuItemButton : NavigationMenuItemButton
    {
        public NavigationNextMenuItemButton() : base(NavigationMenuItemButtonDirection.Next) { }
    }
}
