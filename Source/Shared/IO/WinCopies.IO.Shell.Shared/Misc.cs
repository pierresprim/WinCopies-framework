using WinCopies.IO.ComponentSources.Item;
using WinCopies.IO.ObjectModel;

using static WinCopies.ThrowHelper;

namespace WinCopies.IO
{
    namespace Shell.ComponentSources.Item
    {
        public abstract class ItemSource<TBrowsableObjectInfo, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> : ItemSourceBase3<TBrowsableObjectInfo, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> where TBrowsableObjectInfo : IBrowsableObjectInfo<TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> where TSelectorDictionary : IEnumerableSelectorDictionary<TDictionaryItems, IBrowsableObjectInfo>
        {
            protected ItemSource(in TBrowsableObjectInfo browsableObjectInfo) : base(browsableObjectInfo) { /* Left empty. */ }

            internal new System.Collections.Generic.IEnumerable<IBrowsableObjectInfo>
#if CS8
                    ?
#endif
                GetItems(System.Collections.Generic.IEnumerable<TDictionaryItems>
#if CS8
                    ?
#endif
                items) => base.GetItems(items);
        }
    }

    public struct ShellObjectInitInfo
    {
        public string Path { get; }

        public FileType FileType { get; }

        public ShellObjectInitInfo(string path, FileType fileType)
        {
            Path = path;

            FileType = fileType;
        }
    }

    public sealed class ShellLinkBrowsabilityOptions : IBrowsabilityOptions, DotNetFix.IDisposable
    {
        private IShellObjectInfoBase _shellObjectInfo;

        public bool IsDisposed => _shellObjectInfo == null;

        public Browsability Browsability => Browsability.RedirectsToBrowsableItem;

        public ShellLinkBrowsabilityOptions(in IShellObjectInfoBase shellObjectInfo) => _shellObjectInfo = shellObjectInfo ?? throw GetArgumentNullException(nameof(shellObjectInfo));

        public IBrowsableObjectInfo RedirectToBrowsableItem() => IsDisposed ? throw GetExceptionForDispose(false) : _shellObjectInfo;

        public void Dispose() => _shellObjectInfo = null;
    }
}