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

using Microsoft.Win32;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.AccessControl;
using WinCopies.Util;
using static WinCopies.Util.Util;

namespace WinCopies.IO
{
    // todo: check if registry keys are the authorized ones.

    public static class Registry
    {

        //private const string DefaultIcon = "DefaultIcon";
        //private const string OpenCommandPath = "shell\\open\\command";
        //private const string DefaultIconDllPath = "%SystemRoot%\\System32\\SHELL32.dll";
        //private const string PerceivedType = "PerceivedType";
        //private const string SystemFileAssociations = "SystemFileAssociations";
        //private const string SoftwareClasses = "Software\\Classes";
        //private const string shellBackslash = "shell\\";
        //private const string UserChoice = "UserChoice";
        //private const string BackslashCommand = "\\command";
        //private const string ProgId = "ProgId";
        //private const string OpenWithProgids = "OpenWithProgids";
        //private const string OpenWithList = "OpenWithList";
        //private const string MRUList = "MRUList";
        //private const string SoftwareClassesApplications = "Software\\Classes\\Applications";
        //private const string ApplicationsBackslash = "Applications\\";
        //private const string SoftwareMicrosoftWindowsCurrentVersionExplorerFileExts = "Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\FileExts";

        ///// <summary>
        ///// Gets the Windows registry open-with file type for a given extension.
        ///// </summary>
        ///// <param name="extension">The file extension from which look for the associated file type.</param>
        ///// <returns>If found, the Windows registry open-with file type associated to the given extension, otherwise null.</returns>
        //public static string GetFileTypeFromExtension(string extension)

        //{

        //    if (extension == null)

        //        throw new ArgumentNullException(nameof(extension));

        //    if (string.IsNullOrEmpty(extension) || string.IsNullOrWhiteSpace(extension))

        //        throw new ArgumentException(string.Format(StringParameterEmptyOrWhiteSpaces, nameof(extension)));

        //    RegistryKey value = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(SoftwareClasses + IO.Path.PathSeparator + extension)
        //        ?? Microsoft.Win32.Registry.CurrentUser.OpenSubKey(SoftwareMicrosoftWindowsCurrentVersionExplorerFileExts + IO.Path.PathSeparator + extension);

        //    if (value == null)

        //    {

        //        value = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(extension)
        //        ?? Microsoft.Win32.Registry.LocalMachine.OpenSubKey(SoftwareClasses + IO.Path.PathSeparator + extension);

        //        return value?.GetValue("") as string;

        //    }

        //    else

        //    {

        //        return value.OpenSubKey(UserChoice).GetValue(ProgId) as string;

        //    }
        //}

        ///// <summary>
        ///// Gets the Windows registry command associated to a given extension.
        ///// </summary>
        ///// <param name="commandName">The command name.</param>
        ///// <param name="extension">The extension from which look for the associated command.</param>
        ///// <returns>The Windows registry command associated to the given extension.</returns>
        //public static string GetCommandFromExtension(string commandName, string extension)

        //{

        //    // todo: public method to avoid argument test for each call of the GetCommandFromExtension and GetFileTypeFromExtension methods.

        //    if (If(ComparisonType.Or, ComparisonMode.Logical, WinCopies.Util.Util.Comparison.Equal, out string key, /*EqualityComparer<object>.Default, GetCommonPredicate<object>(),*/ null, GetKeyValuePair(nameof(commandName), (object)commandName), GetKeyValuePair(nameof(extension), (object)extension)))

        //        throw new ArgumentNullException(key);

        //    if (If(ComparisonType.Or, ComparisonMode.Logical, WinCopies.Util.Util.Comparison.Equal, out key, true, GetKeyValuePair(nameof(commandName), IsNullEmptyOrWhiteSpace(commandName)), GetKeyValuePair(nameof(extension), IsNullEmptyOrWhiteSpace(extension))))

        //        throw new ArgumentException(string.Format(StringParameterEmptyOrWhiteSpaces, key));

        //    string fileType = GetFileTypeFromExtension(extension);

