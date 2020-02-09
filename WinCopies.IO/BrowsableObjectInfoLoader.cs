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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;

using WinCopies.Collections;
using WinCopies.Util;

using BackgroundWorker = WinCopies.Util.BackgroundWorker;
using IDisposable = WinCopies.Util.IDisposable;

namespace WinCopies.IO
{

    //    /// <summary>
    //    /// Provides a base class for <see cref="IBrowsableObjectInfo"/> loading. See the Remarks section.
    //    /// </summary>
    //    /// <remarks>This class provides the <see cref="PathOverride"/> protected property. This property is for interoperability with generic classes based on this one. You should override this property in order to seal it.</remarks>
    //    public abstract class BrowsableObjectInfoLoader : IBrowsableObjectInfoLoader/*, IBackgroundWorker2*/

    //    {

    //#pragma warning disable IDE0069 // Disposed in the Dispose method overrides.
    //        private readonly BackgroundWorker backgroundWorker = new BackgroundWorker();
    //#pragma warning restore IDE0069
    //#pragma warning disable CS0649 // Set up using reflection
    //        private readonly IFileSystemObjectComparer<IFileSystemObject> _fileSystemObjectComparer;
    //        private readonly IEnumerable<string> _filter;
    //#pragma warning restore CS0649

    //        public IFileSystemObjectComparer<IFileSystemObject> FileSystemObjectComparer { get => _fileSystemObjectComparer; set => this.SetBackgroundWorkerProperty(nameof(FileSystemObjectComparer), nameof(_fileSystemObjectComparer), value, typeof(BrowsableObjectInfoLoader), true); }

    //        //public void changePath(IBrowsableObjectInfo newValue)

    //        //{



    //        //}

    //        //protected virtual BrowsableObjectInfo PathOverride { get => _path; set => _path = value; }

    //        public IEnumerable<string> Filter { get => _filter; set => this.SetBackgroundWorkerProperty(nameof(Filter), nameof(_filter), value, typeof(BrowsableObjectInfoLoader), true); }

    //        /// <summary>
    //        /// Gets a value that indicates whether the thread is busy.
    //        /// </summary>
    //        public bool IsBusy => backgroundWorker.IsBusy;

    //        /// <summary>
    //        /// Gets or sets a value that indicates whether the thread can notify of the progress.
    //        /// </summary>
    //        public bool WorkerReportsProgress { get => backgroundWorker.WorkerReportsProgress; set => backgroundWorker.WorkerReportsProgress = value; }

    //        /// <summary>
    //        /// Gets or sets a value that indicates whether the thread supports cancellation.
    //        /// </summary>
    //        public bool WorkerSupportsCancellation { get => backgroundWorker.WorkerSupportsCancellation; set => backgroundWorker.WorkerSupportsCancellation = value; }

    //        /// <summary>
    //        /// Gets the <see cref="System.Threading.ApartmentState"/> of this thread.
    //        /// </summary>
    //        public ApartmentState ApartmentState { get => backgroundWorker.ApartmentState; set => backgroundWorker.ApartmentState = value; }

    //        /// <summary>
    //        /// Gets a value that indicates whether the thread must try to cancel before finished the background tasks.
    //        /// </summary>
    //        public bool CancellationPending => backgroundWorker.CancellationPending;

    //        /// <summary>
    //        /// Gets a value that indicates whether the working has been cancelled.
    //        /// </summary>
    //        public bool IsCancelled => backgroundWorker.IsCancelled;

    //        /// <summary>
    //        /// Gets the current progress of the working in percent.
    //        /// </summary>
    //        public int Progress => backgroundWorker.Progress;

    //        /// <summary>
    //        /// Gets or sets the <see cref="ISite"/> associated with the <see cref="IComponent"/>.
    //        /// </summary>
    //        /// <value>The <see cref="ISite"/> object associated with the component; or <see langword="null"/>, if the component does not have a site.</value>
    //        /// <remarks>Sites can also serve as a repository for container-specific, per-component information, such as the component name.</remarks>
    //        public ISite Site { get => backgroundWorker.Site; set => backgroundWorker.Site = value; }

    //        /// <summary>
    //        /// <para>Called when the background thread starts. Put your background working code here.</para>
    //        /// <para>The event handler is running in the background thread.</para>
    //        /// </summary>
    //        public event DoWorkEventHandler DoWork;

