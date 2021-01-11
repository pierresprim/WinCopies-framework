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

namespace WinCopies.IO.Reflection.PropertySystem
{
    public interface IDotNetItemInfoProperties
    {
        DotNetItemType ItemType { get; }
    }

    public interface IDotNetTypeOrMemberInfoProperties: IDotNetItemInfoProperties
    {
        abstract Type? DeclaringType { get; }
    }

    public interface IDotNetTypeOrMethodItemInfoProperties: IDotNetTypeOrMemberInfoProperties
    {
        bool IsAbstract { get; }

        bool IsSealed { get; }

        IReadOnlyList<Type> GenericTypeParameters { get; }

        IReadOnlyList<Type> GenericTypeArguments { get; }
    }

    public interface IDotNetTypeInfoProperties : IDotNetTypeOrMethodItemInfoProperties
    {
        AccessModifier AccessModifier { get; }

        bool? IsRootType { get; }
    }

    public interface IDotNetPropertyInfoProperties:IDotNetTypeOrMethodItemInfoProperties
    {
        Type PropertyType { get; }
    }
}
