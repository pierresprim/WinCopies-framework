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

using WinCopies.Collections.DotNetFix;
using WinCopies.Collections.DotNetFix.Generic;

using static WinCopies.Temp;

namespace WinCopies.IO.Process.ObjectModel
{
    public static partial class ProcessObjectModelTypes<TItems, TFactory, TError, TProcessDelegates, TProcessDelegateParam>
    {
        public abstract partial class Process : IProcess<TItems, TError>, WinCopies.DotNetFix.IDisposable
        {
            public class Parameter<T1, T2> : DelegateValueProvider<T1, T2> where T2 : ISimpleLinkedListBase
            {
                public Parameter(in T1 value, in Func<T1, T2> func) : base(value, func)
                {
                    // Left empty.
                }

                protected override void ValidateResultValue(in T2 value)
                {
                    if (value == null)

                        throw new ArgumentException("The given func must return a not null queue.");

                    if (!value.IsReadOnly)

                        throw new ArgumentException("The given func must return a read-only queue.");
                }
            }

            public class QueueParameter : Parameter<ProcessTypes<TItems>.ProcessCollection, ProcessTypes<TItems>.ProcessCollection>
            {
                public QueueParameter(in ProcessTypes<TItems>.ProcessCollection value, in Func<ProcessTypes<TItems>.ProcessCollection, ProcessTypes<TItems>.ProcessCollection> func) : base(value, func)
                {
                    // Left empty.
                }

                protected override void ValidateValue(in ProcessTypes<TItems>.ProcessCollection value) => ThrowIfNullOrReadOnly(value, nameof(value));
            }

            public class LinkedListParameter : Parameter<IEnumerableInfoLinkedList<IProcessErrorItem<TItems, TError>>, IQueue<IProcessErrorItem<TItems, TError>>>
            {
                public LinkedListParameter(in IEnumerableInfoLinkedList<IProcessErrorItem<TItems, TError>> value, in Func<IEnumerableInfoLinkedList<IProcessErrorItem<TItems, TError>>, IQueue<IProcessErrorItem<TItems, TError>>> func) : base(value, func)
                {
                    // Left empty.
                }

                protected override void ValidateValue(in IEnumerableInfoLinkedList<IProcessErrorItem<TItems, TError>> value) => ThrowIfNullOrReadOnly(value, nameof(value));
            }
        }
    }
}
