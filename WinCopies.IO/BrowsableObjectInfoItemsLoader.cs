﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;

using WinCopies.Collections;
using WinCopies.Util;

using BackgroundWorker = WinCopies.Util.BackgroundWorker;

namespace WinCopies.IO
{

    public interface IBrowsableObjectInfoItemsLoader : IBackgroundWorker, IDisposable
    {

        IEnumerable<string> Filter { get; set; }

        IBrowsableObjectInfo Path { get; }

        bool CheckFilter(string path);

        void LoadItems();

        void LoadItemsAsync();

        /// <summary>
        /// Disposes the current <see cref="IBrowsableObjectInfoItemsLoader"/>.
        /// </summary>
        /// <exception cref="InvalidOperationException">This <see cref="IBrowsableObjectInfoItemsLoader"/> is busy and does not support cancellation.</exception>
        new void Dispose();

        /// <summary>
        /// Disposes the current <see cref="IBrowsableObjectInfoItemsLoader"/> and optionally disposes the related <see cref="Path"/>.
        /// </summary>
        /// <param name="disposePath">Whether to dispose the related <see cref="Path"/>. If this parameter is set to <see langword="true"/>, the <see cref="IBrowsableObjectInfo.ItemsLoader"/>s of the parent and childs of the related <see cref="Path"/> will be disposed recursively.</param>
        /// <exception cref="InvalidOperationException">This <see cref="IBrowsableObjectInfoItemsLoader"/> is busy and does not support cancellation.</exception>
        void Dispose(bool disposePath);

    }

    /// <summary>
    /// The base class for the <see cref="IBrowsableObjectInfo"/> items loaders.
    /// </summary>
    public abstract class BrowsableObjectInfoItemsLoader : IBrowsableObjectInfoItemsLoader, IBackgroundWorker

    {

        private readonly BackgroundWorker backgroundWorker = new BackgroundWorker();
        private readonly IComparer<IFileSystemObject> _fileSystemObjectComparer;
        private BrowsableObjectInfo _path;
        private readonly IEnumerable<string> _filter;

        public IComparer<IFileSystemObject> FileSystemObjectComparer { get => _fileSystemObjectComparer; set => this.SetBackgroundWorkerProperty(nameof(FileSystemObjectComparer), nameof(_fileSystemObjectComparer), value, typeof(BrowsableObjectInfoItemsLoader), true); }

        //public void changePath(IBrowsableObjectInfo newValue)

        //{



        //}

        IBrowsableObjectInfo IBrowsableObjectInfoItemsLoader.Path => Path;

        /// <summary>
        /// Gets the path from which to load the items.
        /// </summary>
        public BrowsableObjectInfo Path
        {
            get => PathOverride; internal set

            {

                OnPathChanging(value);

                PathOverride = value;

                OnPathChanged(value);

            }
        }

        protected virtual BrowsableObjectInfo PathOverride
        {
            get => _path; set => _path = value;
        }

        protected virtual void OnItemsChanging(NotifyCollectionChangedEventArgs e)
        {

            if (e.NewItems != null)

                foreach (var item in e.NewItems)

                    if (item is BrowsableObjectInfo _browsableObjectInfo)

                        _browsableObjectInfo.Parent = (IBrowsableObjectInfo)Path;

        }

        private void ItemsChanging(object sender, NotifyCollectionChangedEventArgs e) => OnItemsChanging(e);

        /// <summary>
        /// Provides for classes that derive from this one to do operations when the path is changing and to throw if the path is not from an expected type.
        /// </summary>
        /// <param name="path">The new path to set the <see cref="Path"/> property with.</param>
        protected abstract void OnPathChanging(BrowsableObjectInfo path);

        protected virtual void OnPathChanged(BrowsableObjectInfo path)
        {

            ((INotifyCollectionChanging)_path.Items).CollectionChanging -= ItemsChanging;

            ((INotifyCollectionChanging)path.Items).CollectionChanging += ItemsChanging;

        }

        public IEnumerable<string> Filter { get => _filter; set => this.SetBackgroundWorkerProperty(nameof(Filter), nameof(_filter), value, typeof(BrowsableObjectInfoItemsLoader), true); }

        /// <summary>
        /// Gets a value that indicates whether the thread is busy.
        /// </summary>
        public bool IsBusy => backgroundWorker.IsBusy;

        /// <summary>
        /// Gets or sets a value that indicates whether the thread can notify of the progress.
        /// </summary>
        public bool WorkerReportsProgress { get => backgroundWorker.WorkerReportsProgress; set => backgroundWorker.WorkerReportsProgress = value; }

        /// <summary>
        /// Gets or sets a value that indicates whether the thread supports cancellation.
        /// </summary>
        public bool WorkerSupportsCancellation { get => backgroundWorker.WorkerSupportsCancellation; set => backgroundWorker.WorkerSupportsCancellation = value; }

