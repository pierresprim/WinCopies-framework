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

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

using WinCopies.Commands;
using WinCopies.GUI.Controls.Models;
using WinCopies.Util.Data;

#if !CS8
using System.Collections;
#endif

namespace WinCopies.GUI.Controls.ViewModels
{
    [TypeForDataTemplate(typeof(IGroupBoxModel))]
    public class GroupBoxViewModel<T> : HeaderedContentControlViewModel<T>, IGroupBoxModel where T : IGroupBoxModel
    {
        public GroupBoxViewModel(T model) : base(model) { }
    }

    [TypeForDataTemplate(typeof(IGroupBoxModel))]
    public class GroupBoxViewModel<TModel, THeader, TContent> : HeaderedContentControlViewModel<TModel, THeader, TContent>, IGroupBoxModel<THeader, TContent> where TModel : IGroupBoxModel<THeader, TContent>
    {
        public GroupBoxViewModel(TModel model) : base(model) { }
    }

    [TypeForDataTemplate(typeof(ITabItemModel))]
    public class TabItemViewModel<T> : HeaderedContentControlViewModel<T>, ITabItemModel where T : ITabItemModel
    {
        public TabItemViewModel(T model) : base(model) { }
    }

    [TypeForDataTemplate(typeof(ITabItemModel))]
    public class TabItemViewModel<TModel, THeader, TContent> : HeaderedContentControlViewModel<TModel, THeader, TContent>, ITabItemModel<THeader, TContent> where TModel : ITabItemModel<THeader, TContent>
    {
        public TabItemViewModel(TModel model) : base(model) { }
    }

    //[TypeForDataTemplate(typeof(IPropertyTabItemModel))]
    //public class PropertyTabItemViewModel<T> : HeaderedItemsControlViewModel<T, object, IGroupBoxModel>, IPropertyTabItemModel where T : IPropertyTabItemModel
    //{
    //    public PropertyTabItemViewModel(T model) : base(model) { }
    //}

    //[TypeForDataTemplate(typeof(IPropertyTabItemModel))]
    //public class PropertyTabItemViewModel<TModel, TItemHeader, TGroupBoxHeader, TGroupBoxContent> : HeaderedItemsControlViewModel<TModel, TItemHeader, IGroupBoxModel<TGroupBoxHeader, TGroupBoxContent>>, IPropertyTabItemModel<TItemHeader, TGroupBoxHeader, TGroupBoxContent> where TModel : IPropertyTabItemModel<TItemHeader, TGroupBoxHeader, TGroupBoxContent>
    //{
    //    IEnumerable<IGroupBoxModel> IPropertyTabItemModel.Items { get => Items; set => Items = GetOrThrowIfNotType<IEnumerable<IGroupBoxModel<TGroupBoxHeader, TGroupBoxContent>>>(value, nameof(value)); }

    //    public PropertyTabItemViewModel(TModel model) : base(model) { }
    //}

    [TypeForDataTemplate(typeof(IButtonModel))]
    public class ButtonViewModel<T> : ContentControlViewModel<T>, IButtonModel where T : IButtonModel
    {
        public ICommand Command { get => ModelGeneric.Command; set => Update(nameof(Command), value, typeof(IButtonModel)); }

        public object CommandParameter { get => ModelGeneric.CommandParameter; set => Update(nameof(CommandParameter), value, typeof(IButtonModel)); }

        public IInputElement CommandTarget { get => ModelGeneric.CommandTarget; set => Update(nameof(CommandTarget), value, typeof(IButtonModel)); }

        public ButtonViewModel(T model) : base(model) { }
    }

    [TypeForDataTemplate(typeof(IButtonModel))]
    public class ButtonViewModel<TModel, TContent> : ContentControlViewModel<TModel, TContent>, IButtonModel<TContent> where TModel : IButtonModel<TContent>
    {
        public ICommand Command { get => ModelGeneric.Command; set => Update(nameof(Command), value, typeof(IButtonModel)); }

        public object CommandParameter { get => ModelGeneric. CommandParameter; set => Update(nameof(CommandParameter), value, typeof(IButtonModel)); }

        public IInputElement CommandTarget { get => ModelGeneric.CommandTarget; set => Update(nameof(CommandTarget), value, typeof(IButtonModel)); }

        public ButtonViewModel(TModel model) : base(model) { /* Left empty. */ }
    }

    [TypeForDataTemplate(typeof(IButtonModel))]
    public class ButtonViewModel<TModel, TContent, TCommandParameter> : ContentControlViewModel<TModel, TContent>, IButtonModel<TContent, TCommandParameter> where TModel : IButtonModel<TContent, TCommandParameter>
    {
        public ICommand<TCommandParameter> Command { get => ModelGeneric.Command; set => Update(nameof(Command), value, typeof(IButtonModel)); }

        ICommand ICommandSource.Command => Command;

