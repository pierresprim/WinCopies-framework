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

using WinCopies.Collections.DotNetFix.Generic;

namespace WinCopies.IO
{
    public interface IBrowsableObjectInfoStack<T>
    {
        void Push(T obj);
    }

    public abstract class BrowsableObjectInfoStackBase<TItems, TStack> where TStack : IStack<TItems>
    {
        protected TStack Stack { get; set; }

        protected BrowsableObjectInfoStackBase(in TStack stack) => Stack = stack
#if CS8
        ??
#else
== null ?
#endif
            throw ThrowHelper.GetArgumentNullException(nameof(stack))
#if !CS8
: stack
#endif
            ;
    }

    public abstract class BrowsableObjectInfoStack<TItems, TStack> : BrowsableObjectInfoStackBase<TItems, TStack>, IBrowsableObjectInfoStack<TItems> where TStack : IStack<TItems>
    {
        protected BrowsableObjectInfoStack(in TStack stack) : base(stack) { }

        public void Push(TItems obj) => Stack.Push(obj);
    }

    public abstract class BrowsableObjectInfoEnumerableStack<T> : BrowsableObjectInfoStack<T, IEnumerableStack<T>>
    {
        protected BrowsableObjectInfoEnumerableStack(in IEnumerableStack<T> stack) : base(stack) { }
    }
}
