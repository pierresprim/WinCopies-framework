//* Copyright © Pierre Sprimont, 2020
// *
// * This file is part of the WinCopies Framework.
// *
// * The WinCopies Framework is free software: you can redistribute it and/or modify
// * it under the terms of the GNU General Public License as published by
// * the Free Software Foundation, either version 3 of the License, or
// * (at your option) any later version.
// *
// * The WinCopies Framework is distributed in the hope that it will be useful,
// * but WITHOUT ANY WARRANTY; without even the implied warranty of
// * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// * GNU General Public License for more details.
// *
// * You should have received a copy of the GNU General Public License
// * along with the WinCopies Framework.  If not, see <https://www.gnu.org/licenses/>. */

//using Microsoft.WindowsAPICodePack.Shell;
//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.IO;
//using System.Text;
//using WinCopies.Util;
//using static WinCopies.Util.Util;
//using IfCT = WinCopies.Util.Util.ComparisonType;
//using IfCM = WinCopies.Util.Util.ComparisonMode;
//using IfComp = WinCopies.Util.Util.Comparison;
//using System.Linq;
//using WinCopies.Collections;
//using System.Security;
//using System.Reflection;
//using SevenZip;
//using System.Collections.ObjectModel;
//using System.Collections;

using Microsoft.Win32;
using SevenZip;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Text;
using WinCopies.Util;
using static WinCopies.Util.Util;

namespace WinCopies.IO
{
    public static class Path
    {
        public const char PathSeparator = '\\';

        //public static readonly string[] PathEnvironmentVariables = { "AllUserProfile", "AppData", "CommonProgramFiles", "CommonProgramFiles(x86)", "HomeDrive", "LocalAppData", "ProgramData", "ProgramFiles", "ProgramFiles(x86)", "Public", "SystemDrive", "SystemRoot", "Temp", "UserProfile" };

        private static readonly Dictionary<InArchiveFormat, string[]> dic = new Dictionary<InArchiveFormat, string[]>();

        public static ReadOnlyDictionary<InArchiveFormat, string[]> InArchiveFormats { get; }

        // public new event PropertyChangedEventHandler PropertyChanged;

