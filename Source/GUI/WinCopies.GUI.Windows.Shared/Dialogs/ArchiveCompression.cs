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

using System.Collections;
using System.Windows;
using System.Windows.Input;
using static WinCopies.Util.Util;

//namespace WinCopies.GUI.Windows.Dialogs
//{
//    public class ArchiveCompression : DialogWindow
//    {

//        private bool isAutomaticPathChange = false;

//        // public static readonly string[] ArchiveFormats = { "Zip", "SevenZip", "GZip", "BZip2", "Tar", "XZ" };

//        // public static readonly string[] CompressionLevels = { (string)ResourcesHelper.Instance.ResourceDictionary["CompressionLevel_None"], (string)ResourcesHelper.Instance.ResourceDictionary["CompressionLevel_Fast"], (string)ResourcesHelper.Instance.ResourceDictionary["CompressionLevel_Low"], (string)ResourcesHelper.Instance.ResourceDictionary["CompressionLevel_Normal"], (string)ResourcesHelper.Instance.ResourceDictionary["CompressionLevel_High"], (string)ResourcesHelper.Instance.ResourceDictionary["CompressionLevel_Ultra"] };

//        // public static WinCopies.Util.CheckableString[] CompressionMethods { get; private set; } = null;

//        // public static readonly string[] CompressionMethods = { (string)ResourcesHelper.Instance.ResourceDictionary["CompressionMethod_Copy"], (string)ResourcesHelper.Instance.ResourceDictionary["CompressionMethod_Deflate"], (string)ResourcesHelper.Instance.ResourceDictionary["CompressionMethod_Deflate64"], (string)ResourcesHelper.Instance.ResourceDictionary["CompressionMethod_BZip2"], (string)ResourcesHelper.Instance.ResourceDictionary["CompressionMethod_Lzma"], (string)ResourcesHelper.Instance.ResourceDictionary["CompressionMethod_Lzma2"], (string)ResourcesHelper.Instance.ResourceDictionary["CompressionMethod_Ppmd"], (string)ResourcesHelper.Instance.ResourceDictionary["CompressionMethod_Default"] };

//        // public static readonly string[] CompressionModes = { (string)ResourcesHelper.Instance.ResourceDictionary["AppendToAnExistingFile"], (string)ResourcesHelper.Instance.ResourceDictionary["CreateANewFile"] };

//        /// <summary>
//        /// Identifies the <see cref="SourcePath"/> dependency property.
//        /// </summary>
//        public static readonly DependencyProperty SourcePathProperty = DependencyProperty.Register(nameof(SourcePath), typeof(string), typeof(ArchiveCompression), new PropertyMetadata(null,

//            (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
//            {

//                ArchiveCompression archiveCompression = (ArchiveCompression)d;

//                if (archiveCompression.isAutomaticPathChange) return;

//                string AddPaths(string[] paths)

//                {

//                    string _paths = null;

//                    foreach (string path in paths)

//                        _paths += path + "\n";

//                    return _paths;

//                }

//                    string sourcePath = archiveCompression.SourcePath;

//                    string pathRoot = null;

//                    if (sourcePath.EndsWith(":"))

//                        sourcePath += "\\";

//                    pathRoot = System.IO.Path.GetPathRoot(sourcePath);

//                    if (pathRoot != sourcePath && sourcePath.EndsWith("\\"))

//                        sourcePath = sourcePath.Substring(0, sourcePath.Length - 1);

//                    archiveCompression.isAutomaticPathChange = true;

//                    archiveCompression.SourcePath = sourcePath;

//                    archiveCompression.isAutomaticPathChange = false;

//                    archiveCompression.SetValue(PathsPropertyKey, AddPaths(Concatenate<string>(System.IO.Directory.GetDirectories(sourcePath), System.IO.Directory.GetFiles(sourcePath))));

//                    if (string.IsNullOrEmpty((string)archiveCompression.GetValue(DestPathProperty)))

//                    {

//                        archiveCompression.isAutomaticPathChange = true;

//                        archiveCompression.DestPath = pathRoot == sourcePath ? sourcePath + pathRoot[0] : new System.IO.DirectoryInfo(sourcePath).Parent.FullName + "\\" + System.IO.Path.GetFileName(sourcePath) + archiveCompression.GetExtension();

//                        archiveCompression.isAutomaticPathChange = false;

//                    }

//            }

//            ));

//        private string GetExtension()

//        {

//            switch (ArchiveFormat)

//            {

//                case OutArchiveFormat.Zip:

//                    return ".zip";

