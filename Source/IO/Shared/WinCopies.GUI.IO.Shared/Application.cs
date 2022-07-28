//* Copyright © Pierre Sprimont, 2022
//*
//* This file is part of the WinCopies Framework.
//*
//* The WinCopies Framework is free software: you can redistribute it and/or modify
//* it under the terms of the GNU General Public License as published by
//* the Free Software Foundation, either version 3 of the License, or
//* (at your option) any later version.
//*
//* The WinCopies Framework is distributed in the hope that it will be useful,
//* but WITHOUT ANY WARRANTY; without even the implied warranty of
//* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//* GNU General Public License for more details.
//*
//* You should have received a copy of the GNU General Public License
//* along with the WinCopies Framework.  If not, see <https://www.gnu.org/licenses/>. */

#if CS8 && !NETSTANDARD
using System;
using System.Collections.Generic;
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

namespace WinCopies.GUI.IO
{
    public interface IApplication
    {
        WinCopies.Extensions.Logger Logger { get; }

        void Shutdown(int errorCode);
    }

    public partial class Application : System.Windows.Application, IApplication
    {
        protected IEnumerable<IBrowsableObjectInfoPlugin> PluginParameters { get; private set; }

        public WinCopies.Extensions.Logger Logger { get; } = FileLogger.GetLogger();

        public static new Application Current => (Application)System.Windows.Application.Current;

        public static void Initialize(IApplication application, in Action<IBrowsableObjectInfoPlugin> action, Action<int> shutdownAction, out IEnumerable<IBrowsableObjectInfoPlugin> __pluginParameters)
        {
            void showMessage(in string message, in string caption) => _ = MessageBox.Show(message, caption, MessageBoxButton.OK, MessageBoxImage.Error);
            void shutdown() => shutdownAction(Environment.ExitCode = (int)Microsoft.WindowsAPICodePack.Win32Native.ErrorCode.BadEnvironment);

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
                                application.Logger(msg, null, LoggingLevel.Error);

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

                                            File.Delete(assemblyName);

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
                                application.Logger(ex.Message, null, LoggingLevel.Error);

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

                        action(_pluginParameters);

                        pluginParameters.Enqueue(_pluginParameters);

                        plugins++;
                    }

                    application.Logger($"{found} plugins found; {loaded} loaded; {plugins} parsed successfully.", null, LoggingLevel.Information);

                    if (pluginParameters.HasItems)
                    {
                        __pluginParameters = pluginParameters.AsReadOnlyEnumerable();

                        void _showMessage(in string msg) => showMessage($"{msg} be loaded. See the log file for more information.", "Plugin load error");

                        if (error)

                            if (pluginParameters.HasItems)

                                _showMessage("One or more plugins could not");

                            else

                                _shutdown(() => _showMessage("No plugin could"));
                    }

                    else
                    {
                        __shutdown("No plugin successfully parsed.", "Plugin parse error.");

                        __pluginParameters = null;
                    }

                    return;
                }

                catch (Exception ex) when (ex.Is(false, typeof(System.IO.IOException), typeof(SecurityException), typeof(UnauthorizedAccessException))) { /* Left empty. */ }

            __shutdown("No plugin found. You have to install at least one plugin for the program to work.", "No plugin found");

            __pluginParameters = null;
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            Initialize(this, pluginParameters =>
            {
                pluginParameters.RegisterBrowsabilityPaths();
                pluginParameters.RegisterBrowsableObjectInfoSelectors();
                pluginParameters.RegisterItemSelectors();
                pluginParameters.RegisterProcessSelectors();
            }, Shutdown, out IEnumerable<IBrowsableObjectInfoPlugin> pluginParameters);

            PluginParameters = pluginParameters;
        }
    }
}
#endif
