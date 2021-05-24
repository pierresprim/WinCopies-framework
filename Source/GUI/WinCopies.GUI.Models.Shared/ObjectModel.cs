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

using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using WinCopies.Commands;

#if WinCopies3
using static WinCopies.ThrowHelper;
#else
using WinCopies.Util;
using WinCopies.Util.Data;
using static WinCopies.Util.Util;
#endif

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
        /// Gets or sets the <see cref="Windows.DialogButton"/> value of this <see cref="IDialogModel"/>.
        /// </summary>
        DialogButton DialogButton { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Windows.DefaultButton"/> value of this <see cref="IDialogModel"/>.
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
            ThrowIfNull(dataTemplateSelectorsModel, nameof(dataTemplateSelectorsModel));

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
            ThrowIfNull(dataTemplateSelectorsModel, nameof(dataTemplateSelectorsModel));

            if (value is IDataTemplateSelectorsModel oldDataTemplateSelectorsModel && object.ReferenceEquals(dataTemplateSelectorsModel.ModelDataTemplateSelectors, oldDataTemplateSelectorsModel.ModelDataTemplateSelectors))
            {
                oldDataTemplateSelectorsModel.AutoAddDataTemplateSelectors = false;

                oldDataTemplateSelectorsModel.ModelDataTemplateSelectors = null;

            }
        }
    }

    public class ControlModel : IControlModel
    {
        public bool IsEnabled { get; set; } = true;
    }

    /// <summary>
    /// Represents a model that corresponds to a default view for <see cref="ContentControl"/>s.
    /// </summary>
    [TypeForDataTemplate(typeof(IContentControlModel))]
    public class ContentControlModel : ControlModel, IDataTemplateSelectorsModel
    {
        private object _content;

        /// <summary>
        /// Gets or sets the content of this <see cref="ContentControlModel"/>.
        /// </summary>
        public object Content { get => _content; set { object oldValue = _content; _content = value; OnContentChanged(oldValue); } }

        public IModelDataTemplateSelectors ModelDataTemplateSelectors { get; set; }

        public bool AutoAddDataTemplateSelectors { get; set; }

        public BindingDirection BindingDirection { get; } = Models.BindingDirection.OneWay;

        public ContentControlModel() { /* Left empty. */ }

        public ContentControlModel(in object content) => Content = content;

        public ContentControlModel(in BindingDirection bindingDirection) => BindingDirection = bindingDirection;

        public ContentControlModel(in object content, in BindingDirection bindingDirection)
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
    [TypeForDataTemplate(typeof(IContentControlModel))]
    public class ContentControlModel<T> : ControlModel, IContentControlModel<T>, IDataTemplateSelectorsModel
    {
        private T _content;

        /// <summary>
        /// Gets or sets the content of this <see cref="ContentControlModel{T}"/>.
        /// </summary>
        public T Content { get => _content; set { T oldValue = _content; _content = value; OnContentChanged(oldValue); } }

        object IContentControlModel.Content { get => Content; set => Content = (value ?? throw GetArgumentNullException(nameof(value))) is T _value ? _value : throw GetExceptionForInvalidType<T>(value.GetType().ToString(), nameof(value)); }

        public IModelDataTemplateSelectors ModelDataTemplateSelectors { get; set; }

        public bool AutoAddDataTemplateSelectors { get; set; }

        public BindingDirection BindingDirection { get; } = BindingDirection.OneWay;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentControlModel"/> class.
        /// </summary>
        public ContentControlModel() { /* Left empty. */ }

        public ContentControlModel(in T content) => Content = content;

        public ContentControlModel(in BindingDirection bindingDirection) => BindingDirection = bindingDirection;

        public ContentControlModel(in T content, in BindingDirection bindingDirection)
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
        public HeaderedContentControlModel() { /* Left empty. */ }

        public HeaderedContentControlModel(in object header, in object content) : base(content) => Header = header;

        public HeaderedContentControlModel(in BindingDirection bindingDirection) : base(bindingDirection) { /* Left empty. */ }

        public HeaderedContentControlModel(in object header, in object content, in BindingDirection bindingDirection) : base(content, bindingDirection) => Header = header;

        protected virtual void OnHeaderChanged(object oldValue)
        {
            this.TryReset(oldValue);

            this.TryUpdate(_header, BindingDirection);

        }
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

        object IHeaderedControlModel.Header { get => Header; set => Header = (value ?? throw GetArgumentNullException(nameof(value))) is THeader _value ? _value : throw GetExceptionForInvalidType<THeader>(value.GetType().ToString(), nameof(value)); }

        /// <summary>
        /// Initializes a new instance of the <see cref="HeaderedContentControlModel{THeader, TContent}"/> class.
        /// </summary>
        public HeaderedContentControlModel() { /* Left empty. */ }

        public HeaderedContentControlModel(in THeader header, in TContent content) : base(content) => Header = header;

        public HeaderedContentControlModel(in BindingDirection bindingDirection) : base(bindingDirection) { }

        public HeaderedContentControlModel(in THeader header, in TContent content, in BindingDirection bindingDirection) : base(content, bindingDirection) => Header = header;

        protected virtual void OnHeaderChanged(THeader oldValue)
        {
            this.TryReset(oldValue);

            this.TryUpdate(_header, BindingDirection);
        }
    }

    /// <summary>
    /// Represents a model that corresponds to a default view for <see cref="ItemsControl"/>s.
    /// </summary>
    [TypeForDataTemplate(typeof(IItemsControlModel))]
    public class ItemsControlModel : Control, IItemsControlModel, IDataTemplateSelectorsModel
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
        public ItemsControlModel() { /* Left empty. */ }

        public ItemsControlModel(in IEnumerable items) => Items = items;

        public ItemsControlModel(in BindingDirection bindingDirection) => BindingDirection = bindingDirection;

        public ItemsControlModel(in IEnumerable items, in BindingDirection bindingDirection)
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
    [TypeForDataTemplate(typeof(IItemsControlModel))]
    public class ItemsControlModel<T> : ControlModel, IItemsControlModel<T>, IDataTemplateSelectorsModel
    {
        private IEnumerable<T> _items;

        /// <summary>
        /// Gets or sets the items of this <see cref="ItemsControlModel{T}"/>.
        /// </summary>
        public IEnumerable<T> Items { get => _items; set { IEnumerable<T> oldItems = _items; _items = value; OnItemsChanged(oldItems); } }

        IEnumerable IItemsControlModel.Items { get => Items; set => Items = (value ?? throw GetArgumentNullException(nameof(value))) is IEnumerable<T> _value ? _value : throw GetExceptionForInvalidType<IEnumerable<T>>(value.GetType().ToString(), nameof(value)); }

        public IModelDataTemplateSelectors ModelDataTemplateSelectors { get; set; }

        public bool AutoAddDataTemplateSelectors { get; set; }

        public BindingDirection BindingDirection { get; } = BindingDirection.OneWay;

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemsControlModel{T}"/> class.
        /// </summary>
        public ItemsControlModel() { /* Left empty. */ }

        public ItemsControlModel(in IEnumerable<T> items) => Items = items;

        public ItemsControlModel(in BindingDirection bindingDirection) => BindingDirection = bindingDirection;

        public ItemsControlModel(in IEnumerable<T> items, in BindingDirection bindingDirection)
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
        public HeaderedItemsControlModel() { /* Left empty. */ }

        public HeaderedItemsControlModel(in object header, in IEnumerable items) : base(items) => Header = header;

        public HeaderedItemsControlModel(in BindingDirection bindingDirection) : base(bindingDirection) { /* Left empty. */ }

        public HeaderedItemsControlModel(in object header, in IEnumerable items, in BindingDirection bindingDirection) : base(items, bindingDirection) => Header = header;

        protected virtual void OnHeaderChanged(object oldValue)
        {
            this.TryReset(oldValue);

            this.TryUpdate(_header, BindingDirection);
        }
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
        public HeaderedItemsControlModel() { /* Left empty. */ }

        public HeaderedItemsControlModel(in THeader header, in IEnumerable<TItems> items) : base(items) => Header = header;

        public HeaderedItemsControlModel(in BindingDirection bindingDirection) : base(bindingDirection) { /* Left empty. */ }

        public HeaderedItemsControlModel(in THeader header, in IEnumerable<TItems> items, in BindingDirection bindingDirection) : base(items, bindingDirection) => Header = header;

        protected virtual void OnHeaderChanged(THeader oldValue)
        {
            this.TryReset(oldValue);

            this.TryUpdate(_header, BindingDirection);
        }
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
        public GroupBoxModel() { /* Left empty. */ }

        public GroupBoxModel(in object header, in object content) : base(header, content) { /* Left empty. */ }

        public GroupBoxModel(in BindingDirection bindingDirection) : base(bindingDirection) { /* Left empty. */ }

        public GroupBoxModel(in object header, in object content, in BindingDirection bindingDirection) : base(header, content, bindingDirection) { /* Left empty. */ }
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
        public GroupBoxModel() { /* Left empty. */ }

        public GroupBoxModel(in THeader header, in TContent content) : base(header, content) { /* Left empty. */ }

        public GroupBoxModel(in BindingDirection bindingDirection) : base(bindingDirection) { /* Left empty. */ }

        public GroupBoxModel(in THeader header, in TContent content, in BindingDirection bindingDirection) : base(header, content, bindingDirection) { /* Left empty. */ }
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
        public TabItemModel() { /* Left empty. */ }

        public TabItemModel(in object header, in object content) : base(header, content) { /* Left empty. */ }

        public TabItemModel(in BindingDirection bindingDirection) : base(bindingDirection) { /* Left empty. */ }

        public TabItemModel(in object header, in object content, in BindingDirection bindingDirection) : base(header, content, bindingDirection) { /* Left empty. */ }
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
        public TabItemModel() { /* Left empty. */ }

        public TabItemModel(in THeader header, in TContent content) : base(header, content) { /* Left empty. */ }

        public TabItemModel(in BindingDirection bindingDirection) : base(bindingDirection) { /* Left empty. */ }

        public TabItemModel(in THeader header, in TContent content, in BindingDirection bindingDirection) : base(header, content, bindingDirection) { /* Left empty. */ }
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
    // Left empty.
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
        public ButtonModel() { /* Left empty. */ }

        /// <summary>
        /// Initializes a new instance of the <see cref="ButtonModel"/> class with a custom content.
        /// </summary>
        public ButtonModel(in object content) : base(content) { /* Left empty. */ }

        /// <summary>
        /// Initializes a new instance of the <see cref="ButtonModel"/> class with a custom <see cref="BindingDirection"/>.
        /// </summary>
        public ButtonModel(in BindingDirection bindingDirection) : base(bindingDirection) { /* Left empty. */ }

        /// <summary>
        /// Initializes a new instance of the <see cref="ButtonModel"/> class with a custom content and <see cref="BindingDirection"/>.
        /// </summary>
        public ButtonModel(in object content, in BindingDirection bindingDirection) : base(content, bindingDirection) { /* Left empty. */ }
    }

    /// <summary>
    /// Represents a model that corresponds to a default view for <see cref="Button"/>s.
    /// </summary>
    [TypeForDataTemplate(typeof(IButtonModel))]
    public class ButtonModel<TContent> : ContentControlModel<TContent>, IButtonModel<TContent>
    {
        /// <summary>
        /// Gets or sets the command of this <see cref="ButtonModel{TContent,TCommandParameter}"/>.
        /// </summary>
        public ICommand Command { get; set; }

        /// <summary>
        /// Gets or sets the command parameter of this <see cref="ButtonModel{TContent,TCommandParameter}"/>.
        /// </summary>
        public object CommandParameter { get; set; }

        /// <summary>
        /// Gets or sets the command target of this <see cref="ButtonModel{TContent,TCommandParameter}"/>.
        /// </summary>
        public IInputElement CommandTarget { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ButtonModel{TContent,TCommandParameter}"/> class.
        /// </summary>
        public ButtonModel() { /* Left empty. */ }

        public ButtonModel(in TContent content) : base(content) { /* Left empty. */ }

        public ButtonModel(in BindingDirection bindingDirection) : base(bindingDirection) { /* Left empty. */ }

        public ButtonModel(in TContent content, in BindingDirection bindingDirection) : base(content, bindingDirection) { /* Left empty. */ }
    }

    /// <summary>
    /// Represents a model that corresponds to a default view for <see cref="Button"/>s.
    /// </summary>
    [TypeForDataTemplate(typeof(IButtonModel))]
    public class ButtonModel<TContent, TCommandParameter> : ContentControlModel<TContent>, IButtonModel<TContent, TCommandParameter>
    {
        /// <summary>
        /// Gets or sets the command of this <see cref="ButtonModel{TContent,TCommandParameter}"/>.
        /// </summary>
        public ICommand<TCommandParameter> Command { get; set; }

        /// <summary>
        /// Gets or sets the command parameter of this <see cref="ButtonModel{TContent,TCommandParameter}"/>.
        /// </summary>
        public TCommandParameter CommandParameter { get; set; }

        object ICommandSource.CommandParameter => CommandParameter;

        object IButtonModel.CommandParameter { get => CommandParameter; set => CommandParameter = (TCommandParameter)value; }

        /// <summary>
        /// Gets or sets the command target of this <see cref="ButtonModel{TContent,TCommandParameter}"/>.
        /// </summary>
        public IInputElement CommandTarget { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ButtonModel{TContent,TCommandParameter}"/> class.
        /// </summary>
        public ButtonModel() { /* Left empty. */ }

        public ButtonModel(in TContent content) : base(content) { /* Left empty. */ }

        public ButtonModel(in BindingDirection bindingDirection) : base(bindingDirection) { /* Left empty. */ }

        public ButtonModel(in TContent content, in BindingDirection bindingDirection) : base(content, bindingDirection) { /* Left empty. */ }

#if CS8
        ICommand ICommandSource.Command => Command;
#else
        ICommand IButtonModel.Command { get => Command; set => Command = (ICommand<TCommandParameter>)value; }

        ICommand ICommandSource.Command => Command;
#endif
    }

    public class ExtendedButtonModel<TContent, TCommandParameter> : ButtonModel<TContent, TCommandParameter>, IExtendedButtonModel<TContent, TCommandParameter>
    {
        public object ToolTip { get; set; }

        public object ContentDecoration { get; set; }

        public ExtendedButtonModel() { /* Left empty. */ }

        public ExtendedButtonModel(in TContent content) : base(content) { /* Left empty. */ }

        public ExtendedButtonModel(in BindingDirection bindingDirection) : base(bindingDirection) { /* Left empty. */ }

        public ExtendedButtonModel(in TContent content, in BindingDirection bindingDirection) : base(content, bindingDirection) { /* Left empty. */ }
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
        public ToggleButtonModel() { /* Left empty. */ }

        public ToggleButtonModel(in object content) : base(content) { /* Left empty. */ }

        public ToggleButtonModel(in BindingDirection bindingDirection) : base(bindingDirection) { /* Left empty. */ }

        public ToggleButtonModel(in object content, in BindingDirection bindingDirection) : base(content, bindingDirection) { /* Left empty. */ }
    }

    /// <summary>
    /// Represents a model that corresponds to a default view for <see cref="ToggleButton"/>s.
    /// </summary>
    [TypeForDataTemplate(typeof(IToggleButtonModel))]
    public class ToggleButtonModel<TContent, TCommandParameter> : ButtonModel<TContent, TCommandParameter>, IToggleButtonModel<TContent, TCommandParameter>
    {
        /// <summary>
        /// Gets or sets a value that indeicates whether this <see cref="ToggleButtonModel{TContent, TCommandParameter}"/> is checked.
        /// </summary>
        public bool? IsChecked { get; set; }

        /// <summary>
        /// Gets or sets a value that indeicates whether this <see cref="ToggleButtonModel{TContent, TCommandParameter}"/> is three state.
        /// </summary>
        public bool IsThreeState { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ToggleButtonModel{TContent, TCommandParameter}"/> class.
        /// </summary>
        public ToggleButtonModel() { /* Left empty. */ }

        public ToggleButtonModel(in TContent content) : base(content) { /* Left empty. */ }

        public ToggleButtonModel(in BindingDirection bindingDirection) : base(bindingDirection) { /* Left empty. */ }

        public ToggleButtonModel(in TContent content, in BindingDirection bindingDirection) : base(content, bindingDirection) { /* Left empty. */ }
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
        public CheckBoxModel() { /* Left empty. */ }

        public CheckBoxModel(in object content) : base(content) { /* Left empty. */ }

        public CheckBoxModel(in BindingDirection bindingDirection) : base(bindingDirection) { /* Left empty. */ }

        public CheckBoxModel(in object content, in BindingDirection bindingDirection) : base(content, bindingDirection) { /* Left empty. */ }
    }

    /// <summary>
    /// Represents a model that corresponds to a default view for <see cref="CheckBox"/>'.
    /// </summary>
    [TypeForDataTemplate(typeof(ICheckBoxModel))]
    public class CheckBoxModel<TContent, TCommandParameter> : ToggleButtonModel<TContent, TCommandParameter>, ICheckBoxModel<TContent, TCommandParameter>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CheckBoxModel{TContent, TCommandParameter}"/> class.
        /// </summary>
        public CheckBoxModel() { /* Left empty. */ }

        public CheckBoxModel(in TContent content) : base(content) { /* Left empty. */ }

        public CheckBoxModel(in BindingDirection bindingDirection) : base(bindingDirection) { /* Left empty. */ }

        public CheckBoxModel(in TContent content, in BindingDirection bindingDirection) : base(content, bindingDirection) { /* Left empty. */ }
    }

    /// <summary>
    /// Represents a model that corresponds to a default view for <see cref="TextBox"/>'s.
    /// </summary>
    [TypeForDataTemplate(typeof(ITextBoxModelTextOriented))]
    public class TextBoxModelTextOriented : ControlModel, ITextBoxModelTextOriented
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
    [TypeForDataTemplate(typeof(IRadioButtonCollection))]
    public class RadioButtonCollection : System.Collections.Generic.List<IRadioButtonModel>, IRadioButtonCollection
    {
        public string GroupName { get; set; }
    }

    /// <summary>
    /// Represents a model that corresponds to a default view for <see cref="RadioButton"/> collection.
    /// </summary>
    [TypeForDataTemplate(typeof(IRadioButtonCollection))]
    public class RadioButtonCollection<TContent, TCommandParameter> : List<IRadioButtonModel<TContent, TCommandParameter>>, IRadioButtonCollection<TContent, TCommandParameter>
    {
        public string GroupName { get; set; }

        IEnumerator<IRadioButtonModel> IEnumerable<IRadioButtonModel>.GetEnumerator() => GetEnumerator();
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
        public RadioButtonModel() { /* Left empty. */ }

        public RadioButtonModel(in object content) : base(content) { /* Left empty. */ }

        public RadioButtonModel(in BindingDirection bindingDirection) : base(bindingDirection) { /* Left empty. */ }

        public RadioButtonModel(in object content, in BindingDirection bindingDirection) : base(content, bindingDirection) { /* Left empty. */ }
    }

    /// <summary>
    /// Represents a model that corresponds to a default view for <see cref="RadioButton"/>s.
    /// </summary>
    [TypeForDataTemplate(typeof(IRadioButtonModel))]
    public class RadioButtonModel<TContent, TCommandParameter> : ToggleButtonModel<TContent, TCommandParameter>, IRadioButtonModel<TContent, TCommandParameter>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RadioButtonModel{TContent, TCommandParameter}"/> class.
        /// </summary>
        public RadioButtonModel() { /* Left empty. */ }

        public RadioButtonModel(in TContent content) : base(content) { /* Left empty. */ }

        public RadioButtonModel(in BindingDirection bindingDirection) : base(bindingDirection) { /* Left empty. */ }

        public RadioButtonModel(in TContent content, in BindingDirection bindingDirection) : base(content, bindingDirection) { /* Left empty. */ }
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
        public GroupingRadioButtonModel() { /* Left empty. */ }

        public GroupingRadioButtonModel(in object content) : base(content) { /* Left empty. */ }

        public GroupingRadioButtonModel(in BindingDirection bindingDirection) : base(bindingDirection) { /* Left empty. */ }

        public GroupingRadioButtonModel(in object content, in BindingDirection bindingDirection) : base(content, bindingDirection) { /* Left empty. */ }
    }

    /// <summary>
    /// Represents a model that corresponds to a default view for <see cref="RadioButton"/>s.
    /// </summary>
    [TypeForDataTemplate(typeof(IGroupingRadioButtonModel))]
    public class GroupingRadioButtonModel<TContent, TCommandParameter> : RadioButtonModel<TContent, TCommandParameter>, IGroupingRadioButtonModel<TContent, TCommandParameter>
    {
        public string GroupName { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="GroupingRadioButtonModel{TContent, TCommandParameter}"/> class.
        /// </summary>
        public GroupingRadioButtonModel() { /* Left empty. */ }

        public GroupingRadioButtonModel(in TContent content) : base(content) { /* Left empty. */ }

        public GroupingRadioButtonModel(in BindingDirection bindingDirection) : base(bindingDirection) { /* Left empty. */ }

        public GroupingRadioButtonModel(in TContent content, in BindingDirection bindingDirection) : base(content, bindingDirection) { /* Left empty. */ }
    }

    public abstract class MenuItemModelBase<T> : ControlModel
    {
        public T Header { get; set; }

        protected MenuItemModelBase() { /* Left empty. */ }

        protected MenuItemModelBase(in T header) => Header = header;
    }

    [TypeForDataTemplate(typeof(IMenuItemModel))]
    public class MenuItemModel<T> : MenuItemModelBase<T>, IMenuItemModel<T>
    {
        public IEnumerable Items { get; set; }

        public ICommand Command { get; set; }

        public object CommandParameter { get; set; }

        public IInputElement CommandTarget { get; set; }

        public MenuItemModel() { /* Left empty. */ }

        public MenuItemModel(in T header, in ICommand command) : base(header) => Command = command;

        public MenuItemModel(in T header, in ICommand command, in object commandParameter) : this(header, command) => CommandParameter = commandParameter;

#if !CS8
        object IHeaderedControlModel.Header { get => Header; set => Header = (T)value; }
#endif
    }

    [TypeForDataTemplate(typeof(IMenuItemModel))]
    public class MenuItemModel<THeader, TItems, TCommandParameter> : MenuItemModelBase<THeader>, IMenuItemModel<THeader, TItems, TCommandParameter>
    {
        public System.Collections.Generic.IEnumerable<TItems> Items { get; set; }

        public ICommand<TCommandParameter> Command { get; set; }

        public TCommandParameter CommandParameter { get; set; }

        public IInputElement CommandTarget { get; set; }

        public MenuItemModel() { /* Left empty. */ }

        public MenuItemModel(in THeader header, in ICommand<TCommandParameter> command) : base(header) => Command = command;

        public MenuItemModel(in THeader header, in ICommand<TCommandParameter> command, in TCommandParameter commandParameter) : this(header, command) => CommandParameter = commandParameter;

#if !CS8
        object IHeaderedControlModel.Header { get => Header; set => Header = (THeader)value; }

        ICommand ICommandSource.Command => Command;

        ICommand IMenuItemModel.Command { get => Command; set => Command = (ICommand<TCommandParameter>)value; }

        object ICommandSource.CommandParameter => CommandParameter;

        object IMenuItemModel.CommandParameter { get => CommandParameter; set => CommandParameter = (TCommandParameter)value; }

        IEnumerable IItemsControlModel.Items { get => Items; set => Items=(IEnumerable<TItems>)value; }
#endif
    }
}
