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

using Microsoft.WindowsAPICodePack.Shell;
using SevenZip;
using System.Collections.ObjectModel;
using WinCopies.IO;

namespace WinCopies.GUI.Explorer
{
    //    public interface Explorer.IBrowsableObjectInfo

    //    {



    //    }

    public class ShellObjectInfo : IO.ShellObjectInfo, Explorer.IBrowsableObjectInfo, Explorer.IBrowsableObjectInfoInternal, Explorer.IBrowsableObjectInfoHelper
    {

        //private void SetProperty(string propertyName, string fieldName, object newValue)

        //{

        //    BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic |
        //                 BindingFlags.Static | BindingFlags.Instance |
        //                 BindingFlags.DeclaredOnly;

        //    object previousValue = null;

        //    FieldInfo field = GetType().GetField(fieldName, flags);

        //    previousValue = field.GetValue(this);

        //    if (!newValue.Equals(previousValue))

        //    {

        //        field.SetValue(this, newValue);

        //        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName, previousValue, newValue));

        //    }

        //}

        //protected virtual void OnPropertyChanged(string propertyName, string fieldName, object newValue, Type declaringType = null)

        //{

        //    var result = ((INotifyPropertyChanged)this).SetProperty(propertyName, fieldName, newValue, declaringType);

        //    if (result.propertyChanged) OnPropertyChanged(propertyName, result.oldValue, newValue);

        //}

        //protected virtual void OnPropertyChanged(string propertyName, object oldValue, object newValue) => PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName)); 

        public bool IsSelected { get; set; }

        public Explorer.IBrowsableObjectInfo SelectedItem { get; set; }

        public bool IsCheckBoxEnabled { get; set; }

        public ReadOnlyObservableCollection<Explorer.IBrowsableObjectInfo> SelectedItems { get; internal set; } = null;

        ReadOnlyObservableCollection<Explorer.IBrowsableObjectInfo> Explorer.IBrowsableObjectInfoHelper.SelectedItems { set => SelectedItems = value; }

        ObservableCollection<Explorer.IBrowsableObjectInfo> Explorer.IBrowsableObjectInfoInternal.SelectedItems { get; set; } = new ObservableCollection<Explorer.IBrowsableObjectInfo>();

        //public class Machin { public string Truc { get; set; } = "Bidule"; }

        //public Machin[] Columns
        //{

        //    get; set;

        //} = new Machin[] { new Machin() { Truc = "Chose" } };

        // public event SelectionChangedEventHandler SelectionChanged;

        public ShellObjectInfo(ShellObject shellObject, string path) : base(shellObject, path) =>            // ((INotifyCollectionChanged)Items).CollectionChanged += (object sender, NotifyCollectionChangedEventArgs e) => OnItemsCollectionChanged(e);

            BrowsableObjectInfoHelper.Init(this);

        public ShellObjectInfo(ShellObject shellObject, string path, FileType fileType, SpecialFolders specialFolder) : base(shellObject, path, fileType, specialFolder) => BrowsableObjectInfoHelper.Init(this);

        public ShellObjectInfo(IO.ShellObjectInfo shellObjectInfo) : this(shellObjectInfo.ShellObject, shellObjectInfo.Path, shellObjectInfo.FileType, shellObjectInfo.SpecialFolder) { }

        public override IO.IBrowsableObjectInfo GetBrowsableObjectInfo(ShellObject shellObject, string path) => new ShellObjectInfo(shellObject, path);

        public override IO.IBrowsableObjectInfo GetBrowsableObjectInfo(ShellObject shellObject, string path, FileType fileType, SpecialFolders specialFolder) => new ShellObjectInfo(shellObject, path, fileType, specialFolder);

        public override IO.IBrowsableObjectInfo GetBrowsableObjectInfo(IO.ShellObjectInfo archiveShellObject, ArchiveFileInfo? archiveFileInfo, string path, FileType fileType) => new ArchiveItemInfo(archiveShellObject, archiveFileInfo, path, fileType);

    }

    //public class FolderLoader : WinCopies.IO.FolderLoader
    //{

    //    public FolderLoader() : base(true, true)
    //    {

    //        #region Comments

