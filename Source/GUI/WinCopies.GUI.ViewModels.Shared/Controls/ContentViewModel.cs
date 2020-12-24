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

using System.ComponentModel;

using WinCopies.Util.Data;

namespace WinCopies.GUI.Controls.ViewModels
{
    public interface ContentModel:INotifyPropertyChanged
    {
        object Content { get; set; }

        object Dialog { get; set; }
    }

    public class ContentViewModel<TContent, TDialog> : ViewModelBase
    {
        private TContent _content;
        private TDialog _dialog;

        public TContent Content
        {
            get => _content;

            set
            {
                _content = value;

                OnPropertyChanged(nameof(Content));
            }
        }

        public TDialog Dialog
        {
            get => _dialog;

            set
            {
                _dialog = value;

                OnPropertyChanged(nameof(Dialog));
            }
        }
    }
}