        //    return fileType == null
        //        ? (string)null
        //        : GetFileTypeRegistryKey(fileType).OpenSubKey(shellBackslash + commandName + BackslashCommand)?.GetValue("") is string valueAsString ? valueAsString : null;
        //}

        ///// <summary>
        ///// Gets the Windows registry command associated to a given open-with file type.
        ///// </summary>
        ///// <param name="commandName">The command name.</param>
        ///// <param name="fileType">The Windows registry open-with file type from which look for the associated command.</param>
        ///// <returns>The Windows registry command associated to the given extension.</returns>
        //public static string GetCommandFromFileType(string commandName, string fileType)

        //{

        //    if (If(ComparisonType.Or, ComparisonMode.Logical, WinCopies.Util.Util.Comparison.Equal, out string key, null, GetKeyValuePair(nameof(commandName), commandName), GetKeyValuePair(nameof(commandName), fileType)))

        //        throw new ArgumentNullException(key);

        //    if (If(ComparisonType.Or, ComparisonMode.Logical, WinCopies.Util.Util.Comparison.Equal, out key, true, GetKeyValuePair(nameof(commandName), IsNullEmptyOrWhiteSpace(commandName)), GetKeyValuePair(nameof(fileType), IsNullEmptyOrWhiteSpace(fileType))))

        //        throw new ArgumentException(string.Format(StringParameterEmptyOrWhiteSpaces, key));

        //    using (RegistryKey fileTypeRegistryKey = GetFileTypeRegistryKey(fileType)?.OpenSubKey(shellBackslash + commandName + BackslashCommand))

        //        return fileTypeRegistryKey?.GetValue("") as string;

        //}

        ///// <summary>
        ///// Gets the software path part from a Windows registry open-with command.
        ///// </summary>
        ///// <param name="command">The Windows registry open-with command to parse</param>
        ///// <returns>The software path part of the command.</returns>
        //public static string GetOpenWithSoftwarePathFromCommand(string command)

        //{

        //    if (command == null)

        //        throw new ArgumentNullException(nameof(command));

        //    if (IsNullEmptyOrWhiteSpace(command))

        //        throw new ArgumentException(string.Format(StringParameterEmptyOrWhiteSpaces, nameof(command)));

        //    return command.Count(c => c == '"') > 1 && command.StartsWith("\"") ? command.Substring(1, command.Substring(1).IndexOf('"')) : command.Contains(' ') ? command.Substring(0, command.IndexOf(' ')) : command;

        //}

        //public static string GetOpenWithSoftwareArgumentsFromCommand(string command)

        //{

        //    if (command == null)

        //        throw new ArgumentNullException(nameof(command));

        //    if (IsNullEmptyOrWhiteSpace(command))

        //        throw new ArgumentException(string.Format(StringParameterEmptyOrWhiteSpaces, nameof(command)));

        //    if (command.Count(c => c == '"') > 2 && command.StartsWith("\""))

        //    {

        //        int value = command.Substring(1).IndexOf('"') + 1;

        //        if (value < command.Length)

        //        {

        //            string _command = command.Substring(value);

        //            return _command.StartsWith(" ") ? _command.Length > 1 ? _command.Substring(1) : "" : _command;

        //        }

        //        else

        //            return "";

        //    }

        //    else if (command.Contains(' '))

        //    {

        //        int value = command.IndexOf(' ') + 1;

        //        return value < command.Length ? command.Substring(value) : "";

        //    }

        //    else

        //        return "";

        //}

        //public static ProcessStartInfo GetOpenWithSoftwareProcessStartInfoFromCommand(string command, string fileName)

        //{

        //    if (If(ComparisonType.Or, ComparisonMode.Logical, WinCopies.Util.Util.Comparison.Equal, out string key, null, GetKeyValuePair(nameof(command), command), GetKeyValuePair(nameof(fileName), fileName)))

        //        throw new ArgumentNullException(key);

        //    if (If(ComparisonType.Or, ComparisonMode.Logical, WinCopies.Util.Util.Comparison.Equal, out key, true, GetKeyValuePair(nameof(command), IsNullEmptyOrWhiteSpace(command)), GetKeyValuePair(nameof(fileName), IsNullEmptyOrWhiteSpace(fileName))))

