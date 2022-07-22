using System;
using System.Collections.Generic;
using System.Linq;

using WinCopies.IO.ObjectModel;
using WinCopies.IO.Process;

#if !(WinCopies3 && CS8)
using System.Collections;
#endif

using static WinCopies.ThrowHelper;
using static WinCopies.UtilHelpers;

namespace WinCopies.IO
{
    public interface IProcessSettings
#if !WinCopies3
        <T>
#endif
        : DotNetFix.IDisposable
    {
        IProcessFactory
#if CS8
            ?
#endif
            ProcessFactory
        { get; }

#if WinCopies3
        Collections.DotNetFix.Generic.IDisposableEnumerable<IProcessInfo>
#else
        T
#endif
#if CS9 || (CS8 && WinCopies3)
            ?
#endif
            CustomProcesses
        { get; }
    }

    public struct ProcessSettings
#if !WinCopies3
        <T>
#endif
        : IProcessSettings
    {
        private IProcessFactory
#if CS8
            ?
#endif
            _processFactory;
        private
#if WinCopies3
        Collections.DotNetFix.Generic.IDisposableEnumerable<IProcessInfo>
#else
        T
#endif
#if CS9 || (CS8 && WinCopies3)
            ?
#endif
            _customProcesses;

        public bool IsDisposed { get; private set; }

        public IProcessFactory
#if CS8
                    ?
#endif
                    ProcessFactory => _processFactory;

        public
#if WinCopies3
        Collections.DotNetFix.Generic.IDisposableEnumerable<IProcessInfo>
#else
        T
#endif
#if CS9 || (CS8 && WinCopies3)
            ?
#endif
            CustomProcesses => _customProcesses;

        public ProcessSettings(in IProcessFactory
#if CS8
            ?
#endif
            processFactory, in
#if WinCopies3
        Collections.DotNetFix.Generic.IDisposableEnumerable<IProcessInfo>
#else
        T
#endif
#if CS9 || (CS8 && WinCopies3)
            ?
#endif
            customProcesses)
        {
            _processFactory = processFactory;
            _customProcesses = customProcesses;
            IsDisposed = false;
        }

        public ProcessSettings(in IProcessFactory
#if CS8
            ?
#endif
            processFactory, in IEnumerable<IProcessInfo> customProcessesEnumerable) => this = new ProcessSettings(processFactory, new Collections.Generic.DisposableEnumerable2<IProcessInfo>(customProcessesEnumerable ?? Enumerable.Empty<IProcessInfo>(), null));

        public void Dispose()
        {
            _ = TryDispose(ref _processFactory);
            _ = TryDispose(ref _customProcesses);

            IsDisposed = true;
        }
    }

    namespace ComponentSources.Item
    {
        public interface IItemSource :
#if WinCopies3
            Collections.DotNetFix.Generic.IDisposableEnumerable
#else
            DotNetFix.IDisposable, System.Collections.Generic.IEnumerable
#endif
            <IBrowsableObjectInfo>
        {
            string Name { get; }

            string Description { get; }

            bool IsPaginationSupported { get; }

            Interval? Interval { get; set; }

            IProcessSettings
#if CS8
            ?
#endif
                ProcessSettings
            { get; }
        }

        public interface IItemSource<out T> : IItemSource
        {
            System.Collections.Generic.IEnumerable<IBrowsableObjectInfo>
#if CS8
            ?
#endif
            GetItems(Predicate<T>
#if CS8
                ?
#endif
                predicate = null);
        }

        public abstract class ItemSourceBase : DisposableBase, IItemSource
        {
            protected const string MAIN = "Main";
            protected const string DEFAULT_DATA_SOURCE = "Default data source.";
            private Interval? _interval;

            protected abstract IProcessSettings
#if CS8
                ?
#endif
                ProcessSettingsOverride
            { get; }

            public string Name { get; }

            public string Description { get; }

            public abstract bool IsPaginationSupported { get; }

            public Interval? Interval { get => _interval; set => _interval = IsPaginationSupported ? value : throw new NotSupportedException(); }

