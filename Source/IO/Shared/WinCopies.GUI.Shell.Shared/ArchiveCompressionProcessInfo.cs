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

#region WinCopies
using WinCopies.GUI.Windows;
using WinCopies.IO;
using WinCopies.IO.ObjectModel;
using WinCopies.IO.Process;
using WinCopies.IO.PropertySystem;
using WinCopies.Linq;
using WinCopies.Util;
using WinCopies.Util.Data;
#endregion WinCopies

using static WinCopies.IO.Resources.ExceptionMessages;
using static WinCopies.ThrowHelper;

namespace WinCopies.GUI.IO.Process
{
    public interface IArchiveProcessParameters
    {
        string DestinationPath { get; set; }

        IProcessParameters ToProcessParameters(string sourcePath, IEnumerable<string> paths);

        object ToArchiveProcess();
    }

    public interface IArchiveProcessParameters<T> : IArchiveProcessParameters
    {
        new T ToArchiveProcess();
    }

    public interface IArchiveCompressionParameters : IArchiveProcessParameters<SevenZipCompressor>
    {
        OutArchiveFormat ArchiveFormat { get; set; }

        CompressionLevel CompressionLevel { get; set; }

        CompressionMethod CompressionMethod { get; set; }

        bool FastCompression { get; set; }

        bool IncludeEmptyDirectories { get; set; }

        bool PreserveDirectoryRoot { get; set; }
    }

    public interface IArchiveExtractionParameters : IArchiveProcessParameters<Converter<string, SevenZipExtractor>>
    {
        bool PreserveDirectoryStructure { get; set; }
    }

    public abstract class ArchiveProcessParameters : IArchiveProcessParameters
    {
        public string DestinationPath { get; set; }

        public abstract string Guid { get; }

        public IProcessParameters ToProcessParameters(string sourcePath, IEnumerable<string> paths)
        {
            System.Reflection.PropertyInfo[] properties = GetType().GetProperties();

            var values = new string[properties.Length];

            for (int i = 0; i < properties.Length; i++)

                values[i] = properties[i].GetValue(this).ToString();

            return new ProcessParameters(Guid, values.Surround(Enumerable.Repeat(sourcePath, 1), paths));
        }

        public static void InitFromProcessParameters<T>(T obj, IEnumerator<string> enumerator) where T : ArchiveProcessParameters
        {
            ThrowIfNull(obj, nameof(obj));
            ThrowIfNull(enumerator, nameof(enumerator));

            System.Reflection.PropertyInfo[] enumerable = obj.GetType().GetProperties();

            try
            {
                ActionIn<System.Reflection.PropertyInfo> action = _action;

                void _action(in System.Reflection.PropertyInfo _p)
                {
                    void __action(in System.Reflection.PropertyInfo __p)
                    {
                        if (enumerator.MoveNext())

                            __p.SetValue(obj, typeof(Enum).IsAssignableFrom(__p.PropertyType) ? Enum.Parse(__p.PropertyType, enumerator.Current) : Convert.ChangeType(enumerator.Current, __p.PropertyType));

                        else

                            throw new InvalidOperationException(ProcessParametersCouldNotBeParsedCorrectly);
                    }

                    if (_p.Name == nameof(Guid))
                    {
                        action = __action;

                        _ = enumerator.MoveNext();

                        return;
                    }

                    __action(_p);
                }

                foreach (System.Reflection.PropertyInfo p in enumerable)

                    action(p);
            }

            catch (Exception ex) when (ex.Is(false, typeof(InvalidCastException), typeof(FormatException), typeof(OverflowException), typeof(ArgumentNullException)))
            {
                throw new InvalidOperationException(ProcessParametersCouldNotBeParsedCorrectly, ex);
            }
        }

