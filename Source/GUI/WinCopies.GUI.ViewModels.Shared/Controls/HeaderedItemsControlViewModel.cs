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

using WinCopies.GUI.Controls.Models;
using WinCopies.Util;

using static WinCopies.
    #if !WinCopies3
    Util.Util
#else
    ThrowHelper
    #endif
    ;

namespace WinCopies.GUI.Controls.ViewModels
{
    [TypeForDataTemplate(typeof(IHeaderedItemsControlModel))]
    public class HeaderedItemsControlViewModel<T> : ItemsControlViewModel<T>, IHeaderedItemsControlModel where T : IHeaderedItemsControlModel
    {
        public object Header
        {
            get => ModelGeneric.Header; set
            {
                object oldValue = ModelGeneric.Header;

                ModelGeneric.Header = value;

                OnHeaderChanged(oldValue);
            }
        }

        public HeaderedItemsControlViewModel(T model) : base(model) { }

        protected virtual void OnHeaderChanged(object oldValue)
        {
            if (!(ModelGeneric is IDataTemplateSelectorsModel))
            {
                this.TryReset(oldValue);

                this.TryUpdate(ModelGeneric.Header, BindingDirection);
            }

            OnPropertyChanged(nameof(Header));
        }
    }

    [TypeForDataTemplate(typeof(IHeaderedItemsControlModel))]
    public class HeaderedItemsControlViewModel<TModel, THeader, TItems> : ItemsControlViewModel<TModel, TItems>, IHeaderedItemsControlModel<THeader, TItems> where TModel : IHeaderedItemsControlModel<THeader, TItems>
    {
        public THeader Header { get => ModelGeneric.Header; set { object oldValue = ModelGeneric.Header; ModelGeneric.Header = value; OnHeaderChanged(oldValue); } }

        object IHeaderedControlModel.Header { get => Header; set => Header = (value ?? GetArgumentNullException(nameof(value))) is THeader _value ? _value : throw GetExceptionForInvalidType<THeader>(value.GetType().ToString(), nameof(value)); }

        public HeaderedItemsControlViewModel(TModel model) : base(model) { }

        protected virtual void OnHeaderChanged(object oldValue)
        {
            if (!(ModelGeneric is IDataTemplateSelectorsModel))
            {
                this.TryReset(oldValue);

                this.TryUpdate(ModelGeneric.Header, BindingDirection);
            }

            OnPropertyChanged(nameof(Header));
        }
    }
}