        //        throw new ArgumentException(string.Format(StringParameterEmptyOrWhiteSpaces, key));

        //    string softwareFileName = command.Count(c => c == '"') > 2 && command.StartsWith("\"") ? command.Substring(1, command.Substring(1).IndexOf('"')) : command.Contains(' ') ? command.Substring(0, command.IndexOf(' ')) : command;

        //    string commandLineArguments = command;

        //    if (commandLineArguments.StartsWith("\""))

        //        commandLineArguments = commandLineArguments.Substring(1);

        //    if (commandLineArguments.Length > softwareFileName.Length)

        //    {

        //        if (commandLineArguments.Length > softwareFileName.Length)

        //        {

        //            commandLineArguments = commandLineArguments.Substring(softwareFileName.Length);

        //            char[] charsToRemove = { '\"', ' ' };

        //            foreach (char c in charsToRemove)

        //                if (commandLineArguments.StartsWith(c))

        //                    commandLineArguments = commandLineArguments.Substring(1);

        //            commandLineArguments = commandLineArguments.Replace("%1", fileName);

        //        }

        //        else

        //            commandLineArguments = "";

        //    }

        //    else

        //        commandLineArguments = "";

        //    return new ProcessStartInfo(softwareFileName, commandLineArguments);

        //}

        //// todo: add gesture for the 'HKEY_LOCAL_MACHINE\SOFTWARE\Clients' key (ex.: HKEY_LOCAL_MACHINE\SOFTWARE\Clients\Media\Windows Media Player\Capabilities\FileAssociations, HKEY_LOCAL_MACHINE\SOFTWARE\Clients\Media\Windows Media Player\Capabilities\MimeAssociations, HKEY_LOCAL_MACHINE\SOFTWARE\Clients\Media\Windows Media Player\Capabilities\URLAssociations, ...).

        //public static DesktopAppInfo[] GetDesktopAppInfosFromExtension(string extension)

        //{

        //    if (extension == null)

        //        throw new ArgumentNullException(nameof(extension));

        //    if (string.IsNullOrEmpty(extension) || string.IsNullOrWhiteSpace(extension))

        //        throw new ArgumentException(string.Format(StringParameterEmptyOrWhiteSpaces, nameof(extension)));

        //    RegistryKey[] subKeys = { Microsoft.Win32.Registry.CurrentUser.OpenSubKey(SoftwareClasses), Microsoft.Win32.Registry.CurrentUser.OpenSubKey(SoftwareMicrosoftWindowsCurrentVersionExplorerFileExts), Microsoft.Win32.Registry.ClassesRoot, Microsoft.Win32.Registry.LocalMachine.OpenSubKey(SoftwareClasses) };

        //    RegistryKey _registryKey = null;

        //    RegistryKey disposeAndGetNewRegistryKey(RegistryKey oldRegistryKey, RegistryKey newRegistryKey)

        //    {

        //        oldRegistryKey?.Dispose();

        //        return newRegistryKey;

        //    }

        //    var fileTypes = new ArrayAndListBuilder<DesktopAppInfo>();

        //    void checkAndAddFileTypeIfSucceeded(string fileType, RegistryKey registryKey)

        //    {

        //        if (GetFileTypeRegistryKey(fileType, registryKey)?.OpenSubKey(OpenCommandPath)?.GetValue("") is string)

        //        {

        //            var desktopAppInfo = new DesktopAppInfo(fileType);

        //            foreach (DesktopAppInfo _desktopAppInfo in fileTypes)

        //                if (GetOpenWithSoftwarePathFromCommand(_desktopAppInfo.Command) == GetOpenWithSoftwarePathFromCommand(desktopAppInfo.Command) && GetOpenWithSoftwareArgumentsFromCommand(_desktopAppInfo.Command) == GetOpenWithSoftwareArgumentsFromCommand(desktopAppInfo.Command)) return;

        //            _ = fileTypes.AddLast(desktopAppInfo);

        //        }

        //    }

        //    void onOpenWithList(RegistryKey mainRegistryKey, RegistryKey registryKey, string extensionOrPerceivedType)

        //    {

