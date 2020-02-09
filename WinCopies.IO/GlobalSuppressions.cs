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


// This file is used by Code Analysis to maintain SuppressMessage 
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given 
// a specific target and scoped to a namespace, type, member, etc.

[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "IDE0069:Disposable fields should be disposed", Justification = "Disposed in the Dispose() method.", Scope = "member", Target = "~F:WinCopies.IO.BrowsableObjectInfo._parent")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "IDE0069:Disposable fields should be disposed", Justification = "Disposed in the Dispose method overrides.", Scope = "member", Target = "~F:WinCopies.IO.BrowsableObjectInfoLoader`1.backgroundWorker")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1032:Implement standard exception constructors", Justification = "These constructors are implemented with more parameters.", Scope = "type", Target = "~T:WinCopies.IO.IOException")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1032:Implement standard exception constructors", Justification = "These constructors are implemented with more parameters.", Scope = "type", Target = "~T:WinCopies.IO.DirectoryNotFoundException")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1032:Implement standard exception constructors", Justification = "These constructors are implemented with more parameters.", Scope = "type", Target = "~T:WinCopies.IO.DriveNotFoundException")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1032:Implement standard exception constructors", Justification = "These constructors are implemented with more parameters.", Scope = "type", Target = "~T:WinCopies.IO.EndOfStreamException")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1032:Implement standard exception constructors", Justification = "These constructors are implemented with more parameters.", Scope = "type", Target = "~T:WinCopies.IO.FileLoadException")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1032:Implement standard exception constructors", Justification = "These constructors are implemented with more parameters.", Scope = "type", Target = "~T:WinCopies.IO.FileNotFoundException")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1032:Implement standard exception constructors", Justification = "These constructors are implemented with more parameters.", Scope = "type", Target = "~T:WinCopies.IO.PathTooLongException")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1032:Implement standard exception constructors", Justification = "These constructors are implemented with more parameters.", Scope = "type", Target = "~T:WinCopies.IO.PipeException")]