    //        //            bgWorker.ProgressChanged += (object sender, ProgressChangedEventArgs e) =>
    //        //            {
    //        //                //#if DEBUG
    //        //                //                Console.WriteLine("new_Path.Thumbnail.BitmapSource.Height: " + ((IO.ShellObjectInfo)e.UserState).ShellObject.Thumbnail.BitmapSource.Height);
    //        //                //                Console.WriteLine("new_Path.Thumbnail.SmallBitmapSource.Height; " + ((IO.ShellObjectInfo)e.UserState).ShellObject.Thumbnail.SmallBitmapSource.Height);
    //        //                //                Console.WriteLine("new_Path.Thumbnail.MediumBitmapSource.Height: " + ((IO.ShellObjectInfo)e.UserState).ShellObject.Thumbnail.MediumBitmapSource.Height);
    //        //                //                Console.WriteLine("new_Path.Thumbnail.LargeBitmapSource.Height: " + ((IO.ShellObjectInfo)e.UserState).ShellObject.Thumbnail.LargeBitmapSource.Height);
    //        //                //                Console.WriteLine("new_Path.Thumbnail.ExtraLargeBitmapSource.Height: " + ((IO.ShellObjectInfo)e.UserState).ShellObject.Thumbnail.ExtraLargeBitmapSource.Height);
    //        //                //#endif

    //        //                Paths.Add(new ShellObjectInfo((IO.ShellObjectInfo)e.UserState));

    //        //            };

    //        //            bgWorker.DoWork += (object sender, DoWorkEventArgs e) =>
    //        //            {
    //        //#if DEBUG 
    //        //                Console.WriteLine("Dowork event started.");
    //        //#endif
    //        //                //List<string> directories = new List<string>();

    //        //                //List<string> files = new List<string>();

    //        //                List<IPathContainer> directories = new List<IPathContainer>();

    //        //                List<IPathContainer> files = new List<IPathContainer>();

    //        //                var comp = FolderLoader.comp.GetInstance();

    //        //                void AddDirectory(ref PathInfo pathInfo, bool isSpecialFolder = false)

    //        //                {

    //        //                    if (isSpecialFolder)

    //        //                        pathInfo.FileType = FileTypes.SpecialFolder;

    //        //                    else

    //        //                        pathInfo.FileType = FileTypes.Folder;

    //        //                    directories.Add(pathInfo);

    //        //                }

    //        //                void AddFile(ref PathInfo pathInfo, bool isLink)

    //        //                {

    //        //                    if (isLink)

    //        //                        pathInfo.FileType = FileTypes.Link;

    //        //                    // todo: add other in archive formats supported

    //        //                    else if (pathInfo.Path.EndsWith(".zip"))

    //        //                        pathInfo.FileType = FileTypes.Archive;

    //        //                    else

    //        //                        pathInfo.FileType = FileTypes.File;

    //        //                    files.Add(pathInfo);

    //        //                }

    //        //#if DEBUG

    //        //                Console.WriteLine("Path == null: " + (Path == null).ToString());

    //        //                Console.WriteLine("Path.Path: " + Path.Path);

    //        //                Console.WriteLine("Path.ShellObject: " + Path.ShellObject.ToString());

    //        //#endif

    //        //                foreach (Microsoft.WindowsAPICodePack.Shell.ShellObject so in (Microsoft.WindowsAPICodePack.Shell.ShellContainer)Path.ShellObject)

    //        //                    if (so.GetType() == typeof(Microsoft.WindowsAPICodePack.Shell.ShellFile))

    //        //                    {

    //        //#if DEBUG 
    //        //                        Console.WriteLine("a1");
    //        //                        Console.WriteLine(((Microsoft.WindowsAPICodePack.Shell.ShellFile)so).Path);
    //        //                        Console.WriteLine("Path: " + ((Microsoft.WindowsAPICodePack.Shell.ShellFile)so).Path);
    //        //                        Console.WriteLine("b");
    //        //#endif

    //        //                        files.Add(new FolderLoader.PathInfo() { Path = ((Microsoft.WindowsAPICodePack.Shell.ShellFile)so).Path, Normalized_Path = null, Shell_Object = so, FileType = FileTypes.File });

    //        //                    }

    //        //                    else if (so is Microsoft.WindowsAPICodePack.Shell.FileSystemKnownFolder)

