//using Microsoft.Win32;
//using System;
//using System.Collections.Generic;
//using System.Linq; 
//using WinCopies.Util;
//using Windows.Foundation;
//using Windows.Storage;
//using Windows.System;

//using static WinCopies.IO.Registry;
//using static WinCopies.Util.Generic;

//namespace WinCopies.IO
//{
//    public class WindowsStoreAppLauncherInterop
//    {

//        //private static Dictionary<string, WinRTAppLauncherInterop> _winRTAppLauncherInterop = new Dictionary<string, WinRTAppLauncherInterop>();

//        //public static ReadOnlyDictionary<string, WinRTAppLauncherInterop> WinRTAppLauncherInteropObjects { get; } = new ReadOnlyDictionary<string, WinRTAppLauncherInterop>(_winRTAppLauncherInterop);

//        public bool IsLoaded { get; private set; } = false;

//        public string FileName { get; } = null;

//        public StorageFile File { get; private set; } = null;

//        public event EventHandler Loaded;

//        public event SucceededEventHandler FileOpened;

//        public WindowsStoreAppLauncherInterop(string fileName)
//        {

//            FileName = fileName;

//            string extension = System.IO.Path.GetExtension(fileName);

//            // _winRTAppLauncherInterop.Add(extension, this);

//            Launcher.FindFileHandlersAsync(extension).Completed = (IAsyncOperation<IReadOnlyList<Windows.ApplicationModel.AppInfo>> asyncInfo, AsyncStatus asyncStatus) =>

//            {

//                StorageFile.GetFileFromPathAsync(fileName).Completed = (IAsyncOperation<StorageFile> _asyncInfo, AsyncStatus _asyncStatus) => File = _asyncInfo.GetResults();

//                IsLoaded = true;

//                Loaded?.Invoke(this, new EventArgs());

//            };

//        }

//        public WindowsStoreAppLauncherInterop(StorageFile file)
//        {

//            File = file;

//            FileName = file.Path;

//            string extension = System.IO.Path.GetExtension(file.Path);

//            // _winRTAppLauncherInterop.Add(extension, this);

//            Launcher.FindFileHandlersAsync(extension).Completed = (IAsyncOperation<IReadOnlyList<Windows.ApplicationModel.AppInfo>> asyncInfo, AsyncStatus asyncStatus) =>

//            {

//                IsLoaded = true;

//                Loaded?.Invoke(this, new EventArgs());

//            };

//        }

//        public void OpenFileUsingDefaultApp(bool displayApplicationPicker) => Launcher.LaunchFileAsync(File, new LauncherOptions() { DisplayApplicationPicker = displayApplicationPicker }).Completed = OnOpening;

//        public void OpenFileUsingCustomApp(Windows.ApplicationModel.AppInfo appInfo, bool displayApplicationPicker)

//        {

//            LauncherOptions options = new LauncherOptions()
//            {

//                PreferredApplicationDisplayName = appInfo.DisplayInfo.DisplayName,

//                PreferredApplicationPackageFamilyName = appInfo.PackageFamilyName,

//                TargetApplicationPackageFamilyName = appInfo.PackageFamilyName,

//                DisplayApplicationPicker = displayApplicationPicker

//            };

//            try
//            {

//                Launcher.LaunchFileAsync(File, options).Completed = OnOpening;

//            }
//            catch { }

//        }

//        private void OnOpening(IAsyncOperation<bool> asyncInfo, AsyncStatus asyncStatus) => FileOpened?.Invoke(this, new SucceededEventArgs(asyncInfo.GetResults()));

//    }

//    public class WindowsStoreAppHandlersInterop

//    {

//        public Windows.ApplicationModel.AppInfo OpenWithAppInfo { get; private set; } = null;

//        public bool IsOpenWithAppInfoLoaded { get; private set; } = false;

//        public IReadOnlyList<Windows.ApplicationModel.AppInfo> OpenWithAppInfos { get; private set; } = null;

//        public bool AreOpenWithAppInfosLoaded { get; private set; } = false;

//        public string Extension { get; } = null;

//        public event EventHandler AppInfoLoaded;

//        public event EventHandler OpenWithAppInfoLoaded;

//        public WindowsStoreAppHandlersInterop(string extension) => Extension = extension;

//        public void GetFileHandlers() => Launcher.FindFileHandlersAsync(Extension).Completed = (IAsyncOperation<IReadOnlyList<Windows.ApplicationModel.AppInfo>> asyncInfo, AsyncStatus asyncStatus) =>

//                                                         {

//                                                             OpenWithAppInfos = asyncInfo.GetResults();

//                                                             AreOpenWithAppInfosLoaded = true;

