/* Copyright © Pierre Sprimont, 2021
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

using System;

using WinCopies.Collections.DotNetFix.Generic;

namespace WinCopies.IO
{
    public class BrowsableObjectInfoPluginStack : BrowsableObjectInfoEnumerableStack<Action>
    {
        private Action _runActions = EmptyVoid;
        private Action<Action> _pushAction;

        public BrowsableObjectInfoPluginStack() : base(new EnumerableStack<Action>()) => InitializePushAction();

        private void _Push(Action action) => Stack.Push(action);

        private void InitializePushAction() => _pushAction = action =>
        {
            InitializeRunAction();

            _Push(action);

            _pushAction = _Push;
        };

        private void InitializeRunAction() => _runActions = () =>
        {
            foreach (Action action in Stack)

                action();

            _runActions = EmptyVoid;

            Stack.Clear();

            InitializePushAction();
        };

        protected static void EmptyVoid() { /* Left empty. */ }

        public void Push(Action obj) => _pushAction(obj);

        protected internal void RunActions() => _runActions();
    }
}