        /// <summary>
        /// Gets the <see cref="System.Threading.ApartmentState"/> of this thread.
        /// </summary>
        public ApartmentState ApartmentState { get => backgroundWorker.ApartmentState; set => backgroundWorker.ApartmentState = value; }

        /// <summary>
        /// Gets a value that indicates whether the thread must try to cancel before finished the background tasks.
        /// </summary>
        public bool CancellationPending => backgroundWorker.CancellationPending;

        /// <summary>
        /// Gets a value that indicates whether the working has been cancelled.
        /// </summary>
        public bool IsCancelled => backgroundWorker.IsCancelled;

        /// <summary>
        /// Gets the current progress of the working in percent.
        /// </summary>
        public int Progress => backgroundWorker.Progress;

        /// <summary>
        /// Gets or sets the <see cref="ISite"/> associated with the <see cref="IComponent"/>.
        /// </summary>
        /// <value>The <see cref="ISite"/> object associated with the component; or <see langword="null"/>, if the component does not have a site.</value>
        /// <remarks>Sites can also serve as a repository for container-specific, per-component information, such as the component name.</remarks>
        public ISite Site { get => backgroundWorker.Site; set => backgroundWorker.Site = value; }

        /// <summary>
        /// <para>Called when the background thread starts. Put your background working code here.</para>
        /// <para>The event handler is running in the background thread.</para>
        /// </summary>
        public event DoWorkEventHandler DoWork;

        /// <summary>
        /// <para>Called when the background thread reports progress.</para>
        /// <para>The event handler is running in the main thread.</para>
        /// </summary>
        public event ProgressChangedEventHandler ProgressChanged;

        /// <summary>
        /// <para>Called when the background thread has finished working.</para>
        /// <para>The event handler is running in the background thread.</para>
        /// </summary>
        public event RunWorkerCompletedEventHandler RunWorkerCompleted;

        /// <summary>
        /// Represents the method that handles the <see cref="Disposed"/> event of a component.
        /// </summary>
        /// <remarks>When you create a <see cref="Disposed"/> delegate, you identify the method that handles the event. To associate the event with your event handler, add an instance of the delegate to the event. The event handler is called whenever the event occurs, unless you remove the delegate. For more information about event handler delegates, see <a href="https://docs.microsoft.com/fr-fr/dotnet/standard/events/index?view=netframework-4.8">Handling and Raising Events</a>.</remarks>
        public event EventHandler Disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="BrowsableObjectInfoItemsLoader"/> class.
        /// </summary>
        /// <param name="workerReportsProgress">Whether the thread can notify of the progress.</param>
        /// <param name="workerSupportsCancellation">Whether the thread supports the cancellation.</param>
        public BrowsableObjectInfoItemsLoader(bool workerReportsProgress, bool workerSupportsCancellation) : this(workerReportsProgress, workerSupportsCancellation, new FileSystemObjectComparer()) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="BrowsableObjectInfoItemsLoader"/> class using a custom comparer.
        /// </summary>
        /// <param name="workerReportsProgress">Whether the thread can notify of the progress.</param>
        /// <param name="workerSupportsCancellation">Whether the thread supports the cancellation.</param>
        /// <param name="fileSystemObjectComparer">The comparer used to sort the loaded items.</param>
        public BrowsableObjectInfoItemsLoader(bool workerReportsProgress, bool workerSupportsCancellation, IComparer<IFileSystemObject> fileSystemObjectComparer)
        {

            WorkerReportsProgress = workerReportsProgress;

            WorkerSupportsCancellation = workerSupportsCancellation;

            FileSystemObjectComparer = fileSystemObjectComparer;

            ProgressChanged += (object sender, ProgressChangedEventArgs e) => OnProgressChanged(e);

            backgroundWorker.ProgressChanged += (object sender, ProgressChangedEventArgs e) => ProgressChanged(this, e);

            DoWork += (object sender, DoWorkEventArgs e) => OnDoWork(e);

            backgroundWorker.DoWork += (object sender, DoWorkEventArgs e) => DoWork(this, e);

            RunWorkerCompleted += (object sender, RunWorkerCompletedEventArgs e) => OnRunWorkerCompleted(e);

            backgroundWorker.RunWorkerCompleted += (object sender, RunWorkerCompletedEventArgs e) => RunWorkerCompleted(this, e);

            backgroundWorker.Disposed += (object sender, EventArgs e) => Disposed?.Invoke(this, e);

        }

        protected virtual void OnRunWorkerCompleted(RunWorkerCompletedEventArgs e) => ((BrowsableObjectInfo)_path).AreItemsLoaded = true;

        public abstract bool CheckFilter(string path);

