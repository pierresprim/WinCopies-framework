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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using WinCopies.Collections;
using WinCopies.GUI.Controls.Models;
using WinCopies.GUI.Windows.Dialogs;
using WinCopies.Util;
using WinCopies.Util.Data;
using static WinCopies.Util.Util;

namespace WinCopies.GUI.Windows.Dialogs.Models
{

    /// <summary>
    /// Represents a model that corresponds to a default view for dialog windows.
    /// </summary>
    public interface IDialogModel
    {

        /// <summary>
        /// Gets or sets the title of this <see cref="IDialogModel"/>.
        /// </summary>
        string Title { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Dialogs.DialogButton"/> value of this <see cref="IDialogModel"/>.
        /// </summary>
        DialogButton DialogButton { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Dialogs.DefaultButton"/> value of this <see cref="IDialogModel"/>.
        /// </summary>
        DefaultButton DefaultButton { get; set; }

    }

    /// <summary>
    /// Represents a model that corresponds to a default view for dialog windows.
    /// </summary>
    public class DialogModel : IDialogModel
    {

        /// <summary>
        /// Gets or sets the title of this <see cref="DialogModel"/>.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Dialogs.DialogButton"/> value of this <see cref="DialogModel"/>.
        /// </summary>
        public DialogButton DialogButton { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Dialogs.DefaultButton"/> value of this <see cref="DialogModel"/>.
        /// </summary>
        public DefaultButton DefaultButton { get; set; }

    }

    ///// <summary>
    ///// Represents a model that corresponds to a default view for property dialog windows.
    ///// </summary>
    //public interface IPropertyDialogModel : IDialogModel
    //{

    //    /// <summary>
    //    /// Gets or sets the items of this <see cref="IPropertyDialogModel"/>.
    //    /// </summary>
    //    IEnumerable<IPropertyTabItemModel> Items { get; set; }

    //}

    ///// <summary>
    ///// Represents a model that corresponds to a default view for property dialog windows.
    ///// </summary>
    //public class PropertyDialogModel : DialogModel, IPropertyDialogModel
    //{

    //    /// <summary>
    //    /// Gets or sets the items of this <see cref="PropertyDialogModel"/>.
    //    /// </summary>
    //    public IEnumerable<IPropertyTabItemModel> Items { get; set; }

    //}

}

namespace WinCopies.GUI.Controls.Models
{

    public interface IModelDataTemplateSelectors

    {

        DataTemplateSelector HeaderDataTemplateSelector { get; }

        DataTemplateSelector ContentDataTemplateSelector { get; }

        DataTemplateSelector ItemDataTemplateSelector { get; }

    }

    public class AttributeModelDataTemplateSelectors : IModelDataTemplateSelectors

    {

        public DataTemplateSelector HeaderDataTemplateSelector { get; }

        public DataTemplateSelector ContentDataTemplateSelector { get; }

        public DataTemplateSelector ItemDataTemplateSelector { get; }

        public AttributeModelDataTemplateSelectors()

        {

            var attributeDataTemplateSelector = new AttributeDataTemplateSelector();

            HeaderDataTemplateSelector = attributeDataTemplateSelector;

            ContentDataTemplateSelector = attributeDataTemplateSelector;

            ItemDataTemplateSelector = attributeDataTemplateSelector;

        }

    }

    public interface IDataTemplateSelectorsModel

    {

        IModelDataTemplateSelectors ModelDataTemplateSelectors { get; set; }

        bool AutoAddDataTemplateSelectors { get; set; }

        BindingDirection BindingDirection { get; }

    }

    public enum BindingDirection

    {

        OneWay,

        OneWayToSource

    }

    public static class DataTemplateSelectorModelExtensions

    {

        public static void TryUpdate(this IDataTemplateSelectorsModel dataTemplateSelectorsModel, in object value, BindingDirection bindingDirection)

        {

            if (value is IDataTemplateSelectorsModel _dataTemplateSelectorsModel)

            {

                switch (bindingDirection)

                {

                    case BindingDirection.OneWay:

                        if (dataTemplateSelectorsModel.AutoAddDataTemplateSelectors)

                        {

                            _dataTemplateSelectorsModel.ModelDataTemplateSelectors = dataTemplateSelectorsModel.ModelDataTemplateSelectors;

                            _dataTemplateSelectorsModel.AutoAddDataTemplateSelectors = true;

                        }

                        break;

                    case BindingDirection.OneWayToSource:

                        if (_dataTemplateSelectorsModel.AutoAddDataTemplateSelectors)

                        {

                            dataTemplateSelectorsModel.ModelDataTemplateSelectors = _dataTemplateSelectorsModel.ModelDataTemplateSelectors;

                            dataTemplateSelectorsModel.AutoAddDataTemplateSelectors = true;

                        }

                        break;

                }

            }

        }

        public static void TryReset(this IDataTemplateSelectorsModel dataTemplateSelectorsModel, in object value)

        {

            if (value is IDataTemplateSelectorsModel oldDataTemplateSelectorsModel && object.ReferenceEquals(dataTemplateSelectorsModel.ModelDataTemplateSelectors, oldDataTemplateSelectorsModel.ModelDataTemplateSelectors))

            {

                oldDataTemplateSelectorsModel.AutoAddDataTemplateSelectors = false;

                oldDataTemplateSelectorsModel.ModelDataTemplateSelectors = null;

            }

        }

    }

    /// <summary>
    /// Represents a model that corresponds to a default view for <see cref="ContentControl"/>s.
    /// </summary>
    public interface IContentControlModel
    {

        /// <summary>
        /// Gets or sets the content of this <see cref="IContentControlModel"/>.
        /// </summary>
        object Content { get; set; }

    }

    /// <summary>
    /// Represents a model that corresponds to a default view for <see cref="ContentControl"/>s.
    /// </summary>
    [TypeForDataTemplate(typeof(IContentControlModel))]
    public class ContentControlModel : IContentControlModel, IDataTemplateSelectorsModel
    {

        private object _content;

        /// <summary>
        /// Gets or sets the content of this <see cref="ContentControlModel"/>.
        /// </summary>
        public object Content { get => _content; set { object oldValue = _content; _content = value; OnContentChanged(oldValue); } }

        public IModelDataTemplateSelectors ModelDataTemplateSelectors { get; set; }

        public bool AutoAddDataTemplateSelectors { get; set; }

        public BindingDirection BindingDirection { get; } = Models.BindingDirection.OneWay;

        public ContentControlModel() { }

        public ContentControlModel(object content) => Content = content;

        public ContentControlModel(BindingDirection bindingDirection) => BindingDirection = bindingDirection;

        public ContentControlModel(object content, BindingDirection bindingDirection)
        {

            Content = content;

            BindingDirection = bindingDirection;

        }

        protected virtual void OnContentChanged(object oldValue)

        {

            this.TryReset(oldValue);

            this.TryUpdate(_content, BindingDirection);

        }

    }

    /// <summary>
    /// Represents a model that corresponds to a default view for <see cref="ContentControl"/>s.
    /// </summary>
    public interface IContentControlModel<T> : IContentControlModel
    {

        /// <summary>
        /// Gets or sets the content of this <see cref="IContentControlModel{T}"/>.
        /// </summary>
        new T Content { get; set; }

    }

    /// <summary>
    /// Represents a model that corresponds to a default view for <see cref="ContentControl"/>s.
    /// </summary>
    [TypeForDataTemplate(typeof(IContentControlModel))]
    public class ContentControlModel<T> : IContentControlModel<T>, IDataTemplateSelectorsModel

    {

        private T _content;

        /// <summary>
        /// Gets or sets the content of this <see cref="ContentControlModel{T}"/>.
        /// </summary>
        public T Content { get => _content; set { T oldValue = _content; _content = value; OnContentChanged(oldValue); } }

        object IContentControlModel.Content
        {
            get => Content; set => Content = (value ?? throw GetArgumentNullException(nameof(value))) is T _value ? _value : throw GetExceptionForInvalidType<T>(value.GetType().ToString(), nameof(value));
        }

        public IModelDataTemplateSelectors ModelDataTemplateSelectors { get; set; }

        public bool AutoAddDataTemplateSelectors { get; set; }

        public BindingDirection BindingDirection { get; } = BindingDirection.OneWay;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentControlModel"/> class.
        /// </summary>
        public ContentControlModel() { }

        public ContentControlModel(T content) => Content = content;

        public ContentControlModel(BindingDirection bindingDirection) => BindingDirection = bindingDirection;

        public ContentControlModel(T content, BindingDirection bindingDirection)
        {

            Content = content;

            BindingDirection = bindingDirection;

        }

        protected virtual void OnContentChanged(object oldValue)

        {

            this.TryReset(oldValue);

            this.TryUpdate(_content, BindingDirection);

        }

    }

    /// <summary>
    /// Represents a model that corresponds to a default view for headered controls.
    /// </summary>
    public interface IHeaderedControlModel

    {

        /// <summary>
        /// Gets or sets the header of this <see cref="IHeaderedControlModel"/>.
        /// </summary>
        object Header { get; set; }

    }

    /// <summary>
    /// Represents a model that corresponds to a default view for <see cref="HeaderedContentControl"/>s.
    /// </summary>
    public interface IHeaderedContentControlModel : IContentControlModel, IHeaderedControlModel
    {



    }

    /// <summary>
    /// Represents a model that corresponds to a default view for <see cref="HeaderedContentControl"/>s.
    /// </summary>
    [TypeForDataTemplate(typeof(IHeaderedContentControlModel))]
    public class HeaderedContentControlModel : ContentControlModel, IHeaderedContentControlModel

    {

        object _header;

        /// <summary>
        /// Gets or sets the header of this <see cref="HeaderedContentControlModel"/>.
        /// </summary>
        public object Header { get => _header; set { object oldValue = _header; _header = value; OnHeaderChanged(oldValue); } }

        /// <summary>
        /// Initializes a new instance of the <see cref="HeaderedContentControlModel"/> class.
        /// </summary>
        public HeaderedContentControlModel() { }

        public HeaderedContentControlModel(object header, object content) : base(content) => Header = header;

        public HeaderedContentControlModel(BindingDirection bindingDirection) : base(bindingDirection) { }

        public HeaderedContentControlModel(object header, object content, BindingDirection bindingDirection) : base(content, bindingDirection) => Header = header;

        protected virtual void OnHeaderChanged(object oldValue)

        {

            this.TryReset(oldValue);

            this.TryUpdate(_header, BindingDirection);

        }

    }

    /// <summary>
    /// Represents a model that corresponds to a default view for headered controls.
    /// </summary>
    public interface IHeaderedControlModel<T> : IHeaderedControlModel

    {

        /// <summary>
        /// Gets or sets the header of this <see cref="IHeaderedControlModel{T}"/>.
        /// </summary>
        new T Header { get; set; }

    }

    /// <summary>
    /// Represents a model that corresponds to a default view for <see cref="HeaderedContentControl"/>s.
    /// </summary>
    public interface IHeaderedContentControlModel<THeader, TContent> : IContentControlModel<TContent>, IHeaderedControlModel<THeader>, IHeaderedContentControlModel
    {



    }

    /// <summary>
    /// Represents a model that corresponds to a default view for <see cref="HeaderedContentControl"/>s.
    /// </summary>
    [TypeForDataTemplate(typeof(IHeaderedContentControlModel))]
    public class HeaderedContentControlModel<THeader, TContent> : ContentControlModel<TContent>, IHeaderedContentControlModel<THeader, TContent>

    {

        private THeader _header;

        /// <summary>
        /// Gets or sets the header of this <see cref="HeaderedContentControlModel{THeader, TContent}"/>.
        /// </summary>
        public THeader Header { get => _header; set { THeader oldValue = _header; _header = value; OnHeaderChanged(oldValue); } }

        object IHeaderedControlModel.Header { get => Header; set => Header = (value??throw GetArgumentNullException(nameof(value))) is THeader _value ? _value : throw GetExceptionForInvalidType<THeader>(value.GetType().ToString(), nameof(value)); }

        /// <summary>
        /// Initializes a new instance of the <see cref="HeaderedContentControlModel{THeader, TContent}"/> class.
        /// </summary>
        public HeaderedContentControlModel() { }

        public HeaderedContentControlModel(THeader header, TContent content) : base(content) => Header = header;

        public HeaderedContentControlModel(BindingDirection bindingDirection) : base(bindingDirection) { }

        public HeaderedContentControlModel(THeader header, TContent content, BindingDirection bindingDirection) : base(content, bindingDirection) => Header = header;

        protected virtual void OnHeaderChanged(THeader oldValue)

        {

            this.TryReset(oldValue);

            this.TryUpdate(_header, BindingDirection);

        }

    }

    /// <summary>
    /// Represents a model that corresponds to a default view for <see cref="ItemsControl"/>s.
    /// </summary>
    public interface IItemsControlModel
    {

        /// <summary>
        /// Gets or sets the items of this <see cref="IItemsControlModel"/>.
        /// </summary>
        IEnumerable Items { get; set; }

    }

    /// <summary>
    /// Represents a model that corresponds to a default view for <see cref="ItemsControl"/>s.
    /// </summary>
    [TypeForDataTemplate(typeof(IItemsControlModel))]
    public class ItemsControlModel : IItemsControlModel, IDataTemplateSelectorsModel
    {

        private IEnumerable _items;

        /// <summary>
        /// Gets or sets the items of this <see cref="ItemsControlModel"/>.
        /// </summary>
        public IEnumerable Items { get => _items; set { IEnumerable oldItems = _items; _items = value; OnItemsChanged(oldItems); } }

        public IModelDataTemplateSelectors ModelDataTemplateSelectors { get; set; }

        public bool AutoAddDataTemplateSelectors { get; set; }

        public BindingDirection BindingDirection { get; } = BindingDirection.OneWay;

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemsControlModel"/> class.
        /// </summary>
        public ItemsControlModel() { }

        public ItemsControlModel(IEnumerable items) => Items = items;

        public ItemsControlModel(BindingDirection bindingDirection) => BindingDirection = bindingDirection;

        public ItemsControlModel(IEnumerable items, BindingDirection bindingDirection)
        {

            Items = items;

            BindingDirection = bindingDirection;

        }

        protected virtual void OnItemsChanged(IEnumerable oldItems)

        {

            this.TryReset(oldItems);

            this.TryUpdate(_items, BindingDirection);

        }

    }

    /// <summary>
    /// Represents a model that corresponds to a default view for <see cref="ItemsControl"/>s.
    /// </summary>
    public interface IItemsControlModel<T> : IItemsControlModel
    {

        /// <summary>
        /// Gets or sets the items of this <see cref="IItemsControlModel{T}"/>.
        /// </summary>
        new IEnumerable<T> Items { get; set; }

    }

    /// <summary>
    /// Represents a model that corresponds to a default view for <see cref="ItemsControl"/>s.
    /// </summary>
    [TypeForDataTemplate(typeof(IItemsControlModel))]
    public class ItemsControlModel<T> : IItemsControlModel<T>, IDataTemplateSelectorsModel

    {

        private IEnumerable<T> _items;

        /// <summary>
        /// Gets or sets the items of this <see cref="ItemsControlModel{T}"/>.
        /// </summary>
        public IEnumerable<T> Items { get => _items; set { IEnumerable<T> oldItems = _items; _items = value; OnItemsChanged(oldItems); } }

        IEnumerable IItemsControlModel.Items { get => Items; set => Items = (value??throw GetArgumentNullException(nameof(value))) is IEnumerable<T> _value ? _value : throw GetExceptionForInvalidType<IEnumerable<T>>(value.GetType().ToString(), nameof(value)); }

        public IModelDataTemplateSelectors ModelDataTemplateSelectors { get; set; }

        public bool AutoAddDataTemplateSelectors { get; set; }

        public BindingDirection BindingDirection { get; } = BindingDirection.OneWay;

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemsControlModel{T}"/> class.
        /// </summary>
        public ItemsControlModel() { }

        public ItemsControlModel(IEnumerable<T> items) => Items = items;

        public ItemsControlModel(BindingDirection bindingDirection) => BindingDirection = bindingDirection;

        public ItemsControlModel(IEnumerable<T> items, BindingDirection bindingDirection)
        {

            Items = items;

            BindingDirection = bindingDirection;

        }

        protected virtual void OnItemsChanged(IEnumerable<T> oldItems)

        {

            this.TryReset(oldItems);

            this.TryUpdate(_items, BindingDirection);

        }

    }

    /// <summary>
    /// Represents a model that corresponds to a default view for <see cref="HeaderedItemsControl"/>s.
    /// </summary>
    public interface IHeaderedItemsControlModel : IItemsControlModel, IHeaderedControlModel
    {



    }

    /// <summary>
    /// Represents a model that corresponds to a default view for <see cref="HeaderedItemsControl"/>s.
    /// </summary>
    [TypeForDataTemplate(typeof(IHeaderedItemsControlModel))]
    public class HeaderedItemsControlModel : ItemsControlModel, IHeaderedItemsControlModel, IDataTemplateSelectorsModel

    {

        private object _header;

        /// <summary>
        /// Gets or sets the header of this <see cref="HeaderedItemsControlModel"/>.
        /// </summary>
        public object Header { get => _header; set { object oldValue = _header; _header = value; OnHeaderChanged(oldValue); } }

        /// <summary>
        /// Initializes a new instance of the <see cref="HeaderedItemsControlModel"/> class.
        /// </summary>
        public HeaderedItemsControlModel() { }

        public HeaderedItemsControlModel(object header, IEnumerable items) : base(items) => Header = header;

        public HeaderedItemsControlModel(BindingDirection bindingDirection) : base(bindingDirection) { }

        public HeaderedItemsControlModel(object header, IEnumerable items, BindingDirection bindingDirection) : base(items, bindingDirection) => Header = header;

        protected virtual void OnHeaderChanged(object oldValue)

        {

            this.TryReset(oldValue);

            this.TryUpdate(_header, BindingDirection);

        }

    }

    /// <summary>
    /// Represents a model that corresponds to a default view for <see cref="HeaderedItemsControl"/>s.
    /// </summary>
    public interface IHeaderedItemsControlModel<THeader, TItems> : IItemsControlModel<TItems>, IHeaderedControlModel<THeader>, IHeaderedItemsControlModel
    {



    }

    /// <summary>
    /// Represents a model that corresponds to a default view for <see cref="HeaderedItemsControl"/>s.
    /// </summary>
    [TypeForDataTemplate(typeof(IHeaderedItemsControlModel))]
    public class HeaderedItemsControlModel<THeader, TItems> : ItemsControlModel<TItems>, IHeaderedItemsControlModel<THeader, TItems>

    {

        private THeader _header;

        /// <summary>
        /// Gets or sets the header of this <see cref="HeaderedItemsControlModel{THeader, TItems}"/>.
        /// </summary>
        public THeader Header { get => _header; set { THeader oldValue = _header; _header = value; OnHeaderChanged(oldValue); } }

        object IHeaderedControlModel.Header { get => Header; set => Header = (value ?? throw GetArgumentNullException(nameof(value))) is THeader _value ? _value : throw GetArgumentNullException(nameof(value)); }

        /// <summary>
        /// Initializes a new instance of the <see cref="HeaderedItemsControlModel{THeader, TItems}"/> class.
        /// </summary>
        public HeaderedItemsControlModel() { }

        public HeaderedItemsControlModel(THeader header, IEnumerable<TItems> items) : base(items) => Header = header;

        public HeaderedItemsControlModel(BindingDirection bindingDirection) : base(bindingDirection) { }

        public HeaderedItemsControlModel(THeader header, IEnumerable<TItems> items, BindingDirection bindingDirection) : base(items, bindingDirection) => Header = header;

        protected virtual void OnHeaderChanged(THeader oldValue)

        {

            this.TryReset(oldValue);

            this.TryUpdate(_header, BindingDirection);

        }

    }

    /// <summary>
    /// Represents a model that corresponds to a default view for <see cref="GroupBox"/> controls.
    /// </summary>
    public interface IGroupBoxModel : IHeaderedContentControlModel
    {



    }

    /// <summary>
    /// Represents a model that corresponds to a default view for <see cref="GroupBox"/> controls.
    /// </summary>
    public interface IGroupBoxModel<THeader, TContent> : IGroupBoxModel, IHeaderedContentControlModel<THeader, TContent>
    {



    }

    /// <summary>
    /// Represents a model that corresponds to a default view for <see cref="GroupBox"/> controls.
    /// </summary>
    [TypeForDataTemplate(typeof(IGroupBoxModel))]
    public class GroupBoxModel : HeaderedContentControlModel, IGroupBoxModel
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="GroupBoxModel"/> class.
        /// </summary>
        public GroupBoxModel() { }

        public GroupBoxModel(object header, object content) : base(header, content) { }

        public GroupBoxModel(BindingDirection bindingDirection) : base(bindingDirection) { }

        public GroupBoxModel(object header, object content, BindingDirection bindingDirection) : base(header, content, bindingDirection) { }

    }

    /// <summary>
    /// Represents a model that corresponds to a default view for <see cref="GroupBox"/> controls.
    /// </summary>
    [TypeForDataTemplate(typeof(IGroupBoxModel))]
    public class GroupBoxModel<THeader, TContent> : HeaderedContentControlModel<THeader, TContent>, IGroupBoxModel<THeader, TContent>
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="GroupBoxModel{THeader, TContent}"/> class.
        /// </summary>
        public GroupBoxModel() { }

        public GroupBoxModel(THeader header, TContent content) : base(header, content) { }

        public GroupBoxModel(BindingDirection bindingDirection) : base(bindingDirection) { }

        public GroupBoxModel(THeader header, TContent content, BindingDirection bindingDirection) : base(header, content, bindingDirection) { }

    }

