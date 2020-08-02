// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Design", "CA1032:Implement standard exception constructors", Justification = "These constructors are implemented with more parameters.", Scope = "type", Target = "~T:WinCopies.IO.IOException")]
[assembly: SuppressMessage("Design", "CA1032:Implement standard exception constructors", Justification = "These constructors are implemented with more parameters.", Scope = "type", Target = "~T:WinCopies.IO.DirectoryNotFoundException")]
[assembly: SuppressMessage("Design", "CA1032:Implement standard exception constructors", Justification = "These constructors are implemented with more parameters.", Scope = "type", Target = "~T:WinCopies.IO.DriveNotFoundException")]
[assembly: SuppressMessage("Design", "CA1032:Implement standard exception constructors", Justification = "These constructors are implemented with more parameters.", Scope = "type", Target = "~T:WinCopies.IO.EndOfStreamException")]
[assembly: SuppressMessage("Design", "CA1032:Implement standard exception constructors", Justification = "These constructors are implemented with more parameters.", Scope = "type", Target = "~T:WinCopies.IO.FileLoadException")]
[assembly: SuppressMessage("Design", "CA1032:Implement standard exception constructors", Justification = "These constructors are implemented with more parameters.", Scope = "type", Target = "~T:WinCopies.IO.FileNotFoundException")]
[assembly: SuppressMessage("Design", "CA1032:Implement standard exception constructors", Justification = "These constructors are implemented with more parameters.", Scope = "type", Target = "~T:WinCopies.IO.PathTooLongException")]
[assembly: SuppressMessage("Design", "CA1032:Implement standard exception constructors", Justification = "These constructors are implemented with more parameters.", Scope = "type", Target = "~T:WinCopies.IO.PipeException")]
[assembly: SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "Validation made by the ThrowIfNull method.", Scope = "member", Target = "~M:WinCopies.IO.ArchiveItemInfo.From(WinCopies.IO.IShellObjectInfo,SevenZip.ArchiveFileInfo)~WinCopies.IO.ArchiveItemInfo")]
[assembly: SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "The Dispose method is implemented only for the IEnumerator interface implementation and does not dispose unmanaged code.", Scope = "member", Target = "~M:WinCopies.IO.ShellObjectInfo.GetArchiveItemInfoItems~System.Collections.Generic.IEnumerable{WinCopies.IO.IBrowsableObjectInfo}")]
[assembly: SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "The Dispose method is implemented only for the IEnumerator interface implementation and does not dispose unmanaged code.", Scope = "member", Target = "~M:WinCopies.IO.ArchiveItemInfo.GetItems~System.Collections.Generic.IEnumerable{WinCopies.IO.IBrowsableObjectInfo}")]
[assembly: SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "Validation made by the ThrowIfNull method.", Scope = "member", Target = "~M:WinCopies.IO.FileSystemEntryEnumerator.#ctor(System.Collections.Generic.IEnumerable{System.String},System.Collections.Generic.IEnumerable{System.String})")]
