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
using WinCopies.Util.Data;

using static WinCopies.
#if WinCopies3
    ThrowHelper
#else
    Util.Util;

using WinCopies.Util
    #endif
    ;

namespace WinCopies.GUI.Controls.ViewModels
{
    [TypeForDataTemplate(typeof(IContentControlModel))]
    public class ContentControlViewModel<T> : ViewModel<T>, IContentControlModel, IDataTemplateSelectorsModel where T : IContentControlModel
    {
        private IModelDataTemplateSelectors _modelDataTemplateSelectors;

        private bool _autoAddDataTemplateSelectors;

        public object Content
        {
            get => ModelGeneric.Content;

            set
            {
                object oldValue = ModelGeneric.Content;

                ModelGeneric.Content = value;

                OnContentChanged(oldValue);
            }
        }

        public IModelDataTemplateSelectors ModelDataTemplateSelectors
        {
            get => (ModelGeneric as IDataTemplateSelectorsModel)?.ModelDataTemplateSelectors ?? _modelDataTemplateSelectors;

            set
            {
                if (ModelGeneric is IDataTemplateSelectorsModel dataTemplateSelectorsModel)

                    dataTemplateSelectorsModel.ModelDataTemplateSelectors = value;

                else

                    _modelDataTemplateSelectors = value;

                OnPropertyChanged(nameof(ModelDataTemplateSelectors));
            }
        }

        public bool AutoAddDataTemplateSelectors
        {
            get => (ModelGeneric as IDataTemplateSelectorsModel)?.AutoAddDataTemplateSelectors ?? _autoAddDataTemplateSelectors;

            set
            {
                if (ModelGeneric is IDataTemplateSelectorsModel dataTemplateSelectorsModel)

                    dataTemplateSelectorsModel.AutoAddDataTemplateSelectors = value;

                else

                    _autoAddDataTemplateSelectors = value;

                OnPropertyChanged(nameof(AutoAddDataTemplateSelectors));
            }
        }

        public BindingDirection BindingDirection { get; } = BindingDirection.OneWay;

        public ContentControlViewModel(T model) : base(model) { }

        protected virtual void OnContentChanged(object oldValue)
        {
            if (!(ModelGeneric is IDataTemplateSelectorsModel))
            {
                this.TryReset(oldValue);

                this.TryUpdate(ModelGeneric.Content, BindingDirection);
            }

            OnPropertyChanged(nameof(Content));
        }
    }

    [TypeForDataTemplate(typeof(IContentControlModel))]
    public class ContentControlViewModel<TModel, TContent> : ViewModel<TModel>, IContentControlModel<TContent>, IDataTemplateSelectorsModel where TModel : IContentControlModel<TContent>
    {
        private IModelDataTemplateSelectors _modelDataTemplateSelectors;

        private bool _autoAddDataTemplateSelectors;

        public TContent Content
        {
            get => ModelGeneric.Content;

            set
            {
                object oldValue = ModelGeneric.Content;

                ModelGeneric.Content = value;

                OnContentChanged(oldValue);
            }
        }

        object IContentControlModel.Content { get => Content; set => Content = (value ?? throw GetArgumentNullException(nameof(value))) is TContent _value ? _value : throw GetExceptionForInvalidType<TContent>(value.GetType().ToString(), nameof(value)); }

        public IModelDataTemplateSelectors ModelDataTemplateSelectors
        {
            get => (ModelGeneric as IDataTemplateSelectorsModel)?.ModelDataTemplateSelectors ?? _modelDataTemplateSelectors;

            set
            {
                if (ModelGeneric is IDataTemplateSelectorsModel dataTemplateSelectorsModel)

                    dataTemplateSelectorsModel.ModelDataTemplateSelectors = value;

                else

                    _modelDataTemplateSelectors = value;

                OnPropertyChanged(nameof(ModelDataTemplateSelectors));
            }
        }

        public bool AutoAddDataTemplateSelectors
        {
            get => (ModelGeneric as IDataTemplateSelectorsModel)?.AutoAddDataTemplateSelectors ?? _autoAddDataTemplateSelectors;

            set
            {
                if (ModelGeneric is IDataTemplateSelectorsModel dataTemplateSelectorsModel)

                    dataTemplateSelectorsModel.AutoAddDataTemplateSelectors = value;

                else

                    _autoAddDataTemplateSelectors = value;

                OnPropertyChanged(nameof(AutoAddDataTemplateSelectors));
            }
        }

        public BindingDirection BindingDirection { get; } = BindingDirection.OneWay;

        public ContentControlViewModel(TModel model) : base(model) { }

        protected virtual void OnContentChanged(object oldValue)
        {
            if (!(ModelGeneric is IDataTemplateSelectorsModel))
            {
                this.TryReset(oldValue);

                this.TryUpdate(ModelGeneric.Content, BindingDirection);
            }

            OnPropertyChanged(nameof(Content));
        }
    }
}