    //        //                    {

    //        //#if DEBUG
    //        //                        Console.WriteLine("a3");
    //        //                        Console.WriteLine(((Microsoft.WindowsAPICodePack.Shell.FileSystemKnownFolder)so).ParsingName);
    //        //                        // Console.WriteLine("Path: " + ((Microsoft.WindowsAPICodePack.Shell.FileSystemKnownFolder)so).Path);
    //        //                        Console.WriteLine("b");
    //        //#endif

    //        //                        String path = ((Microsoft.WindowsAPICodePack.Shell.ShellFileSystemFolder)so).ParsingName;

    //        //                        PathInfo pathInfo = new PathInfo() { Path = path, Normalized_Path = null, Shell_Object = so };

    //        //                        if (System.IO.File.Exists(path))

    //        //                        {

    //        //                            AddFile(ref pathInfo, so.IsLink);

    //        //                        }

    //        //                        else

    //        //                        {

    //        //                            AddDirectory(ref pathInfo);

    //        //                        }

    //        //                    }

    //        //                    else if (so is Microsoft.WindowsAPICodePack.Shell.IKnownFolder)

    //        //                    {

    //        //#if DEBUG
    //        //                        Console.WriteLine("a2");
    //        //                        Console.WriteLine(((Microsoft.WindowsAPICodePack.Shell.IKnownFolder)so).ParsingName);
    //        //                        Console.WriteLine("Path: " + ((Microsoft.WindowsAPICodePack.Shell.IKnownFolder)so).Path);
    //        //                        Console.WriteLine("b");
    //        //#endif

    //        //                        String path = ((Microsoft.WindowsAPICodePack.Shell.ShellFileSystemFolder)so).ParsingName;

    //        //                        PathInfo pathInfo = new PathInfo() { Path = path, Normalized_Path = null, Shell_Object = so };

    //        //                        if (System.IO.File.Exists(path))

    //        //                            AddFile(ref pathInfo, so.IsLink);

    //        //                        else

    //        //                            AddDirectory(ref pathInfo);

    //        //                    }

    //        //                    else if (so is Microsoft.WindowsAPICodePack.Shell.NonFileSystemKnownFolder)

    //        //                    {

    //        //#if DEBUG
    //        //                        Console.WriteLine("a4");
    //        //                        Console.WriteLine(((Microsoft.WindowsAPICodePack.Shell.NonFileSystemKnownFolder)so).Path);
    //        //                        Console.WriteLine("Path: " + ((Microsoft.WindowsAPICodePack.Shell.NonFileSystemKnownFolder)so).Path);
    //        //                        Console.WriteLine("b");
    //        //#endif

    //        //                        String path = ((Microsoft.WindowsAPICodePack.Shell.ShellFileSystemFolder)so).ParsingName;

    //        //                        PathInfo pathInfo = new PathInfo() { Path = path, Normalized_Path = null, Shell_Object = so };

    //        //                        if (System.IO.File.Exists(path))

    //        //                        {

    //        //                            AddFile(ref pathInfo, so.IsLink);

    //        //                        }

    //        //                        else

    //        //                        {

    //        //                            AddDirectory(ref pathInfo);

    //        //                        }

    //        //                    }

    //        //                    else if (so is Microsoft.WindowsAPICodePack.Shell.ShellNonFileSystemFolder)

    //        //                    {

    //        //#if DEBUG
    //        //                        Console.WriteLine("a5");
    //        //                        Console.WriteLine(((Microsoft.WindowsAPICodePack.Shell.ShellNonFileSystemFolder)so).Name);
    //        //                        Console.WriteLine("Path: " + ((Microsoft.WindowsAPICodePack.Shell.ShellNonFileSystemFolder)so).Name);
    //        //                        Console.WriteLine("b");
    //        //#endif

    //        //                        String path = ((Microsoft.WindowsAPICodePack.Shell.ShellFileSystemFolder)so).ParsingName;

    //        //                        PathInfo pathInfo = new PathInfo() { Path = path, Normalized_Path = null, Shell_Object = so };

    //        //                        if (System.IO.File.Exists(path))

    //        //                        {

    //        //                            AddFile(ref pathInfo, so.IsLink);