        static Path()
        {
            // todo: to add the other 'in' archive formats

            dic.Add(InArchiveFormat.Zip, new string[] { ".zip" });

            dic.Add(InArchiveFormat.SevenZip, new string[] { ".7z" });

            dic.Add(InArchiveFormat.Arj, new string[] { ".arj" });

            dic.Add(InArchiveFormat.BZip2, new string[] { ".bz2", ".tar", ".xz" });

            dic.Add(InArchiveFormat.Cab, new string[] { ".cab" });

            dic.Add(InArchiveFormat.Chm, new string[] { ".chm" });

            dic.Add(InArchiveFormat.Compound, new string[] { ".cfb" });

            dic.Add(InArchiveFormat.Cpio, new string[] { ".cpio" });

            dic.Add(InArchiveFormat.CramFS, null);

            dic.Add(InArchiveFormat.Deb, new string[] { ".deb", ".udeb" });

            dic.Add(InArchiveFormat.Dmg, new string[] { ".dmg" });

            dic.Add(InArchiveFormat.Elf, new string[] { ".axf", ".bin", ".elf", ".o", ".prx", ".puff", ".ko", ".mod", ".so" });

            dic.Add(InArchiveFormat.Fat, null);

            dic.Add(InArchiveFormat.Flv, new string[] { ".flv" });

            dic.Add(InArchiveFormat.GZip, new string[] { ".gz" });

            dic.Add(InArchiveFormat.Hfs, new string[] { ".hfs" });

            dic.Add(InArchiveFormat.Iso, new string[] { ".iso" });

            dic.Add(InArchiveFormat.Lzh, new string[] { ".lzh" });

            dic.Add(InArchiveFormat.Lzma, new string[] { "lzma" });

            dic.Add(InArchiveFormat.Lzma86, new string[] { ".lzma86" });

            dic.Add(InArchiveFormat.Lzw, new string[] { ".lzw" });

            dic.Add(InArchiveFormat.MachO, new string[] { ".o", ".dylib", ".bundle" });

            dic.Add(InArchiveFormat.Mbr, new string[] { ".mbr" });

            dic.Add(InArchiveFormat.Msi, new string[] { ".msi", ".msp" });

            dic.Add(InArchiveFormat.Mslz, new string[] { ".mslz" });

            dic.Add(InArchiveFormat.Mub, new string[] { ".mub" });

            dic.Add(InArchiveFormat.Nsis, new string[] { ".exe" });

            dic.Add(InArchiveFormat.Ntfs, null);

            dic.Add(InArchiveFormat.PE, new string[] { ".dll", ".ocx", ".sys", ".scr", ".drv", ".efi" });

            dic.Add(InArchiveFormat.Ppmd, new string[] { ".ppmd" });

            dic.Add(InArchiveFormat.Rar, new string[] { ".rar" });

            dic.Add(InArchiveFormat.Rar4, null);

            dic.Add(InArchiveFormat.Rpm, new string[] { ".rpm" });

            dic.Add(InArchiveFormat.Split, new string[] { ".split" });

            dic.Add(InArchiveFormat.SquashFS, null);

            dic.Add(InArchiveFormat.Swf, new string[] { ".swf" });

            dic.Add(InArchiveFormat.Swfc, null);

            dic.Add(InArchiveFormat.Tar, new string[] { ".tar", "tar.gz", "tar.bz2", "tar.xz" });

            dic.Add(InArchiveFormat.TE, null);

            dic.Add(InArchiveFormat.Udf, null);

            dic.Add(InArchiveFormat.UEFIc, null);

            dic.Add(InArchiveFormat.UEFIs, null);

            dic.Add(InArchiveFormat.Vhd, new string[] { ".vhd" });

            dic.Add(InArchiveFormat.Wim, new string[] { ".wim", ".swm" });

            dic.Add(InArchiveFormat.Xar, new string[] { ".xar" });

            dic.Add(InArchiveFormat.XZ, new string[] { ".xz" });

            InArchiveFormats = new ReadOnlyDictionary<InArchiveFormat, string[]>(dic);
        }

        //        public static ITreeNode< FileSystemObjectInfo > GetBrowsableObjectInfoFromPath(string path, bool parent/*, bool deepArchiveCheck*/)

        //        {

        //            path = path.Replace('/', PathSeparator);

        //            if (path.EndsWith(PathSeparator.ToString()) && !path.EndsWith(":\\") && !path.EndsWith(":\\\\"))

        //                path = path.Substring(0, path.LastIndexOf(PathSeparator));

        //            path = GetRealPathFromEnvironmentVariables(path);

        //            string[] paths = path.Split(PathSeparator);

        //            var shellObject = ShellObject.FromParsingName(paths[0]);

        //            var browsableObjectInfo = new BrowsableObjectTreeNode<FileSystemObjectInfo, FileSystemObjectInfo, BrowsableObjectInfoFactory>( shellObject.IsFileSystemObject
        //                ? new ShellObjectInfo(paths[0], FileType.Drive, SpecialFolder.OtherFolderOrFile, shellObject, ShellObjectInfo.DefaultShellObjectDeepClone)
        //                                : new ShellObjectInfo(paths[0], FileType.SpecialFolder, GetSpecialFolder(shellObject), shellObject, ShellObjectInfo.DefaultShellObjectDeepClone), new ShellObjectInfoFactory());

        //            if (paths.Length == 1)

        //                return browsableObjectInfo;

        //            // int archiveSubpathsCount = 0;

        //            bool dispose = !parent;

        //            FileSystemObjectInfo getBrowsableObjectInfo( FileSystemObjectInfo newValue )

        //            {

        //                if (dispose)

        //                {

        //                    var temp = ( FileSystemObjectInfo )newValue.DeepClone();

