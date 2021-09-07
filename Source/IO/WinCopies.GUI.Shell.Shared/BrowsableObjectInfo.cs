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

using WinCopies.GUI.IO.Process;
using WinCopies.IO;
using WinCopies.IO.ObjectModel;
using WinCopies.IO.Process;

namespace WinCopies.GUI.Shell.ObjectModel
{
    public class BrowsableObjectInfoPlugin : WinCopies.IO.Shell.BrowsableObjectInfoPlugin
    {
        public BrowsableObjectInfoPlugin() => RegisterProcessSelectorsStack.Push(() =>
          {
              ShellObjectInfo.DefaultCustomProcessesSelectorDictionary.Push(item => item.InnerObject.IsFileSystemObject, item => new IProcessInfo[] { new ArchiveCompressionProcessInfo() });

              WinCopies.IO.ObjectModel.BrowsableObjectInfo.DefaultProcessSelectorDictionary.Push(item => WinCopies.IO.ObjectModel.BrowsableObjectInfo.Predicate(item, typeof(WinCopies.IO.Guids.Process)), TryGetArchiveProcess);
          });

        public static WinCopies.IO.Process.ObjectModel.IProcess TryGetArchiveProcess(ProcessFactorySelectorDictionaryParameters processParameters)
        {
            string guid = processParameters.ProcessParameters.Guid.ToString();

            System.Collections.Generic.IEnumerator<string> enumerator = null;

            try
            {
                switch (guid)
                {
                    case WinCopies.IO.Guids.Process.ArchiveCompression:

                        enumerator = processParameters.ProcessParameters.Parameters.GetEnumerator();

                        WinCopies.IO.IPathInfo sourcePath = enumerator.MoveNext() ? new PathTypes<WinCopies.IO.IPathInfo>.RootPath(enumerator.Current, true) : throw new InvalidOperationException(WinCopies.IO.Shell.Resources.ExceptionMessages.ProcessParametersCouldNotBeParsedCorrectly);

                        var parameters = ArchiveCompressionParameters.FromProcessParameters(enumerator);

                        return new WinCopies.IO.Process.ObjectModel.Compression<ProcessErrorFactory<WinCopies.IO.IPathInfo, object>>(
                            ProcessHelper<WinCopies.IO.IPathInfo>.GetInitialPaths(enumerator, sourcePath, path => path),
                            sourcePath,
                            new PathTypes<WinCopies.IO.IPathInfo>.RootPath(parameters.DestinationPath, true),
                            processParameters.Factory.GetProcessCollection<WinCopies.IO.IPathInfo>(),
                            processParameters.Factory.GetProcessLinkedList<
                                WinCopies.IO.IPathInfo,
                                ProcessError,
                                ProcessTypes<WinCopies.IO.IPathInfo, ProcessError, object>.ProcessErrorItem,
                                object>(),
                            ProcessHelper<WinCopies.IO.IPathInfo>.GetDefaultProcessDelegates(),
                            new CompressionProcessErrorFactory(),
                            parameters.ToArchiveCompressor());
                }
            }

            finally
            {
                enumerator?.Dispose();
            }

            return null;
        }

        public static WinCopies.IO.Process.ObjectModel.IProcess GetArchiveProcess(ProcessFactorySelectorDictionaryParameters processParameters) => TryGetArchiveProcess(processParameters) ?? throw new InvalidOperationException("No process could be generated.");
    }

    public static class BrowsableObjectInfo
    {
        public static IBrowsableObjectInfoPlugin PluginParameters { get; } = new BrowsableObjectInfoPlugin();
    }
}