    /// <summary>
    /// Represents a model that corresponds to a default view for <see cref="TabItem"/> controls.
    /// </summary>
    public interface ITabItemModel : IHeaderedContentControlModel
    {



    }

    /// <summary>
    /// Represents a model that corresponds to a default view for <see cref="TabItem"/> controls.
    /// </summary>
    public interface ITabItemModel<THeader, TContent> : ITabItemModel, IHeaderedContentControlModel<THeader, TContent>
    {



    }

    /// <summary>
    /// Represents a model that corresponds to a default view for <see cref="TabItem"/> controls.
    /// </summary>
    [TypeForDataTemplate(typeof(ITabItemModel))]
    public class TabItemModel : HeaderedContentControlModel, ITabItemModel
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="TabItemModel"/> class.
        /// </summary>
        public TabItemModel() { }

        public TabItemModel(object header, object content) : base(header, content) { }

        public TabItemModel(BindingDirection bindingDirection) : base(bindingDirection) { }

        public TabItemModel(object header, object content, BindingDirection bindingDirection) : base(header, content, bindingDirection) { }

    }

    /// <summary>
    /// Represents a model that corresponds to a default view for <see cref="TabItem"/> controls.
    /// </summary>
    [TypeForDataTemplate(typeof(ITabItemModel))]
    public class TabItemModel<THeader, TContent> : HeaderedContentControlModel<THeader, TContent>, ITabItemModel<THeader, TContent>
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="TabItemModel{THeader, TContent}"/> class.
        /// </summary>
        public TabItemModel() { }