        //                    browsableObjectInfo.ItemsLoader.Dispose();

        //                    browsableObjectInfo.Dispose();

        //                    return temp;

        //                }

        //                else

        //                {

        //                    // todo: uncomment

        //                    // newValue.ItemsLoader.Dispose();

        //                    return newValue;

        //                }

        //            }

        //            for (int i = 1; i < paths.Length; i++)

        //            {

        //                if (!browsableObjectInfo.Value.IsBrowsable && i < paths.Length - 1)

        //                    throw new DirectoryNotFoundException("The path isn't valid.", browsableObjectInfo.Value);

        //                if (If(IfCT.Xor, IfCM.Logical, IfComp.Equal, out bool key, true, GetKeyValuePair(false, browsableObjectInfo.Value.FileType == FileType.Archive), GetKeyValuePair(true, browsableObjectInfo.Value is ArchiveItemInfo)))

        //                {

        //                    // archiveSubpathsCount++;

        //                    // todo: re-use:

        //                    using (var archiveLoader = new ArchiveLoader<ArchiveItemInfo, ArchiveItemInfo, ArchiveItemInfo, ArchiveItemInfoFactory, ArchiveItemInfoFactory>( new BrowsableObjectTreeNode<ArchiveItemInfo, ArchiveItemInfo, ArchiveItemInfoFactory>( (ArchiveItemInfo) browsableObjectInfo.Value, new ArchiveItemInfoFactory() ), GetAllEnumFlags<FileTypes>(), true, false))

        //                        archiveLoader.LoadItems();

        //                    string s = paths[i].ToLower();

        //                    browsableObjectInfo = new BrowsableObjectTreeNode<FileSystemObjectInfo, FileSystemObjectInfo, BrowsableObjectInfoFactory>( getBrowsableObjectInfo( ((System.Collections.Generic.IList< TreeNode< FileSystemObjectInfo >>) browsableObjectInfo).FirstOrDefault(item => item.Value.Path.Substring(item.Value.Path.LastIndexOf(IO.Path.PathSeparator) + 1).ToLower() == s).Value as FileSystemObjectInfo ?? throw new FileNotFoundException("The path could not be found.", browsableObjectInfo.Value)), new ShellObjectInfoFactory());

        //                }

        //                else if (key && i < paths.Length - 1    /*&& deepArchiveCheck*/)

        //                    throw new IOException("The 'Open from archive' feature is currently not supported by the WinCopies framework.", browsableObjectInfo.Value);

        //                else

        //                {

        //                    shellObject = ((IShellObjectInfo)browsableObjectInfo).ShellObject;

        //                    string s = shellObject.ParsingName;

        //                    if (!s.EndsWith(PathSeparator.ToString()))

        //                        s += PathSeparator;

        //                    s += paths[i];

        //                    // string _s = s.Replace(IO.Path.PathSeparator, "\\\\");

        //                    shellObject = ((ShellContainer)shellObject).FirstOrDefault(item => If(IfCT.Or, IfCM.Logical, IfComp.Equal, paths[i], item.Name, item.GetDisplayName(DisplayNameType.RelativeToParent))) as ShellObject ?? throw new FileNotFoundException("The path could not be found.", browsableObjectInfo.Value);

        //                    SpecialFolder specialFolder = GetSpecialFolder(shellObject);

        //                    FileType fileType = specialFolder == SpecialFolder.OtherFolderOrFile ? shellObject.IsLink ? FileType.Link : shellObject is ShellFile shellFile ? IsSupportedArchiveFormat(System.IO.Path.GetExtension(s)) ? FileType.Archive : FileType.File : FileType.Drive : FileType.SpecialFolder;

        //                    //if (shellObject.IsFileSystemObject)

        //                    //if (Directory.Exists(_s) || File.Exists(_s)) // We also check the files because the path can be an archive.

        //                    //    browsableObjectInfo = new ShellObjectInfo(ShellObject.FromParsingName(s), s);

        //                    //else

        //                    //#if DEBUG

