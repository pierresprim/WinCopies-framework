using System;
using System.IO;
using System.Linq;

using WinCopies.Data.SQL;
using WinCopies.EntityFramework;
using WinCopies.Reflection.DotNetParser;

namespace WinCopies.Reflection.DotNetDocBuilder
{
    public partial class Writer
    {
        public void UpdateNamespaces()
        {
            WriteLine("Updating namespaces", true);

            using
#if !CS8
                (
#endif
                ISQLConnection connection = GetConnection()
#if CS8
                ;
#else
                )
#endif

            using
#if !CS8
                (
#endif
                var namespaces = new DBEntityCollection<Namespace>(connection)
#if CS8
                ;
#else
                )
            {
#endif
            string wholeNamespace;

            WriteLine("Parsing DB.", true);

            uint i = 0;

            ulong rows = 0;

            foreach (Namespace
#if CS8
                ?
#endif
                @namespace in namespaces.GetItems(connection.GetOrderByColumns(OrderBy.Desc, nameof(Namespace.Id))))
            {
                wholeNamespace = GetWholeNamespace(@namespace.Id);

                WriteLine($"{++i}: Searching {wholeNamespace} in all packages.", true, ConsoleColor.DarkYellow);

                if (GetAllNamespacesInPackages().Any(_namespace => _namespace.Path == wholeNamespace))

                    WriteLine($"{wholeNamespace} found. Not removed from DB.", null, ConsoleColor.DarkGreen);

                else
                {
                    WriteLine($"{wholeNamespace} not found in any package. Deleting.", true, ConsoleColor.DarkRed);

                    DeleteTypes(wholeNamespace);

                    rows = @namespace.Remove(out uint tables);

                    if (rows > 0)
                    {
                        WriteLine($"Removed {rows} {nameof(rows)} in {tables} {nameof(tables)}.", null);

                        Directory.Delete(GetWholePath(wholeNamespace), true);

                        WriteLine($"{wholeNamespace} successfully deleted.", false);
                    }

                    else

                        throw new InvalidOperationException($"Could not remove {wholeNamespace}.");
                }

                WriteLine($"Processing {wholeNamespace} completed.", false);
            }

            wholeNamespace = null;

            WriteLine("Parsing DB completed.", false);

            long? id;

            i = 0;

            foreach (DotNetNamespace
#if CS8
                ?
#endif
                @namespace in GetAllNamespacesInPackages())
            {
                WriteLine($"{++i}: Processing {@namespace.Path}.", true, ConsoleColor.DarkYellow);

                bool process()
                {
                    foreach (Namespace _namespace in namespaces)

                        if (_namespace.Name == @namespace.Name && _namespace.ParentId == (@namespace.Parent == null ? null : GetNamespaceId(@namespace.Parent.Path)))
                        {
                            WriteLine("Exists. Updating.", true, ConsoleColor.DarkGreen);

                            _namespace.FrameworkId = PackageInfo.FrameworkId;

                            WriteLine($"Updated {_namespace.Update(out uint tables)} rows in {tables} {nameof(tables)}. Id: {_namespace.Id}.", false);

                            return false;
                        }

                    return true;
                }

                if (process())
                {
                    WriteLine("Does not exist. Adding.", true, ConsoleColor.DarkRed);

                    var _namespace = new Namespace(namespaces)
                    {
                        Name = @namespace.Name,

                        ParentId = @namespace.Parent == null ? null : GetNamespaceId(@namespace.Parent.Path),

                        FrameworkId = PackageInfo.FrameworkId
                    };

                    id = namespaces.Add(_namespace, out uint tables, out rows);

                    if (id.HasValue)
                    {
                        WriteLine($"Added {rows} {nameof(rows)} in {tables} {nameof(tables)}. Id: {id.Value}", null);

                        WriteLine($"Creating file for {@namespace.Path}.", null);

                        CreateNamespaceFile(@namespace, (ulong)id.Value);

                        WriteLine($"File created for {@namespace.Path}.", false);
                    }

                    else

                        throw new InvalidOperationException($"Failed to add {@namespace.Path}.");
                }

                WriteLine($"Processed {@namespace.Path}.", false);
            }
#if !CS8
            }
#endif

            WriteLine("Updating namespaces completed.", false);
        }
    }
}