        private static T __FromProcessParameters<T>(in IProcessParameters processParameters, in Converter<IEnumerator<string>, T> func) where T : ArchiveProcessParameters
        {
            using
#if !CS8
            (
#endif
                IEnumerator<string> enumerator = processParameters.Parameters.GetEnumerator()
#if CS8
            ;
#else
            )
#endif
            return enumerator.MoveNext() ? func(enumerator) : throw new InvalidOperationException(ProcessParametersCouldNotBeParsedCorrectly);
        }

        private static IProcessParameters GetProcessParameters(in IProcessParameters processParameters, in string name) => processParameters ?? throw GetArgumentNullException(name);

        private protected static T _FromProcessParameters<T>(in IProcessParameters processParameters, in Converter<IEnumerator<string>, T> func) where T : ArchiveProcessParameters => __FromProcessParameters(GetProcessParameters(processParameters, nameof(processParameters)), func);

        public static T FromProcessParameters<T>(in IProcessParameters processParameters, in Converter<IEnumerator<string>, T> func) where T : ArchiveProcessParameters => __FromProcessParameters(GetProcessParameters(processParameters, nameof(processParameters)), func ?? throw GetArgumentNullException(nameof(func)));

        public abstract object ToArchiveProcess();
    }

    public abstract class ArchiveProcessParameters<T> : ArchiveProcessParameters, IArchiveProcessParameters<T>
    {
        public abstract T ToArchiveProcessGeneric();

        public sealed override object ToArchiveProcess() => ToArchiveProcessGeneric();

        T IArchiveProcessParameters<T>.ToArchiveProcess() => ToArchiveProcessGeneric();
    }

    [TypeForDataTemplate(typeof(IArchiveCompressionParameters))]
    public sealed class ArchiveCompressionParameters : ArchiveProcessParameters<SevenZipCompressor>, IArchiveCompressionParameters
    {
        public override string Guid => WinCopies.IO.Consts.Guids.Shell.Process.Archive.Compression;

        public OutArchiveFormat ArchiveFormat { get; set; } = OutArchiveFormat.Zip;

        public CompressionLevel CompressionLevel { get; set; } = CompressionLevel.Normal;

        public CompressionMethod CompressionMethod { get; set; } = CompressionMethod.Default;

        public bool FastCompression { get; set; }

        public bool IncludeEmptyDirectories { get; set; }

        public bool PreserveDirectoryRoot { get; set; }

        public override SevenZipCompressor ToArchiveProcessGeneric() => new
#if !CS9
            SevenZipCompressor
#endif
            ()
        { ArchiveFormat = ArchiveFormat, CompressionLevel = CompressionLevel, CompressionMethod = CompressionMethod, FastCompression = FastCompression, IncludeEmptyDirectories = IncludeEmptyDirectories, PreserveDirectoryRoot = PreserveDirectoryRoot };

        public static ArchiveCompressionParameters FromProcessParameters(IEnumerator<string> enumerator)
        {
            var result = new ArchiveCompressionParameters();

            InitFromProcessParameters(result, enumerator);

            return result;
        }

        public static ArchiveCompressionParameters FromProcessParameters(in IProcessParameters processParameters) => _FromProcessParameters(processParameters, FromProcessParameters);
    }

    [TypeForDataTemplate(typeof(IArchiveExtractionParameters))]
    public sealed class ArchiveExtractionParameters : ArchiveProcessParameters<Converter<string, SevenZipExtractor>>, IArchiveExtractionParameters
    {
        public override string Guid => WinCopies.IO.Consts.Guids.Shell.Process.Archive.Extraction;

        public bool PreserveDirectoryStructure { get; set; } = true;

        public override Converter<string, SevenZipExtractor> ToArchiveProcessGeneric() => path => new
#if !CS9
            SevenZipExtractor
#endif
            (path)
        { PreserveDirectoryStructure = PreserveDirectoryStructure };

        public static ArchiveExtractionParameters FromProcessParameters(IEnumerator<string> enumerator)
        {
            var result = new ArchiveExtractionParameters();

            InitFromProcessParameters(result, enumerator);

            return result;
        }