        //                    //                    {

        //                    //#endif

        //#pragma warning disable IDE0068 // Disposed manually when needed
        //                    browsableObjectInfo = new BrowsableObjectTreeNode<FileSystemObjectInfo, FileSystemObjectInfo, BrowsableObjectInfoFactory>( getBrowsableObjectInfo(new ShellObjectInfo(s, fileType, specialFolder, shellObject, ShellObjectInfo.DefaultShellObjectDeepClone)), new ShellObjectInfoFactory());
        //#pragma warning restore IDE0068

        //#if DEBUG

        //                    Debug.WriteLine(((IShellObjectInfo)browsableObjectInfo).ShellObject.GetDisplayName(DisplayNameType.RelativeToParent));

        //                    //}

        //#endif

        //                }

        //            }

        //            return new BrowsableObjectTreeNode<FileSystemObjectInfo, FileSystemObjectInfo, BrowsableObjectInfoFactory>( getBrowsableObjectInfo(browsableObjectInfo.Value), new ShellObjectInfoFactory());

        //        }

        public static bool IsSupportedArchiveFormat(in string extension)
        {
            foreach (KeyValuePair<InArchiveFormat, string[]> value in InArchiveFormats)

                if (value.Value != null)

                    foreach (string _extension in value.Value)

                        if (_extension == extension)

                            return true;

            return false;
        }

        //        public static bool MatchToFilter(string path, string filter)

        //        {

        //            string pathWithoutExtension = System.IO.Path.GetDirectoryName(path) + IO.Path.PathSeparator + System.IO.Path.GetFileNameWithoutExtension(path);

        //            bool checkFilters(string[] filters)

        //            {

        //                foreach (string _filter in filters)

        //                {

        //                    if (string.IsNullOrEmpty(_filter)) continue;

        //                    if (pathWithoutExtension.Length >= _filter.Length && pathWithoutExtension.Contains(_filter))

        //                        pathWithoutExtension = pathWithoutExtension.Substring(pathWithoutExtension.IndexOf(_filter) + _filter.Length);

        //                    else return false;

        //                }

        //                return true;

        //            }

        //            if (filter.Contains("."))

        //            {

        //                int i = filter.LastIndexOf(".");

        //                string[] filters1 = filter.Substring(0, i).Split('*');

        //                string[] filters2 = filter.Length > i + 1 ? filter.Substring(i + 1).Split('*') : null;

        //                return checkFilters(filters1) && checkFilters(filters2);

        //            }

        //            else

        //                return checkFilters(filter.Split('*'));

        //        }

        //        //        public static ShellObjectInfo GetNormalizedOSPath(string basePath)

        //        //        {

        //        //            ShellObject shellObject = null;

        //        //            FileType fileType = FileType.None;

        //        //            SpecialFolders specialFolder = SpecialFolders.OtherFolderOrFile;

        //        //            string path = null;

        //        //            void setSpecialFolder(SpecialFolders specialFolder_)

        //        //            {

        //        //                specialFolder = specialFolder_;

        //        //                fileType = FileType.SpecialFolder;

        //        //            }

        //        //            // if (fileType != FileTypes.File && fileType != FileTypes.Folder) throw new ArgumentException("Invalid fileType parameter value. Accepted values are FileTypes.File or FileTypes.Folder.") ; 

        //        //#if DEBUG

        //        //            Debug.WriteLine("KnownFolders.Libraries.CanonicalName: " + KnownFolders.Downloads.CanonicalName);

        //        //            Debug.WriteLine("KnownFolders.Libraries.LocalizedName: " + KnownFolders.Downloads.LocalizedName);

        //        //            Debug.WriteLine("KnownFolders.Libraries.Path: " + KnownFolders.Downloads.Path);

        //        //            Debug.WriteLine("KnownFolders.Libraries.ParsingName: " + KnownFolders.Downloads.ParsingName);

        //        //#endif

        //        //            if (basePath.EndsWith(IO.Path.PathSeparator))

