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

using WinCopies.GUI.Windows.Dialogs.Models;
using WinCopies.Util.Data;

namespace WinCopies.GUI.Windows.Dialogs.ViewModels
{
    public class DialogViewModel<T> : ViewModel<T>, IDialogModel where T : IDialogModel
    {
        /// <summary>
        /// Gets or sets the title of this <see cref="DialogViewModel{T}"/>.
        /// </summary>
        public string Title { get => ModelGeneric.Title; set => Update(nameof(Title), value, typeof(IDialogModel)); }

        /// <summary>
        /// Gets or sets the <see cref="Dialogs.DialogButton"/> value of this <see cref="DialogViewModel{T}"/>.
        /// </summary>
        public DialogButton DialogButton { get => ModelGeneric.DialogButton; set => Update(nameof(DialogButton), value, typeof(IDialogModel)); }

        /// <summary>
        /// Gets or sets the <see cref="Dialogs.DefaultButton"/> value of this <see cref="DialogViewModel{T}"/>.
        /// </summary>
        public DefaultButton DefaultButton { get => ModelGeneric.DefaultButton; set => Update(nameof(DefaultButton), value, typeof(IDialogModel)); }

        /// <summary>
        /// Initializes a new instance of the <see cref="DialogViewModel{T}"/> class.
        /// </summary>
        /// <param name="model">The <typeparamref name="T"/> model to wrap in this <see cref="ViewModel{T}"/>.</param>
        public DialogViewModel(T model) : base(model) { }
    }

    //public class PropertyDialogViewModel<T> : DialogViewModel<T>, IPropertyDialogModel where T : IPropertyDialogModel
    //{
    //    /// <summary>
    //    /// Gets or sets the items of this <see cref="PropertyDialogViewModel{T}"/>.
    //    /// </summary>
    //    public IEnumerable<IPropertyTabItemModel> Items { get => ModelGeneric.Items; set => Update(nameof(Items), value, typeof(IPropertyDialogModel)); }

    //    /// <summary>
    //    /// Initializes a new instance of the <see cref="PropertyDialogViewModel{T}"/> class.
    //    /// </summary>
    //    /// <param name="model">The <typeparamref name="T"/> model to wrap in this <see cref="ViewModel{T}"/>.</param>
    //    public PropertyDialogViewModel(T model) : base(model) { }
    //}
}