        //        if ((registryKey = disposeAndGetNewRegistryKey(registryKey, registryKey?.OpenSubKey(OpenWithList))) == null) return;

        //        // object obj = registryKey.GetValueKind("MRUList");

        //        object valueAsObject = registryKey.GetValue(MRUList);

        //        string[] keyValueNames = registryKey.GetValueNames();

        //        if (!(valueAsObject == null || registryKey.GetValueKind(MRUList) != RegistryValueKind.String || IsNullEmptyOrWhiteSpace((string)valueAsObject)))

        //        {

        //            string _value = null;

        //            while (((string)valueAsObject).Length > 0)

        //            {

        //                _value = ((string)valueAsObject).Substring(0, 1);

        //                valueAsObject = ((string)valueAsObject).Substring(1);

        //                if (keyValueNames.Contains(_value) && registryKey.GetValue(_value) is string valueAsString)

        //                    checkAndAddFileTypeIfSucceeded(valueAsString, mainRegistryKey);

        //            }

        //        }

        //        registryKey = disposeAndGetNewRegistryKey(registryKey, mainRegistryKey.OpenSubKey(extensionOrPerceivedType + IO.Path.PathSeparator + OpenWithList));

        //        keyValueNames = registryKey?.GetSubKeyNames();

        //        foreach (string ___value in keyValueNames)

        //            checkAndAddFileTypeIfSucceeded(___value, mainRegistryKey.Name.EndsWith(IO.Path.PathSeparator + SystemFileAssociations) ? OpenRegistryKey(mainRegistryKey.Name.Substring(0, mainRegistryKey.Name.LastIndexOf(IO.Path.PathSeparator + SystemFileAssociations))) : mainRegistryKey);

        //    }

        //    foreach (RegistryKey registryKey in subKeys)

        //    {

        //        _registryKey = disposeAndGetNewRegistryKey(_registryKey, registryKey.OpenSubKey(extension));

        //        if (_registryKey == null) continue;

        //        #region Progids

        //        _registryKey = disposeAndGetNewRegistryKey(_registryKey, _registryKey.OpenSubKey(OpenWithProgids));

        //        if (_registryKey != null)

        //        {

        //            foreach (string value in _registryKey.GetValueNames())

        //            {

        //                if (value == "") continue;

        //                checkAndAddFileTypeIfSucceeded(value, registryKey);

        //            }

        //        }

        //        #endregion

        //        #region OpenWith

        //        _registryKey = disposeAndGetNewRegistryKey(_registryKey, registryKey.OpenSubKey(extension));

        //        onOpenWithList(registryKey, _registryKey, extension);

        //        #endregion

        //        #region PerceivedType

        //        _registryKey = registryKey.OpenSubKey(extension);

        //        object valueAsObject = _registryKey.GetValue(PerceivedType);

        //        if (valueAsObject == null || _registryKey.GetValueKind(PerceivedType) != RegistryValueKind.String || (string)valueAsObject == "") continue;

        //        // checkAndAddFileTypeIfSucceeded((string)valueAsObject, registryKey, true);

        //        onOpenWithList(_registryKey.OpenSubKey(SystemFileAssociations) ?? Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(SystemFileAssociations), _registryKey.OpenSubKey(SystemFileAssociations + IO.Path.PathSeparator + (string)valueAsObject) ?? Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(SystemFileAssociations + IO.Path.PathSeparator + (string)valueAsObject), (string)valueAsObject);

        //        #endregion

        //        registryKey.Dispose();

        //    }

        //    return fileTypes.ToArray();

        //}

        //public static RegistryKey GetFileTypeRegistryKey(string fileType)

        //{

        //    if (fileType == null)

        //        throw new ArgumentNullException(nameof(fileType));

        //    if (IsNullEmptyOrWhiteSpace(fileType))

        //        throw new ArgumentException(string.Format(StringParameterEmptyOrWhiteSpaces, nameof(fileType)));

