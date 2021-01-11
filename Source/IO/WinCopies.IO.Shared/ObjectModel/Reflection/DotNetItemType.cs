/* Copyright © Pierre Sprimont, 2020
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

namespace WinCopies.IO.Reflection
{
    public enum AccessModifier : byte
    {
        Public = 1,

        Protected = 2,

        ProtectedInternal = 3,

        Internal = 4,

        PrivateProtected = 5,

        Private = 6
    }

    public enum DotNetItemType : byte
    {
        /// <summary>
        /// The item is a .Net namespace.
        /// </summary>
        Namespace = 1,

        /// <summary>
        /// The item is a .Net struct.
        /// </summary>
        Struct = 2,

        /// <summary>
        /// The item is a .Net enum.
        /// </summary>
        Enum = 3,

        /// <summary>
        /// The item is a .Net class.
        /// </summary>
        Class = 4,

        /// <summary>
        /// The item is a .Net interface.
        /// </summary>
        Interface = 5,

        /// <summary>
        /// The item is a .Net delegate.
        /// </summary>
        Delegate = 6,

        /// <summary>
        /// The item is a .Net attribute.
        /// </summary>
        Attribute = 7,

        /// <summary>
        /// The item is a .Net field.
        /// </summary>
        Field = 8,

        /// <summary>
        /// The item is a .Net property.
        /// </summary>
        Property = 9,

        /// <summary>
        /// The item is a .Net event.
        /// </summary>
        Event = 10,

        /// <summary>
        /// The item is a .Net constructor.
        /// </summary>
        Constructor = 11,

        /// <summary>
        /// The item is a .Net method.
        /// </summary>
        Method = 12,

        /// <summary>
        /// The item is a .Net implemented interfaces.
        /// </summary>
        ImplementedInterface = 13,

        /// <summary>
        /// The item is a .Net parameter.
        /// </summary>
        Parameter = 14,

        /// <summary>
        /// The item is a .Net generic parameter.
        /// </summary>
        GenericParameter = 15,

        /// <summary>
        /// The item is a .Net generic argument.
        /// </summary>
        GenericArgument = 16,

        ReturnParameter=17
    }
}