    //        /// <summary>
    //        /// <para>Called when the background thread reports progress.</para>
    //        /// <para>The event handler is running in the main thread.</para>
    //        /// </summary>
    //        public event ProgressChangedEventHandler ProgressChanged;

    //        /// <summary>
    //        /// <para>Called when the background thread has finished working.</para>
    //        /// <para>The event handler is running in the background thread.</para>
    //        /// </summary>
    //        public event RunWorkerCompletedEventHandler RunWorkerCompleted;

    //        /// <summary>
    //        /// Represents the method that handles the <see cref="Disposed"/> event of a component.
    //        /// </summary>
    //        /// <remarks>When you create a <see cref="Disposed"/> delegate, you identify the method that handles the event. To associate the event with your event handler, add an instance of the delegate to the event. The event handler is called whenever the event occurs, unless you remove the delegate. For more information about event handler delegates, see <a href="https://docs.microsoft.com/en-us/dotnet/standard/events/index?view=netframework-4.8">Handling and Raising Events</a>.</remarks>
    //        public event EventHandler Disposed;

    //        /// <summary>
    //        /// Initializes a new instance of the <see cref="BrowsableObjectInfoLoader{TPath, TItems, TSubItems, TFactory}"/> class.
    //        /// </summary>
    //        /// <param name="workerReportsProgress">Whether the thread can notify of the progress.</param>
    //        /// <param name="workerSupportsCancellation">Whether the thread supports the cancellation.</param>
    //        protected BrowsableObjectInfoLoader(bool workerReportsProgress, bool workerSupportsCancellation) : this(workerReportsProgress, workerSupportsCancellation, new FileSystemObjectComparer<IFileSystemObject>()) { }

    //        /// <summary>
    //        /// Initializes a new instance of the <see cref="BrowsableObjectInfoLoader{TPath, TItems, TSubItems, TFactory}"/> class using a custom comparer.
    //        /// </summary>
    //        /// <param name="workerReportsProgress">Whether the thread can notify of the progress.</param>
    //        /// <param name="workerSupportsCancellation">Whether the thread supports the cancellation.</param>
    //        /// <param name="fileSystemObjectComparer">The comparer used to sort the loaded items.</param>
    //        protected BrowsableObjectInfoLoader(bool workerReportsProgress, bool workerSupportsCancellation, IFileSystemObjectComparer<IFileSystemObject> fileSystemObjectComparer)
    //        {

    //            WorkerReportsProgress = workerReportsProgress;

    //            WorkerSupportsCancellation = workerSupportsCancellation;

    //            FileSystemObjectComparer = fileSystemObjectComparer;

    //            ProgressChanged += (object sender, ProgressChangedEventArgs e) => OnProgressChanged(e);

    //            backgroundWorker.ProgressChanged += (object sender, ProgressChangedEventArgs e) => ProgressChanged(this, e);

    //            DoWork += (object sender, DoWorkEventArgs e) => OnDoWork(e);

    //            backgroundWorker.DoWork += (object sender, DoWorkEventArgs e) => DoWork(this, e);

    //            RunWorkerCompleted += (object sender, RunWorkerCompletedEventArgs e) => OnRunWorkerCompleted(e);

    //            backgroundWorker.RunWorkerCompleted += (object sender, RunWorkerCompletedEventArgs e) => RunWorkerCompleted(this, e);

    //            backgroundWorker.Disposed += (object sender, EventArgs e) => Disposed?.Invoke(this, e);

    //        }

    //        public bool IsDisposing { get; internal set; }

    //        public bool IsDisposed { get; internal set; }

    //        /// <summary>
    //        /// This method does anything because it is designed for deep cloning from constructor and not from an <see cref="object.MemberwiseClone"/> operation, and is here for overriding only. If you perform a deep cloning operation using an <see cref="object.MemberwiseClone"/> operation in <see cref="DeepCloneOverride(bool)"/>, you'll have to override this method if your class has to reinitialize members.
    //        /// </summary>
    //        /// <param name="browsableObjectInfoLoader">The cloned <see cref="BrowsableObjectInfoLoader{TPath, TItems, TSubItems, TFactory}"/>.</param>
    //        /// <param name="preserveIds">Whether to preserve IDs, if any, or to create new IDs.</param>
    //        protected virtual void OnDeepClone(BrowsableObjectInfoLoader browsableObjectInfoLoader, bool? preserveIds) { }

