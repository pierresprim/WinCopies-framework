using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using WinCopies.IO.ObjectModel;
using WinCopies.IO.Process;
using WinCopies.Linq;

namespace WinCopies.IO.ComponentSources.Item
{
    public interface IItemSourcesProvider :
#if WinCopies3 && CS8
        Collections.DotNetFix
#else
        System.Collections
#endif
        .Generic.IEnumerable<IItemSource>
    {
        IItemSource Default { get; }

        IEnumerable<IItemSource>
#if CS8
            ?
#endif
            ExtraItemSources
        { get; }
    }

    public interface IItemSourcesProviderBase<T> : IItemSourcesProvider, IAsEnumerable<T> where T : IItemSource
    {
        new T Default { get; }

        new IEnumerable<T>
#if CS8
             ?
#endif
             ExtraItemSources
        { get; }
    }

    public interface IItemSourcesProvider<TItemSource, out TPredicateTypeParameter> : IItemSourcesProviderBase<TItemSource> where TItemSource : IItemSource<TPredicateTypeParameter>
    {
        // Left empty.
    }

    public interface IItemSourcesProvider<T> : IItemSourcesProvider<IItemSource<T>, T>
    {
        // Left empty.
    }

    public interface IItemSourcesProvider<TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> : IItemSourcesProvider<IItemSource<TPredicateTypeParameter>, TPredicateTypeParameter> where TSelectorDictionary : IEnumerableSelectorDictionary<TDictionaryItems, IBrowsableObjectInfo>
    {
        // Left empty.
    }

    public class ItemSourcesProvider<T> : IItemSourcesProviderBase<T> where T : IItemSource
    {
        private readonly IEnumerable<T> _all;

        public T Default { get; }

        public IEnumerable<T>
#if CS8
            ?
#endif
            ExtraItemSources
        { get; }

        IItemSource IItemSourcesProvider.Default => Default;

        IEnumerable<IItemSource>
#if CS8
            ?
#endif
            IItemSourcesProvider.ExtraItemSources => ExtraItemSources?.Cast<IItemSource>();

        public ItemSourcesProvider(in T @default, in IEnumerable<T>
#if CS8
            ?
#endif
            extraItemSources = null)
        {
            Default = @default;
            ExtraItemSources = extraItemSources;

            _all = extraItemSources == null ? Collections.Enumerable.GetEnumerable(@default) : extraItemSources.Prepend(@default);
        }

        public ItemSourcesProvider(in T @default, params T[] extraItemSources) : this(@default, extraItemSources.AsEnumerable()) { /* Left empty. */ }

        public IEnumerator<T> GetEnumerator() => _all.GetEnumerator();
        public IEnumerable<T> AsEnumerable() => _all;

#if !(WinCopies3 && CS8)
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
#endif
        IEnumerator<IItemSource> IEnumerable<IItemSource>.GetEnumerator() => _all.Cast<IItemSource>().GetEnumerator();
    }

    public class ItemSourcesProvider : ItemSourcesProvider<IItemSource>, IItemSourcesProvider
    {
        public ItemSourcesProvider(in IItemSource @default, in IEnumerable<IItemSource>
#if CS8
            ?
#endif
            extraItemSources = null) : base(@default, extraItemSources) { /* Left empty. */ }

        public ItemSourcesProvider(in string name, in string description, in Func<IEnumerable<IBrowsableObjectInfo>> func, in IProcessSettings
#if CS8
            ?
#endif
            processSettings = null) : this(new ItemSource(name, description, func, processSettings)) { /* Left empty. */ }
        public ItemSourcesProvider(in string name, in string description, in Func<IEnumerator<IBrowsableObjectInfo>> func, in IProcessSettings
#if CS8
            ?
#endif
            processSettings = null) : this(new ItemSource(name, description, func, processSettings)) { /* Left empty. */ }

        public ItemSourcesProvider(in Func<IEnumerable<IBrowsableObjectInfo>> func, in IProcessSettings
#if CS8
            ?
#endif
            processSettings = null) : this(new ItemSource(func, processSettings)) { /* Left empty. */ }
        public ItemSourcesProvider(in Func<IEnumerator<IBrowsableObjectInfo>> func, in IProcessSettings
#if CS8
            ?
#endif
            processSettings = null) : this(new ItemSource(func, processSettings)) { /* Left empty. */ }

        public static ItemSourcesProvider<TItemSource, TPredicateTypeParameter> Construct<TItemSource, TPredicateTypeParameter>(in TItemSource itemSource) where TItemSource : IItemSource<TPredicateTypeParameter> => new
#if !CS9
            ItemSourcesProvider<TItemSource, TPredicateTypeParameter>
#endif
            (itemSource);

        public static ItemSourcesProvider<TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> Construct<TBrowsableObjectInfo, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems>(in ItemSourceBase<TBrowsableObjectInfo, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> itemSource) where TSelectorDictionary : IEnumerableSelectorDictionary<TDictionaryItems, IBrowsableObjectInfo> where TBrowsableObjectInfo : IBrowsableObjectInfo<TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> => new
#if !CS9
            ItemSourcesProvider<TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems>
#endif
            (itemSource);
    }

    public class ItemSourcesProvider<TItemSource, TPredicateTypeParameter> : ItemSourcesProvider<TItemSource>, IItemSourcesProvider<TItemSource, TPredicateTypeParameter> where TItemSource : IItemSource<TPredicateTypeParameter>
    {
        public ItemSourcesProvider(in TItemSource @default, in IEnumerable<TItemSource>
#if CS8
            ?
#endif
            extraItemSources = null) : base(@default, extraItemSources) { /* Left empty. */ }

        public ItemSourcesProvider(in TItemSource @default, params TItemSource[] extraItemSources) : this(@default, extraItemSources.AsEnumerable()) { /* Left empty. */ }
    }

    public class ItemSourcesProvider<TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> : ItemSourcesProvider<IItemSource<TPredicateTypeParameter>, TPredicateTypeParameter>, IItemSourcesProvider<TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems>, IItemSourcesProvider<TPredicateTypeParameter> where TSelectorDictionary : IEnumerableSelectorDictionary<TDictionaryItems, IBrowsableObjectInfo>
    {
        public ItemSourcesProvider(in IItemSource<TPredicateTypeParameter> @default, in IEnumerable<IItemSource<TPredicateTypeParameter>>
#if CS8
            ?
#endif
            extraItemSources = null) : base(@default, extraItemSources) { /* Left empty. */ }

        public ItemSourcesProvider(in IItemSource<TPredicateTypeParameter> @default, params IItemSource<TPredicateTypeParameter>[] extraItemSources) : this(@default, extraItemSources.AsEnumerable()) { /* Left empty. */ }

        public ItemSourcesProvider(in IBrowsableObjectInfo<TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> browsableObjectInfo, in Converter<Predicate<TPredicateTypeParameter>, IEnumerable<TDictionaryItems>> converter, in IProcessSettings
#if CS8
            ?
#endif
            processSettings = null) : this(new ItemSource<IBrowsableObjectInfo<TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems>, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems>(browsableObjectInfo, converter, processSettings)) { /* Left empty. */ }
    }
}