        //        //                basePath = basePath.Substring(0, basePath.Length - 1);

        //        //#if DEBUG

        //        //            Debug.WriteLine(basePath);

        //        //#endif

        //        //            if (basePath.EndsWith(":"))

        //        //            {

        //        //                shellObject = ShellObject.FromParsingName(basePath);

        //        //                path = basePath;

        //        //                fileType = FileType.Drive;

        //        //            }

        //        //            else if (basePath.Contains(":"))

        //        //            {

        //        //                shellObject = ShellObject.FromParsingName(basePath);

        //        //                if (shellObject is IKnownFolder)

        //        //                {

        //        //                    string shellObjectDisplayName = shellObject.GetDisplayName(DisplayNameType.Default);

        //        //                    if (shellObjectDisplayName == KnownFolders.Libraries.LocalizedName)

        //        //                        setSpecialFolder(SpecialFolders.UsersLibraries);

        //        //                    else if (shellObjectDisplayName == KnownFolders.Desktop.LocalizedName)

        //        //                        setSpecialFolder(SpecialFolders.Desktop);

        //        //                    else

        //        //                        fileType = FileType.Folder;

        //        //                }

        //        //                else if (shellObject is ShellFile)

        //        //                    fileType = FileType.File;

        //        //                path = basePath;

        //        //            }

        //        //            else

        //        //            {

        //        //                if (basePath == Util.LibrariesName || basePath == Util.LibrariesLocalizedName)

        //        //                {

        //        //                    shellObject = (ShellObject)KnownFolders.Libraries;

        //        //                    setSpecialFolder(SpecialFolders.UsersLibraries);

        //        //                    path = KnownFolders.Libraries.Path;

        //        //                }

        //        //                else if (basePath == KnownFolders.Desktop.CanonicalName || basePath == KnownFolders.Desktop.LocalizedName)

        //        //                {

        //        //                    shellObject = (ShellObject)KnownFolders.Desktop;

        //        //                    setSpecialFolder(SpecialFolders.Desktop);

        //        //                    path = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);

        //        //                }

        //        //                else

        //        //                    fileType = FileType.Folder;

        //        //                // else if (basePath.StartsWith(LibrariesName + IO.Path.PathSeparator) || basePath.StartsWith(LibrariesLocalizedName)) { shellObject=(ShellObject)KnownFolders.Libraries KnownFolders.Libraries.Path + basePath.Substring(KnownFolders.Libraries.LocalizedName.Length);

        //        //                // else if

        //        //            }

        //        //#if DEBUG

        //        //            Debug.WriteLine("Path: " + path);

        //        //#endif

        //        //            return new ShellObjectInfo(shellObject, path, fileType, specialFolder);

        //        //        }

        ///// <summary>
        ///// Returns the <see cref="IO.SpecialFolder"/> value for a given <see cref="Microsoft.WindowsAPICodePack.Shell.ShellObject"/>.
        ///// </summary>
        ///// <param name="shellObject">The <see cref="Microsoft.WindowsAPICodePack.Shell.ShellObject"/> from which to return a <see cref="IO.SpecialFolder"/> value.</param>
        ///// <returns>A <see cref="IO.SpecialFolder"/> value that correspond to the given <see cref="Microsoft.WindowsAPICodePack.Shell.ShellObject"/>.</returns>
        //public static SpecialFolder GetSpecialFolder(ShellObject shellObject)

        //{

        //    SpecialFolder? value = null;

        //    PropertyInfo[] knownFoldersProperties = typeof(KnownFolders).GetProperties();

        //    for (int i = 1; i < knownFoldersProperties.Length; i++)

        //        // try
        //        // {

        //        // for (; i < knownFoldersProperties.Length; i++)

        //        if (shellObject.ParsingName == knownFoldersProperties[i].Name)

        //            value = (SpecialFolder)typeof(SpecialFolder).GetField(knownFoldersProperties[i].Name).GetValue(null);

        //    // break;

        //    // }