    //        /// <summary>
    //        /// When overridden in a derived class, gets a deep clone of this <see cref="BrowsableObjectInfoLoader{TPath, TItems, TSubItems, TFactory}"/>. The <see cref="OnDeepClone(BrowsableObjectInfoLoader{TPath}, bool)"/> method already has an implementation for deep cloning from constructor and not from an <see cref="object.MemberwiseClone"/> operation. If you perform a deep cloning operation using an <see cref="object.MemberwiseClone"/> operation in <see cref="DeepCloneOverride(bool)"/>, you'll have to override this method if your class has to reinitialize members.
    //        /// </summary>
    //        /// <param name="preserveIds">Whether to preserve IDs, if any, or to create new IDs.</param>
    //        protected abstract BrowsableObjectInfoLoader DeepCloneOverride(bool? preserveIds);

    //        public object DeepClone(bool? preserveIds)

    //        {

    //            ((IDisposable)this).ThrowIfDisposingOrDisposed();

    //            BrowsableObjectInfoLoader browsableObjectInfoLoader = DeepCloneOverride(preserveIds);

    //            OnDeepClone(browsableObjectInfoLoader, preserveIds);

    //            return browsableObjectInfoLoader;

    //        }

    //        public virtual bool NeedsObjectsOrValuesReconstruction => FileSystemObjectComparer.NeedsObjectsOrValuesReconstruction;

    //        public virtual bool CheckFilter(string path)

    //        {

    //            if (Filter is null) return true;

    //            foreach (string filter in Filter)

    //                if (!IO.Path.MatchToFilter(path, filter)) return false;

    //            return true;

    //        }

    //        /// <summary>
    //        /// Notifies of the progress.
    //        /// </summary>
    //        /// <param name="percentProgress">
    //        /// Progress percentage.
    //        /// </param>
    //        public void ReportProgress(int percentProgress) => backgroundWorker.ReportProgress(percentProgress);

    //        /// <summary>
    //        /// Notifies of the progress.
    //        /// </summary>
    //        /// <param name="percentProgress">
    //        /// Progress percentage.
    //        /// </param>
    //        /// <param name="userState">
    //        /// User object.
    //        /// </param>
    //        public void ReportProgress(int percentProgress, object userState) => backgroundWorker.ReportProgress(percentProgress, userState);

    //        public static FileTypes FileTypeToFileTypeFlags(FileType fileType)

    //        {

    //            fileType.ThrowIfNotValidEnumValue();

    //            if (fileType == FileType.SpecialFolder) throw new ArgumentException("'" + nameof(fileType) + "' must be None, Folder, File, Drive, Link or Archive. '" + nameof(fileType) + "' is " + fileType.ToString() + ".");

    //            switch (fileType)

    //            {

    //                case FileType.Other:

    //                    return FileTypes.None;

    //                case FileType.Folder:

    //                    return FileTypes.Folder;

    //                case FileType.File:

    //                    return FileTypes.File;

    //                case FileType.Drive:

    //                    return FileTypes.Drive;

    //                case FileType.Link:

    //                    return FileTypes.Link;

    //                case FileType.Archive:

    //                    return FileTypes.Archive;

    //                default:

    //                    // This point should never be reached.

    //                    throw new NotImplementedException();

    //            }

    //        }

    //        /// <summary>
    //        /// When overridden in a derived class, provides a handler for the <see cref="DoWork"/> event.
    //        /// </summary>
    //        /// <param name="e">Event args for the current event</param>
    //        protected abstract void OnDoWork(DoWorkEventArgs e);

    //        // /// <summary>
    //        // /// Initializes a new instance of the <see cref="BrowsableObjectInfoItemsLoader"/> class with an <see cref="IBrowsableObjectInfo"/>.
    //        // /// </summary>
    //        // /// <param name="path">The path from which load items.</param>
    //        // public BrowsableObjectInfoItemsLoader(IBrowsableObjectInfo path) { path.ItemsLoader = this; }

    //        /// <summary>
    //        /// Loads the items of the <see cref="IBrowsableObjectInfoLoader.Path"/> object.
    //        /// </summary>
    //        public virtual void LoadItems() => OnDoWork(new DoWorkEventArgs(null));

    //        /// <summary>
    //        /// Loads the items of the <see cref="IBrowsableObjectInfoLoader.Path"/> object asynchronously.
    //        /// </summary>
    //        public virtual void LoadItemsAsync() => backgroundWorker.RunWorkerAsync();

