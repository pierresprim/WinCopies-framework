/* Copyright © Pierre Sprimont, 2021
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
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows.Media.Imaging;

namespace WinCopies.IO
{
    public interface IBitmapSourcesLinker : IBitmapSources
    {
        void LoadSmall();

        void LoadMedium();

        void LoadLarge();

        void LoadExtraLarge();

        void Load();

        void OnSmallLoaded();

        void OnMediumLoaded();

        void OnLargeLoaded();

        void OnExtraLargeLoaded();

        void OnBitmapSourcesLoaded();
    }

    public class BitmapSourcesLinker : BitmapSources<IBitmapSourceProvider>, IBitmapSourcesLinker, INotifyPropertyChanged
    {
        private const BindingFlags _flags = BindingFlags.Public | BindingFlags.Instance;

        private BitmapSource _small;
        private BitmapSource _medium;
        private BitmapSource _large;
        private BitmapSource _extraLarge;

        protected override BitmapSource SmallOverride => _small;

        protected override BitmapSource MediumOverride => _medium;

        protected override BitmapSource LargeOverride => _large;

        protected override BitmapSource ExtraLargeOverride => _extraLarge;

        public event PropertyChangedEventHandler PropertyChanged;

        public BitmapSourcesLinker(in IBitmapSourceProvider bitmapSourceProvider) : base(bitmapSourceProvider)
        {
            UpdateBitmapSource(ref _small, bitmapSourceProvider.Default.Small);
            UpdateBitmapSource(ref _medium, bitmapSourceProvider.Default.Medium);
            UpdateBitmapSource(ref _large, bitmapSourceProvider.Default.Large);
            UpdateBitmapSource(ref _extraLarge, bitmapSourceProvider.Default.ExtraLarge);
        }

        protected void UpdateBitmapSource(ref BitmapSource value, in BitmapSource newValue) => (value = newValue).Freeze();

        protected virtual void RaisePropertyChangedEvent(in PropertyChangedEventArgs e) => PropertyChanged?.Invoke(this, e);

        protected virtual void RaisePropertyChangedEvent(in string propertyName) => RaisePropertyChangedEvent(new PropertyChangedEventArgs(propertyName));

        public virtual void OnSmallLoaded() => RaisePropertyChangedEvent(nameof(Small));

        public virtual void OnMediumLoaded() => RaisePropertyChangedEvent(nameof(Medium));

        public virtual void OnLargeLoaded() => RaisePropertyChangedEvent(nameof(Large));

        public virtual void OnExtraLargeLoaded() => RaisePropertyChangedEvent(nameof(ExtraLarge));

        public virtual void OnBitmapSourcesLoaded() => InvokeMethods(name => name != nameof(OnBitmapSourcesLoaded) && name.StartsWith("On") && name.EndsWith("Loaded"));

        public void LoadSmall() => UpdateBitmapSource(ref _small, InnerObject.Sources.Small);

        public void LoadMedium() => UpdateBitmapSource(ref _medium, InnerObject.Sources.Medium);

        public void LoadLarge() => UpdateBitmapSource(ref _large, InnerObject.Sources.Large);

        public void LoadExtraLarge() => UpdateBitmapSource(ref _extraLarge, InnerObject.Sources.ExtraLarge);

        public virtual void Load() => InvokeMethods(name => name.Length > nameof(Load).Length && name.StartsWith(nameof(Load)));

        private void InvokeMethods(Predicate<string> predicate)
        {
            System.Collections.Generic.IEnumerable<MethodInfo> methods = GetType().GetMethods(_flags).Where(m => predicate(m.Name) && m.GetParameters().Length == 0);

            foreach (MethodInfo method in methods)

                _ = method.Invoke(this, null);
        }
    }
}