    //        //                        }

    //        //                        else

    //        //                        {

    //        //                            AddDirectory(ref pathInfo, true);

    //        //                        }

    //        //                    }

    //        //#if DEBUG 
    //        //                Console.WriteLine("c");
    //        //#endif

    //        //                for (int i = 0; i < directories.Count; i++)

    //        //                {

    //        //                    FolderLoader.PathInfo directory = (FolderLoader.PathInfo)directories[i];

    //        //                    Boolean showFolder = false;

    //        //                    try
    //        //                    {

    //        //                        DirectoryInfo folder = new DirectoryInfo(directory.Path);
    //        //#if DEBUG 
    //        //                        Console.WriteLine("folder == null: " + (folder == null).ToString());
    //        //                        Console.WriteLine("directory.Path: " + directory.Path);
    //        //#endif
    //        //                        showFolder = !folder.Attributes.HasFlag(FileAttributes.Hidden);

    //        //                    }
    //        //                    catch { showFolder = true; }

    //        //                    String CurrentFile_Normalized = "";

    //        //                    if (showFolder)

    //        //                    {

    //        //                        CurrentFile_Normalized = directory.Path.Normalize(System.Text.NormalizationForm.FormD);

    //        //                        String currentFile_Normalized_ = String.Empty;

    //        //                        foreach (char ch in CurrentFile_Normalized)

    //        //                            if (Char.GetUnicodeCategory(ch) != UnicodeCategory.NonSpacingMark) currentFile_Normalized_ += ch;



    //        //                        CurrentFile_Normalized = currentFile_Normalized_.Normalize(System.Text.NormalizationForm.FormC);

    //        //                        // directories.Add(new FolderLoader.PathInfo { Path = directory, Normalized_Path = CurrentFile_Normalized });

    //        //                        directory.Normalized_Path = CurrentFile_Normalized;

    //        //                        directories[i] = directory;



    //        //                    }

    //        //                    else directories.RemoveAt(i);

    //        //                }

    //        //                directories.Sort(comp);

    //        //                for (int i = 0; i < files.Count; i++)

    //        //                {

    //        //                    FolderLoader.PathInfo file = (FolderLoader.PathInfo)files[i];

    //        //                    Boolean showFile = false;

    //        //                    try
    //        //                    {

    //        //                        FileInfo file_ = new FileInfo(file.Path);

    //        //                        showFile = !file_.Attributes.HasFlag(FileAttributes.Hidden);

    //        //                    }
    //        //                    catch { showFile = true; }

    //        //                    String CurrentFile_Normalized = "";

    //        //                    if (showFile)

    //        //                    {

    //        //                        CurrentFile_Normalized = file.Path.Normalize(System.Text.NormalizationForm.FormD);

    //        //                        String currentFile_Normalized_ = String.Empty;

    //        //                        foreach (char ch in CurrentFile_Normalized)

    //        //                            if (Char.GetUnicodeCategory(ch) != UnicodeCategory.NonSpacingMark) currentFile_Normalized_ += ch;

    //        //                        CurrentFile_Normalized = currentFile_Normalized_.Normalize(System.Text.NormalizationForm.FormC);

    //        //                        // pathInfosFiles.Add(new FolderLoader.PathInfo { Path = file, Normalized_Path = CurrentFile_Normalized });

    //        //                        file.Normalized_Path = CurrentFile_Normalized;

    //        //                        files[i] = file;



    //        //                    }

    //        //                    else files.RemoveAt(i);

    //        //                }

    //        //                files.Sort(comp);



    //        //                void reportProgressAndAddNewPathToObservableCollection(FolderLoader.PathInfo path_)

    //        //                {
    //        //#if DEBUG
    //        //                    Console.WriteLine("Current thread is background: " + System.Threading.Thread.CurrentThread.IsBackground);
    //        //                    Console.WriteLine("path_.Path: " + path_.Path);
    //        //                    Console.WriteLine("path_.Normalized_Path: " + path_.Normalized_Path);
    //        //                    Console.WriteLine("path_.Shell_Object: " + path_.Shell_Object);
    //        //#endif

    //        //                    var new_Path = path_.Shell_Object;
    //        //                    // new_Path.LoadThumbnail();