        //    // catch (ShellException) { i++; }

        //    return value ?? SpecialFolder.None;

        //}

        // todo:

        //        public static SpecialFolder GetSpecialFolderFromPath(string path, ShellObject shellObject)

        //        {

        //            SpecialFolder specialFolder = SpecialFolder.OtherFolderOrFile;

        //            if (path.EndsWith(IO.Path.PathSeparator)) path = path.Substring(0, path.Length - 1);

        //#if DEBUG

        //            Debug.WriteLine(path);

        //#endif

        //            if (path.EndsWith(":"))

        //                specialFolder = SpecialFolder.OtherFolderOrFile;

        //            else if (path.Contains(":"))

        //                // todo: to add other values

        //                if (shellObject is IKnownFolder)

        //                {

        //                    string shellObjectParsingName = shellObject.ParsingName;

        //                    if (shellObjectParsingName == KnownFolders.Libraries.ParsingName) specialFolder = SpecialFolder.UsersLibraries;

        //                    else if (shellObjectParsingName == KnownFolders.Desktop.ParsingName) specialFolder = SpecialFolder.Desktop;

        //                }

        //                else

        //                if (path == KnownFolders.UsersLibraries.ParsingName || path == KnownFolders.UsersLibraries.LocalizedName)

        //                    specialFolder = SpecialFolder.UsersLibraries;

        //            // else if (basePath.StartsWith(LibrariesName + IO.Path.PathSeparator) || basePath.StartsWith(LibrariesLocalizedName)) { shellObject=(ShellObject)KnownFolders.Libraries KnownFolders.Libraries.Path + basePath.Substring(KnownFolders.Libraries.LocalizedName.Length);

        //            // else if

        //            return specialFolder;

        //        }

        //        public static string RenamePathWithAutomaticNumber(string path, string destPath)

        //        {

        //            string newPath = destPath + IO.Path.PathSeparator + System.IO.Path.GetFileName(path);

        //            if (!(Directory.Exists(newPath) || File.Exists(newPath)))

        //                //if (System.IO.Directory.Exists(path))

        //                //    System.IO.Directory.Move(path, newPath);

        //                //else if (System.IO.File.Exists(path))

        //                //    System.IO.File.Move(path, newPath);

        //                return newPath;



        //            long pathParenthesesNumber = -1;

        //            string getFileNameWithoutParentheses(string fileName, out long parenthesesNumber)

        //            {

        //                // We remove, if any, the last parentheses that are in the file name if they contain a number and if this number is lesser than long.MaxValue.

        //                if (fileName.Contains(" (") && fileName.EndsWith(")"))

        //                {

        //                    int index = fileName.LastIndexOf(" (");

        //                    string parenthesesContent = fileName.Substring(index + 2, fileName.Length - (index + 3));

        //                    if (/*parenthesesContent.Length > 0 &&*/ long.TryParse(parenthesesContent, out parenthesesNumber) && parenthesesNumber >= 0)

        //                        return fileName.Substring(0, index);

        //                }

        //                parenthesesNumber = -1;

        //                return fileName;

        //            }

        //            // Variables initialization

        //            // long number = 1;

        //            string _fileNameWithoutExtension = "";

        //            long _parenthesesNumber = -1;



        //            // We get all items that are in the same folder as the destPath parameter.

        //            string[] directories = Directory.GetDirectories(destPath);

        //            string[] files = Directory.GetFiles(destPath);



        //            // Then, we get the file name of the current path without its extension.

        //            string fileNameWithoutExtension = getFileNameWithoutParentheses(System.IO.Path.GetFileNameWithoutExtension(path), out pathParenthesesNumber);



        //            foreach (string directory in directories)

        //            {

        //                // de nouveau on reprend le nom de l'élément, ici, le dossier, sans son extension éventuelle:

        //                _fileNameWithoutExtension = getFileNameWithoutParentheses(System.IO.Path.GetFileNameWithoutExtension(directory), out _parenthesesNumber);