    //        /// <summary>
    //        /// Cancels the working asynchronously.
    //        /// </summary>
    //        public void CancelAsync(object stateInfo) => backgroundWorker.CancelAsync(stateInfo);

    //        public void CancelAsync() => CancelAsync(null);

    //        /// <summary>
    //        /// Cancels the working.
    //        /// </summary>
    //        public void Cancel(object stateInfo) => backgroundWorker.Cancel(stateInfo);

    //        public void Cancel() => Cancel(null);

    //        /// <summary>
    //        /// Suspends the current thread.
    //        /// </summary>
    //        public void Suspend() => backgroundWorker.Suspend();

    //        /// <summary>
    //        /// Resumes the current thread.
    //        /// </summary>
    //        public void Resume() => backgroundWorker.Resume();

    //        IBrowsableObjectInfo Path => PathOverride;

    //        /// <summary>
    //        /// This property is only itented for use in this class and in classes that derive directly from this class and sould be sealed in derived classes and not used directly. You can use the <see cref="Path"/> property instead.
    //        /// </summary>
    //        /// <seealso cref="Path"/>
    //        protected abstract IBrowsableObjectInfo PathOverride { get; set; }

    //#pragma warning disable CA1063 // Implement IDisposable Correctly: Implementation of IDisposable is enhanced for this class.
    //        /// <summary>
    //        /// Disposes the current <see cref="BrowsableObjectInfoLoader{TPath, TItems, TSubItems, TFactory}"/>.
    //        /// </summary>
    //        public void Dispose() => Dispose(false);

    //        /// <summary>
    //        /// Disposes the current <see cref="BrowsableObjectInfoLoader{TPath, TItems, TSubItems, TFactory}"/>.
    //        /// </summary>
    //        /// <exception cref="InvalidOperationException">This <see cref="BrowsableObjectInfoLoader{TPath, TItems, TSubItems, TFactory}"/> is busy and does not support cancellation.</exception>
    //        public void Dispose(bool disposePath)

    //        {

    //            IsDisposing = true;

    //            Dispose(true, disposePath);

    //            GC.SuppressFinalize(this);

    //            IsDisposed = true;

    //            IsDisposing = false;

    //        }

    //        ///// <summary>
    //        ///// Disposes the current <see cref="BrowsableObjectInfoLoader{TPath, TItems, TSubItems, TFactory}"/> and optionally disposes the related <see cref="Path"/>.
    //        ///// </summary>
    //        ///// <exception cref="InvalidOperationException">This <see cref="BrowsableObjectInfoLoader{TPath, TItems, TSubItems, TFactory}"/> is busy and does not support cancellation.</exception>
    //        //protected sealed override void DisposeOverride(bool disposing) => Dispose(disposing, false);

    //        /// <summary>
    //        /// Disposes the current <see cref="BrowsableObjectInfoLoader{TPath, TItems, TSubItems, TFactory}"/> and optionally disposes the related <see cref="Path"/>.
    //        /// </summary>
    //        /// <param name="disposePath">Whether to dispose the related <see cref="Path"/>. If this parameter is set to <see langword="true"/>, the <see cref="IBrowsableObjectInfo.ItemsLoader"/>s of the parent and childs of the related <see cref="Path"/> will be disposed recursively.</param>
    //        /// <exception cref="InvalidOperationException">This <see cref="BrowsableObjectInfoLoader{TPath, TItems, TSubItems, TFactory}"/> is busy and does not support cancellation.</exception>
    //        protected virtual void Dispose(bool disposing, bool disposePath)

    //        {

    //            // base.DisposeOverride(disposing);

    //            if (disposePath)

    //                PathOverride.Dispose();

    //            if (disposing)

    //                PathOverride = null;

    //        }

    //        protected abstract void OnRunWorkerCompleted(RunWorkerCompletedEventArgs e);
    //        protected abstract void OnAddingPath(IBrowsableObjectInfo path);
    //        protected abstract void OnProgressChanged(ProgressChangedEventArgs e);

    //#pragma warning restore CA1063 // Implement IDisposable Correctly: Implementation of IDisposable is enhanced for this class.

    //        /// <summary>
    //        /// Disposes the current <see cref="BrowsableObjectInfoLoader{TPath, TItems, TSubItems, TFactory}"/> and optionally disposes the related <see cref="Path"/>.
    //        /// </summary>
    //        /// <exception cref="InvalidOperationException">This <see cref="BrowsableObjectInfoLoader{TPath, TItems, TSubItems, TFactory}"/> is busy and does not support cancellation.</exception>
    //        protected virtual void DisposeOverride(bool disposing) => backgroundWorker.Dispose();