//                                                             AppInfoLoaded?.Invoke(this, new EventArgs());

//                                                         };

//        public void GetOpenWithAppInfo()

//        {

//            void onAppInfoLoaded(object sender, EventArgs e)

//            {

//                AppInfoLoaded -= onAppInfoLoaded;

//                RegistryKey registryKey = GetFileTypeRegistryKey(GetFileTypeFromExtension(Extension));

//                if (registryKey?.OpenSubKey("shell\\open\\command") != null && registryKey.OpenSubKey("Application")?.GetValue("AppUserModelId") is string valueAsString)

//                    foreach (Windows.ApplicationModel.AppInfo appInfo in OpenWithAppInfos)

//                        if (appInfo.AppUserModelId == valueAsString)

//                        {

//                            IsOpenWithAppInfoLoaded = true;

//                            OpenWithAppInfo = appInfo;

//                            OpenWithAppInfoLoaded?.Invoke(this, new EventArgs());

//                            return;

//                        }

//            }

//            if (!AreOpenWithAppInfosLoaded)

//            {

//                AppInfoLoaded += onAppInfoLoaded;

//                GetFileHandlers();

//            }

//            else onAppInfoLoaded(null, null);

//        }

//    }

//    public class WinShellAppInfoInterop

//    {

//        public AppInfo OpenWithAppInfo { get; private set; } = null;

//        public bool IsOpenWithAppInfoLoaded { get; private set; } = false;

//        public AppInfo[] OpenWithAppInfos { get; private set; } = null;

//        public bool AreOpenWithAppInfosLoaded { get; private set; } = false;

//        public string Extension { get; } = null;

//        public event EventHandler OpenWithAppInfoLoaded;

//        public event EventHandler OpenWithAppInfosLoaded;

//        public WinShellAppInfoInterop(string extension) => Extension = extension;

//        public void LoadAppInfo()

//        {

//            string openWithSoftwareCommand = GetCommandFromExtension("open", Extension);

//            if (openWithSoftwareCommand == null)

//            {

//                WindowsStoreAppHandlersInterop winRTAppHandlersInterop = new WindowsStoreAppHandlersInterop(Extension);

//                winRTAppHandlersInterop.OpenWithAppInfoLoaded += (object sender, EventArgs e) =>

//                {

//                    OpenWithAppInfo = new WindowsStoreAppInfo(winRTAppHandlersInterop.OpenWithAppInfo);

//                    IsOpenWithAppInfoLoaded = true;

//                    OpenWithAppInfoLoaded?.Invoke(this, new EventArgs());

//                };

//                winRTAppHandlersInterop.GetOpenWithAppInfo();

//            }

//            else

//            {

//                OpenWithAppInfo = new DesktopAppInfo(GetFileTypeFromExtension(Extension));

//                IsOpenWithAppInfoLoaded = true;

//                OpenWithAppInfoLoaded?.Invoke(this, new EventArgs());

//            }

//        }

//        public void LoadAppInfos()

//        {

//            if (Extension == null)

//                throw new ArgumentNullException(nameof(Extension));

//            if (string.IsNullOrEmpty(Extension) || string.IsNullOrWhiteSpace(Extension))

//                throw new ArgumentException(string.Format((string)StringParameterEmptyOrWhiteSpaces, nameof(Extension)));

//            // RegistryKey[] subKeys = { Microsoft.Win32.Registry.CurrentUser.OpenSubKey("Software\\Classes"), Microsoft.Win32.Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\FileExts"), Microsoft.Win32.Registry.ClassesRoot };

//            // string fileType = null;

//            ArrayAndListBuilder<AppInfo> appInfos = new ArrayAndListBuilder<AppInfo>();

//            // foreach (RegistryKey value in subKeys)

//            // {

//            appInfos.AddRangeLast(GetAppInfosByExtension(Extension/*, value*/));

//            // }

//            WindowsStoreAppHandlersInterop windowsStoreAppHandlersInterop = new WindowsStoreAppHandlersInterop(Extension);

//            windowsStoreAppHandlersInterop.AppInfoLoaded += (object sender, EventArgs e) =>

//            {

//                foreach (Windows.ApplicationModel.AppInfo value in windowsStoreAppHandlersInterop.OpenWithAppInfos)

//                    appInfos.AddLast(new WindowsStoreAppInfo(value));

//                OpenWithAppInfos = appInfos.ToArray();

//                AreOpenWithAppInfosLoaded = true;

//                OpenWithAppInfosLoaded?.Invoke(this, new EventArgs());

//            };

//            windowsStoreAppHandlersInterop.GetFileHandlers();

//        }

//    }
//}
