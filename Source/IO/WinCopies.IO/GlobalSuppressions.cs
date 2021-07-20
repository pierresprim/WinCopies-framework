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
[assembly: SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "Validated by ThrowIfNull method.", Scope = "member", Target = "~M:WinCopies.IO.Process.ObjectModel.Copy`1.CopyFile(WinCopies.IO.IPathInfo,System.String@,Microsoft.WindowsAPICodePack.Win32Native.Shell.CopyFileFlags@)~System.Boolean")]
[assembly: SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "Validated by ThrowIfNull method.", Scope = "member", Target = "~M:WinCopies.IO.Process.ObjectModel.Copy`1.RenameOnDuplicate(WinCopies.IO.IPathInfo@,System.String@,System.Boolean@)")]