    //        ~BrowsableObjectInfoLoader()

    //        {

    //            DisposeOverride(false);

    //        }

    //    }

    public abstract class BrowsableObjectInfoLoader<TPath, TItems, TSubItems, TFactory> : IBrowsableObjectInfoLoader<TPath, TItems> where TPath : BrowsableObjectInfo where TItems : BrowsableObjectInfo where TSubItems : BrowsableObjectInfo where TFactory : BrowsableObjectInfoFactory

    {

        private readonly BackgroundWorker backgroundWorker = new BackgroundWorker();
        // #pragma warning disable CS0649 // Set up using reflection
        private readonly IFileSystemObjectComparer<IFileSystemObject> _fileSystemObjectComparer;
        private readonly IEnumerable<string> _filter;

        public IFileSystemObjectComparer<IFileSystemObject> FileSystemObjectComparer { get => _fileSystemObjectComparer; set => this.SetBackgroundWorkerProperty(nameof(FileSystemObjectComparer), nameof(_fileSystemObjectComparer), value, typeof(BrowsableObjectInfoLoader<TPath, TItems, TSubItems, TFactory>), true); }

        //public void changePath(IBrowsableObjectInfo newValue)

        //{



        //}

        //protected virtual BrowsableObjectInfo PathOverride { get => _path; set => _path = value; }

        public IEnumerable<string> Filter { get => _filter; set => this.SetBackgroundWorkerProperty(nameof(Filter), nameof(_filter), value, typeof(BrowsableObjectInfoLoader<TPath, TItems, TSubItems, TFactory>), true); }

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
        /// <remarks>When you create a <see cref="Disposed"/> delegate, you identify the method that handles the event. To associate the event with your event handler, add an instance of the delegate to the event. The event handler is called whenever the event occurs, unless you remove the delegate. For more information about event handler delegates, see <a href="https://docs.microsoft.com/en-us/dotnet/standard/events/index?view=netframework-4.8">Handling and Raising Events</a>.</remarks>
        public event EventHandler Disposed;

        public bool IsDisposing { get; internal set; }

        public bool IsDisposed { get; internal set; }

        /// <summary>
        /// This method does anything because it is designed for deep cloning from constructor and not from an <see cref="object.MemberwiseClone"/> operation, and is here for overriding only. If you perform a deep cloning operation using an <see cref="object.MemberwiseClone"/> operation in <see cref="DeepCloneOverride()"/>, you'll have to override this method if your class has to reinitialize members.
        /// </summary>
        /// <param name="browsableObjectInfoLoader">The cloned <see cref="BrowsableObjectInfoLoader{TPath, TItems, TSubItems, TFactory}"/>.</param>
        protected virtual void OnDeepClone(BrowsableObjectInfoLoader<TPath, TItems, TSubItems, TFactory> browsableObjectInfoLoader) { }

        /// <summary>
        /// When overridden in a derived class, gets a deep clone of this <see cref="BrowsableObjectInfoLoader{TPath, TItems, TSubItems, TFactory}"/>. The <see cref="OnDeepClone(BrowsableObjectInfoLoader{TPath, TItems, TFactory})"/> method already has an implementation for deep cloning from constructor and not from an <see cref="object.MemberwiseClone"/> operation. If you perform a deep cloning operation using an <see cref="object.MemberwiseClone"/> operation in <see cref="DeepCloneOverride()"/>, you'll have to override this method if your class has to reinitialize members.
        /// </summary>
        protected abstract BrowsableObjectInfoLoader<TPath, TItems, TSubItems, TFactory> DeepCloneOverride();

        public object DeepClone()

        {

            ((IDisposable)this).ThrowIfDisposingOrDisposed();

            BrowsableObjectInfoLoader<TPath, TItems, TSubItems, TFactory> browsableObjectInfoLoader = DeepCloneOverride();

            OnDeepClone(browsableObjectInfoLoader);

            return browsableObjectInfoLoader;

        }

        public virtual bool NeedsObjectsOrValuesReconstruction => FileSystemObjectComparer.NeedsObjectsOrValuesReconstruction;

        public virtual bool CheckFilter(string path)

        {

            if (Filter is null) return true;

            foreach (string filter in Filter)

                if (!IO.Path.MatchToFilter(path, filter)) return false;

            return true;

        }

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