        //                // On fait ensuite une comparaison du nom de l'élément introduit par l'utilisateur avec le nom du dossier véirifé actuellement :

        //                if (_fileNameWithoutExtension.ToLower() == fileNameWithoutExtension.ToLower() && _parenthesesNumber > pathParenthesesNumber)

        //#if DEBUG

        //                {

        //                    Debug.WriteLine(pathParenthesesNumber.ToString() + " " + _parenthesesNumber.ToString());

        //#endif

        //                    pathParenthesesNumber = _parenthesesNumber;

        //#if DEBUG

        //                }

        //#endif

        //            }



        //            foreach (string file in files)

        //            {

        //                // de nouveau on reprend le nom de l'élément, ici, le dossier, sans son extension éventuelle:

        //                _fileNameWithoutExtension = getFileNameWithoutParentheses(System.IO.Path.GetFileNameWithoutExtension(file), out _parenthesesNumber);



        //                // On fait ensuite une comparaison du nom de l'élément introduit par l'utilisateur avec le nom du dossier véirifé actuellement :

        //                if (_fileNameWithoutExtension.ToLower() == fileNameWithoutExtension.ToLower() && _parenthesesNumber > pathParenthesesNumber)

        //#if DEBUG

        //                {

        //                    // if (long.TryParse(partOfName, out number_2))

        //                    // {

        //                    Debug.WriteLine(pathParenthesesNumber.ToString() + " " + _parenthesesNumber.ToString());

        //#endif

        //                    pathParenthesesNumber = _parenthesesNumber;

        //                    // }



        //#if DEBUG

        //                }

        //#endif

        //            }



        //            return destPath + PathSeparator + fileNameWithoutExtension + " (" + (pathParenthesesNumber + 1).ToString() + ")" + System.IO.Path.GetExtension(path);


        //            // string new_Name = destPath + IO.Path.PathSeparator + fileNameWithoutExtension + " (" + (pathParenthesesNumber + 1).ToString() + ")" + System.IO.Path.GetExtension(path);



        //            // TODO : pertinent ? si oui, utiliser WinCopies.IO.FilesProcesses (avec un boolean pour voir s'il faut l'uitliser ou pas) ?



        //            // if (Directory.Exists(path) || File.Exists(path))

        //            // System.IO.Directory.Move(path, new_Name);


        //            // DirectoryInfo.MoveTo(Rename_Window.NewFullName)

        //            // else if (File.Exists(path))

        //            // System.IO.File.Move(path, new_Name);

        //            // return new_Name;

        //            // return null;

        //            // FileInfo.MoveTo(

        //            // Case FileTypes.Drive, FileTypes.Folder, FileTypes.File



        //            // ProcessDialogResult = True

        //            // Close()

        //        }

        public static string GetRealPathFromEnvironmentVariables(in string path)
        {
            string[] subPaths = path.Split(WinCopies.IO.Path.PathSeparator);

            var stringBuilder = new StringBuilder();

            int count = 0;

            foreach (string subPath in subPaths)
            {
                count++;

                if (subPath.StartsWith("%"))

                    _ = subPath.EndsWith("%") ? stringBuilder.Append(Environment.GetEnvironmentVariable(subPath

#if NETFRAMEWORK

                            .Substring(1, subPath.Length - 2)

#else

                        [1..^1]

#endif

                        )) : throw new ArgumentException("'path' is not a well-formatted environment variables path.");

                else

                    _ = stringBuilder.Append(subPath);

                if (count < subPaths.Length)

                    _ = stringBuilder.Append(WinCopies.IO.Path.PathSeparator);
            }

            return stringBuilder.ToString();
        }

        public static bool IsFileSystemPath(in string path)
        {
            if (IsNullEmptyOrWhiteSpace(path))

                throw GetArgumentNullException(nameof(path));

            char[] invalidChars = System.IO.Path.GetInvalidPathChars();

            return !path.ContainsOneOrMoreValues(invalidChars) && System.IO.Path.IsPathRooted(path);
        }