    //        //                    bgWorker.ReportProgress(0, new IO.ShellObjectInfo(new_Path, path_.Path, path_.FileType, WinCopies.IO.Util.GetSpecialFolderFromPath(path_.Path, new_Path)));

    //        //#if DEBUG

    //        //                    Console.WriteLine("Ceci est un " + new_Path.GetType().ToString());

    //        //#endif

    //        //                }



    //        //                foreach (FolderLoader.PathInfo path_ in directories)

    //        //                {

    //        //                    reportProgressAndAddNewPathToObservableCollection(path_);

    //        //                }

    //        //                foreach (FolderLoader.PathInfo path_ in files)

    //        //                {

    //        //                    reportProgressAndAddNewPathToObservableCollection(path_);

    //        //                    //for (long i = 0; i <= 100000000000; i++)

    //        //                    //{



    //        //                    //}

    //        //                }

    //        //                //foreach (ShellObject so in (ShellFolder)Path.ShellObject)

    //        //                //{

    //        //                //    Console.WriteLine("Is known folder: " + (so is IKnownFolder).ToString());

    //        //                //    Console.WriteLine("Is shell library: " + (so is ShellLibrary).ToString());

    //        //                //    Console.WriteLine("Shell object type: " + so.GetType().ToString());

    //        //                //    Console.WriteLine("Shell MIME type: " + so.Properties.System.MIMEType.Value);

    //        //                //    try

    //        //                //    {

    //        //                //        Console.WriteLine(ShellLibrary.Load(so.Name, KnownFolders.Libraries.Path, false).Name);

    //        //                //    }
    //        //                //    catch (Exception ex) { Console.WriteLine(ex.Message); }

    //        //                //    bgWorker.ReportProgress(0, new IO.ShellObjectInfo(so, so.ParsingName));

    //        //                //}
    //        //            };

    //        #endregion

    //    }

    //    protected override IO.Explorer.IBrowsableObjectInfo OnAddingNewBrowsableObjectInfo(IO.Explorer.IBrowsableObjectInfo newBrowsableObjectInfo) => new ShellObjectInfo((WinCopies.IO.ShellObjectInfo)newBrowsableObjectInfo);

    //    //        public event PropertyChangedEventHandler PropertyChanged;

    //    //        public void getitems(IO.ShellObjectInfo path)

    //    //        {

    //    //            this.Path = path;

    //    //            Paths = new ObservableCollection<ShellObjectInfo>();

    //    //            if (bgWorker.IsBusy) { bgWorker.Cancel(); }
    //    //            bgWorker.RunWorkerAsync();

    //    //            // return paths_;






    //    //        }

    //    //        public struct PathInfo : WinCopies.IO.IPathContainer

    //    //        {

    //    //            public String Path { get; set; }

    //    //            public String Normalized_Path { get; set; }

    //    //            String IPathContainer.Path { get => Normalized_Path; }

    //    //            public Microsoft.WindowsAPICodePack.Shell.ShellObject Shell_Object { get; set; }

    //    //            public WinCopies.IO.FileTypes FileType { get; set; }

    //    //        }

    //    //        public class comp : Comparer<WinCopies.IO.IPathContainer> // Variable locale pour stocker une référence vers l'instance

    //    //        {


    //    //            private static comp instance = null;

    //    //            private static readonly Object mylock = new Object();

    //    //            public StringComparer bidule { get; set; } = StringComparer.Create(CultureInfo.CurrentCulture, true);

    //    //            // Le constructeur est Private
    //    //            private comp()
    //    //            {
    //    //                //
    //    //            }

    //    //            // La méthode GetInstance doit être Shared
    //    //            public static comp GetInstance()
    //    //            {
    //    //                if (instance == null)

    //    //                    lock (mylock)

    //    //                        // Si pas d'instance existante on en crée une...
    //    //                        if (instance == null)

    //    //                            instance = new comp();

    //    //                // On retourne l'instance de Singleton
    //    //                return instance;

    //    //            }

    //    //            public override Int32 Compare(WinCopies.IO.IPathContainer x, WinCopies.IO.IPathContainer y)
    //    //            {
    //    //                return bidule.Compare(x.Path, y.Path);
    //    //            }

    //    //        }
    //    //    }

    //}

}
