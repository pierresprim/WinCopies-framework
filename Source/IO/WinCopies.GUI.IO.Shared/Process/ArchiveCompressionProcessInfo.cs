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

using SevenZip;
using System;
using System.Collections.Generic;
using System.Linq;
using WinCopies.GUI.Windows;
using WinCopies.IO.ObjectModel;
using WinCopies.IO.Process;
using WinCopies.IO.Resources;
using WinCopies.Util;
using WinCopies.Util.Data;
using WinCopies.Linq;

using static WinCopies.ThrowHelper;
using System.IO;

namespace WinCopies.GUI.IO.Process
{
    public interface IArchiveCompressionParameters
    {
        OutArchiveFormat ArchiveFormat { get; set; }

        CompressionLevel CompressionLevel { get; set; }

        CompressionMethod CompressionMethod { get; set; }

        string DestinationPath { get; set; }

        bool FastCompression { get; set; }

        bool IncludeEmptyDirectories { get; set; }

        bool PreserveDirectoryRoot { get; set; }

        SevenZipCompressor ToArchiveCompressor();

        IProcessParameters ToProcessParameters(string sourcePath, System.Collections.Generic.IEnumerable<string> paths);
    }

    [TypeForDataTemplate(typeof(IArchiveCompressionParameters))]
    public sealed class ArchiveCompressionParameters : IArchiveCompressionParameters
    {
        public OutArchiveFormat ArchiveFormat { get; set; } = OutArchiveFormat.Zip;

        public CompressionLevel CompressionLevel { get; set; } = CompressionLevel.Normal;

        public CompressionMethod CompressionMethod { get; set; } = CompressionMethod.Default;

        public string DestinationPath { get; set; }

        public bool FastCompression { get; set; }

        public bool IncludeEmptyDirectories { get; set; }

        public bool PreserveDirectoryRoot { get; set; }

        public SevenZipCompressor ToArchiveCompressor() => new SevenZipCompressor() { ArchiveFormat = ArchiveFormat, CompressionLevel = CompressionLevel, CompressionMethod = CompressionMethod, FastCompression = FastCompression, IncludeEmptyDirectories = IncludeEmptyDirectories, PreserveDirectoryRoot = PreserveDirectoryRoot };

        public IProcessParameters ToProcessParameters(string sourcePath, System.Collections.Generic.IEnumerable<string> paths)
        {
            System.Reflection.PropertyInfo[] properties = typeof(ArchiveCompressionParameters).GetProperties();

            var values = new string[properties.Length];

            for (int i = 0; i < properties.Length; i++)

                values[i] = properties[i].GetValue(this).ToString();

            return new ProcessParameters(WinCopies.IO.Guids.Process.ArchiveCompression, values.Surround(Enumerable.Repeat(sourcePath, 1), paths));
        }

        public static ArchiveCompressionParameters FromProcessParameters(in System.Collections.Generic.IEnumerator<string> enumerator)
        {
            var result = new ArchiveCompressionParameters();

            try
            {
                foreach (System.Reflection.PropertyInfo p in typeof(ArchiveCompressionParameters).GetProperties())

                    if (enumerator.MoveNext())

                        p.SetValue(result, typeof(Enum).IsAssignableFrom(p.PropertyType) ? Enum.Parse(p.PropertyType, enumerator.Current) : Convert.ChangeType(enumerator.Current, p.PropertyType));

                    else

                        throw new InvalidOperationException(ExceptionMessages.ProcessParametersCouldNotBeParsedCorrectly);
            }

            catch (Exception ex) when (ex.Is(false, typeof(InvalidCastException), typeof(FormatException), typeof(OverflowException), typeof(ArgumentNullException)))
            {
                throw new InvalidOperationException(ExceptionMessages.ProcessParametersCouldNotBeParsedCorrectly, ex);
            }

            return result;
        }

