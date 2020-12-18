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

using static WinCopies.Util.Util;

namespace WinCopies.GUI.Controls.ViewModels
{
    [TypeForDataTemplate(typeof(IHeaderedContentControlModel))]
    public class HeaderedContentControlViewModel<T> : ContentControlViewModel<T>, IHeaderedContentControlModel where T : IHeaderedContentControlModel
    {
        public object Header
        {
            get => ModelGeneric.Header;

            set
            {
                object oldValue = ModelGeneric.Header;

                ModelGeneric.Header = value;

                OnHeaderChanged(oldValue);
            }
        }

        public HeaderedContentControlViewModel(T model) : base(model) { }

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

    [TypeForDataTemplate(typeof(IHeaderedContentControlModel))]
    public class HeaderedContentControlViewModel<TModel, THeader, TContent> : ContentControlViewModel<TModel, TContent>, IHeaderedContentControlModel<THeader, TContent> where TModel : IHeaderedContentControlModel<THeader, TContent>
    {
        public THeader Header
        {
            get => ModelGeneric.Header;

            set
            {
                object oldValue = ModelGeneric.Header;

                ModelGeneric.Header = value;

                OnHeaderChanged(oldValue);
            }
        }

        object IHeaderedControlModel.Header { get => Header; set => Header = (value ?? throw GetArgumentNullException(nameof(value))) is THeader _value ? _value : throw GetExceptionForInvalidType<THeader>(value.GetType().ToString(), nameof(value)); }

        public HeaderedContentControlViewModel(TModel model) : base(model) { }

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