            public IProcessSettings
#if CS8
                ?
#endif
                ProcessSettings => GetValueIfNotDisposed(() => ProcessSettingsOverride);

            public ItemSourceBase(in string name, in string description)
            {
                Name = name;
                Description = description;
            }

            protected T GetValueIfNotDisposed<T>(in T value) => GetOrThrowIfDisposed(this, value);
            protected T GetValueIfNotDisposed<T>(in Func<T> value) => GetOrThrowIfDisposed(this, value);

            protected override void DisposeUnmanaged() => ProcessSettingsOverride?.Dispose();

            public abstract System.Collections.Generic.IEnumerator<IBrowsableObjectInfo> GetEnumerator();
#if !(WinCopies3 && CS8)
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
#endif
        }

        public class ItemSource : ItemSourceBase
        {
            private readonly Func<System.Collections.Generic.IEnumerator<IBrowsableObjectInfo>> _func;
            private bool _isDisposed;
            private IProcessSettings
#if CS8
            ?
#endif
                _processSettings;

            protected override IProcessSettings
#if CS8
            ?
#endif
                ProcessSettingsOverride => _processSettings;

            public override bool IsPaginationSupported => false;

            public override bool IsDisposed => _isDisposed;

            public ItemSource(in string name, in string description, in Func<System.Collections.Generic.IEnumerator<IBrowsableObjectInfo>> func, in IProcessSettings
#if CS8
            ?
#endif
                processSettings = null) : base(name, description)
            {
                _func = func;
                _processSettings = processSettings;
            }

            public ItemSource(in string name, in string description, Func<System.Collections.Generic.IEnumerable<IBrowsableObjectInfo>> func, in IProcessSettings
#if CS8
            ?
#endif
                processSettings = null) : this(name, description, () => func().GetEnumerator(), processSettings) { /* Left empty. */ }

            public ItemSource(in Func<System.Collections.Generic.IEnumerator<IBrowsableObjectInfo>> func, in IProcessSettings
#if CS8
            ?
#endif
                processSettings = null) : this(MAIN, DEFAULT_DATA_SOURCE, func, processSettings) { /* Left empty. */ }
            public ItemSource(in Func<System.Collections.Generic.IEnumerable<IBrowsableObjectInfo>> func, in IProcessSettings
#if CS8
            ?
#endif
                processSettings = null) : this(MAIN, DEFAULT_DATA_SOURCE, func, processSettings) { /* Left empty. */ }

            public sealed override System.Collections.Generic.IEnumerator<IBrowsableObjectInfo> GetEnumerator() => _func();

            protected override void DisposeManaged()
            {
                _processSettings = null;
                _isDisposed = true;
            }
        }

        public abstract class ItemSourceBase<TBrowsableObjectInfo, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> : ItemSourceBase, IItemSource<TPredicateTypeParameter> where TBrowsableObjectInfo : IBrowsableObjectInfo<TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> where TSelectorDictionary : IEnumerableSelectorDictionary<TDictionaryItems, IBrowsableObjectInfo>
        {
            public TBrowsableObjectInfo BrowsableObjectInfo { get; }

            protected ItemSourceBase(in string name, in string description, in TBrowsableObjectInfo browsableObjectInfo) : base(name, description) => BrowsableObjectInfo = browsableObjectInfo
#if CS8
                ??
#else
                == null ?
#endif
                throw GetArgumentNullException(nameof(browsableObjectInfo))
#if !CS8
                : browsableObjectInfo
#endif
                ;

            protected ItemSourceBase(in TBrowsableObjectInfo browsableObjectInfo) : base(MAIN, DEFAULT_DATA_SOURCE) => BrowsableObjectInfo = browsableObjectInfo;

            protected abstract System.Collections.Generic.IEnumerable<TDictionaryItems>
#if CS8
                ?
#endif
                GetItemProviders();

            protected abstract System.Collections.Generic.IEnumerable<TDictionaryItems>
#if CS8
                ?
#endif
                GetItemProviders(Predicate<TPredicateTypeParameter> predicate);

