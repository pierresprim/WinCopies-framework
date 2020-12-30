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

using WinCopies.GUI.Controls.Models;
using WinCopies.Util.Data;

#if !WinCopies3
using WinCopies.Util;
#endif

using static WinCopies.
    #if WinCopies2
    Util.Util
#else
    ThrowHelper
    #endif
    ;

namespace WinCopies.GUI.Controls.ViewModels
{
    [TypeForDataTemplate(typeof(IItemsControlModel))]
    public class ItemsControlViewModel<T> : ViewModel<T>, IItemsControlModel, IDataTemplateSelectorsModel where T : IItemsControlModel
    {
        private IModelDataTemplateSelectors _modelDataTemplateSelectors;

        private bool _autoAddDataTemplateSelectors;

        public IEnumerable Items
        {
            get => ModelGeneric.Items;

            set
            {
                IEnumerable oldItems = ModelGeneric.Items;

                ModelGeneric.Items = value;

                OnItemsChanged(oldItems);
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

        public ItemsControlViewModel(T model) : base(model) { }

        protected virtual void OnItemsChanged(IEnumerable oldItems)
        {
            if (!(ModelGeneric is IDataTemplateSelectorsModel))
            {
                this.TryReset(oldItems);

                this.TryUpdate(ModelGeneric.Items, BindingDirection);
            }

            OnPropertyChanged(nameof(Items));
        }
    }

    [TypeForDataTemplate(typeof(IItemsControlModel))]
    public class ItemsControlViewModel<TModel, TItems> : ViewModel<TModel>, IItemsControlModel<TItems>, IDataTemplateSelectorsModel where TModel : IItemsControlModel<TItems>
    {
        private IModelDataTemplateSelectors _modelDataTemplateSelectors;

        private bool _autoAddDataTemplateSelectors;

        public IEnumerable<TItems> Items
        {
            get => ModelGeneric.Items;

            set
            {
                IEnumerable oldItems = ModelGeneric.Items;

                ModelGeneric.Items = value;

                OnItemsChanged(oldItems);
            }
        }

        IEnumerable IItemsControlModel.Items { get => Items; set => Items = (value ?? throw GetArgumentNullException(nameof(value))) is IEnumerable<TItems> _value ? _value : throw GetExceptionForInvalidType<IEnumerable<TItems>>(value.GetType().ToString(), nameof(value)); }

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

        public ItemsControlViewModel(TModel model) : base(model) { }

        protected virtual void OnItemsChanged(IEnumerable oldItems)
        {

            if (!(ModelGeneric is IDataTemplateSelectorsModel))
            {
                this.TryReset(oldItems);

                this.TryUpdate(ModelGeneric.Items, BindingDirection);
            }

            OnPropertyChanged(nameof(Items));
        }
    }
}
