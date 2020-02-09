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
using Microsoft.WindowsAPICodePack.Shell.PropertySystem;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using WinCopies.IO;
using WinCopies.Util.Data;
using Registry = WinCopies.IO.Registry;
using static WinCopies.Util.Util;

namespace WinCopies.GUI.Windows.Dialogs
{

    public delegate void MoveHandler(string sourcePath, string destPath);

    public class FilePropertiesDialog : DialogWindow
    {

        /// <summary>
        /// System.Kind
        /// </summary>
        public const string System_Kind = "System.Kind";

        /// <summary>
        /// The 'movie' property kind
        /// </summary>
        public const string Movie = "movie";

        /// <summary>
        /// The 'music' property kind
        /// </summary>
        public const string Music = "music";

        /// <summary>
        /// The 'recordedtv' property kind
        /// </summary>
        public const string RecordedTV = "recordedtv";

        /// <summary>
        /// The 'video' property kind
        /// </summary>
        public const string Video = "video";

        /// <summary>
        /// The 'calendar' property kind
        /// </summary>
        public const string Calendar = "calendar";

        /// <summary>
        /// The 'document' property kind
        /// </summary>
        public const string Document = "document";

        /// <summary>
        /// The 'picture' property kind
        /// </summary>
        public const string Picture = "picture";

        /// <summary>
        /// The 'Audio' property category
        /// </summary>
        public const string Audio = "Audio";

        /// <summary>
        /// The 'Calendar' property category
        /// </summary>
        public const string CalendarPropertyCategory = "Calendar";

        /// <summary>
        /// The 'Document' property category
        /// </summary>
        public const string DocumentPropertyCategory = "Document";

        /// <summary>
        /// The 'Image' property category
        /// </summary>
        public const string ImagePropertyCategory = "Image";

        /// <summary>
        /// The 'Media' property category
        /// </summary>
        public const string Media = "Media";

        /// <summary>
        /// The 'Music' property category
        /// </summary>
        public const string MusicPropertyCategory = "Music";

        /// <summary>
        /// The 'Photo' property category
        /// </summary>
        public const string Photo = "Photo";