        /// <summary>
        /// When overridden in a derived class, provides a handler for the <see cref="DoWork"/> event.
        /// </summary>
        /// <param name="e">Event args for the current event</param>
        protected abstract void OnDoWork(DoWorkEventArgs e);

        // /// <summary>
        // /// Initializes a new instance of the <see cref="BrowsableObjectInfoItemsLoader"/> class with an <see cref="IBrowsableObjectInfo"/>.
        // /// </summary>
        // /// <param name="path">The path from which load items.</param>
        // public BrowsableObjectInfoItemsLoader(IBrowsableObjectInfo path) { path.ItemsLoader = this; }

        /// <summary>
        /// Loads the items of the <see cref="IBrowsableObjectInfoLoader.Path"/> object.
        /// </summary>
        public virtual void LoadItems() => OnDoWork(new DoWorkEventArgs(null));

        /// <summary>
        /// Loads the items of the <see cref="IBrowsableObjectInfoLoader.Path"/> object asynchronously.
        /// </summary>
        public virtual void LoadItemsAsync() => backgroundWorker.RunWorkerAsync();

        /// <summary>
        /// Cancels the working asynchronously.
        /// </summary>
        public void CancelAsync(object stateInfo) => backgroundWorker.CancelAsync(stateInfo);

        public void CancelAsync() => CancelAsync(null);

        /// <summary>
        /// Cancels the working.
        /// </summary>
        public void Cancel(object stateInfo) => backgroundWorker.Cancel(stateInfo);

        public void Cancel() => Cancel(null);

        /// <summary>
        /// Suspends the current thread.
        /// </summary>
        public void Suspend() => backgroundWorker.Suspend();

        /// <summary>
        /// Resumes the current thread.
        /// </summary>
        public void Resume() => backgroundWorker.Resume();

        //IBrowsableObjectInfo Path => PathOverride;

        ///// <summary>
        ///// This property is only itented for use in this class and in classes that derive directly from this class and sould be sealed in derived classes and not used directly. You can use the <see cref="Path"/> property instead.
        ///// </summary>
        ///// <seealso cref="Path"/>
        //protected abstract IBrowsableObjectInfo PathOverride { get; set; }

        /// <summary>
        /// Disposes the current <see cref="BrowsableObjectInfoLoader{TPath, TItems, TSubItems, TFactory}"/>.
        /// </summary>
        /// <exception cref="InvalidOperationException">This <see cref="BrowsableObjectInfoLoader{TPath, TItems, TSubItems, TFactory}"/> is busy and does not support cancellation.</exception>
        public void Dispose()

        {

            IsDisposing = true;

            Dispose(true);

            GC.SuppressFinalize(this);

            IsDisposed = true;

            IsDisposing = false;

        }

        ///// <summary>
        ///// Disposes the current <see cref="BrowsableObjectInfoLoader{TPath, TItems, TSubItems, TFactory}"/> and optionally disposes the related <see cref="Path"/>.
        ///// </summary>
        ///// <exception cref="InvalidOperationException">This <see cref="BrowsableObjectInfoLoader{TPath, TItems, TSubItems, TFactory}"/> is busy and does not support cancellation.</exception>
        //protected sealed override void DisposeOverride(bool disposing) => Dispose(disposing, false);

        /// <summary>
        /// Disposes the current <see cref="BrowsableObjectInfoLoader{TPath, TItems, TSubItems, TFactory}"/> and optionally disposes the related <see cref="Path"/>.
        /// </summary>
        /// <exception cref="InvalidOperationException">This <see cref="BrowsableObjectInfoLoader{TPath, TItems, TSubItems, TFactory}"/> is busy and does not support cancellation.</exception>
        protected virtual void Dispose(bool disposing)

        {

            // base.DisposeOverride(disposing);

            backgroundWorker.Dispose();

            if (disposing)

                Path = null;

        }

        //protected abstract void OnRunWorkerCompleted(RunWorkerCompletedEventArgs e);

        // protected abstract void OnAddingPath(IBrowsableObjectInfo path);

