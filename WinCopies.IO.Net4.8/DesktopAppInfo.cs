//using Microsoft.WindowsAPICodePack.Shell;
//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using static WinCopies.IO.Registry;

//namespace WinCopies.IO
//{
//    //public abstract class AppInfo

//    //{

//    //    /// <summary>
//    //    /// When overridden in a derived class, gets the display name of this <see cref="AppInfo"/>.
//    //    /// </summary>
//    //    public abstract string DisplayName { get; }

//    //    /// <summary>
//    //    /// When overridden in a derived class, opens a given <see cref="ShellObject"/> using the application represented by this <see cref="AppInfo"/>.
//    //    /// </summary>
//    //    /// <param name="shellObject">The <see cref="ShellObject"/> to open.</param>
//    //    public abstract void Open(ShellObject shellObject);

//    //    /// <summary>
//    //    /// When overridden in a derived class, opens a given <see cref="StorageFile"/> using the application represented by this <see cref="AppInfo"/>.
//    //    /// </summary>
//    //    /// <param name="storageFile">The <see cref="StorageFile"/> to open.</param>
//    //    public abstract void Open(StorageFile storageFile);

//    //    /// <summary>
//    //    /// When overridden in a derived class, opens a given file using the application represented by this <see cref="AppInfo"/>.
//    //    /// </summary>
//    //    /// <param name="fileName">The path to the file to open.</param>
//    //    public abstract void Open(string fileName);

//    //}

//    //public class DesktopAppInfo // : AppInfo
//    //{

//    //    private ShellObject _shellObject;

//    //    /// <summary>
//    //    /// Gets the display name of this <see cref="DesktopAppInfo"/>.
//    //    /// </summary>
//    //    public string DisplayName => (_shellObject ?? (_shellObject = ShellObject.FromParsingName(Path))).Properties.System.FileDescription.Value;

//    //    private string _path;

//    //    /// <summary>
//    //    /// Gets the path of this <see cref="DesktopAppInfo"/>.
//    //    /// </summary>
//    //    public string Path => _path ?? (_path = GetOpenWithSoftwarePathFromCommand(Command));

//    //    /// <summary>
//    //    /// Gets the full command (software path and command line args) of this <see cref="DesktopAppInfo"/>.
//    //    /// </summary>
//    //    public string Command { get; } = null;

//    //    /// <summary>
//    //    /// Gets the Windows Registry file type of this <see cref="DesktopAppInfo"/>.
//    //    /// </summary>
//    //    public string FileType { get; } = null;

//    //    /// <summary>
//    //    /// Initializes a new instance of the <see cref="DesktopAppInfo"/> class for a given Windows Registry file type.
//    //    /// </summary>
//    //    /// <param name="fileType">The Windows Registry file type of the new instance of the <see cref="DesktopAppInfo"/>.</param>
//    //    public DesktopAppInfo(string fileType)

//    //    {

//    //        FileType = fileType;

//    //        Command = GetCommandFromFileType("open", FileType);

//    //    }

//    //    public void Open(ShellObject shellObject) => Open(shellObject.ParsingName, "open");

//    //    public bool Open(ShellObject shellObject, string commandName) => Open(shellObject.ParsingName, commandName);

//    //    public void Open(string fileName) => Open(fileName, "open");

//    //    public bool Open(string fileName, string commandName)

//    //    {

//    //        _ = Process.Start(GetOpenWithSoftwareProcessStartInfoFromCommand(GetCommandFromFileType(commandName, FileType), fileName));

//    //        return true;

//    //    }

//    //}

//    //public class WindowsStoreAppInfo : AppInfo

//    //{

//    //    /// <summary>
//    //    /// Gets the display name of this <see cref="WindowsStoreAppInfo"/>.
//    //    /// </summary>
//    //    public override string DisplayName => AppInfo.DisplayInfo.DisplayName;

//    //    /// <summary>
//    //    /// Gets the <see cref="Windows.ApplicationModel.AppInfo"/> of this <see cref="WindowsStoreAppInfo"/>.
//    //    /// </summary>
//    //    public Windows.ApplicationModel.AppInfo AppInfo { get; } = null;

//    //    /// <summary>
//    //    /// Initializes a new instance of the <see cref="WindowsStoreAppInfo"/> class for a given <see cref="Windows.ApplicationModel.AppInfo"/>.
//    //    /// </summary>
//    //    /// <param name="appInfo">The <see cref="Windows.ApplicationModel.AppInfo"/> of the instance of the <see cref="WindowsStoreAppInfo"/>.</param>
//    //    public WindowsStoreAppInfo(Windows.ApplicationModel.AppInfo appInfo) => AppInfo = appInfo;

//    //    public override void Open(ShellObject shellObject) => Open(shellObject.ParsingName);

//    //    public override void Open(StorageFile storageFile)

//    //    {

//    //        WindowsStoreAppLauncherInterop windowsStoreAppLauncherInterop = new WindowsStoreAppLauncherInterop(storageFile);

//    //        windowsStoreAppLauncherInterop.Loaded += (object sender, EventArgs e) => Open(windowsStoreAppLauncherInterop, false);

//    //        if (windowsStoreAppLauncherInterop.IsLoaded) Open(windowsStoreAppLauncherInterop, false);

//    //    }

//    //    public override void Open(string fileName)

//    //    {

//    //        WindowsStoreAppLauncherInterop winRTAppLauncherInterop = new WindowsStoreAppLauncherInterop(fileName);

//    //        winRTAppLauncherInterop.Loaded += (object sender, EventArgs e) => Open(winRTAppLauncherInterop, false);

//    //        if (winRTAppLauncherInterop.IsLoaded) Open(winRTAppLauncherInterop, false);

//    //    }

//    //    public void Open(WindowsStoreAppLauncherInterop winRTAppLauncherInterop, bool displayApplicationPicker) => winRTAppLauncherInterop.OpenFileUsingCustomApp(AppInfo, displayApplicationPicker);

//    //}
//}