        public static ArchiveExtractionParameters FromProcessParameters(in IProcessParameters processParameters) => _FromProcessParameters(processParameters, FromProcessParameters);
    }

    public abstract class ArchiveProcessParametersViewModel<T> : ViewModel<T>, IArchiveProcessParameters where T : IArchiveProcessParameters
    {
        public string DestinationPath { get => ModelGeneric.DestinationPath; set { ModelGeneric.DestinationPath = value; OnPropertyChanged(nameof(DestinationPath)); } }

        protected ArchiveProcessParametersViewModel(in T parameters) : base(parameters) { /* Left empty. */ }

        IProcessParameters IArchiveProcessParameters.ToProcessParameters(string sourcePath, IEnumerable<string> paths) => ModelGeneric.ToProcessParameters(sourcePath, paths);

        protected abstract object ToArchiveProcess();

        object IArchiveProcessParameters.ToArchiveProcess() => ToArchiveProcess();
    }

    public abstract class ArchiveProcessParametersViewModel<TParameters, TArchiveProcess> : ArchiveProcessParametersViewModel<TParameters>, IArchiveProcessParameters<TArchiveProcess> where TParameters : IArchiveProcessParameters<TArchiveProcess>
    {
        protected ArchiveProcessParametersViewModel(in TParameters parameters) : base(parameters) { /* Left empty. */ }

        protected TArchiveProcess ToArchiveProcessGeneric() => ModelGeneric.ToArchiveProcess();

        protected sealed override object ToArchiveProcess() => ToArchiveProcessGeneric();

        TArchiveProcess IArchiveProcessParameters<TArchiveProcess>.ToArchiveProcess() => ToArchiveProcessGeneric();
    }

    public sealed class ArchiveCompressionParametersViewModel : ArchiveProcessParametersViewModel<IArchiveCompressionParameters, SevenZipCompressor>, IArchiveCompressionParameters
    {
        public OutArchiveFormat ArchiveFormat { get => ModelGeneric.ArchiveFormat; set { ModelGeneric.ArchiveFormat = value; OnPropertyChanged(nameof(ArchiveFormat)); } }

        public CompressionLevel CompressionLevel { get => ModelGeneric.CompressionLevel; set { ModelGeneric.CompressionLevel = value; OnPropertyChanged(nameof(CompressionLevel)); } }

        public CompressionMethod CompressionMethod { get => ModelGeneric.CompressionMethod; set { ModelGeneric.CompressionMethod = value; OnPropertyChanged(nameof(CompressionMethod)); } }

        public bool FastCompression { get => ModelGeneric.FastCompression; set { ModelGeneric.FastCompression = value; OnPropertyChanged(nameof(FastCompression)); } }

        public bool IncludeEmptyDirectories { get => ModelGeneric.IncludeEmptyDirectories; set { ModelGeneric.IncludeEmptyDirectories = value; OnPropertyChanged(nameof(IncludeEmptyDirectories)); } }

        public bool PreserveDirectoryRoot { get => ModelGeneric.PreserveDirectoryRoot; set { ModelGeneric.PreserveDirectoryRoot = value; OnPropertyChanged(nameof(PreserveDirectoryRoot)); } }

        public ArchiveCompressionParametersViewModel(in IArchiveCompressionParameters parameters) : base(parameters) { /* Left empty. */ }
    }

    public sealed class ArchiveExtractionParametersViewModel : ArchiveProcessParametersViewModel<IArchiveExtractionParameters, Converter<string, SevenZipExtractor>>, IArchiveExtractionParameters
    {
        public bool PreserveDirectoryStructure { get => ModelGeneric.PreserveDirectoryStructure; set { ModelGeneric.PreserveDirectoryStructure = value; OnPropertyChanged(nameof(PreserveDirectoryStructure)); } }

        public ArchiveExtractionParametersViewModel(in IArchiveExtractionParameters parameters) : base(parameters) { /* Left empty. */ }
    }

    public abstract class ArchiveProcessInfo : IProcessInfo
    {
        public string GroupName => "Archive";

