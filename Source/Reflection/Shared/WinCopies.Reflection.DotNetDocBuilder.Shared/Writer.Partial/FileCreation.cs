using System;
using System.IO;
using System.Linq;

using WinCopies.Collections;
using WinCopies.Collections.DotNetFix.Generic;
using WinCopies.Reflection.DotNetParser;
using WinCopies.Util;

namespace WinCopies.Reflection.DotNetDocBuilder
{
    public struct GetPathDelegates<T>
    {
        public Func<string> GetNamespace { get; }

        public Converter<T, string> GetName { get; }

        public Converter<T, T
#if CS9
            ?
#endif
            > GetParentType
        { get; }

        public Converter<T, byte> GetGenericTypeParameterLength { get; }

        public GetPathDelegates(in Func<string> getNamespace, in Converter<T, string> getName, in Converter<T, T
#if CS9
            ?
#endif
            > getParentType, in Converter<T, byte> getGenericTypeParameterLength)
        {
            GetNamespace = getNamespace;
            GetName = getName;
            GetParentType = getParentType;
            GetGenericTypeParameterLength = getGenericTypeParameterLength;
        }
    }

    public partial class Writer
    {
        public string GetPath<T>(T type, GetPathDelegates<T> delegates, out string namespacePath, out string[] paths)
        {
            IEnumerableStack<T> types = new EnumerableStack<T>();

            ActionIn<T> action = (in T _type) => action = (in T __type) => types.Push(__type);

            foreach (T _type in Collections.Enumerable.GetNullCheckWhileEnumerableC(type, delegates.GetParentType))

                action(_type);

            paths = new string[types.AsFromType<IUIntCountable>().Count + 2u];

            string getName() => delegates.GetName(type);

            paths[0] = GetWholePath(namespacePath = delegates.GetNamespace());
            paths.SetLast(getName());

            byte getGenericTypeParameterLength() => delegates.GetGenericTypeParameterLength(type);

            byte genericTypeParameterLength = getGenericTypeParameterLength();

            for (int i = 1; types.TryPop(out type); i++)
            {
                paths[i] = delegates.GetName(type);

                genericTypeParameterLength += getGenericTypeParameterLength();
            }

            string path = Path.Combine(paths);

            _ = Directory.CreateDirectory(path);

            return path = Path.Combine(path, GetFileName(genericTypeParameterLength > 0 ? (ushort
#if !CS9
            ?
#endif
            )genericTypeParameterLength : null));
        }

        protected void CreateTypeFile(System.Type type, in ulong id, in Func<byte[]> typeDataFunc)
        {
            string path = GetPath(type, new GetPathDelegates<System.Type>(() => type.Namespace, _type => _type.GetRealName(), _type => _type.DeclaringType, _type => _type.GetRealGenericTypeParameterLength()), out string namespacePath, out string[] paths);

            FileStream
#if CS8
                ?
#endif
                writer = File.Create(path);

            namespacePath = namespacePath.Replace('\\', '.');

            byte[] data = typeDataFunc();

            writer.Write(data, 0, data.Length);

            writer.Dispose();

            string getDocHeader() => $"{PackageInfo.Header} Doc";

            File.WriteAllText(path, File.ReadAllText(path).Replace("{PackageId}", PackageInfo.FrameworkId.ToString())
                .Replace("{ItemId}", id.ToString())
                .Replace("{ItemName}", type.GetRealName())
                .Replace("{DocURL}", $"2 => '<a href=\"/doc/{PackageInfo.URL}/{namespacePath.TryTruncateOrOriginal('.')}/index.php\">{getDocHeader()}</a>', {GetNamespaceURLArray(namespacePath, paths, out _)}")
                .Replace("{PackageUrl}", PackageInfo.URL));
        }

        protected void CreateTypeFile(DotNetType type, in ulong id, in Func<byte[]> typeDataFunc) => CreateTypeFile(type.Type, id, typeDataFunc);

        public void CreateEnumFile(DotNetEnum @enum, ulong id) => CreateTypeFile(@enum, id, GetEnumData);
        public void CreateInterfaceFile(DotNetInterface @interface, ulong id) => CreateTypeFile(@interface, id, GetInterfaceData);
        public void CreateClassFile(DotNetClass @class, ulong id) => CreateTypeFile(@class, id, GetClassData);
        public void CreateStructFile(DotNetStruct @struct, ulong id) => CreateTypeFile(@struct, id, GetStructData);

        public void CreateNamespaceFile(DotNetNamespace @namespace, ulong id)
        {
            string path;
            string namespacePath;

            _ = Directory.CreateDirectory(path = GetWholePath(namespacePath = @namespace.Path));

            FileStream
#if CS8
                ?
#endif
                writer = File.Create(path = Path.Combine(path, GetFileName(null)));

            byte[] data = GetNamespaceData();

            writer.Write(data, 0, data.Length);

            writer.Dispose();

            string getDocHeader() => $"{PackageInfo.Header} Doc";

            int dotIndex = namespacePath.IndexOf('.');

            File.WriteAllText(path, File.ReadAllText(path).Replace("{PackageId}", PackageInfo.FrameworkId.ToString())
                .Replace("{ItemId}", id.ToString())
                .Replace("{ItemName}", @namespace.Name)
                .Replace("{DocURL}", dotIndex > -1 ? $"2 => '<a href=\"/doc/{PackageInfo.URL}/{namespacePath.Truncate(dotIndex)}/index.php\">{getDocHeader()}</a>', {GetNamespaceURLArray(namespacePath, null, out _)}" : $"2 => '{getDocHeader()}'")
                .Replace("{PackageUrl}", PackageInfo.URL));
        }
    }
}