        /// <summary>
        /// Notifies of the progress.
        /// </summary>
        /// <param name="percentProgress">
        /// Progress percentage.
        /// </param>
        public void ReportProgress(int percentProgress) => backgroundWorker.ReportProgress(percentProgress);

        /// <summary>
        /// Notifies of the progress.
        /// </summary>
        /// <param name="percentProgress">
        /// Progress percentage.
        /// </param>
        /// <param name="userState">
        /// User object.
        /// </param>
        public void ReportProgress(int percentProgress, object userState) => backgroundWorker.ReportProgress(percentProgress, userState);

        public static FileTypes FileTypeToFileTypeFlags(FileType fileType)

        {

            fileType.ThrowIfNotValidEnumValue();

            if (fileType == FileType.SpecialFolder) throw new ArgumentException("'" + nameof(fileType) + "' must be None, Folder, File, Drive, Link or Archive. '" + nameof(fileType) + "' is " + fileType.ToString() + ".");

            switch (fileType)

            {

                case FileType.Other:

                    return FileTypes.None;

                case FileType.Folder:

                    return FileTypes.Folder;

                case FileType.File:

                    return FileTypes.File;

                case FileType.Drive:

                    return FileTypes.Drive;

                case FileType.Link:

                    return FileTypes.Link;

                case FileType.Archive:

                    return FileTypes.Archive;

                default:

                    // This point should never be reached.

                    throw new NotImplementedException();

            }

        }

        protected abstract void OnDoWork();

        /// <summary>
        /// When overridden in a derived class, provides a handler for the <see cref="DoWork"/> event.
        /// </summary>
        /// <param name="e">Event args for the current event</param>
        protected virtual void OnDoWork(DoWorkEventArgs e) => OnDoWork();

        protected virtual void OnProgressChanged(ProgressChangedEventArgs e) => OnAddingPath(e.UserState as IBrowsableObjectInfo);

        // /// <summary>
        // /// Initializes a new instance of the <see cref="BrowsableObjectInfoItemsLoader"/> class with an <see cref="IBrowsableObjectInfo"/>.
        // /// </summary>
        // /// <param name="path">The path from which load items.</param>
        // public BrowsableObjectInfoItemsLoader(IBrowsableObjectInfo path) { path.ItemsLoader = this; }

        private void InitLoad()

        {

            if (_path == null) throw new NullReferenceException("'Path' is null.");

            if (!_path.IsBrowsable)

                throw new InvalidOperationException(string.Format(Generic.NotBrowsableObject, _path.FileType.ToString(), _path.ToString()));

            else if (backgroundWorker.IsBusy)

                backgroundWorker.Cancel();

            Path.items.Clear();

        }

        /// <summary>
        /// Loads the items of the <see cref="Path"/> object.
        /// </summary>
        public void LoadItems()

        {

            InitLoad();

            OnDoWork();

        }

        /// <summary>
        /// Loads the items of the <see cref="Path"/> object asynchronously.
        /// </summary>
        public void LoadItemsAsync()

        {

            InitLoad();

            backgroundWorker.RunWorkerAsync();

        }

        /// <summary>
        /// Cancels the working asynchronously.
        /// </summary>
        public void CancelAsync() => backgroundWorker.CancelAsync();

        /// <summary>
        /// Cancels the working.
        /// </summary>
        public void Cancel() => backgroundWorker.Cancel();

        /// <summary>
        /// Disposes the current <see cref="BrowsableObjectInfoItemsLoader"/>.
        /// </summary>
        /// <exception cref="InvalidOperationException">This <see cref="BrowsableObjectInfoItemsLoader"/> is busy and does not support cancellation.</exception>
        public virtual void Dispose() => Dispose(false);

        /// <summary>
        /// Disposes the current <see cref="BrowsableObjectInfoItemsLoader"/> and optionally disposes the related <see cref="Path"/>.
        /// </summary>
        /// <param name="disposePath">Whether to dispose the related <see cref="Path"/>. If this parameter is set to <see langword="true"/>, the <see cref="IBrowsableObjectInfo.ItemsLoader"/>s of the parent and childs of the related <see cref="Path"/> will be disposed recursively.</param>
        /// <exception cref="InvalidOperationException">This <see cref="BrowsableObjectInfoItemsLoader"/> is busy and does not support cancellation.</exception>
        public virtual void Dispose(bool disposePath)

        {

            backgroundWorker.Dispose();

            if (disposePath)

                Path.Dispose(true);

        }

        /// <summary>
        /// Suspends the current thread.
        /// </summary>
        public void Suspend() => backgroundWorker.Suspend();

        /// <summary>
        /// Resumes the current thread.
        /// </summary>
        public void Resume() => backgroundWorker.Resume();

        protected virtual void OnAddingPath(IBrowsableObjectInfo path) => (Path as BrowsableObjectInfo)?.items.Add(path);
    }

}