        public abstract string Name { get; }

        bool IProcessFactoryProcessInfoBase.UserConfirmationRequired => false;

        protected abstract bool CanRun(IBrowsableObjectInfo path);

        protected virtual bool CanRunOverride(object parameter, IBrowsableObjectInfo sourcePath, IEnumerable<IBrowsableObjectInfo> paths)
        {
            foreach (IBrowsableObjectInfo path in paths)

                if (!CanRun(path)) return false;

            return true;
        }

        public bool CanRun(object parameter, IBrowsableObjectInfo sourcePath, IEnumerable<IBrowsableObjectInfo> paths) => sourcePath != null && paths != null && CanRunOverride(parameter, sourcePath, paths);

        public IProcessParameters GetProcessParameters(object parameter, IBrowsableObjectInfo sourcePath, IEnumerable<IBrowsableObjectInfo> paths) => TryGetProcessParameters(parameter, sourcePath, paths) ?? throw new InvalidOperationException();

        public abstract IProcessParameters TryGetProcessParameters(object parameter, IBrowsableObjectInfo sourcePath, IEnumerable<IBrowsableObjectInfo> paths);

        string IProcessFactoryProcessInfoBase.GetUserConfirmationText() => null;
    }

    public abstract class ArchiveProcessInfo<T> : ArchiveProcessInfo where T : IArchiveProcessParameters
    {
        protected abstract T GetParameters();

        protected abstract object GetParametersViewModel(T parameters);

        public override IProcessParameters TryGetProcessParameters(object parameter, IBrowsableObjectInfo sourcePath, IEnumerable<IBrowsableObjectInfo> paths)
        {
            var parameters = GetParameters();

            return new DialogWindow(Name) { ContentTemplateSelector = new InterfaceDataTemplateSelector(), Content = GetParametersViewModel(parameters), DialogButton = DialogButton.OKCancel }.ShowDialog() == true ? parameters.ToProcessParameters(sourcePath.Path, paths.Select(path => path.Path)) : null;
        }
    }

    public abstract class ArchiveObjectProcessInfo<T> : ArchiveProcessInfo<T> where T : IArchiveProcessParameters
    {
        protected virtual bool CanRun(IShellObjectInfoBase2 path) => path.InnerObject.IsFileSystemObject;

        protected override bool CanRun(IBrowsableObjectInfo path) => path is IShellObjectInfoBase2 _path && CanRun(_path); // todo: check parent paths
    }

    public class ArchiveCompressionProcessInfo : ArchiveObjectProcessInfo<IArchiveCompressionParameters>
    {
        public override string Name => Shell.Properties.Resources.Compression;

        protected override IArchiveCompressionParameters GetParameters() => new ArchiveCompressionParameters();

        protected override object GetParametersViewModel(IArchiveCompressionParameters parameters) => new ArchiveCompressionParametersViewModel(parameters);
    }

    public class ArchiveExtractionProcessInfo : ArchiveObjectProcessInfo<IArchiveExtractionParameters>
    {
        public override string Name => Shell.Properties.Resources.Extraction;

        protected override bool CanRunOverride(object parameter, IBrowsableObjectInfo sourcePath, IEnumerable<IBrowsableObjectInfo> paths)
        {
            bool ok = false;

            foreach (IBrowsableObjectInfo path in paths)

                if (ok)

                    return false;

                else

                    ok = true;

            return ok && base.CanRunOverride(parameter, sourcePath, paths);
        }

        protected override bool CanRun(IShellObjectInfoBase2 path) => base.CanRun(path) && path.ObjectProperties is IFileSystemObjectInfoProperties properties && properties.FileType == FileType.Archive;

        protected override IArchiveExtractionParameters GetParameters() => new ArchiveExtractionParameters();

        protected override object GetParametersViewModel(IArchiveExtractionParameters parameters) => new ArchiveExtractionParametersViewModel(parameters);
    }
}