            protected System.Collections.Generic.IEnumerable<IBrowsableObjectInfo>
#if CS8
                ?
#endif
                GetItems(System.Collections.Generic.IEnumerable<TDictionaryItems>
#if CS8
                ?
#endif
                items) => items == null ? null : BrowsableObjectInfo.GetSelectorDictionary().Select(items);

            public System.Collections.Generic.IEnumerable<IBrowsableObjectInfo>
#if CS8
                ?
#endif
                GetItems(Predicate<TPredicateTypeParameter>
#if CS8
                ?
#endif
                predicate = null) => predicate == null ? GetItems(GetItemProviders()) : GetItems(GetItemProviders(predicate));

            public sealed override System.Collections.Generic.IEnumerator<IBrowsableObjectInfo> GetEnumerator() => (GetItems() ?? Enumerable.Empty<IBrowsableObjectInfo>()).GetEnumerator();
        }

        public abstract class ItemSourceBase2<TBrowsableObjectInfo, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> : ItemSourceBase<TBrowsableObjectInfo, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> where TBrowsableObjectInfo : IBrowsableObjectInfo<TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> where TSelectorDictionary : IEnumerableSelectorDictionary<TDictionaryItems, IBrowsableObjectInfo>
        {
            private readonly Converter<Predicate<TPredicateTypeParameter>, System.Collections.Generic.IEnumerable<TDictionaryItems>> _converter;

            public ItemSourceBase2(in string name, in string description, in TBrowsableObjectInfo browsableObjectInfo, in Converter<Predicate<TPredicateTypeParameter>, System.Collections.Generic.IEnumerable<TDictionaryItems>> converter) : base(name, description, browsableObjectInfo) => _converter = converter;

            public ItemSourceBase2(in TBrowsableObjectInfo browsableObjectInfo, in Converter<Predicate<TPredicateTypeParameter>, System.Collections.Generic.IEnumerable<TDictionaryItems>> converter) : this(MAIN, DEFAULT_DATA_SOURCE, browsableObjectInfo, converter) { /* Left empty. */ }

            protected sealed override System.Collections.Generic.IEnumerable<TDictionaryItems>
#if CS8
            ?
#endif
                GetItemProviders(Predicate<TPredicateTypeParameter> predicate) => _converter(predicate);
        }

        public abstract class ItemSourceBase3<TBrowsableObjectInfo, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> : ItemSourceBase<TBrowsableObjectInfo, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> where TBrowsableObjectInfo : IBrowsableObjectInfo<TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> where TSelectorDictionary : IEnumerableSelectorDictionary<TDictionaryItems, IBrowsableObjectInfo>
        {
            public ItemSourceBase3(in string name, in string description, in TBrowsableObjectInfo browsableObjectInfo) : base(name, description, browsableObjectInfo) { /* Left empty. */ }

            public ItemSourceBase3(in TBrowsableObjectInfo browsableObjectInfo) : this(MAIN, DEFAULT_DATA_SOURCE, browsableObjectInfo)
            { /* Left empty. */ }

            protected override System.Collections.Generic.IEnumerable<TDictionaryItems>
#if CS8
                ?
#endif
                GetItemProviders() => GetItemProviders(Bool.True);
        }

        public abstract class ItemSourceBase4<TBrowsableObjectInfo, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> : ItemSourceBase3<TBrowsableObjectInfo, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> where TBrowsableObjectInfo : IBrowsableObjectInfo<TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> where TSelectorDictionary : IEnumerableSelectorDictionary<TDictionaryItems, IBrowsableObjectInfo>
        {
            private bool _isDisposed;

            public override bool IsDisposed => _isDisposed;

            public ItemSourceBase4(in string name, in string description, in TBrowsableObjectInfo browsableObjectInfo) : base(name, description, browsableObjectInfo) { /* Left empty. */ }

            public ItemSourceBase4(in TBrowsableObjectInfo browsableObjectInfo) : this(MAIN, DEFAULT_DATA_SOURCE, browsableObjectInfo)
            { /* Left empty. */ }

