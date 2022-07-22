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

namespace WinCopies.IO.ComponentSources.Bitmap
{
    public enum BitmapSourcesLinkerLoadStep : byte
    {
        Default = 0,

        Intermediate = 1,

        Completed = 2
    }

    public interface IBitmapSourcesLinker : IBitmapSources
    {
        IBitmapSourcesLinkerLoadSteps LoadSteps { get; }

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

    public interface IBitmapSourcesLinkerLoadSteps
    {
        BitmapSourcesLinkerLoadStep Small { get; }

        BitmapSourcesLinkerLoadStep Medium { get; }

        BitmapSourcesLinkerLoadStep Large { get; }

        BitmapSourcesLinkerLoadStep ExtraLarge { get; }
    }

    public struct BitmapSourcesLinkerLoadSteps
    {
        public BitmapSourcesLinkerLoadStep Small;

        public BitmapSourcesLinkerLoadStep Medium;

        public BitmapSourcesLinkerLoadStep Large;

        public BitmapSourcesLinkerLoadStep ExtraLarge;
    }

    public struct _BitmapSourcesLinkerLoadSteps : IBitmapSourcesLinkerLoadSteps
    {
        internal BitmapSourcesLinkerLoadSteps _loadSteps;

        public BitmapSourcesLinkerLoadStep Small => _loadSteps.Small;

        public BitmapSourcesLinkerLoadStep Medium => _loadSteps.Medium;

        public BitmapSourcesLinkerLoadStep Large => _loadSteps.Large;

        public BitmapSourcesLinkerLoadStep ExtraLarge => _loadSteps.ExtraLarge;
    }

    public struct BitmapSourcesLinkerStruct
    {
        public BitmapSource Small;
        public BitmapSource Medium;
        public BitmapSource Large;
        public BitmapSource ExtraLarge;
    }

    public class BitmapSourcesLinker : BitmapSources<IBitmapSourceProvider>, IBitmapSourcesLinker, INotifyPropertyChanged
    {
        private const BindingFlags _flags = BindingFlags.Public | BindingFlags.Instance;
        private _BitmapSourcesLinkerLoadSteps _loadSteps;
        private BitmapSourcesLinkerStruct _bitmapSources;

        protected override BitmapSource SmallOverride => _bitmapSources.Small;

        protected override BitmapSource MediumOverride => _bitmapSources.Medium;

        protected override BitmapSource LargeOverride => _bitmapSources.Large;

        protected override BitmapSource ExtraLargeOverride => _bitmapSources.ExtraLarge;

        public _BitmapSourcesLinkerLoadSteps LoadSteps => _loadSteps;

        IBitmapSourcesLinkerLoadSteps IBitmapSourcesLinker.LoadSteps => LoadSteps;

        public event PropertyChangedEventHandler PropertyChanged;

        public BitmapSourcesLinker(in IBitmapSourceProvider bitmapSourceProvider) : base(bitmapSourceProvider)
        {
            UpdateBitmapSource(ref _bitmapSources.Small, bitmapSourceProvider.Default.Small);
            UpdateBitmapSource(ref _bitmapSources.Medium, bitmapSourceProvider.Default.Medium);
            UpdateBitmapSource(ref _bitmapSources.Large, bitmapSourceProvider.Default.Large);
            UpdateBitmapSource(ref _bitmapSources.ExtraLarge, bitmapSourceProvider.Default.ExtraLarge);
        }

        protected void UpdateBitmapSource(ref BitmapSource value, in BitmapSource newValue) => (value = newValue).Freeze();

        protected virtual void RaisePropertyChangedEvent(in PropertyChangedEventArgs e) => PropertyChanged?.Invoke(this, e);

        protected virtual void RaisePropertyChangedEvent(in string propertyName) => RaisePropertyChangedEvent(new PropertyChangedEventArgs(propertyName));

        public virtual void OnSmallLoaded() => RaisePropertyChangedEvent(nameof(Small));

        public virtual void OnMediumLoaded() => RaisePropertyChangedEvent(nameof(Medium));

        public virtual void OnLargeLoaded() => RaisePropertyChangedEvent(nameof(Large));

        public virtual void OnExtraLargeLoaded() => RaisePropertyChangedEvent(nameof(ExtraLarge));

        public virtual void OnBitmapSourcesLoaded() => InvokeMethods(name => name != nameof(OnBitmapSourcesLoaded) && name.StartsWith("On") && name.EndsWith("Loaded"));

        protected static void Run(in Action action, in BitmapSourcesLinkerLoadStep? loadStep)
        {
            if (loadStep.HasValue && loadStep.Value == BitmapSourcesLinkerLoadStep.Completed)

                return;

            action();
        }

        protected void Run2(ref BitmapSource value, Func<IBitmapSources, BitmapSource> selector, ref BitmapSourcesLinkerLoadStep loadStep) => UtilHelpers.Lambda((in Action action, in BitmapSourcesLinkerLoadStep _loadStep) => Run(action, _loadStep), (ref BitmapSourcesLinkerLoadStep __loadStep, ref BitmapSource __value) =>
        {
            IBitmapSources
#if CS8
            ?
#endif
            bitmapSources = GetBitmapSources(__loadStep);

            if (bitmapSources != null)

                UpdateBitmapSource(ref __value, selector(bitmapSources));

            __loadStep++;
        }, ref loadStep, ref value);

        protected IBitmapSources
#if CS8
            ?
#endif
            GetBitmapSources(BitmapSourcesLinkerLoadStep loadStep)
#if CS8
            =>
#else
        {
            switch (
#endif
            loadStep
#if CS8
            switch
#else
            )
#endif
            {
#if !CS8
                case
#endif
                BitmapSourcesLinkerLoadStep.Default
#if CS8
                =>
#else
                :
                    return
#endif
                    InnerObject.Intermediate
#if CS8
                ,
#else
                    ;
                case
#endif
                BitmapSourcesLinkerLoadStep.Intermediate
#if CS8
                =>
#else
                :
                    return
#endif
                    InnerObject.Sources
#if CS8
                ,
                _ =>
#else
                    ;
                default:
#endif
                    throw new InvalidOperationException("All bitmap sources have already been loaded.")
#if CS8
            };
#else
                    ;
            }
        }
#endif

        public void LoadSmall() => Run2(ref _bitmapSources.Small, bitmapSources => bitmapSources.Small, ref _loadSteps._loadSteps.Small);

        public void LoadMedium() => Run2(ref _bitmapSources.Medium, bitmapSources => bitmapSources.Medium, ref _loadSteps._loadSteps.Medium);

        public void LoadLarge() => Run2(ref _bitmapSources.Large, bitmapSources => bitmapSources.Large, ref _loadSteps._loadSteps.Large);

        public void LoadExtraLarge() => Run2(ref _bitmapSources.ExtraLarge, bitmapSources => bitmapSources.ExtraLarge, ref _loadSteps._loadSteps.ExtraLarge);

        public virtual void Load() => Run(() => InvokeMethods(name => name.Length > nameof(Load).Length && name.StartsWith(nameof(Load))), null);

        private void InvokeMethods(Predicate<string> predicate)
        {
            System.Collections.Generic.IEnumerable<MethodInfo> methods = GetType().GetMethods(_flags).Where(m => predicate(m.Name) && m.GetParameters().Length == 0);

            foreach (MethodInfo method in methods)

                _ = method.Invoke(this, null);
        }
    }
}