        //    return Microsoft.Win32.Registry.CurrentUser.OpenSubKey(SoftwareClasses + IO.Path.PathSeparator + fileType) ?? Microsoft.Win32.Registry.CurrentUser.OpenSubKey(SoftwareClassesApplications + IO.Path.PathSeparator + fileType) ?? Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(fileType) ?? Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(ApplicationsBackslash + fileType) ?? Microsoft.Win32.Registry.LocalMachine.OpenSubKey(SoftwareClasses + IO.Path.PathSeparator + fileType) ?? Microsoft.Win32.Registry.LocalMachine.OpenSubKey(SoftwareClassesApplications + IO.Path.PathSeparator + fileType) ?? null;

        //}

        //public static RegistryKey GetFileTypeRegistryKey(string fileType, RegistryKey registryKey)

        //{

        //    if (If(ComparisonType.Or, ComparisonMode.Logical, Util.Util.Comparison.Equal, out string key, null, GetKeyValuePair(nameof(fileType), (object)fileType), GetKeyValuePair(nameof(registryKey), (object)registryKey)))

        //        throw new ArgumentNullException(key);

        //    if (IsNullEmptyOrWhiteSpace(fileType))

        //        throw new ArgumentException(string.Format(StringParameterEmptyOrWhiteSpaces, nameof(fileType)));

        //    string fileExtsPath = Microsoft.Win32.Registry.CurrentUser.Name + IO.Path.PathSeparator + SoftwareMicrosoftWindowsCurrentVersionExplorerFileExts;

        //    string classesRootName = nameof(Microsoft.Win32.Registry.ClassesRoot.Name);

        //    if (If(ComparisonType.Or, ComparisonMode.Logical, Util.Util.Comparison.Equal, registryKey.Name, Microsoft.Win32.Registry.CurrentUser.Name + SoftwareClasses, fileExtsPath, classesRootName, "HKEY_LOCAL_MACHINE\\SOFTWARE\\Classes"))

        //    {

        //        if (registryKey.Name == fileExtsPath)

        //            registryKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(SoftwareClasses);

        //        RegistryKey _registryKey = registryKey.Name.StartsWith(classesRootName)
        //            ? registryKey.OpenSubKey(fileType) ?? registryKey.OpenSubKey(ApplicationsBackslash + fileType)
        //            : registryKey.OpenSubKey(fileType) ?? registryKey.OpenSubKey(ApplicationsBackslash + fileType) ?? Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(fileType) ?? Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(ApplicationsBackslash + fileType);

        //        registryKey.Dispose();

        //        return _registryKey;
        //    }

        //    else throw new ArgumentException(string.Format(Generic.NotValidRegistryKey, registryKey));



        //}

        public static RegistryKey OpenRegistryKey(string name) => OpenRegistryKey(name, RegistryKeyPermissionCheck.Default, RegistryRights.ReadKey);

        public static RegistryKey OpenRegistryKey(string name, bool writable) => writable ?    OpenRegistryKey(name, RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.WriteKey) : OpenRegistryKey(name, RegistryKeyPermissionCheck.ReadSubTree, RegistryRights.ReadKey);

        public static RegistryKey OpenRegistryKey(string name, RegistryKeyPermissionCheck registryKeyPermissionCheck, RegistryRights registryRights)

        {

            ThrowIfNullEmptyOrWhiteSpace(name, nameof(name));

            string originalName = name;

            string registryKeyName;

            if (name.Contains(IO.Path.PathSeparator, out int result))

            {

                registryKeyName = name.Substring(0, result);

                name = name.Length == 1 ? "" : name.Substring(result + 1);

            }

            else

            {

                registryKeyName = name;

                name = "";

            }

            FieldInfo[] fields = typeof(Microsoft.Win32.Registry).GetFields();

            var registryKeys = new KeyValuePair<RegistryKey, string>[fields.Length];

            RegistryKey item;

            for (int i = 0; i < fields.Length; i++)

            {

                item = (RegistryKey)fields[i].GetValue(null);

                registryKeys[i] = new KeyValuePair<RegistryKey, string>(item, item.Name);

            }

            if (If(ComparisonType.Or, ComparisonMode.Logical, WinCopies.Util.Util.Comparison.Equal, out RegistryKey registryKey, registryKeyName, registryKeys))

                return name.Length > 0 ? registryKey.OpenSubKey(name, registryKeyPermissionCheck, registryRights) : registryKey;

            else throw new RegistryException(string.Format(Generic.RegistryKeyNotExists, originalName), originalName);

        }

