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

#region Usings
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
using WinCopies.IO.ComponentSources.Bitmap;
using WinCopies.IO.ComponentSources.Item;
using WinCopies.IO.ObjectModel;
using WinCopies.IO.Process;
using WinCopies.IO.PropertySystem;
using WinCopies.PropertySystem;
using WinCopies.Util.Commands.Primitives;
#endregion WinCopies
#endregion Usings

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
        void AddCommands(IEnumerable<MenuItemInfo> menuItem);

        void AddExtensionCommands(IEnumerable<KeyValuePair<ExtensionCommand, MenuItemInfo>> menuItems);

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

    public abstract class ConnectionParameterAbstract
    {
        public abstract string Description { get; }

        public abstract string Placeholder { get; }
    }

    public class ConnectionParameter : ConnectionParameterAbstract
    {
        public override string
#if CS8
            ?
#endif
            Description
        { get; }

        public override string
#if CS8
            ?
#endif
            Placeholder
        { get; }

        public ConnectionParameter(in string
#if CS8
            ?
#endif
            description, in string
#if CS8
            ?
#endif
            placeholder)
        {
            Description = description;
            Placeholder = placeholder;
        }

        public string Value { get; set; }
    }

    namespace ObjectModel
    {
        /// <summary>
        /// Provides interoperability for interacting with browsable items.
        /// </summary>
        public interface IBrowsableObjectInfo : IBrowsableObjectInfoBase, IRecursiveEnumerable<IBrowsableObjectInfo>, DotNetFix.IDisposable
        {
            #region Properties
            /// <summary>
            /// Gets the item sources for this <see cref="IBrowsableObjectInfo"/>.
            /// </summary>
            IItemSourcesProvider
#if CS8
                ?
#endif
            ItemSources
            { get; }

            /// <summary>
            /// Gets a value indicating whether this <see cref="IBrowsableObjectInfo"/> can be monitored. If this value is <see langword="true"/>, a callback can be passed to the current item so that it will call it when changes are made on its context, i.e. when a new item is added or when properties have been updated.
            /// </summary>
            /// <seealso cref="IsMonitoring"/>
            /// <seealso cref="StartMonitoring"/>
            /// <seealso cref="StopMonitoring"/>
            bool IsMonitoringSupported { get; }

            /// <summary>
            /// Gets a value indicating whether this <see cref=" IBrowsableObjectInfo"/> is currently notifying when changes are made on its context.
            /// </summary>
            /// <seealso cref="IsMonitoringSupported"/>
            /// <seealso cref="StartMonitoring"/>
            /// <seealso cref="StopMonitoring"/>
            bool IsMonitoring { get; }

            IBrowsabilityOptions Browsability { get; }

            IEnumerable<IBrowsabilityPath> BrowsabilityPaths { get; }

            /// <summary>
            /// Gets a value indicating whether this <see cref="IBrowsableObjectInfo"/> is the root of its context.
            /// </summary>
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
            IPropertySystemCollection<PropertyId, ShellPropertyGroup>
#if CS8
                ?
#endif
                ObjectPropertySystem
            { get; }

            /// <summary>
            /// Gets the <see cref="IBrowsableObjectInfo"/> parent of this one. Returns <see langword="null"/> if this object is the root object of a hierarchy.
            /// </summary>
            IBrowsableObjectInfo Parent { get; }

            IBitmapSourceProvider BitmapSourceProvider { get; }

            IBitmapSources
#if CS8
                ?
#endif
                BitmapSources
            { get; }

            /// <summary>
            /// Gets the name of this item's type. This value is shown to the user.
            /// </summary>
            string ItemTypeName { get; }

            /// <summary>
            /// Gets a description of this object. This value is shown to the user.
            /// </summary>
            string
#if CS8
                ?
#endif
                Description
            { get; }

            bool IsSpecialItem { get; }

            ClientVersion ClientVersion { get; }

            IBrowsableObjectInfoContextCommandEnumerable
#if CS8
                ?
#endif
                ContextCommands
            { get; }

            /// <summary>
            /// Gets a value indicating the protocol to use in order to reach this item when typing its URI or path, if supported, in the address bar. This value is used to identify the selector to use to convert the given URI or path to an <see cref="IBrowsableObjectInfo"/>.
            /// </summary>
            string
#if CS8
                ?
#endif
                Protocol
            { get; }

            /// <summary>
            /// Gets a value indicating the URI of the current item. See the Remarks section.
            /// </summary>
            /// <remarks>This is the identifier for the current item. For system paths, this value can be equal to <see cref="Path"/>. On other data structures, this value may be different, i.e. when an item is reached through a succession of steps (c is an item of b, itself an item of a), but has also a unique identifier like a GUID, for example.</remarks>
            string URI { get; }

            DisplayStyle DisplayStyle { get; }

            /// <summary>
            /// For items that need to connect to a server before being able to retrieve their own items, this property can be used to ask the user for the credentials and server parameters to use to open the connection. See the Remarks section.
            /// </summary>
            /// <remarks><para>This property's type is a read-only dictionary that takes the parameter name as keys and an instance of the <see cref="ConnectionParameter"/> class as values. The values typed by the user are stored in the <see cref="ConnectionParameter.Value"/> property of the value instances.</para>
            /// <para>This property is used by the engine before it retrieves the <see cref="ItemSources"/> property's value. So the method implementation of the <see cref="IItemSource"/> interface does not need to ask themselves the user for these parameters.</para></remarks>
            IReadOnlyDictionary<string, ConnectionParameter>
#if CS8
                ?
#endif
                ConnectionParameters
            { get; }
            #endregion Properties

            #region Methods
            /// <summary>
            /// Returns an <see cref="IContextMenu"/>. See the Remarks section.
            /// </summary>
            /// <param name="extendedVerbs">A value indicating whether to retrieve a full or partial context menu.</param>
            /// <returns>The <see cref="IContextMenu"/> that is retrieved by this method.</returns>
            /// <remarks><para>This method can be used to retrieve a native context menu, for which the commands cannot be managed directly on this layer.</para>
            /// <para>This method overload is used to retrieve the context menu specific to the current item. To retrieve the context menu for children items, call the <see cref="GetContextMenu(IEnumerable{IBrowsableObjectInfo}, bool)"/> overload.</para>
            /// </remarks>
            IContextMenu
#if CS8
                ?
#endif
                GetContextMenu(bool extendedVerbs);

            IContextMenu
#if CS8
                ?
#endif
                GetContextMenu(IEnumerable<IBrowsableObjectInfo> children, bool extendedVerbs);

            IBrowsableObjectInfoCallback RegisterCallback(Action<BrowsableObjectInfoCallbackArgs> callback);

            /// <summary>
            /// Starts monitoring the current item.
            /// </summary>
            /// <exception cref="InvalidOperationException"><see cref="IsMonitoringSupported"/> is set to <see langword="false"/>.</exception>
            void StartMonitoring();

            /// <summary>
            /// Stops monitoring the current item.
            /// </summary>
            /// <exception cref="InvalidOperationException"><see cref="IsMonitoringSupported"/> is set to <see langword="false"/>.</exception>
            void StopMonitoring();

            ArrayBuilder<IBrowsableObjectInfo>
#if CS8
            ?
#endif
            GetRootItems();

            IEnumerable<IBrowsableObjectInfo>
#if CS8
                ?
#endif
                GetSubRootItems();

            IEnumerable<ICommand>
#if CS8
                ?
#endif
                GetCommands(IEnumerable<IBrowsableObjectInfo>
#if CS8
                ?
#endif
                items);
            #endregion
        }

        public interface IBrowsableObjectInfo<out TObjectProperties> : IBrowsableObjectInfo
        {
            /// <summary>
            /// Generic version of <see cref="IBrowsableObjectInfo.ObjectProperties"/>.
            /// </summary>
            new TObjectProperties
#if CS9
                ?
#endif
                ObjectProperties
            { get; }
        }

        public interface IBrowsableObjectInfo3<out TParent> : IBrowsableObjectInfo where TParent : IBrowsableObjectInfo
        {
            /// <summary>
            /// Generic version of <see cref="IBrowsableObjectInfo.Parent"/>.
            /// </summary>
            new TParent
#if CS9
                ?
#endif
                Parent
            { get; }
        }

        public interface IBrowsableObjectInfo<out TParent, out TObjectProperties> : IBrowsableObjectInfo<TObjectProperties>, IBrowsableObjectInfo3<TParent> where TParent : IBrowsableObjectInfo
        {
            // Left empty.
        }

        public interface IEncapsulatorBrowsableObjectInfo<out T> : IBrowsableObjectInfo
        {
            /// <summary>
            /// Generic version of <see cref="IBrowsableObjectInfo.InnerObject"/>.
            /// </summary>
            new T InnerObject { get; }
        }

        public interface IBrowsableObjectInfo2<TPredicateTypeParameter> : IBrowsableObjectInfo
        {
            /// <summary>
            /// Generic version of <see cref="IBrowsableObjectInfo.ItemSources"/>.
            /// </summary>
            new IItemSourcesProvider<TPredicateTypeParameter>
#if CS8
            ?
#endif
            ItemSources
            { get; }
        }

        public interface IBrowsableObjectInfo<TPredicateTypeParameter, out TSelectorDictionary, out TDictionaryItems> : IBrowsableObjectInfo2<TPredicateTypeParameter> where TSelectorDictionary : IEnumerableSelectorDictionary<TDictionaryItems, IBrowsableObjectInfo>
        {
            TSelectorDictionary GetSelectorDictionary();
        }

        public interface IBrowsableObjectInfo<out TParent, out TObjectProperties, out TInnerObject, TPredicateTypeParameter, out TSelectorDictionary, out TDictionaryItems> : IBrowsableObjectInfo<TParent, TObjectProperties>, IEncapsulatorBrowsableObjectInfo<TInnerObject>, IBrowsableObjectInfo<TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> where TParent : IBrowsableObjectInfo where TSelectorDictionary : IEnumerableSelectorDictionary<TDictionaryItems, IBrowsableObjectInfo>
        {
            // Left empty.
        }

        public interface IBrowsableObjectInfo<out TObjectProperties, out TInnerObject, TPredicateTypeParameter, out TSelectorDictionary, out TDictionaryItems> : IBrowsableObjectInfo<IBrowsableObjectInfo, TObjectProperties, TInnerObject, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> where TSelectorDictionary : IEnumerableSelectorDictionary<TDictionaryItems, IBrowsableObjectInfo>
        {
            // Left empty.
        }
    }
}
