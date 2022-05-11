using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

using WinCopies.Data.SQL;
using WinCopies.Linq;

namespace WinCopies.Reflection.DotNetDocBuilder
{
    public partial class Writer
    {
        public void UpdateTypeDoc()
        {
            WriteLine("Updating XML doc for types.", true);

            int length = TYPE_PREFIX.Length;
            string
#if CS8
                ?
#endif
                _name;
            int index;
            (string @namespace, string name) fullName;
            XElement
#if CS8
                ?
#endif
                node;
            XElement
#if CS8
                ?
#endif
                tmpNode;
            int genericTypeCount;
            string nodeText;
            int i;

            string getTypeName(in Type type)
            {
                string typeName = type.Name;

                if ((genericTypeCount = type.GenericTypeCount) > 0)

                    typeName += $"`{genericTypeCount}";

                return typeName;
            }

            string parentTypesPath;

            void setParentTypesPath(in Type type)
            {
                Collections.Generic.EnumerableHelper<string>.IEnumerableStack path = Collections.Generic.EnumerableHelper<string>.GetEnumerableStack();

                ActionIn<Type> action = (in Type _type) => action = (in Type __type) => path.Push(getTypeName(__type));

                foreach (Type _type in Collections.Enumerable.GetNullCheckWhileEnumerable(type, _type => _type.ParentType?.Type))

                    action(_type);

                var stringBuilder = new StringBuilder();

                while (path.TryPop(out string typeName))
                {
                    stringBuilder.Append('.');

                    stringBuilder.Append(typeName);
                }

                parentTypesPath = stringBuilder.ToString();
            }

            using
#if !CS8
                (
#endif
                var collection = new DBEntityCollection<Type>(GetConnection())
#if CS8
                ;
#else
                )
#endif

            foreach (Type type in collection)
            {
                WriteLine($"Searching XML doc for type {type}.", true, ConsoleColor.DarkYellow);

                setParentTypesPath(type);

                if ((node = Packages.Select(package => File.Exists(package.Path) ? XElement.Load(Path.ChangeExtension(package.Path, "xml")).Descendants("member") : null).Join(false).SelectWhere(doc => (_name = doc?.Attribute("name")?.Value)?.StartsWith(TYPE_PREFIX) == true && (index = (_name = _name.Substring(length)).LastIndexOf('.')) > -1 && (fullName = _name.Split(index)).@namespace == GetWholeNamespace(type.Namespace.Id) + parentTypesPath && fullName.name == getTypeName(type) ? doc /*.Elements().FirstOrDefault()*/ : null, doc => doc != null).FirstOrDefault()) == null)
                {
                    WriteLine($"No XML doc found for type {type}.", false);

                    continue;
                }

                void writeLine(in string suffix, in string
#if CS8
                ?
#endif
                otherMsg, in bool? increment, in ConsoleColor? color) => WriteLine($"Updat{suffix} XML doc for type {type}.{otherMsg}", increment, color);

                writeLine("ing", null, null, ConsoleColor.DarkGreen);

                void setValue(string nodeName, in ActionIn<string> action)
                {
                    if ((tmpNode = node.Descendants(nodeName).FirstOrDefault()) != null)
                    {
                        i = 0;

                        void parse()
                        {
                            foreach (char c in (nodeText = (nodeText = tmpNode.ToString().Substring(nodeName.Length + 2)).Remove(nodeText.LastIndexOf(nodeName) - 2)))
                            {
                                switch (c)
                                {
                                    case ' ':
                                    case '\t':
                                    case '\n':
                                    case '\r':

                                        i++;

                                        continue;
                                }

                                return;
                            }
                        }

                        parse();

                        action(i < nodeText.Length ? nodeText.Substring(i) : nodeText);
                    }
                }

                setValue("summary", (in string comment) => type.Comment = comment);
                setValue("remarks", (in string remarks) => type.CommentRemarks = remarks);

                writeLine("ed", $" Updated {type.Update(out uint tables)} rows in {tables} {nameof(tables)}.", false, null);
            }

            WriteLine("Updated XML doc for types.", false);
        }

        public void UpdateDoc()
        {
            WriteLine("Updating XML doc.", true);

            UpdateTypeDoc();

            WriteLine("Updated XML doc.", false);
        }

        public void UpdateTypes()
        {
            UpdateEnums();
            UpdateInterfaces();
            UpdateStructs();
            UpdateClasses();
        }

        public void UpdateAll()
        {
            UpdateNamespaces();
            UpdateTypes();
            UpdateDoc();
        }
    }
}
