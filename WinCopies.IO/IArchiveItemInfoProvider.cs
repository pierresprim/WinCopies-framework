﻿namespace WinCopies.IO
{
    public interface IArchiveItemInfoProvider : IBrowsableObjectInfo

    {

        // IBrowsableObjectInfo GetBrowsableObjectInfo(ShellObjectInfo archiveShellObject, ArchiveFileInfo? archiveFileInfo, string path, FileType fileType);

        IArchiveItemInfoFactory ArchiveItemInfoFactory { get; set; }

        // todo: throws if not a shellobjectinfo archive

        ShellObjectInfo ArchiveShellObject { get; }

    }

    //public abstract class ArchiveItemInfoProvider : BrowsableObjectInfo, IArchiveItemInfoProvider
    //{
    //    public ArchiveItemInfoProvider(string path, FileType fileType) : base(path, fileType) { }

    //    public IArchiveItemInfoFactory ArchiveItemInfoFactory { get; set; }

    //    protected abstract ShellObjectInfo ArchiveShellObjectOverride { get; }

    //    ShellObjectInfo IArchiveItemInfoProvider.ArchiveShellObject => ArchiveShellObjectOverride;



    //}
}
