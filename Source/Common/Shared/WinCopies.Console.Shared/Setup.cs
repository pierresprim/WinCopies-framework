/*using Microsoft.WindowsAPICodePack.Shell;
using SevenZip;

using System;
using System.Diagnostics;
using System.IO;
#if !CS8
using System.Linq;
#endif
using System.Runtime.InteropServices;
using WinCopies.Collections.Generic;

using static System.Console;

using static WinCopies.Console.Console;

namespace WinCopies.Console
{
    [Flags]
    public enum CreateShortcutsTo
    {
        None = 0,

        Desktop = 1,

        StartMenu = 2,

        Both = Desktop | StartMenu
    }

    public abstract class Setup
    {
        private Screen _screen;

        public IScreen Screen => _screen;

        public abstract System.Collections.Generic.IEnumerable<string> Welcome { get; }

        public abstract string DefaultPath { get; }

        public string Path { get; private set; }

        public abstract string SoftwareFileName { get; }

        public abstract InArchiveFormat InArchiveFormat { get; }

        public Setup() => Path = DefaultPath;

        public abstract System.IO.Stream GetStream();

        public void Main()
        {
            Label addLabel(in string text) => _screen.AddLabel(text);

            WinCopies.Console.Console.Instance.Initialize(new CursorPosition(120, 30));

            _screen = Instance.GetScreen();

            Instance.Screen = _screen;

            foreach (string welcome in Welcome)

                _ = addLabel(welcome);

            _ = addLabel("Use:");

            _ = addLabel("\tTab to browse controls;");

            _ = addLabel("\tUp and Down to browse options (highlighted controls);");

            _ = addLabel("\tLeft and Right to browse sub-controls (for controls on the current line);");

            _ = addLabel("\tand Space to click on the selected buttons.");

            _ = addLabel("Current path:");

            Label path = addLabel(Path);

            _ = _screen.Select(_screen.AddSelectable(new ActionButton(() =>
             {
                 Clear();

                 Path = ReadLine();

                 path.Text = Path;

                 Instance.Render();

                 return true;
             })
            { Text = "Change path" }));

            _ = addLabel("Create shortcuts to:");

            var enumerator = new EnumLoopEnumerator<CreateShortcutsTo>();

            ScrollableSelect _enumerator = _screen.AddSelectable(new ScrollableSelect());

            _enumerator.Items = enumerator;

            bool install = false;

            var buttons = new ControlElement[] { new ActionButton() { Text = "Install", Action = () => { install = true; return false; } }, new Label() { Text = " " }, new ActionButton() { Text = "Cancel", Action = Bool.False } };

            var _buttons = new ReadOnlyArray<ControlElement>(buttons);

            _ = _screen.Add(new Control(_buttons));

            _ = _screen.AddSelectable((ActionButton)buttons[0]);

            _ = _screen.AddSelectable((ActionButton)buttons[2]);

            Instance.Loop(_enumerator);

            if (install)
            {
                System.IO.Stream stream = GetStream() ?? throw new InvalidOperationException("Could not retrieve stream.");

                //string _path = $"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\\Temp\\{SoftwareName}{IO.Path.InArchiveFormats[InArchiveFormat][0]}";

                void onError(in int hresult, in string errorMessage)
                {
                    _ = addLabel("An error occurred. Press a key to exit.");

                    _ = addLabel("Error message: " + errorMessage);

                    _ = ReadKey();

                    Environment.Exit(hresult);
                }

                SevenZipExtractor extractor = null;

                try
                {
                    //const int bufferLength = 4096;
                    //byte[] buffer;
                    //int bytesRead;

                    //var fileStream = new FileStream(_path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read, bufferLength, FileOptions.DeleteOnClose);

                    //_ = addLabel("Extracting archive...");

                    ProgressBar totalProgress = null;

                    void initProgressBar(ref ProgressBar progressBar, in uint maximum)
                    {
                        progressBar = new ProgressBar
                        {
                            Size = 100u,

                            Minimum = 0u,

                            Maximum = maximum
                        };

                        _ = _screen.Add(progressBar);
                    }

                    //initProgressBar(ref totalProgress, (uint)stream.Length);

                    //void read() => buffer = stream.Read(bufferLength, out bytesRead);

                    //read();

                    //while (bytesRead != 0)
                    //{
                    //    fileStream.Write(buffer, 0, bytesRead);

                    //    totalProgress.Value += (uint)bytesRead;

                    //    read();
                    //}

                    //_ = addLabel("Archive successfully extracted.");

                    var fileOperation = new FileOperation();

                    if (IO.Path.Exists(Path))
                    {
                        _ = addLabel("Removing old files...");

                        //initProgressBar(ref totalProgress, 0u);

                        //var progress = new Microsoft.WindowsAPICodePack.Shell.Temp.FileOperationProgressSink();

                        //void updateProgress(in uint ___progress) => totalProgress.Value = ___progress;

                        //progress.UpdateProgress = (_totalProgress, _progress) =>
                        //{
                        //    progress.UpdateProgress = (__totalProgress, __progress) => updateProgress(__progress);

                        //    totalProgress.Maximum = _totalProgress;

                        //    updateProgress(_progress);
                        //};

                        fileOperation.DeleteItem(ShellObjectFactory.Create(Path), pfopsItem: null /* progress *//*);

                        // fileOperation.SetOperationFlags(ShowElevationPrompt |  NoConfirmation | NoErrorUI );

                        fileOperation.PerformOperations();

                        if (fileOperation.GetAnyOperationsAborted())

                            onError(Marshal.GetLastWin32Error(), null);

                        _ = addLabel("Old files successfully removed.");
                    }

                    _ = addLabel("Creating directory...");

                    _ = Directory.CreateDirectory(Path);

                    _ = addLabel("Directory successfully created.");

                    _ = addLabel("Extracting files...");

                    extractor = new SevenZipExtractor(stream, InArchiveFormat);

                    initProgressBar(ref totalProgress, extractor.FilesCount);

                    const string totalProgressText = "Total progress:";

                    int count = 0;

                    Label totalProgressLabel = addLabel($"{totalProgressText} 0/{extractor.FilesCount}");

                    ProgressBar currentFileProgress = null;

                    initProgressBar(ref currentFileProgress, 0u);

                    const string currentFileText = "Current file:";

                    Label currentFileLabel = addLabel(currentFileText);

                    extractor.FileExtractionStarted += (object sender, FileInfoEventArgs e) =>
                    {
                        currentFileLabel.Text = $"{currentFileText} {e.FileInfo.FileName}";

                        currentFileProgress.Value = 0;

                        currentFileProgress.Maximum = (uint)e.FileInfo.Size;
                    };

                    extractor.Extracting += (object sender, ProgressEventArgs e) => currentFileProgress.Value = e.PercentDone;

                    extractor.FileExtractionFinished += (object sender, FileInfoEventArgs e) =>
                    {
                        currentFileProgress.Value = (uint)e.FileInfo.Size;

                        totalProgressLabel.Text = $"{totalProgressText} {++count}/{extractor.FilesCount}";

                        totalProgress.Value++;
                    };

                    extractor.ExtractArchive(Path);

                    currentFileLabel.Text = currentFileText;

                    _ = addLabel("Extraction successfully completed.");

                    string sourcePath = $"{Path}\\{SoftwareFileName}";

                    string destPath = $"\\{(SoftwareFileName.Contains('.') ? SoftwareFileName.Remove(SoftwareFileName.LastIndexOf('.')) : SoftwareFileName)}.lnk";

                    void createShortcut(in string _destPath) => ShellLink.Create(sourcePath, _destPath + destPath, false);

                    if (enumerator.Current.HasFlag(CreateShortcutsTo.StartMenu))

                        createShortcut(KnownFolders.CommonPrograms.Path);

                    if (enumerator.Current.HasFlag(CreateShortcutsTo.Desktop))

                        createShortcut(KnownFolders.PublicDesktop.Path);

                    var startProgram = new CheckBoxButton();

                    _ = _screen.Add(new CheckBox(startProgram));

                    startProgram.Text = "Start program";

                    _ = _screen.AddSelectable(startProgram);

                    _screen.AddSelectable(new ActionButton(() =>
                    {
                        if (startProgram.IsChecked)

                            _ = Process.Start($"{Path}\\{SoftwareFileName}");

                        return false;
                    })).Text = "Exit setup";

                    Instance.Loop(startProgram);
                }

                catch (Exception ex)
                {
                    onError(ex.HResult, ex.Message);
                }

                finally
                {
                    extractor?.Dispose();

                    stream.Dispose();
                }
            }
        }
    }
}
*/
