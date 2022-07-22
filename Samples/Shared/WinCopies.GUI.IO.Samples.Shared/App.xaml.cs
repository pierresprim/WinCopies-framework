using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Windows;

using WinCopies.Collections.Generic;
using WinCopies.Extensions;
using WinCopies.IO;
using WinCopies.Linq;
using WinCopies.Util;

using Directory = System.IO.Directory;

namespace WinCopies.GUI.IO.Samples
{
    public partial class App : Application
    {
        private class AssemblyLoadContext : WinCopies.AssemblyLoadContext
        {
            public AssemblyLoadContext(string path) : base(path, false) { }

            //protected override Assembly Load(AssemblyName assemblyName) => Default.Assemblies.FirstOrDefault(assembly => assemblyName.Name == "WinCopies.IO" && assembly.FullName == assemblyName.FullName) ?? base.Load(assemblyName);
        }

        internal System.Collections.Generic.IEnumerable<IBrowsableObjectInfoPlugin> PluginParameters { get; private set; }

        public readonly WinCopies.Extensions.Logger Logger = WinCopies.Extensions.FileLogger.GetLogger();
        public static new App Current => (App)Application.Current;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            void showMessage(in string message, in string caption) => _ = MessageBox.Show(message, caption, MessageBoxButton.OK, MessageBoxImage.Error);
            void shutdown() => Shutdown((int)Microsoft.WindowsAPICodePack.Win32Native.ErrorCode.BadEnvironment);

            void _shutdown(in Action action)
            {
                action();
                shutdown();
            }

            void __shutdown(string message, string caption) => _shutdown(() => showMessage(message, caption));

            string directoryName;
            string assemblyPath;
            Version version;
            AssemblyLoadContext assemblyLoadContext;
            string directory = System.IO.Path.Combine(Environment.CurrentDirectory, "plugins");

            if (Directory.Exists(directory))

                try
                {
                    Assembly? assembly;
                    EnumerableHelper<IBrowsableObjectInfoPlugin>.IEnumerableQueue pluginParameters = EnumerableHelper<IBrowsableObjectInfoPlugin>.GetEnumerableQueue();
                    bool error = false;
                    ulong found = 0ul;
                    ulong loaded = 0ul;
                    ulong plugins = 0ul;

                    foreach (IBrowsableObjectInfoPlugin? _pluginParameters in Directory.EnumerateDirectories(directory).Select(directory =>
                    {
                        directoryName = System.IO.Path.GetFileName(directory);
                        assemblyPath = System.IO.Path.Combine(directory, $"{directoryName}.dll");

                        bool tryLoadAssembly()
                        {
                            assembly = null;

                            bool onError(in string msg)
                            {
                                Logger(msg, null, LoggingLevel.Error);

                                return !(error = true);
                            }

                            if (File.Exists(assemblyPath))

                                try
                                {
                                    found++;

                                    //assembly = Assembly.LoadFrom(assemblyPath);

                                    assembly = (assemblyLoadContext = new AssemblyLoadContext(assemblyPath)).LoadFromAssemblyPath(assemblyPath);

                                    var paths = Directory.EnumerateFiles(Environment.CurrentDirectory).Select(path => System.IO.Path.GetFileName(path));

                                    foreach (string assemblyName in Directory.EnumerateFiles(directory))

                                        if (paths.Contains(System.IO.Path.GetFileName(assemblyName)))

                                            System.IO.File.Delete(assemblyName);

                                    if (Directory.Exists(assemblyPath = System.IO.Path.Combine(directory, @"runtimes\win\lib")) && (Directory.Exists(directory = System.IO.Path.Combine(assemblyPath, $"net{(version = Environment.Version).Major}.0")) || Directory.Exists(directory = System.IO.Path.Combine(assemblyPath, $"net{version.Major}.{version.Minor}"))))

                                        foreach (string assemblyName in Directory.EnumerateFiles(directory))

                                            //_ = Assembly.LoadFrom(assemblyName);

                                            _ = assemblyLoadContext.LoadFromAssemblyPath(assemblyName);

                                    loaded++;

                                    return true;
                                }

                                catch (Exception ex) when (ex.Is(false, typeof(System.IO.IOException), typeof(BadImageFormatException), typeof(SecurityException)))
                                {
                                    return onError(ex.Message);
                                }

                            else

                                return onError($"Cannot find {assemblyPath}.");
                        }

                        IBrowsableObjectInfoPlugin? tryGetPlugin()
                        {
                            try
                            {
                                return assembly.DefinedTypes.WhereSelect(type => type.Assembly == assembly && type.Name == nameof(WinCopies.IO.ObjectModel.BrowsableObjectInfo), type => type.GetMethods().FirstOrDefault(method => method.IsStatic && method.Name == "GetPluginParameters" && method.ReturnType == typeof(IBrowsableObjectInfoPlugin))?.Invoke(null, null)).FirstOrDefault() as IBrowsableObjectInfoPlugin;
                            }

                            catch (ReflectionTypeLoadException ex)
                            {
                                Logger(ex.Message, null, LoggingLevel.Error);

                                error = true;

                                return null;
                            }
                        }

                        return tryLoadAssembly() ? tryGetPlugin() : null;
                    }))
                    {
                        if (_pluginParameters == null)
                        {
                            error = true;

                            continue;
                        }

                        _pluginParameters.RegisterBrowsabilityPaths();
                        _pluginParameters.RegisterItemSelectors();
                        _pluginParameters.RegisterProcessSelectors();

                        pluginParameters.Enqueue(_pluginParameters);

                        plugins++;
                    }

                    Logger($"{found} plugins found; {loaded} loaded; {plugins} parsed successfully.", null, LoggingLevel.Information);

                    if (pluginParameters.HasItems)
                    {
                        PluginParameters = pluginParameters.AsReadOnlyEnumerable();

                        void _showMessage(in string msg) => showMessage($"{msg} be loaded. See the log file for more information.", "Plugin load error");

                        if (error)

                            if (pluginParameters.HasItems)

                                _showMessage("One or more plugins could not");

                            else

                                _shutdown(() => _showMessage("No plugin could"));
                    }

                    else

                        __shutdown("No plugin successfully parsed.", "Plugin parse error.");

                    return;
                }
                catch (Exception ex) when (ex.Is(false, typeof(System.IO.IOException), typeof(SecurityException), typeof(UnauthorizedAccessException))) { /* Left empty. */ }

            __shutdown("No plugin found. You have to install at least one plugin for the program to work.", "No plugin found");
        }
    }
}