        public TabItemModel(THeader header, TContent content) : base(header, content) { }

        public TabItemModel(BindingDirection bindingDirection) : base(bindingDirection) { }

        public TabItemModel(THeader header, TContent content, BindingDirection bindingDirection) : base(header, content, bindingDirection) { }

    }

    ///// <summary>
    ///// Represents a model that corresponds to a default view for property tab items.
    ///// </summary>
    //public interface IPropertyTabItemModel : IHeaderedItemsControlModel<object, IGroupBoxModel>
    //{

    //    /// <summary>
    //    /// Gets or sets the header of this <see cref="IPropertyTabItemModel"/>.
    //    /// </summary>
    //    new IEnumerable<IGroupBoxModel> Items { get; set; }

    //}

    ///// <summary>
    ///// Represents a model that corresponds to a default view for property tab items.
    ///// </summary>
    //public interface IPropertyTabItemModel<TItemHeader, TGroupBoxHeader, TGroupBoxContent> : IPropertyTabItemModel, IHeaderedItemsControlModel<TItemHeader, IGroupBoxModel<TGroupBoxHeader, TGroupBoxContent>>

    //{



    //}

    ///// <summary>
    ///// Represents a model that corresponds to a default view for property tab items.
    ///// </summary>
    //[TypeForDataTemplate(typeof(IPropertyTabItemModel))]
    //public class PropertyTabItemModel : HeaderedItemsControlModel<object, IGroupBoxModel>, IPropertyTabItemModel
    //{

    //    /// <summary>
    //    /// Gets or sets the header of this <see cref="PropertyTabItemModel"/>.
    //    /// </summary>
    //    public object Header { get; set; }

    //    /// <summary>
    //    /// Gets or sets the items of this <see cref="PropertyTabItemModel"/>.
    //    /// </summary>
    //    public IEnumerable<IGroupBoxModel> Items { get; set; }

    //    IEnumerable IItemsControlModel.Items { get => Items; set => Items = GetOrThrowIfNotType<IEnumerable<IGroupBoxModel>>(value, nameof(value)); }

    //    public PropertyTabItemModel() { }

    //    public PropertyTabItemModel(object header, IEnumerable<IGroupBoxModel> items) : base(header, items) { }

    //    public PropertyTabItemModel(BindingDirection bindingDirection) : base(bindingDirection) { }

    //    public PropertyTabItemModel(object header, IEnumerable<IGroupBoxModel> items, BindingDirection bindingDirection) : base(header, items, bindingDirection) { }

    //}

    ///// <summary>
    ///// Represents a model that corresponds to a default view for property tab items.
    ///// </summary>
    //[TypeForDataTemplate(typeof(IPropertyTabItemModel))]
    //public class PropertyTabItemModel<TItemHeader, TGroupBoxHeader, TGroupBoxContent> : HeaderedItemsControlModel<TItemHeader, IGroupBoxModel<TGroupBoxHeader, TGroupBoxContent>>, IPropertyTabItemModel<TItemHeader, TGroupBoxHeader, TGroupBoxContent>
    //{

    //    IEnumerable<IGroupBoxModel> IPropertyTabItemModel.Items { get => Items; set => Items = GetOrThrowIfNotType<IEnumerable<IGroupBoxModel<TGroupBoxHeader, TGroupBoxContent>>>(value, nameof(value)); }

    //    public PropertyTabItemModel() { }

    //    public PropertyTabItemModel(TItemHeader header, IEnumerable<IGroupBoxModel<TGroupBoxHeader, TGroupBoxContent>> items) : base(header, items) { }

    //    public PropertyTabItemModel(BindingDirection bindingDirection) : base(bindingDirection) { }

    //    public PropertyTabItemModel(TItemHeader header, IEnumerable<IGroupBoxModel<TGroupBoxHeader, TGroupBoxContent>> items, BindingDirection bindingDirection) : base(header, items, bindingDirection) { }

    //}

    /// <summary>
    /// Represents a model that corresponds to a default view for <see cref="Button"/>s.
    /// </summary>
    public interface IButtonModel : IContentControlModel

    {

        ICommand Command { get; set; }

        object CommandParameter { get; set; }

        IInputElement CommandTarget { get; set; }

    }

    /// <summary>
    /// Represents a model that corresponds to a default view for <see cref="Button"/>s.
    /// </summary>
    [TypeForDataTemplate(typeof(IButtonModel))]
    public class ButtonModel : ContentControlModel, IButtonModel

    {

        /// <summary>
        /// Gets or sets the command of this <see cref="ButtonModel"/>.
        /// </summary>
        public ICommand Command { get; set; }

        /// <summary>
        /// Gets or sets the command parameter of this <see cref="ButtonModel"/>.
        /// </summary>
        public object CommandParameter { get; set; }

        /// <summary>
        /// Gets or sets the command target of this <see cref="ButtonModel"/>.
        /// </summary>
        public IInputElement CommandTarget { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ButtonModel"/> class.
        /// </summary>
        public ButtonModel() { }

        public ButtonModel(object content) : base(content) { }

        public ButtonModel(BindingDirection bindingDirection) : base(bindingDirection) { }

        public ButtonModel(object content, BindingDirection bindingDirection) : base(content, bindingDirection) { }

    }

    /// <summary>
    /// Represents a model that corresponds to a default view for <see cref="Button"/>s.
    /// </summary>
    public interface IButtonModel<T> : IButtonModel, IContentControlModel<T>

    {



    }

    /// <summary>
    /// Represents a model that corresponds to a default view for <see cref="Button"/>s.
    /// </summary>
    [TypeForDataTemplate(typeof(IButtonModel))]
    public class ButtonModel<T> : ContentControlModel<T>, IButtonModel<T>

    {

        /// <summary>
        /// Gets or sets the command of this <see cref="ButtonModel{T}"/>.
        /// </summary>
        public ICommand Command { get; set; }

        /// <summary>
        /// Gets or sets the command parameter of this <see cref="ButtonModel{T}"/>.
        /// </summary>
        public object CommandParameter { get; set; }

        /// <summary>
        /// Gets or sets the command target of this <see cref="ButtonModel{T}"/>.
        /// </summary>
        public IInputElement CommandTarget { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ButtonModel{T}"/> class.
        /// </summary>
        public ButtonModel() { }

        public ButtonModel(T content) : base(content) { }

        public ButtonModel(BindingDirection bindingDirection) : base(bindingDirection) { }

        public ButtonModel(T content, BindingDirection bindingDirection) : base(content, bindingDirection) { }

    }

    /// <summary>
    /// Represents a model that corresponds to a default view for <see cref="ToggleButton"/>s.
    /// </summary>
    public interface IToggleButtonModel : IButtonModel

    {

        /// <summary>
        /// Gets or sets a value that indeicates whether this <see cref="IToggleButtonModel"/> is checked.
        /// </summary>
        bool? IsChecked { get; set; }

        /// <summary>
        /// Gets or sets a value that indeicates whether this <see cref="IToggleButtonModel"/> is three state.
        /// </summary>
        bool IsThreeState { get; set; }

    }

    /// <summary>
    /// Represents a model that corresponds to a default view for <see cref="ToggleButton"/>s.
    /// </summary>
    [TypeForDataTemplate(typeof(IToggleButtonModel))]
    public class ToggleButtonModel : ButtonModel, IToggleButtonModel

    {

        /// <summary>
        /// Gets or sets a value that indeicates whether this <see cref="ToggleButtonModel"/> is checked.
        /// </summary>
        public bool? IsChecked { get; set; }

        /// <summary>
        /// Gets or sets a value that indeicates whether this <see cref="ToggleButtonModel"/> is three state.
        /// </summary>
        public bool IsThreeState { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ToggleButtonModel"/> class.
        /// </summary>
        public ToggleButtonModel() { }

        public ToggleButtonModel(object content) : base(content) { }

        public ToggleButtonModel(BindingDirection bindingDirection) : base(bindingDirection) { }

        public ToggleButtonModel(object content, BindingDirection bindingDirection) : base(content, bindingDirection) { }

    }

    /// <summary>
    /// Represents a model that corresponds to a default view for <see cref="ToggleButton"/>s.
    /// </summary>
    public interface IToggleButtonModel<T> : IToggleButtonModel, IButtonModel<T>

    {



    }

    /// <summary>
    /// Represents a model that corresponds to a default view for <see cref="ToggleButton"/>s.
    /// </summary>
    [TypeForDataTemplate(typeof(IToggleButtonModel))]
    public class ToggleButtonModel<T> : ButtonModel<T>, IToggleButtonModel<T>

    {

        /// <summary>
        /// Gets or sets a value that indeicates whether this <see cref="ToggleButtonModel{T}"/> is checked.
        /// </summary>
        public bool? IsChecked { get; set; }

        /// <summary>
        /// Gets or sets a value that indeicates whether this <see cref="ToggleButtonModel{T}"/> is three state.
        /// </summary>
        public bool IsThreeState { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ToggleButtonModel{T}"/> class.
        /// </summary>
        public ToggleButtonModel() { }

        public ToggleButtonModel(T content) : base(content) { }

        public ToggleButtonModel(BindingDirection bindingDirection) : base(bindingDirection) { }

        public ToggleButtonModel(T content, BindingDirection bindingDirection) : base(content, bindingDirection) { }

    }

    /// <summary>
    /// Represents a model that corresponds to a default view for <see cref="CheckBox"/>'s.
    /// </summary>
    public interface ICheckBoxModel : IToggleButtonModel
    {



    }

    /// <summary>
    /// Represents a model that corresponds to a default view for <see cref="CheckBox"/>'s.
    /// </summary>
    [TypeForDataTemplate(typeof(ICheckBoxModel))]
    public class CheckBoxModel : ToggleButtonModel, ICheckBoxModel

    {

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckBoxModel"/> class.
        /// </summary>
        public CheckBoxModel() { }

        public CheckBoxModel(object content) : base(content) { }

        public CheckBoxModel(BindingDirection bindingDirection) : base(bindingDirection) { }

        public CheckBoxModel(object content, BindingDirection bindingDirection) : base(content, bindingDirection) { }

    }

    /// <summary>
    /// Represents a model that corresponds to a default view for <see cref="CheckBox"/>'.
    /// </summary>
    public interface ICheckBoxModel<T> : IToggleButtonModel<T>, ICheckBoxModel
    {



    }

    /// <summary>
    /// Represents a model that corresponds to a default view for <see cref="CheckBox"/>'.
    /// </summary>
    [TypeForDataTemplate(typeof(ICheckBoxModel))]
    public class CheckBoxModel<T> : ToggleButtonModel<T>, ICheckBoxModel<T>

    {

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckBoxModel{T}"/> class.
        /// </summary>
        public CheckBoxModel() { }

        public CheckBoxModel(T content) : base(content) { }

        public CheckBoxModel(BindingDirection bindingDirection) : base(bindingDirection) { }

        public CheckBoxModel(T content, BindingDirection bindingDirection) : base(content, bindingDirection) { }

    }

    /// <summary>
    /// Represents a model that corresponds to a default view for <see cref="TextBox"/>'s.
    /// </summary>
    public interface ITextBoxModelTextOriented

    {

        /// <summary>
        /// Gets or sets the text of this <see cref="ITextBoxModelTextOriented"/>.
        /// </summary>
        string Text { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether this <see cref="ITextBoxModelTextOriented"/> is read-only.
        /// </summary>
        bool IsReadOnly { get; set; }

    }

    /// <summary>
    /// Represents a model that corresponds to a default view for <see cref="TextBox"/>'s.
    /// </summary>
    [TypeForDataTemplate(typeof(ITextBoxModelTextOriented))]
    public class TextBoxModelTextOriented : ITextBoxModelTextOriented

    {

        /// <summary>
        /// Gets or sets the text of this <see cref="TextBoxModelTextOriented"/>.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether this <see cref="TextBoxModelTextOriented"/> is read-only.
        /// </summary>
        public bool IsReadOnly { get; set; }

    }

    /// <summary>
    /// Represents a model that corresponds to a default view for <see cref="TextBox"/>'s.
    /// </summary>
    public interface ITextBoxModelSelectionOriented : ITextBoxModelTextOriented

    {

        bool IsReadOnlyCaretVisible { get; set; }

        bool AutoWordSelection { get; set; }

        Brush SelectionBrush { get; set; }

        double SelectionOpacity { get; set; }

        Brush SelectionTextBrush { get; set; }

        Brush CaretBrush { get; set; }

        bool IsInactiveSelectionHighlightEnabled { get; set; }

    }

    /// <summary>
    /// Represents a model that corresponds to a default view for <see cref="TextBox"/>'s.
    /// </summary>
    [TypeForDataTemplate(typeof(ITextBoxModelSelectionOriented))]
    public class TextBoxModelSelectionOriented : TextBoxModelTextOriented, ITextBoxModelSelectionOriented

    {

        public int CaretIndex { get; set; }

        public int SelectionLength { get; set; }

        public int SelectionStart { get; set; }

        public string SelectedText { get; set; }

        public bool IsReadOnlyCaretVisible { get; set; }

        public bool AutoWordSelection { get; set; }

        public Brush SelectionBrush { get; set; }

        public double SelectionOpacity { get; set; }

        public Brush SelectionTextBrush { get; set; }

        public Brush CaretBrush { get; set; }

        public bool IsInactiveSelectionHighlightEnabled { get; set; }

    }

    /// <summary>
    /// Represents a model that corresponds to a default view for <see cref="TextBox"/>'s.
    /// </summary>
    public interface ITextBoxModelTextEditingOriented : ITextBoxModelTextOriented

    {

        int MinLines { get; set; }

        int MaxLines { get; set; }

        CharacterCasing CharacterCasing { get; set; }

        int MaxLength { get; set; }

        TextAlignment TextAlignment { get; set; }

        TextDecorationCollection TextDecorations { get; set; }

        TextWrapping TextWrapping { get; set; }

        bool AcceptsReturn { get; set; }

        bool AcceptsTab { get; set; }

        bool IsUndoEnabled { get; set; }

        int UndoLimit { get; set; }

    }

    /// <summary>
    /// Represents a model that corresponds to a default view for <see cref="TextBox"/>'s.
    /// </summary>
    [TypeForDataTemplate(typeof(ITextBoxModelTextEditingOriented))]
    public class TextBoxModelTextEditingOriented : TextBoxModelTextOriented, ITextBoxModelTextEditingOriented

    {

        public int MinLines { get; set; }

        public int MaxLines { get; set; }

        public CharacterCasing CharacterCasing { get; set; }

        public int MaxLength { get; set; }

        public TextAlignment TextAlignment { get; set; }

        public TextDecorationCollection TextDecorations { get; set; }

        public TextWrapping TextWrapping { get; set; }

        public bool AcceptsReturn { get; set; }

        public bool AcceptsTab { get; set; }

        public bool IsUndoEnabled { get; set; }

        public int UndoLimit { get; set; }

    }

    /// <summary>
    /// Represents a model that corresponds to a default view for <see cref="TextBox"/>'s.
    /// </summary>
    public interface ITextBoxModel : ITextBoxModelTextOriented, ITextBoxModelTextEditingOriented, ITextBoxModelSelectionOriented

    {



    }

    /// <summary>
    /// Represents a model that corresponds to a default view for <see cref="TextBox"/>'s.
    /// </summary>
    [TypeForDataTemplate(typeof(ITextBoxModel))]
    public class TextBoxModel : TextBoxModelTextOriented, ITextBoxModel

    {

        public int MinLines { get; set; }

        public int MaxLines { get; set; }

        public CharacterCasing CharacterCasing { get; set; }

        public int MaxLength { get; set; }

        public TextAlignment TextAlignment { get; set; }

        public int LineCount { get; }

        public TextDecorationCollection TextDecorations { get; set; }

        public TextWrapping TextWrapping { get; set; }

        public bool AcceptsReturn { get; set; }

        public bool AcceptsTab { get; set; }

        public double SelectionOpacity { get; set; }

        public bool CanUndo { get; }

        public bool CanRedo { get; }

        public bool IsUndoEnabled { get; set; }

        public int UndoLimit { get; set; }

        public int CaretIndex { get; set; }

        public int SelectionLength { get; set; }

        public int SelectionStart { get; set; }

        public string SelectedText { get; set; }

        public bool IsReadOnlyCaretVisible { get; set; }

        public bool AutoWordSelection { get; set; }

        public Brush SelectionBrush { get; set; }

        public Brush SelectionTextBrush { get; set; }

        public Brush CaretBrush { get; set; }

        public bool IsSelectionActive { get; }

        public bool IsInactiveSelectionHighlightEnabled { get; set; }

    }

    /// <summary>
    /// Represents a model that corresponds to a default view for <see cref="RadioButton"/> collection.
    /// </summary>
    public interface IRadioButtonCollection : IEnumerable<IRadioButtonModel>

    {

        string GroupName { get; set; }

    }

    /// <summary>
    /// Represents a model that corresponds to a default view for <see cref="RadioButton"/> collection.
    /// </summary>
    [TypeForDataTemplate(typeof(IRadioButtonCollection))]
    public class RadioButtonCollection : System.Collections.Generic.List<IRadioButtonModel>, IRadioButtonCollection

    {

        public string GroupName { get; set; }

    }

    /// <summary>
    /// Represents a model that corresponds to a default view for <see cref="RadioButton"/> collection.
    /// </summary>
    public interface IRadioButtonCollection<T> : IRadioButtonCollection, IEnumerable<IRadioButtonModel<T>>

    {



    }

    /// <summary>
    /// Represents a model that corresponds to a default view for <see cref="RadioButton"/> collection.
    /// </summary>
    [TypeForDataTemplate(typeof(IRadioButtonCollection))]
    public class RadioButtonCollection<T> : List<IRadioButtonModel<T>>, IRadioButtonCollection<T>

    {

        public string GroupName { get; set; }

        IEnumerator<IRadioButtonModel> IEnumerable<IRadioButtonModel>.GetEnumerator() => GetEnumerator();

    }

    /// <summary>
    /// Represents a model that corresponds to a default view for <see cref="RadioButton"/>s.
    /// </summary>
    public interface IRadioButtonModel : IToggleButtonModel

    {



    }

    /// <summary>
    /// Represents a model that corresponds to a default view for <see cref="RadioButton"/>s.
    /// </summary>
    [TypeForDataTemplate(typeof(IRadioButtonModel))]
    public class RadioButtonModel : ToggleButtonModel, IRadioButtonModel

    {

        /// <summary>
        /// Initializes a new instance of the <see cref="RadioButtonModel"/> class.
        /// </summary>
        public RadioButtonModel() { }

        public RadioButtonModel(object content) : base(content) { }

        public RadioButtonModel(BindingDirection bindingDirection) : base(bindingDirection) { }

        public RadioButtonModel(object content, BindingDirection bindingDirection) : base(content, bindingDirection) { }

    }

    /// <summary>
    /// Represents a model that corresponds to a default view for <see cref="RadioButton"/>s.
    /// </summary>
    public interface IRadioButtonModel<T> : IRadioButtonModel, IToggleButtonModel<T>

    {



    }

    /// <summary>
    /// Represents a model that corresponds to a default view for <see cref="RadioButton"/>s.
    /// </summary>
    [TypeForDataTemplate(typeof(IRadioButtonModel))]
    public class RadioButtonModel<T> : ToggleButtonModel<T>, IRadioButtonModel<T>

    {

        /// <summary>
        /// Initializes a new instance of the <see cref="RadioButtonModel{T}"/> class.
        /// </summary>
        public RadioButtonModel() { }

        public RadioButtonModel(T content) : base(content) { }

        public RadioButtonModel(BindingDirection bindingDirection) : base(bindingDirection) { }

        public RadioButtonModel(T content, BindingDirection bindingDirection) : base(content, bindingDirection) { }

    }

    /// <summary>
    /// Represents a model that corresponds to a default view for <see cref="RadioButton"/>s.
    /// </summary>
    public interface IGroupingRadioButtonModel : IRadioButtonModel

    {

        string GroupName { get; set; }

    }

    /// <summary>
    /// Represents a model that corresponds to a default view for <see cref="RadioButton"/>s.
    /// </summary>
    public interface IGroupingRadioButtonModel<T> : IGroupingRadioButtonModel, IRadioButtonModel<T>

    {



    }

    /// <summary>
    /// Represents a model that corresponds to a default view for <see cref="RadioButton"/>s.
    /// </summary>
    [TypeForDataTemplate(typeof(IGroupingRadioButtonModel))]
    public class GroupingRadioButtonModel : RadioButtonModel, IGroupingRadioButtonModel

    {

        public string GroupName { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="GroupingRadioButtonModel"/> class.
        /// </summary>
        public GroupingRadioButtonModel() { }

        public GroupingRadioButtonModel(object content) : base(content) { }

        public GroupingRadioButtonModel(BindingDirection bindingDirection) : base(bindingDirection) { }

        public GroupingRadioButtonModel(object content, BindingDirection bindingDirection) : base(content, bindingDirection) { }

    }

    /// <summary>
    /// Represents a model that corresponds to a default view for <see cref="RadioButton"/>s.
    /// </summary>
    [TypeForDataTemplate(typeof(IGroupingRadioButtonModel))]
    public class GroupingRadioButtonModel<T> : RadioButtonModel<T>, IGroupingRadioButtonModel<T>

    {

        public string GroupName { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="GroupingRadioButtonModel{T}"/> class.
        /// </summary>
        public GroupingRadioButtonModel() { }

        public GroupingRadioButtonModel(T content) : base(content) { }

        public GroupingRadioButtonModel(BindingDirection bindingDirection) : base(bindingDirection) { }

        public GroupingRadioButtonModel(T content, BindingDirection bindingDirection) : base(content, bindingDirection) { }

    }

}