        public static bool IsRegistryPath(in string path)
        {
            FieldInfo[] fields = typeof(Microsoft.Win32.Registry).GetFields();

            foreach (FieldInfo field in fields)

                if (path.StartsWith(((RegistryKey)field.GetValue(null)).Name))

                    return true;

            return false;
        }

        public static bool IsWMIPath(in string path) => (IsNullEmptyOrWhiteSpace(path) ? throw GetArgumentNullException(nameof(path)) : path).StartsWith(PathSeparator);

        public static bool Exists(in string path) => System.IO.File.Exists(path) || System.IO.Directory.Exists(path);

        public static uint? PathFileNameContainsParenthesesNumber(string path) => FileNameContainsParenthesesNumber(IsFileSystemPath(path) ? System.IO.Path.GetFileNameWithoutExtension(path) : throw new ArgumentException($"{nameof(path)} is not a file system path."));

        public static uint? FileNameContainsParenthesesNumber(string fileName) => _FileNameContainsParenthesesNumber(IsNullEmptyOrWhiteSpace(fileName) ? throw GetArgumentNullException(nameof(fileName)) : fileName);

        private static uint? _FileNameContainsParenthesesNumber(string fileName)
        {
            Debug.Assert(!IsNullEmptyOrWhiteSpace(fileName), $"{nameof(fileName)} is null, empty or whitespace.");

            if (fileName.EndsWith(
#if NETFRAMEWORK
                ")", StringComparison.OrdinalIgnoreCase
#else
                ')'
#endif
                ) && fileName.Contains('(', StringComparison.OrdinalIgnoreCase))
            {
                var stringBuilder = new StringBuilder();

                for (int i = fileName.Length - 2; i > 0; i--)
                {
                    if (fileName[i] == '(')

                        break;

                    _ = stringBuilder.Append(fileName[i]);
                }

                if (uint.TryParse(stringBuilder.ToString(), NumberStyles.None, CultureInfo.CurrentCulture, out uint result))

                    return result;

                else

                    return null;
            }

            return null;
        }

        public static string RenameDuplicate(string path, in uint first = 2)
        {
            if (IsFileSystemPath(path))

                throw new ArgumentException($"{nameof(path)} is not a file system path.");

            if (path.EndsWith(
#if NETFRAMEWORK
                $"{PathSeparator}", StringComparison.OrdinalIgnoreCase
#else
                PathSeparator
#endif
                ))

                throw new ArgumentException($"{nameof(path)} is a root path.");

            string fileName = System.IO.Path.GetFileNameWithoutExtension(path);

            uint? result = _FileNameContainsParenthesesNumber(fileName);

            string getResult(in uint number) => $"{System.IO.Path.GetDirectoryName(path)}{PathSeparator}{fileName} ({number}){System.IO.Path.GetExtension(path)}";

            return result.HasValue ? getResult(result.Value > first ? result.Value + 1 : first) : getResult(first);
        }

        //        //public static string GetShortcutPath(string path)

        //        //{

        //        //    var paths = new List<KeyValuePair<string, string>>();

        //        //    foreach (string environmentPathVariable in PathEnvironmentVariables)

        //        //    {

        //        //        string _path = Environment.GetEnvironmentVariable(environmentPathVariable);

        //        //        if (_path != null)

        //        //            paths.Add(new KeyValuePair<string, string>(environmentPathVariable, _path));

        //        //    }



        //        //    paths.Sort((KeyValuePair<string, string> x, KeyValuePair<string, string> y) => x.Value.Length < y.Value.Length ? 1 : x.Value.Length == y.Value.Length ? 0 : -1);



        //        //    foreach (KeyValuePair<string, string> _path in paths)

        //        //        if (path.StartsWith(_path.Value))

        //        //        {

        //        //            path = "%" + _path.Key + "%" + path.Substring(_path.Value.Length);

        //        //            break;

        //        //        }

        //        //    return path;

        //        //}

    }
}