            protected override void DisposeManaged() => _isDisposed = true;
        }

        public class ItemSource<TBrowsableObjectInfo, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> : ItemSourceBase2<TBrowsableObjectInfo, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> where TBrowsableObjectInfo : IBrowsableObjectInfo<TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> where TSelectorDictionary : IEnumerableSelectorDictionary<TDictionaryItems, IBrowsableObjectInfo>
        {
            private bool _isDisposed;
            private IProcessSettings
#if CS8
                ?
#endif
                _processSettings;

            protected override IProcessSettings
#if CS8
            ?
#endif
                ProcessSettingsOverride => _processSettings;

            public override bool IsPaginationSupported => false;

            public override bool IsDisposed => _isDisposed;

            public ItemSource(in string name, in string description, in TBrowsableObjectInfo browsableObjectInfo, in Converter<Predicate<TPredicateTypeParameter>, System.Collections.Generic.IEnumerable<TDictionaryItems>> converter, in IProcessSettings
#if CS8
            ?
#endif
                processSettings = null) : base(name, description, browsableObjectInfo, converter) => _processSettings = processSettings;

            public ItemSource(in TBrowsableObjectInfo browsableObjectInfo, in Converter<Predicate<TPredicateTypeParameter>, System.Collections.Generic.IEnumerable<TDictionaryItems>> converter, in IProcessSettings
#if CS8
            ?
#endif
                processSettings = null) : this(MAIN, DEFAULT_DATA_SOURCE, browsableObjectInfo, converter, processSettings) { /* Left empty. */ }

            protected override System.Collections.Generic.IEnumerable<TDictionaryItems>
#if CS8
            ?
#endif
                GetItemProviders() => GetItemProviders(Bool.True);

            protected override void DisposeManaged()
            {
                _processSettings = null;
                _isDisposed = true;
            }
        }

        public class DelegateItemSource<TBrowsableObjectInfo, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> : ItemSourceBase2<TBrowsableObjectInfo, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> where TBrowsableObjectInfo : IBrowsableObjectInfo<TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> where TSelectorDictionary : IEnumerableSelectorDictionary<TDictionaryItems, IBrowsableObjectInfo>
        {
            private readonly Func<System.Collections.Generic.IEnumerable<TDictionaryItems>> _func;
            private bool _isDisposed;
            private IProcessSettings
#if CS8
                ?
#endif
                _processSettings;

            protected override IProcessSettings
#if CS8
            ?
#endif
                ProcessSettingsOverride => _processSettings;

            public override bool IsPaginationSupported => false;

            public override bool IsDisposed => _isDisposed;

            public DelegateItemSource(in string name, in string description, in TBrowsableObjectInfo browsableObjectInfo, in Converter<Predicate<TPredicateTypeParameter>, System.Collections.Generic.IEnumerable<TDictionaryItems>> converter, in Func<System.Collections.Generic.IEnumerable<TDictionaryItems>> func, in IProcessSettings
#if CS8
                ?
#endif
                processSettings = null) : base(name, description, browsableObjectInfo, converter)
            {
                _func = func;
                _processSettings = processSettings;
            }

            public DelegateItemSource(in TBrowsableObjectInfo browsableObjectInfo, in Converter<Predicate<TPredicateTypeParameter>, System.Collections.Generic.IEnumerable<TDictionaryItems>> converter, in Func<System.Collections.Generic.IEnumerable<TDictionaryItems>> func, in IProcessSettings
#if CS8
                ?
#endif
                processSettings = null) : this(MAIN, DEFAULT_DATA_SOURCE, browsableObjectInfo, converter, func, processSettings) { /* Left empty. */ }

            protected sealed override System.Collections.Generic.IEnumerable<TDictionaryItems>
#if CS8
            ?
#endif
                GetItemProviders() => _func();

            protected override void DisposeManaged()
            {
                _processSettings = null;
                _isDisposed = true;
            }
        }
    }
}