        /// <summary>
        /// Identifies the <see cref="ShellObject"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ShellObjectProperty = DependencyProperty.Register(nameof(ShellObject), typeof(ShellObjectInfo), typeof(FilePropertiesDialog), new PropertyMetadata(null, (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
        {

            ((FilePropertiesDialog)d).FilePropertiesDialog_ShellObjectChanged(e);

        }));

        public ShellObjectInfo ShellObject { get => (ShellObjectInfo)GetValue(ShellObjectProperty); set => SetValue(ShellObjectProperty, value); }

        private static readonly DependencyPropertyKey OpenWithSoftwarePropertyKey = DependencyProperty.RegisterReadOnly(nameof(OpenWithSoftware), typeof(ShellObject), typeof(FilePropertiesDialog), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="OpenWithSoftware"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty OpenWithSoftwareProperty = OpenWithSoftwarePropertyKey.DependencyProperty;

        /// <summary>
        /// Gets the open-with software for the file handled by this <see cref="FilePropertiesDialog"/>. This is a dependency property.
        /// </summary>
        public ShellObject OpenWithSoftware => (ShellObject)GetValue(OpenWithSoftwareProperty);

        private static readonly DependencyPropertyKey OpenWithSoftwaresPropertyKey = DependencyProperty.RegisterReadOnly(nameof(OpenWithSoftwares), typeof(ShellObject[]), typeof(FilePropertiesDialog), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="OpenWithSoftwares"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty OpenWithSoftwaresProperty = OpenWithSoftwaresPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets the other open-with softwares for the file handled by this <see cref="FilePropertiesDialog"/>. This is a dependency property.
        /// </summary>
        public ShellObject[] OpenWithSoftwares => (ShellObject[])GetValue(OpenWithSoftwaresProperty);

        private static readonly DependencyPropertyKey PropertiesPropertyKey = DependencyProperty.RegisterReadOnly(nameof(Properties), typeof(ReadOnlyObservableCollection<NamedObject<ShellPropertyContainer>>), typeof(FilePropertiesDialog), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="Properties"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PropertiesProperty = PropertiesPropertyKey.DependencyProperty;

        public ReadOnlyObservableCollection<NamedObject<ShellPropertyContainer>> Properties { get => (ReadOnlyObservableCollection<NamedObject<ShellPropertyContainer>>)GetValue(PropertiesProperty); }

        // todo: to add a default implementation:

        /// <summary>
        /// Identifies the <see cref="MoveAction"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MoveActionProperty = DependencyProperty.Register(nameof(MoveAction), typeof(MoveHandler), typeof(FilePropertiesDialog));

        public MoveHandler MoveAction { get => (MoveHandler)GetValue(MoveActionProperty); set => SetValue(MoveActionProperty, value); }

        static FilePropertiesDialog() => DefaultStyleKeyProperty.OverrideMetadata(typeof(FilePropertiesDialog), new FrameworkPropertyMetadata(typeof(FilePropertiesDialog)));

        // public static RoutedUICommand DefineOpenWithSoftware { get; } = new RoutedUICommand("a", "a", typeof(FilePropertiesDialog), new InputGestureCollection());

        /// <summary>
        /// Initializes a new instance of the <see cref="FilePropertiesDialog"/> class using the specified <see cref="ShellObjectInfo"/> as data context.
        /// </summary>
        /// <param name="shellObject"></param>
        public FilePropertiesDialog(ShellObjectInfo shellObject)

        {

            ShellObject = shellObject;

            // Content = new Control { Template = (ControlTemplate)ResourcesHelper.Instance.ResourceDictionary["FilePropertiesDialogTemplate"] };

        }

        protected override void OnCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {

            (FolderBrowserDialog folderBrowserDialog, bool? result) showDialog()

            {

                FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();

                return (folderBrowserDialog, folderBrowserDialog.ShowDialog());

            }

            switch (e.Parameter)
            {

                case "DefineOpenWithSoftware":

                    break;

                case "MoveCurrentPath":

                    (FolderBrowserDialog folderBrowserDialog, bool? result) result = showDialog();

                    if (result.result == true)

                        MoveAction?.Invoke(ShellObject.Path, result.folderBrowserDialog.ExplorerControl.Path.Path);

                    break;

                default:

                    base.OnCommandExecuted(sender, e);

                    break;

            }

        }

        private void FilePropertiesDialog_ShellObjectChanged(DependencyPropertyChangedEventArgs e)

        {

            if (e.NewValue is IShellObjectInfo)

            {

                ShellObjectInfo new_Value = (ShellObjectInfo)e.NewValue;

                string ext = System.IO.Path.GetExtension(new_Value.Path);

                if (ext != "")

                {

                    // MessageBox.Show(openWithSoftware);

                    string openWithCommand = Registry.GetCommandFromExtension("open", ext);

                    if (openWithCommand != null)

                        SetValue(OpenWithSoftwarePropertyKey, Microsoft.WindowsAPICodePack.Shell.ShellObject.FromParsingName(Registry.GetOpenWithSoftwarePathFromCommand(openWithCommand)));

                    WinShellAppInfoInterop winShellAppInfoInterop = new WinShellAppInfoInterop(ext);

                    winShellAppInfoInterop.OpenWithAppInfosLoaded += (object _sender, EventArgs _e) =>

                    SetValue(OpenWithSoftwaresPropertyKey, winShellAppInfoInterop.OpenWithAppInfos);

                }



                string[] kinds = null;

                foreach (IShellProperty prop in new ShellPropertyCollection(new_Value.ShellObject))

                {



                    // shellObject.ShellObject.

                    if (prop.PropertyKey.PropertyId == SystemProperties.System.Kind.PropertyId)

#if DEBUG

                    {

                        Debug.WriteLine(prop.CanonicalName + " " + prop.Description.DisplayName + " " + prop.Description.EditInvitation + " " + prop.ValueType.ToString() + " " + prop.ValueAsObject as string);

#endif

                        if (prop.CanonicalName == System_Kind)

                        {

                            kinds = (string[])prop.ValueAsObject;

                            break;

                        }

#if DEBUG

                    }

#endif

                }

                List<NamedObject<ShellPropertyContainer>> specificValues = new List<NamedObject<ShellPropertyContainer>>();

                List<NamedObject<ShellPropertyContainer>> commonValues = new List<NamedObject<ShellPropertyContainer>>();

#if DEBUG

                if (kinds != null)

                    foreach (string kind in kinds)

                        Debug.WriteLine($"{System_Kind}: " + kind);

#endif

                if (kinds != null)

                {

                    PropertyInfo[] properties = typeof(ShellProperties.PropertySystem).GetProperties();

                    void addProperties(string propertyKind, Type propertyType, PropertyStoreItems propertyStoreItems)

                    {

                        foreach (PropertyInfo _property in propertyType.GetProperties())

                            specificValues.Add(new NamedObject<ShellPropertyContainer>(propertyKind, new ShellPropertyContainer((IShellProperty)_property.GetValue(propertyStoreItems))));

                        #region Comments

                        // if (shellProperty.Description.DisplayName == "Mode flash")

                        // {

                        // MessageBox.Show(shellProperty.ValueType.ToString());

                        //foreach (ShellPropertyEnumType propertyEnumType in shellProperty.Description.PropertyEnumTypes)

                        //    MessageBox.Show(propertyEnumType.DisplayText + " " + propertyEnumType.EnumType.ToString() + " " + propertyEnumType.ToString());

                        // MessageBox.Show(shellProperty.Description.PropertyEnumTypes[0].DisplayText + " " + shellProperty.Description.PropertyEnumTypes[0].EnumType.ToString() + " " + shellProperty.Description.PropertyEnumTypes[0].ToString());

                        // MessageBox.Show(shellProperty.Description.GroupingRange.ToString());

                        // }

                        #endregion

                    }

                    foreach (PropertyInfo property in properties)

                    {

                        // todo: to add the other properties

                        object propvalue = property.GetValue(new_Value.ShellObject.Properties.System);

                        if (property.PropertyType == typeof(ShellProperties.PropertySystemAudio) && (kinds.Contains(Movie) || kinds.Contains(Music) || kinds.Contains(RecordedTV) || kinds.Contains(Video)))

                            addProperties(Audio, typeof(ShellProperties.PropertySystemAudio), new_Value.ShellObject.Properties.System.Audio);

                        else if (property.PropertyType == typeof(ShellProperties.PropertySystemCalendar) && kinds.Contains(Calendar))

                            addProperties(CalendarPropertyCategory, typeof(ShellProperties.PropertySystemCalendar), new_Value.ShellObject.Properties.System.Calendar);

                        else if (property.PropertyType == typeof(ShellProperties.PropertySystemDocument) && kinds.Contains(Document))

                            // MessageBox.Show((property.PropertyType == typeof(ShellProperties.PropertySystemDocument)).ToString() + (kinds.Contains("document")).ToString());

                            addProperties(DocumentPropertyCategory, typeof(ShellProperties.PropertySystemDocument), new_Value.ShellObject.Properties.System.Document);

                        else if (property.PropertyType == typeof(ShellProperties.PropertySystemImage) && kinds.Contains(Picture))

                            addProperties(ImagePropertyCategory, typeof(ShellProperties.PropertySystemImage), new_Value.ShellObject.Properties.System.Image);

                        else if (property.PropertyType == typeof(ShellProperties.PropertySystemMedia) && (kinds.Contains(Movie) || kinds.Contains(Music) || kinds.Contains(RecordedTV) || kinds.Contains(Video)))

                            addProperties(Media, typeof(ShellProperties.PropertySystemMedia), new_Value.ShellObject.Properties.System.Media);

                        else if (property.PropertyType == typeof(ShellProperties.PropertySystemMusic) && kinds.Contains(Music))

                            // {

                            // MessageBox.Show("music");

                            addProperties(MusicPropertyCategory, typeof(Microsoft.WindowsAPICodePack.Shell.PropertySystem.ShellProperties.PropertySystemMusic), new_Value.ShellObject.Properties.System.Music);

                        // }

                        else if (property.PropertyType == typeof(ShellProperties.PropertySystemPhoto) && kinds.Contains(Picture))

                            // {

                            // MessageBox.Show("photo");

                            addProperties(Photo, typeof(Microsoft.WindowsAPICodePack.Shell.PropertySystem.ShellProperties.PropertySystemPhoto), new_Value.ShellObject.Properties.System.Photo);

                        // }

                        else if (property.GetValue(new_Value.ShellObject.Properties.System) is IShellProperty)

                        {

                            // if (property.GetValue(new_Value.ShellObject.Properties.System) is ShellProperties.PropertySystemDocument) MessageBox.Show(property.PropertyType.ToString());

                            IShellProperty value = (IShellProperty)property.GetValue(new_Value.ShellObject.Properties.System);

                            commonValues.Add(new NamedObject<ShellPropertyContainer>("Common", new ShellPropertyContainer(value)));

                        }

                    }

                }

                else

                {

                    //foreach (PropertyInfo property in properties)

                    //{

                    //    if (property.GetValue(new_Value.ShellObject.Properties.System) is IShellProperty)

                    //{

                    //    // if (property.GetValue(new_Value.ShellObject.Properties.System) is ShellProperties.PropertySystemDocument) MessageBox.Show(property.PropertyType.ToString());

                    //    IShellProperty value = (IShellProperty)property.GetValue(new_Value.ShellObject.Properties.System);

                    //    commonValues.Add(new Util.NamedObject<ShellPropertyContainer>("Common", new ShellPropertyContainer(value)));

                    //    }

                    //}

                    // ObservableCollection<ShellPropertyContainer> properties = new ObservableCollection<ShellPropertyContainer>();

                    if (new_Value.ShellObject.Properties.DefaultPropertyCollection != null)

                        foreach (IShellProperty property in new_Value.ShellObject.Properties.DefaultPropertyCollection)

                            commonValues.Add(new NamedObject<ShellPropertyContainer>("Common", new ShellPropertyContainer(property)));

                }

                List<NamedObject<ShellPropertyContainer>> propertiesOC = new List<NamedObject<ShellPropertyContainer>>(specificValues);

                IEnumerable<NamedObject<ShellPropertyContainer>> _propertiesOC = propertiesOC.Concat(commonValues);

                SetValue(PropertiesPropertyKey, new ReadOnlyObservableCollection<NamedObject<ShellPropertyContainer>>(new ObservableCollection<NamedObject<ShellPropertyContainer>>(_propertiesOC)));

                // Microsoft.WindowsAPICodePack.Shell.PropertySystem.SystemProperties.System.

            }

            else

                // todo:

                throw new ArgumentException("The data context must be a ShellObjectInfo.");

        }
    }
}
