using System;
using System.IO;

namespace WinCopies.IO.ObjectModel
{
    public abstract partial class ShellObjectInfo<TObjectProperties, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems>
    {
        private struct GetStreamStruct : DotNetFix.IDisposable
        {
            private Func<StreamInfo> _getStreamDelegate;
            private StreamInfo _archiveFileStream;

            public StreamInfo ArchiveFileStream => _archiveFileStream.IsDisposed ? (_archiveFileStream = _getStreamDelegate()) : _archiveFileStream;

            public bool IsDisposed
            {
                get
                {
                    if (_getStreamDelegate == null)

                        return true;

                    else if (_archiveFileStream.IsDisposed)
                    {
                        _archiveFileStream = null;
                        _getStreamDelegate = null;

                        return true;
                    }

                    return false;
                }
            }

            public GetStreamStruct(Func<FileStream> func) => _archiveFileStream = (_getStreamDelegate = () => new StreamInfo(func()))();

            public void Dispose()
            {
                if (!IsDisposed)
                {
                    if (!_archiveFileStream.IsDisposed)

                        _archiveFileStream.Dispose();

                    _archiveFileStream = null;
                    _getStreamDelegate = null;
                }
            }
        }
    }
}