        public static ArchiveCompressionParameters FromProcessParameters(in IProcessParameters processParameters)
        {
            using (System.Collections.Generic.IEnumerator<string> enumerator = (processParameters ?? throw GetArgumentNullException(nameof(processParameters))).Parameters.GetEnumerator())

                return enumerator.MoveNext() ? FromProcessParameters(enumerator) : throw new InvalidOperationException(ExceptionMessages.ProcessParametersCouldNotBeParsedCorrectly);
        }
    }

    public sealed class ArchiveCompressionParametersViewModel : ViewModel<IArchiveCompressionParameters>, IArchiveCompressionParameters
    {
        public OutArchiveFormat ArchiveFormat { get => ModelGeneric.ArchiveFormat; set { ModelGeneric.ArchiveFormat = value; OnPropertyChanged(nameof(ArchiveFormat)); } }

        public CompressionLevel CompressionLevel { get => ModelGeneric.CompressionLevel; set { ModelGeneric.CompressionLevel = value; OnPropertyChanged(nameof(CompressionLevel)); } }

        public CompressionMethod CompressionMethod { get => ModelGeneric.CompressionMethod; set { ModelGeneric.CompressionMethod = value; OnPropertyChanged(nameof(CompressionMethod)); } }

        public string DestinationPath { get => ModelGeneric.DestinationPath; set { ModelGeneric.DestinationPath = value; OnPropertyChanged(nameof(DestinationPath)); } }

        public bool FastCompression { get => ModelGeneric.FastCompression; set { ModelGeneric.FastCompression = value; OnPropertyChanged(nameof(FastCompression)); } }

        public bool IncludeEmptyDirectories { get => ModelGeneric.IncludeEmptyDirectories; set { ModelGeneric.IncludeEmptyDirectories = value; OnPropertyChanged(nameof(IncludeEmptyDirectories)); } }

        public bool PreserveDirectoryRoot { get => ModelGeneric.PreserveDirectoryRoot; set { ModelGeneric.PreserveDirectoryRoot = value; OnPropertyChanged(nameof(PreserveDirectoryRoot)); } }

        public ArchiveCompressionParametersViewModel(in IArchiveCompressionParameters parameters) : base(parameters) { /* Left empty. */ }

        SevenZipCompressor IArchiveCompressionParameters.ToArchiveCompressor() => ModelGeneric.ToArchiveCompressor();

        IProcessParameters IArchiveCompressionParameters.ToProcessParameters(string sourcePath, System.Collections.Generic.IEnumerable<string> paths) => ModelGeneric.ToProcessParameters(sourcePath, paths);
    }

    public class ArchiveCompressionProcessInfo : IProcessInfo
    {
        public string GroupName => "Archive";

        public string Name => Properties.Resources.Compression;

        public bool CanRun(object parameter, IBrowsableObjectInfo sourcePath, IEnumerable<IBrowsableObjectInfo> paths)
        {
            if (paths != null)

                foreach (IBrowsableObjectInfo path in paths)

                    if (!(path is IShellObjectInfoBase2 _path && _path.InnerObject.IsFileSystemObject)) // todo: check parent paths

                        return false;

            return true;
        }

        public IProcessParameters TryGetProcessParameters(object parameter, IBrowsableObjectInfo sourcePath, System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> paths)
        {
            var parameters = new ArchiveCompressionParameters();

            return new DialogWindow("Compression") { ContentTemplateSelector = new InterfaceDataTemplateSelector(), Content = new ArchiveCompressionParametersViewModel(parameters), DialogButton = DialogButton.OKCancel }.ShowDialog() == true ? parameters.ToProcessParameters(sourcePath.Path, paths.Select(path => path.Path)) : null;
        }

        public IProcessParameters GetProcessParameters(object parameter, IBrowsableObjectInfo sourcePath, System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> paths) => TryGetProcessParameters(parameter, sourcePath, paths) ?? throw new InvalidOperationException();
    }
}