        //public static Icon[] GetIconVariationsFromFileType(string fileType) => GetIconVariationsFromFileTypeRegistryKey(GetFileTypeRegistryKey(fileType));

        //public static Icon[] GetIconVariationsFromFileType(string fileType, RegistryKey registryKey) => GetIconVariationsFromFileTypeRegistryKey(GetFileTypeRegistryKey(fileType, registryKey));

        //public static Icon[] GetIconVariationsFromFileTypeRegistryKey(RegistryKey registryKey)

        //{
        //    RegistryKey _registryKey;

        //    string iconName;

        //    int? iconIndex;

        //    if ((_registryKey = registryKey.OpenSubKey(DefaultIcon)) == null || !(_registryKey.GetValue("") is string defaultIconPath))

        //    {

        //        defaultIconPath = (_registryKey = registryKey.OpenSubKey(OpenCommandPath)) == null || (defaultIconPath = _registryKey.GetValue("") as string) == null
        //            ? DefaultIconDllPath
        //            : GetOpenWithSoftwarePathFromCommand(defaultIconPath);

        //        iconName = null;

        //        iconIndex = null;

        //    }

        //    else

        //    {

        //        string[] subPaths;

        //        if (defaultIconPath.Contains(','))

        //        {

        //            subPaths = defaultIconPath.Split(',');

        //            string iconId = subPaths[subPaths.Length - 1];

        //            if (iconId.StartsWith("-"))

        //            {

        //                iconName = iconId.Substring(1);

        //                iconIndex = null;

        //            }

        //            else

        //            {

        //                if (int.TryParse(iconId, out int _iconIndex))

        //                {

        //                    iconIndex = _iconIndex;

        //                    iconName = null;

        //                }

        //                else

        //                {

        //                    iconName = iconId;

        //                    iconIndex = null;

        //                }

        //            }

        //            var stringBuilder = new StringBuilder();

        //            for (int i = 0; i < subPaths.Length - 1; i++)

        //                _ = stringBuilder.Append(subPaths[i]);

        //            defaultIconPath = stringBuilder.ToString();

        //            if (defaultIconPath == "")

        //                throw new RegistryException("Invalid DefaultIcon registry value.", null, registryKey.Name);

        //        }

        //        else

        //        {

        //            iconName = null;

        //            iconIndex = null;

        //        }

        //    }

        //    try

        //    {

        //        defaultIconPath = Path.GetRealPathFromEnvironmentVariables(defaultIconPath);

        //    }

        //    catch (Exception)

        //    {

        //        return null;

        //    }

        //    //try
        //    //{

        //    //    var obj = WinCopies.Win32Interop.Icon.ExtractIconFromExe(defaultIconPath, iconIndex.Value, true)?.Split();

        //    //    Debug.WriteLine("obj.Length: " + obj.Length);

        //    //}
        //    //catch (Exception) { }

        //    var m = new System.Drawing.IconLib.MultiIcon
        //    {
        //        SelectedIndex = -1
        //    };

        //    m.Load(defaultIconPath);

        //    System.Drawing.IconLib.SingleIcon obj = null;

        //    if (!iconIndex.HasValue && iconName == null)

        //        iconIndex = 0;

        //    if (iconIndex.HasValue)

        //        obj = m.ToList()[iconIndex.Value];

        //    else /*if (iconName != null)*/

        //    {

        //        foreach (System.Drawing.IconLib.SingleIcon _obj in m)

        //            if (_obj.Name == iconName)

        //            {

        //                obj = _obj;

        //                break;

        //            }

        //        if (obj == null)

        //            obj = m.ToList()[0];

        //    }

        //    return obj.Icon.Split(); //return defaultIconPath.EndsWith(".ico")
        //    //    ? new Icon(defaultIconPath).Split() : !iconIndex.HasValue ? Icon.ExtractAssociatedIcon(defaultIconPath).Split()
        //    //    : WinCopies.Win32Interop.Icon.ExtractIconFromExe(defaultIconPath, iconIndex.Value, true)?.Split();
        //}

    }

}