        //protected abstract void OnProgressChanged(ProgressChangedEventArgs e);

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        ~BrowsableObjectInfoLoader()
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        {

            Dispose(false);

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BrowsableObjectInfoLoader{TPath, TItems, TSubItems, TFactory}"/> class.
        /// </summary>
        /// <param name="workerReportsProgress">Whether the thread can notify of the progress.</param>
        /// <param name="workerSupportsCancellation">Whether the thread supports the cancellation.</param>
        protected BrowsableObjectInfoLoader( BrowsableObjectTreeNode< TPath, TItems, TFactory > path, bool workerReportsProgress, bool workerSupportsCancellation) : this(path, new FileSystemObjectComparer<IFileSystemObject>(), workerReportsProgress, workerSupportsCancellation) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="BrowsableObjectInfoLoader{TPath, TItems, TSubItems, TFactory}"/> class using a custom comparer.
        /// </summary>
        /// <param name="workerReportsProgress">Whether the thread can notify of the progress.</param>
        /// <param name="workerSupportsCancellation">Whether the thread supports the cancellation.</param>
        /// <param name="fileSystemObjectComparer">The comparer used to sort the loaded items.</param>
        protected BrowsableObjectInfoLoader( BrowsableObjectTreeNode< TPath, TItems, TFactory > path, IFileSystemObjectComparer<IFileSystemObject> fileSystemObjectComparer, bool workerReportsProgress, bool workerSupportsCancellation)

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

            // todo: internal set because this is an initialization

            Path = path;

        }

        // protected IPathModifier<TPath, TItems> PathModifier { get; private set; }

        private BrowsableObjectTreeNode<TPath, TItems, TFactory> _path;

        ///// <summary>
        ///// This property is here only for interoperability with the non-generic base class <see cref="BrowsableObjectInfoLoader{TPath, TItems, TSubItems, TFactory}"/> and is only intented for use in the <see cref="BrowsableObjectInfoLoader{TPath}"/> generic class and should not be used directly.
        ///// </summary>
        ///// <seealso cref="BrowsableObjectInfoLoader.Path"/>
        ///// <seealso cref="BrowsableObjectInfoLoader{TPath}.Path"/>
        //protected sealed override IBrowsableObjectInfo PathOverride { get => Path; set => Path = (TPath) value; } 

        IBrowsableObjectTreeNode<TPath, TItems> IBrowsableObjectInfoLoader<TPath, TItems>.Path => Path;

        /// <summary>
        /// Gets the path from which to load the items.
        /// </summary>
        public BrowsableObjectTreeNode< TPath , TItems, TFactory > Path
        {
            get => _path; set

            {

                if (IsBusy)

                    throw new InvalidOperationException("The items loader is busy.");

                if (!(value?.ItemsLoader is null))

                    throw new InvalidOperationException("The given path has already been added to an items loader.");

                OnPathChanging(value);

                if (_path is object)

                    _path.ItemsLoader = null;

                _path = value;

                if (value is object)

                    value.ItemsLoader = this;

                OnPathChanged(value);

            }
        }

        //protected void Reset()

        //{

        //    if (object.ReferenceEquals(_path, null)) return;

        //    if (!_path.IsBrowsable)

        //        throw new InvalidOperationException(string.Format(Generic.NotBrowsableObject, _path.FileType.ToString(), _path.ToString()));

        //    else if (IsBusy)

        //        Cancel();

        //    Path. ItemCollection.Clear();

        //}

        protected virtual void OnRunWorkerCompleted(RunWorkerCompletedEventArgs e) => Path.AreItemsLoaded = true;

        protected virtual void OnProgressChanged(ProgressChangedEventArgs e)
        {

            Debug.WriteLine("e.UserState is BrowsableObjectTreeNode<TItems, TSubItems, TFactory>: " + (e.UserState is BrowsableObjectTreeNode<TItems, TSubItems, TFactory>).ToString());

            Debug.WriteLine("e.UserState.GetType(): " + e.UserState.GetType().ToString());

            try
            {

                if (e.UserState is ITreeNode item)

                    Path.Insert( Path.Count, (ReadOnlyTreeNode<TItems>)item);

                Debug.WriteLine("azerty: " + Path.Count) ; 

            }
            catch (Exception ex) { Debug.WriteLine("azerty: " + ex.Message); }

        }

        /// <summary>
        /// Provides ability for classes that derive from this one to do operations when the path is changing.
        /// </summary>
        /// <param name="path">The new path to set the <see cref="Path"/> property with.</param>
        protected virtual void OnPathChanging( BrowsableObjectTreeNode< TPath, TItems, TFactory > path) { }

        protected virtual void OnPathChanged( BrowsableObjectTreeNode< TPath, TItems, TFactory > path) { }

    }

}