        public TCommandParameter CommandParameter { get => ModelGeneric. CommandParameter; set => Update(nameof(CommandParameter), value, typeof(IButtonModel)); }

        object IButtonModel.CommandParameter { get => ((IButtonModel)ModelGeneric).CommandParameter; set => ((IButtonModel)ModelGeneric).CommandParameter = value; }

        object ICommandSource.CommandParameter => ((ICommandSource)ModelGeneric).CommandParameter;

        public IInputElement CommandTarget { get => ModelGeneric.CommandTarget; set => Update(nameof(CommandTarget), value, typeof(IButtonModel)); }

        public ButtonViewModel(TModel model) : base(model) { /* Left empty. */ }

#if !CS8
        ICommand IButtonModel.Command { get => Command; set => Command = (ICommand<TCommandParameter>)value; }
#endif
    }

    [TypeForDataTemplate(typeof(IExtendedButtonModel))]
    public class ExtendedButtonViewModel<TModel, TContent, TCommandParameter> : ButtonViewModel<TModel, TContent, TCommandParameter>, IExtendedButtonModel<TContent, TCommandParameter> where TModel : IExtendedButtonModel<TContent, TCommandParameter>
    {
        public object ToolTip { get => ModelGeneric.ToolTip; set => Update(nameof(ToolTip), value, typeof(IToolTipControlModel)); }

        public object ContentDecoration { get => ModelGeneric.ContentDecoration; set => Update(nameof(ContentDecoration), value, typeof(IContentDecorationControl)); }

        public ExtendedButtonViewModel(TModel model) : base(model) { }
    }

    [TypeForDataTemplate(typeof(IToggleButtonModel))]
    public class ToggleButtonViewModel<T> : ButtonViewModel<T>, IToggleButtonModel where T : IToggleButtonModel
    {
        public bool? IsChecked { get => ModelGeneric.IsChecked; set => Update(nameof(IsChecked), value, typeof(IToggleButtonModel)); }

        public bool IsThreeState { get => ModelGeneric.IsThreeState; set => Update(nameof(IsThreeState), value, typeof(IToggleButtonModel)); }

        public ToggleButtonViewModel(T model) : base(model) { }
    }

    [TypeForDataTemplate(typeof(IToggleButtonModel))]
    public class ToggleButtonViewModel<TModel, TContent, TCommandParameter> : ButtonViewModel<TModel, TContent, TCommandParameter>, IToggleButtonModel<TContent, TCommandParameter> where TModel : IToggleButtonModel<TContent, TCommandParameter>
    {
        public bool? IsChecked { get => ModelGeneric.IsChecked; set => Update(nameof(IsChecked), value, typeof(IToggleButtonModel)); }

        public bool IsThreeState { get => ModelGeneric.IsThreeState; set => Update(nameof(IsThreeState), value, typeof(IToggleButtonModel)); }

        public ToggleButtonViewModel(TModel model) : base(model) { }
    }

    [TypeForDataTemplate(typeof(ICheckBoxModel))]
    public class CheckBoxViewModel<T> : ToggleButtonViewModel<T>, ICheckBoxModel where T : ICheckBoxModel
    {
        public CheckBoxViewModel(T model) : base(model) { }
    }

    [TypeForDataTemplate(typeof(ICheckBoxModel))]
    public class CheckBoxViewModel<TModel, TContent, TCommandParameter> : ToggleButtonViewModel<TModel, TContent, TCommandParameter>, ICheckBoxModel<TContent, TCommandParameter> where TModel : ICheckBoxModel<TContent, TCommandParameter>
    {
        public CheckBoxViewModel(TModel model) : base(model) { }
    }

    [TypeForDataTemplate(typeof(IRadioButtonCollection))]
    public class ObservableRadioButtonCollection : ObservableCollection<IRadioButtonModel>, IRadioButtonCollection
    {
        public string GroupName
        {
            get => ((RadioButtonCollection)Items).GroupName;

            set
            {
                ((RadioButtonCollection)Items).GroupName = value;

                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs(nameof(GroupName)));
            }
        }

