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

#region WAPICP
using Microsoft.WindowsAPICodePack;
using Microsoft.WindowsAPICodePack.Win32Native.Menus;
#endregion WAPICP

#region System
using System;
using System.Collections.Generic;
using System.Drawing;
#endregion System

#region WinCopies
using WinCopies.Collections.Generic;
using WinCopies.IO.ObjectModel;
using WinCopies.IO.Process;
using WinCopies.IO.PropertySystem;
using WinCopies.PropertySystem;
using WinCopies.Util.Commands.Primitives;
#endregion WinCopies

namespace WinCopies.IO
{
    public interface IBrowsableObjectInfoContextCommandEnumerable : System.Collections.Generic.IEnumerable<ICommand>
    {
        IBrowsableObjectInfo BrowsableObjectInfo { get; }
    }

    public enum ContextMenuCommand : sbyte
    {
        None = 0,

        NewFolder = 1,

        Rename,

        Delete,

        LastDelegatedCommand = Delete,

        Open,

        OpenInNewTab,

        OpenInNewWindow,

        CopyPath,

        CopyName
    }

    public interface IContextMenu : System.IDisposable
    {
        void AddCommands(System.Collections.Generic.IEnumerable<MenuItemInfo> menuItem);

        void AddExtensionCommands(System.Collections.Generic.IEnumerable<KeyValuePair<ExtensionCommand, MenuItemInfo>> menuItems);

        string
#if CS8
            ?
#endif
            GetCommandTooltip(ref uint? command);

        ContextMenuCommand Show(IntPtr hwnd, HookRegistration hookRegistration, Point point, bool ctrl = false, bool shift = false);

        void Open(IBrowsableObjectInfo[] items, Point point, bool ctrl = false, bool shift = false);
    }

    public enum DisplayStyle
    {
        Size1 = 1,

        Size2,

        Size3,

        Size4,

        List,

        Details,

        Tiles,

        Content
    }

    namespace ObjectModel
    {
        /// <summary>
        /// Provides interoperability for interacting with browsable items.
        /// </summary>
        public interface IBrowsableObjectInfo : IBrowsableObjectInfoBase, IRecursiveEnumerable<IBrowsableObjectInfo>, DotNetFix.IDisposable
        {
            bool IsMonitoringSupported { get; }

            bool IsMonitoring { get; }

            IBrowsabilityOptions Browsability { get; }

            System.Collections.Generic.IEnumerable<IBrowsabilityPath> BrowsabilityPaths { get; }

            IProcessFactory ProcessFactory { get; }

            System.Collections.Generic.IEnumerable<IProcessInfo> CustomProcesses { get; }

            bool IsLocalRoot { get; }

            /// <summary>
            /// Gets a value indicating whether this <see cref="IBrowsableObjectInfo"/> is recursively browsable.
            /// </summary>
            bool IsRecursivelyBrowsable { get; }

            /// <summary>
            /// Gets the underlying object of this abstraction interface.
            /// </summary>
            object InnerObject { get; }

            /// <summary>
            /// Gets the common properties of <see cref="InnerObject"/>. These properties may be specific to the underlying object type.
            /// </summary>
            object ObjectProperties { get; }

            /// <summary>
            /// Gets the specific properties of <see cref="InnerObject"/>. These properties are specific to this object.
            /// </summary>
            IPropertySystemCollection<PropertyId, ShellPropertyGroup> ObjectPropertySystem { get; }

            /// <summary>
            /// Gets the <see cref="IBrowsableObjectInfo"/> parent of this <see cref="IBrowsableObjectInfo"/>. Returns <see langword="null"/> if this object is the root object of a hierarchy.
            /// </summary>
            IBrowsableObjectInfo Parent { get; }

            IBitmapSourceProvider BitmapSourceProvider { get; }

            IBitmapSources BitmapSources { get; }

            string ItemTypeName { get; }

            string Description { get; }

            bool IsSpecialItem { get; }

            ClientVersion ClientVersion { get; }

            IBrowsableObjectInfoContextCommandEnumerable ContextCommands { get; }

            string Protocol { get; }

            string URI { get; }

            DisplayStyle DisplayStyle { get; }

            IContextMenu GetContextMenu(bool extendedVerbs);

            IContextMenu GetContextMenu(System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> children, bool extendedVerbs);

            IBrowsableObjectInfoCallback RegisterCallback(Action<BrowsableObjectInfoCallbackArgs> callback);

            void StartMonitoring();

            void StopMonitoring();

            System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> GetItems();

            ArrayBuilder<IBrowsableObjectInfo> GetRootItems();

            System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> GetSubRootItems();

            System.Collections.Generic.IEnumerable<ICommand> GetCommands(System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> items);
        }

        public interface IBrowsableObjectInfo<out T> : IBrowsableObjectInfo
        {
            T ObjectProperties { get; }
        }

        public interface IEncapsulatorBrowsableObjectInfo<out
#if WinCopies3
            T
#else
            TInnerObject
#endif
            > : IBrowsableObjectInfo
        {
#if WinCopies3
            T
#else
TInnerObject
#endif
                InnerObject
            { get; }
        }

        public interface IBrowsableObjectInfo<out TObjectProperties, out TInnerObject, out TPredicateTypeParameter, out TSelectorDictionary, out TDictionaryItems> : IBrowsableObjectInfo<TObjectProperties>, IEncapsulatorBrowsableObjectInfo<TInnerObject> where TSelectorDictionary : IEnumerableSelectorDictionary<TDictionaryItems, IBrowsableObjectInfo>
        {
            System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> GetItems(Predicate<TPredicateTypeParameter> predicate);

            TSelectorDictionary GetSelectorDictionary();
        }
    }
}