//                case OutArchiveFormat.SevenZip:

//                    return ".7z";

//                case OutArchiveFormat.GZip:

//                    return ".gz";

//                case OutArchiveFormat.BZip2:

//                    return ".bz2";

//                case OutArchiveFormat.Tar:

//                    return ".tar";

//                case OutArchiveFormat.XZ:

//                    return ".xz";

//                default:

//                    return "";

//            }

//        }

//        /// <summary>
//        /// Gets or sets the SourcePath. This is a dependency property.
//        /// </summary>
//        public string SourcePath { get => (string)GetValue(SourcePathProperty); set => SetValue(SourcePathProperty, value); }

//        /// <summary>
//        /// Identifies the <see cref="DestPath"/> dependency property.
//        /// </summary>
//        public static readonly DependencyProperty DestPathProperty = DependencyProperty.Register(nameof(DestPath), typeof(string), typeof(ArchiveCompression), new PropertyMetadata(null, (DependencyObject d, DependencyPropertyChangedEventArgs e) =>

//        {

//            ArchiveCompression archiveCompression = (ArchiveCompression)d;

//            if (archiveCompression.isAutomaticPathChange)

//                return;

//            string newPath = (string)e.NewValue + "\\" + System.IO.Path.GetFileNameWithoutExtension(archiveCompression.SourcePath) + archiveCompression.GetExtension();

//            archiveCompression.isAutomaticPathChange = true;

//            d.SetValue(DestPathProperty, newPath);

//            archiveCompression.isAutomaticPathChange = false;

//        }));

//        /// <summary>
//        /// Gets or sets the DestPath. This is a dependency property.
//        /// </summary>
//        public string DestPath { get => (string)GetValue(DestPathProperty); set => SetValue(DestPathProperty, value); }

//        private static readonly DependencyPropertyKey PathsPropertyKey = DependencyProperty.RegisterReadOnly(nameof(Paths), typeof(string), typeof(ArchiveCompression), new PropertyMetadata(null));

//        /// <summary>
//        /// Identifies the <see cref="Paths"/> dependency property.
//        /// </summary>
//        public static readonly DependencyProperty PathsProperty = PathsPropertyKey.DependencyProperty;

//        /// <summary>
//        /// Gets the paths to compress. Note that this property is only here for information and the paths can be modified until the compress process has finished.
//        /// </summary>
//        public string Paths => (string)GetValue(PathsProperty);

//        /// <summary>
//        /// Identifies the <see cref="ArchiveFormat"/> dependency property.
//        /// </summary>
//        public static readonly DependencyProperty ArchiveFormatProperty = DependencyProperty.Register(nameof(ArchiveFormat), typeof(OutArchiveFormat), typeof(ArchiveCompression), new PropertyMetadata(OutArchiveFormat.Zip, (DependencyObject d, DependencyPropertyChangedEventArgs e) =>

//        {

//            if (!string.IsNullOrEmpty((string)d.GetValue(DestPathProperty)) && !string.IsNullOrWhiteSpace((string)d.GetValue(DestPathProperty)))

//            {

//                string path = (string)d.GetValue(DestPathProperty);

//                string newPath = System.IO.Path.GetDirectoryName(path) + "\\" + System.IO.Path.GetFileNameWithoutExtension(path) + ((ArchiveCompression)d).GetExtension();

//                d.SetValue(DestPathProperty, newPath);

//            }

//        }));

//        /// <summary>
//        /// Gets or sets the <see cref="OutArchiveFormat"/> in which compress the paths to. This is a dependency property.
//        /// </summary>
//        public OutArchiveFormat ArchiveFormat { get => (OutArchiveFormat)GetValue(ArchiveFormatProperty); set => SetValue(ArchiveFormatProperty, value); }

//        /// <summary>
//        /// Identifies the <see cref="CompressionLevel"/> dependency property.
//        /// </summary>
//        public static readonly DependencyProperty CompressionLevelProperty = DependencyProperty.Register(nameof(CompressionLevel), typeof(CompressionLevel), typeof(ArchiveCompression), new PropertyMetadata(SevenZip.CompressionLevel.Normal));

//        public CompressionLevel CompressionLevel { get => (CompressionLevel)GetValue(CompressionLevelProperty); set => SetValue(CompressionLevelProperty, value); }

//        /// <summary>
//        /// Identifies the <see cref="CompressionMethod"/> dependency property.
//        /// </summary>
//        public static readonly DependencyProperty CompressionMethodProperty = DependencyProperty.Register(nameof(CompressionMethod), typeof(CompressionMethod), typeof(ArchiveCompression), new PropertyMetadata(CompressionMethod.Default));