        public ObservableRadioButtonCollection(RadioButtonCollection items) : base(items) { }
    }

    [TypeForDataTemplate(typeof(IRadioButtonCollection))]
    public class ObservableRadioButtonCollection<TContent, TCommandParameter> : ObservableCollection<IRadioButtonModel<TContent, TCommandParameter>>, IRadioButtonCollection<TContent, TCommandParameter>
    {
        public string GroupName
        {
            get => ((RadioButtonCollection)Items).GroupName;

            set
            {
                ((RadioButtonCollection)Items).GroupName = value;

                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs(nameof(GroupName)));
            }
        }

        IEnumerator<IRadioButtonModel> IEnumerable<IRadioButtonModel>.GetEnumerator() => GetEnumerator();

        public ObservableRadioButtonCollection(RadioButtonCollection<TContent, TCommandParameter> items) : base(items) { }
    }

    [TypeForDataTemplate(typeof(IGroupingRadioButtonModel))]
    public class GroupingRadioButtonViewModel<T> : ToggleButtonViewModel<T>, IGroupingRadioButtonModel where T : IGroupingRadioButtonModel
    {
        public string GroupName { get => ModelGeneric.GroupName; set => Update(nameof(GroupName), value, typeof(IGroupingRadioButtonModel)); }

        public GroupingRadioButtonViewModel(T model) : base(model) { }
    }

    [TypeForDataTemplate(typeof(IGroupingRadioButtonModel))]
    public class GroupingRadioButtonViewModel<TModel, TContent, TCommandParameter> : ToggleButtonViewModel<TModel, TContent, TCommandParameter>, IGroupingRadioButtonModel where TModel : IGroupingRadioButtonModel<TContent, TCommandParameter>
    {
        public string GroupName { get => ModelGeneric.GroupName; set => Update(nameof(GroupName), value, typeof(IGroupingRadioButtonModel)); }

        public GroupingRadioButtonViewModel(TModel model) : base(model) { }
    }

    public abstract class MenuItemViewModelBase<TModel, THeader> : ViewModel<TModel> where TModel : IHeaderedControlModel<THeader>
    {
        public bool IsEnabled { get => ModelGeneric.IsEnabled; set => Update(nameof(IsEnabled), value, typeof(IControlModel)); }

        public THeader Header { get => ModelGeneric.Header; set { ModelGeneric.Header = value; OnPropertyChanged(nameof(Header)); } }

        protected MenuItemViewModelBase(in TModel model) : base(model) { /* Left empty. */ }
    }


    [TypeForDataTemplate(typeof(IMenuItemModel))]
    public class MenuItemViewModel<TModel, THeader> : MenuItemViewModelBase<TModel, THeader>, IMenuItemModel<THeader> where TModel : IMenuItemModel<THeader>
    {
        public ICommand Command { get => ModelGeneric.Command; set { ModelGeneric.Command = value; OnPropertyChanged(nameof(Command)); } }

        public object CommandParameter { get => ModelGeneric.CommandParameter; set { ModelGeneric.CommandParameter = value; OnPropertyChanged(nameof(CommandParameter)); } }

        public IInputElement CommandTarget { get => ModelGeneric.CommandTarget; set => Update(nameof(CommandTarget), value, typeof(IMenuItemModel)); }

        public System.Collections.IEnumerable Items { get => ModelGeneric.Items; set { ModelGeneric.Items = value; OnPropertyChanged(nameof(Items)); } }

        public MenuItemViewModel(TModel model) : base(model) { /* Left empty. */ }

#if !CS8
        object IHeaderedControlModel.Header { get => Header; set => Header = (THeader)value; }
#endif
    }

    [TypeForDataTemplate(typeof(IMenuItemModel))]
    public class MenuItemViewModel<TModel, THeader, TItems, TCommandParameter> : MenuItemViewModelBase<TModel, THeader>, IMenuItemModel<THeader, TItems, TCommandParameter> where TModel : IMenuItemModel<THeader, TItems, TCommandParameter>
    {
        public bool IsEnabled { get => ModelGeneric.IsEnabled; set => Update(nameof(IsEnabled), value, typeof(IControlModel)); }

        public THeader Header { get => ModelGeneric.Header; set { ModelGeneric.Header = value; OnPropertyChanged(nameof(Header)); } }

        public ICommand<TCommandParameter> Command { get => ModelGeneric.Command; set { ModelGeneric.Command = value; OnPropertyChanged(nameof(Command)); } }

        public TCommandParameter CommandParameter { get => ModelGeneric.CommandParameter; set { ModelGeneric.CommandParameter = value; OnPropertyChanged(nameof(CommandParameter)); } }

        public IInputElement CommandTarget { get => ModelGeneric.CommandTarget; set => Update(nameof(CommandTarget), value, typeof(IMenuItemModel)); }

        public IEnumerable<TItems> Items { get => ModelGeneric.Items; set { ModelGeneric.Items = value; OnPropertyChanged(nameof(Items)); } }

        public MenuItemViewModel(TModel model) : base(model) { /* Left empty. */ }

#if !CS8
        ICommand IMenuItemModel.Command { get => Command; set => Command = (ICommand<TCommandParameter>)value; }

        object IMenuItemModel.CommandParameter { get => CommandParameter; set => CommandParameter = (TCommandParameter)value; }

        IEnumerable IItemsControlModel.Items { get => Items; set => Items = (IEnumerable<TItems>)value; }

        object IHeaderedControlModel.Header { get => Header; set => Header = (THeader)value; }

        ICommand ICommandSource.Command => Command;

        object ICommandSource.CommandParameter => CommandParameter;
#endif
    }
}
