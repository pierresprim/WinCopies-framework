﻿using Microsoft.VisualBasic;
using Microsoft.WindowsAPICodePack.PortableDevices.CommandSystem.Object;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using WinCopies.Util;

namespace WinCopies.IO
{
    public interface IPathInfo
    {
        string Path { get; }

        bool IsDirectory { get; }
    }

    public readonly struct PathInfo : IPathInfo
    {
        public string Path { get; }

        public bool IsDirectory { get; }

        public PathInfo(in string path, bool isDirectory)
        {
            Path = path;

            IsDirectory = isDirectory;
        }
    }

    //public class FileInfoLoader
    //{

    //    ///// <summary>
    //    ///// Gets the paths to browse.
    //    ///// </summary>
    //    //public PathCollection Paths { get; }

    //    //public Size TotalSize { get; private set; } = new Size(0ul);

    //    //private System.Collections.ObjectModel.ObservableCollection<FileSystemInfo> _pathsLoaded { get; /*private*/ set; } = null;

    //    //public System.Collections.ObjectModel.ReadOnlyObservableCollection<FileSystemInfo> PathsLoaded { get; private set; } = null;

    //    //public List<string> Hidden_Folders_With_Subpaths = new List<string>();

    //    // private readonly ActionType actionType = ActionType.Unknown;

    //    /// <summary>
    //    /// Gets or sets the <see cref="FileProcesses. ActionType"/> of this <see cref="FilesInfoLoader"/>.
    //    /// </summary>
    //    public ActionType ActionType
    //    {

    //        get => actionType;

    //        set => OnPropertyChangedWhenNotBusy(nameof(ActionType), nameof(actionType), value, typeof(FilesInfoLoader));

    //    }

    //    //private readonly Search_Terms_Properties search_Terms = null;

    //    //public Search_Terms_Properties Search_Terms
    //    //{

    //    //    get => search_Terms;

    //    //    set => OnPropertyChangedWhenNotBusy(nameof(Search_Terms), nameof(search_Terms), value, typeof(FilesInfoLoader), true);

    //    //}

    //    //private bool loadOnlyItemsWithSearchTermsForAllActions = false;

    //    //public bool LoadOnlyItemsWithSearchTermsForAllActions { get => loadOnlyItemsWithSearchTermsForAllActions; set => OnPropertyChanged(nameof(LoadOnlyItemsWithSearchTermsForAllActions), nameof(loadOnlyItemsWithSearchTermsForAllActions), value, typeof(FilesInfoLoader)); }

    //    // TODO : avec un setter ? gérer les exceptions pour différents répertoires racines

    //    // private IList<FileSystemInfo> paths = null;

    //    // private FileSystemInfo _FileSystemInfoThatIsLoading = null;

    //    // TODO : vraiment utile ?

    //    //TODO : utile ?

    //    // public ObservableCollection<FileSystemInfo> _pathsLoaded = null;

    //    // TODO : vraiment utile ?

    //    // private long totalFolders_ = 0;

    //    // private long _totalFolders = 0;

    //    // public long TotalFolders { get=>_totalFolders; private set=>OnPropertyChanged(nameof(TotalFolders), nameof(_totalFolders), value, typeof(FilesInfoLoader)); } 

    //    // private readonly bool _IsLoaded = false;

    //    public bool IsLoaded { get => _IsLoaded; private set => OnPropertyChanged(nameof(IsLoaded), nameof(_IsLoaded), value, typeof(FilesInfoLoader)); }

    //    /// <summary>
    //    /// Occurs when a property value changes.
    //    /// </summary>
    //    public event PropertyChangedEventHandler PropertyChanged;

    //    protected void OnPropertyChanged(string propertyName, string fieldName, object newValue, Type declaringType)

    //    {

    //        (bool propertyChanged, object oldValue) = ((INotifyPropertyChanged)this).SetProperty(propertyName, fieldName, newValue, declaringType);

    //        if (propertyChanged) OnPropertyChanged(propertyName, oldValue, newValue);

    //    }

    //    protected void OnPropertyChangedWhenNotBusy(string propertyName, string fieldName, object newValue, Type declaringType)

    //    {

    //        (bool propertyChanged, object oldValue) = WinCopies.Util.Util.SetPropertyWhenNotBusy(this, propertyName, fieldName, newValue, declaringType);

    //        if (propertyChanged) OnPropertyChanged(propertyName, oldValue, newValue);

    //    }

    //    protected virtual void OnPropertyChanged(string propertyName, object oldValue, object newValue) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    //    // /// <summary>
    //    // /// Événement déclenché quand la recherche est terminée.
    //    // /// </summary>
    //    // public event EventHandler<FileInfoLoadedEventArgs> FileInfoLoaded;

    //    /// <summary>
    //    /// Initializes a new instance of the <see cref="FilesInfoLoader"/> class.
    //    /// </summary>
    //    public FilesInfoLoader() => SetProperties();

    //    /// <summary>
    //    /// Initializes a new instance of the <see cref="FilesInfoLoader"/> class using custom parameters.
    //    /// </summary>
    //    /// <param name="paths">The paths to browse.</param>
    //    /// <param name="actionType">The <see cref="WinCopies.IO.FileProcesses. ActionType"/> to set this <see cref="FilesInfoLoader"/> for.</param>
    //    public FilesInfoLoader(IList<FileSystemInfo> paths, ActionType actionType)
    //    {

    //        SetProperties();

    //        Paths = paths;

    //        ActionType = actionType;

    //    } // end FilesInfoLoader

    //    private void SetProperties()
    //    {
    //        //TODO:
    //        /*ActionType = ActionType.Unknown;

    //        Search_Terms = null;

    //        LoadOnlyItemsWithSearchTermsForAllActions = false;*/

    //        _pathsLoaded = new System.Collections.ObjectModel.ObservableCollection<FileSystemInfo>();

    //        PathsLoaded = new System.Collections.ObjectModel.ReadOnlyObservableCollection<FileSystemInfo>(_pathsLoaded);

    //        OnPropertyChanged(nameof(PathsLoaded), null, PathsLoaded);

    //    } // end void



    //    // protected virtual void OnPropertyChanged(string propertyName, object oldValue, object newValue) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));


    //}
}