//        public CompressionMethod CompressionMethod { get => (CompressionMethod)GetValue(CompressionMethodProperty); set => SetValue(CompressionMethodProperty, value); }

//        /// <summary>
//        /// Identifies the <see cref="CompressionMode"/> dependency property.
//        /// </summary>
//        public static readonly DependencyProperty CompressionModeProperty = DependencyProperty.Register(nameof(CompressionMode), typeof(CompressionMode), typeof(ArchiveCompression), new PropertyMetadata(CompressionMode.Create));

//        public CompressionMode CompressionMode { get => (CompressionMode)GetValue(CompressionModeProperty); set => SetValue(CompressionModeProperty, value); }

//        /// <summary>
//        /// Identifies the <see cref="DirectoryStructure"/> dependency property.
//        /// </summary>
//        public static readonly DependencyProperty DirectoryStructureProperty = DependencyProperty.Register(nameof(DirectoryStructure), typeof(bool), typeof(ArchiveCompression), new PropertyMetadata(true));

//        public bool DirectoryStructure { get => (bool)GetValue(DirectoryStructureProperty); set => SetValue(DirectoryStructureProperty, value); }

//        /// <summary>
//        /// Identifies the <see cref="IncludeEmptyDirectories"/> dependency property.
//        /// </summary>
//        public static readonly DependencyProperty IncludeEmptyDirectoriesProperty = DependencyProperty.Register(nameof(IncludeEmptyDirectories), typeof(bool), typeof(ArchiveCompression), new PropertyMetadata(true));

//        public bool IncludeEmptyDirectories { get => (bool)GetValue(IncludeEmptyDirectoriesProperty); set => SetValue(IncludeEmptyDirectoriesProperty, value); }



//        static ArchiveCompression() => DefaultStyleKeyProperty.OverrideMetadata(typeof(ArchiveCompression), new FrameworkPropertyMetadata(typeof(ArchiveCompression)));//#if DEBUG//            MessageBox.Show(ResourcesHelper.ResourceDictionary.Contains("SelectPathDialogDescription").ToString());//#endif 



//        public ArchiveCompression() =>

//            // Content = new Control { Template = (ControlTemplate)ResourcesHelper.Instance.ResourceDictionary["ArchiveCompressionDialogTemplate"] };

//            CommandBindings.Add(new CommandBinding(Commands.SelectPath, SelectPathCommand_Executed, SelectPathCommand_CanExecute));

//        #region Comments
//        //#if DEBUG//            MessageBox.Show(Resources .Contains("SelectPathDialogDescription").ToString()); //#endif //WinCopies.Util.CheckableString[] compressionMethods = new WinCopies.Util.CheckableString[8];//string[] compressionMethodsString = { "Copy", "Deflate", "Deflate64", "BZip2", "Lzma", "Lzma2", "Ppmd", "Default" };//WinCopies.Util.CheckableString checkableString = null;//string compressionMethod = null;//for (int i = 0; i <= 1; i++)//{//    compressionMethod = compressionMethodsString[i];//    checkableString = new WinCopies.Util.CheckableString(true, compressionMethod);//    // checkableString.PropertyChanged += CheckableString_PropertyChanged;//    compressionMethods[i] = checkableString;//}//CompressionMethods = compressionMethods;
//        #endregion


//        protected virtual void OnSelectPathCommandCanExecute(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = true;

//        private void SelectPathCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e) => OnSelectPathCommandCanExecute(sender, e);

//        protected virtual void OnSelectPathCommandExecuted(object sender, ExecutedRoutedEventArgs e)

//        {

//            // todo:

//            System.Windows.Forms.FolderBrowserDialog folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();

//            folderBrowserDialog.Description = Themes.Generic.SelectPathDialogDescription;

//            System.Windows.Forms.DialogResult dialogResult = folderBrowserDialog.ShowDialog();

//            if (dialogResult == System.Windows.Forms.DialogResult.OK)

//            {

//                switch ((string)e.Parameter)

//                {

//                    case "SourcePath":

//                        SourcePath = folderBrowserDialog.SelectedPath;

//                        break;

//                    case "DestPath":

//                        DestPath = folderBrowserDialog.SelectedPath;

//                        break;

//                }

//            }

//        }

//        private void SelectPathCommand_Executed(object sender, ExecutedRoutedEventArgs e) => OnSelectPathCommandExecuted(sender, e);

//    }

//}
