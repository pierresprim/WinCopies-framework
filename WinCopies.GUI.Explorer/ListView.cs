/* Copyright © Pierre Sprimont, 2019
 *
 * This file is part of the WinCopies Framework.
 *
 * The WinCopies Framework is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * The WinCopies Framework is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with the WinCopies Framework.  If not, see <https://www.gnu.org/licenses/>. */

using System.Windows;
using System.Windows.Input;
using WinCopies.Util.Commands;

namespace WinCopies.GUI.Explorer
{
    public class ListView : Controls.ListView
    {



        public ExplorerControl ParentExplorerControl { get; internal set; } = null;

        //public override void OnApplyTemplate()

        //{

        //    base.OnApplyTemplate();



        //}    

        public ListView()

        {

            // InputBindings.Add(new InputBinding(Commands.OpenOnLeftClick, new MouseGesture(MouseAction.LeftClick)));

        }

        protected override DependencyObject GetContainerForItemOverride()
        {

            ListViewItem listViewItem = new ListViewItem(ParentExplorerControl);

            bool alreadyFoundCommandBinding = false;

            //foreach (object commandBinding in listViewItem.CommandBindings)

            //    if (((CommandBinding)commandBinding).Command == Commands.Open)

            //    {

            //        alreadyFoundCommandBinding = true;

            //        break;

            //    }

            //if (!alreadyFoundCommandBinding)

            foreach (object inputBinding in listViewItem.InputBindings)

                if (((InputBinding)inputBinding).Command == Commands.Open)

                {

                    alreadyFoundCommandBinding = true;

                    break;

                }

            // listViewItem.CommandBindings.Add(new CommandBinding(Commands.Open, ParentExplorerControl.Open_Executed, ParentExplorerControl.Open_CanExecute));

            if (!alreadyFoundCommandBinding)

                if (ParentExplorerControl.OpenMode == OpenMode.OnFirstClick)

                    listViewItem.InputBindings.Add(new InputBinding(Commands.Open, new MouseGesture(MouseAction.LeftClick)));

                else if (ParentExplorerControl.OpenMode == OpenMode.OnDoubleClick)

                    listViewItem.InputBindings.Add(new InputBinding(Commands.Open, new MouseGesture(MouseAction.LeftDoubleClick)));

            return listViewItem;

        }

        //internal void TryRaiseCommandsByKeyDown(KeyEventArgs e)
        //{

        //    ICommand[] commands = new ICommand[] { ApplicationCommands.Copy, ApplicationCommands.Cut, ApplicationCommands.Paste, FileSystemCommands.Rename, ApplicationCommands.Delete, FileSystemCommands.DeletePermanently };

        //    foreach (ICommand command in commands)

        //        if (KeyCommandHelper.CanRaiseCommand(command, ActionsFromObjects.ListView, ParentExplorerControl, e))

        //            command.Execute(ActionsFromObjects.ListView);

        //}

    }
}
