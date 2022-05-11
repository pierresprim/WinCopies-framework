using System;
using System.Diagnostics;
using System.Linq;

using WinCopies.Data.SQL;
using WinCopies.Extensions;
using WinCopies.Reflection.DotNetParser;
using WinCopies.Util;

namespace WinCopies.Reflection.DotNetDocBuilder
{
    public partial class Writer
    {
        public void UpdateClasses()
        {
            Type _type;
            System.Type tmpType;

            using
#if !CS8
                (
#endif
                var cmColl = new DBEntityCollection<ClassModifier>(Connection)
#if CS8
                ;
#else
                )
#endif
            using
#if !CS8
                (
#endif
                var nColl = new DBEntityCollection<Namespace>(Connection)
#if CS8
                ;
#else
                )
#endif
            using
#if !CS8
                (
#endif

                var ttColl = new DBEntityCollection<TypeType>(Connection)
#if CS8
                ;
#else
                )
#endif
            using
#if !CS8
                (
#endif

                var amColl = new DBEntityCollection<AccessModifier>(Connection)
#if CS8
                ;
#else
                )
#endif
            using
#if !CS8
                (
#endif
                var tColl = new DBEntityCollection<Type>(Connection)
#if CS8
                ;
#else
                )
            {
#endif
            void setClassModifier(Class @class)
            {
                ClassModifier getModifier(in string name) => new
#if !CS9
                ClassModifier
#endif
                (cmColl)
                { Name = name };

                @class.Modifier = tmpType.IsAbstract
                    ? tmpType.IsSealed ? getModifier("static") : getModifier("abstract")
                    : tmpType.IsSealed ? getModifier("sealed") : null;
            }

            void updateInheritance(Class @class, DBEntityCollection<Class> classes)
            {
                bool addInheritance()
                {
                    if (ValidPackages.Contains(tmpType.Assembly.GetName().Name))
                    {
                        (string left, string
#if CS8
                    ?
#endif
                    right) = tmpType.Namespace.SplitL('.');

                        var @namespace = new Namespace(nColl) { FrameworkId = PackageInfo.FrameworkId };

                        if (right == null)

                            @namespace.Name = left;

                        else
                        {
                            @namespace.ParentId = GetNamespaceId(left);

                            @namespace.Name = right;
                        }

                        _type = new Type(tColl) { Namespace = @namespace, Name = tmpType.GetRealName(), GenericTypeCount = tmpType.GetRealGenericTypeParameterLength(), TypeType = new TypeType(ttColl) { Name = "Class" }, AccessModifier = new AccessModifier(amColl) { Name = tmpType.IsPublicType() ? "public" : "protected" } };

                        UpdateParentType(tmpType, _type, tColl, amColl, nColl, ttColl, classes);

                        @class.InheritsFromDocType = new Class(classes) { Type = _type };

                        setClassModifier(@class.InheritsFromDocType);

                        return true;
                    }

                    else if (tmpType != typeof(object))

                        @class.InheritsFrom = tmpType.FullName;

                    return false;
                }

                Func<bool> func = () =>
                {
                    func = () =>
                    {
                        @class = @class.InheritsFromDocType;

                        return addInheritance();
                    };

                    return addInheritance();
                };

                while ((tmpType = tmpType.BaseType) != null && func()) { /* Left empty. */ }
            }

            UpdateTypes<DotNetClass, Class, ClassInterfaceImplementation>("Class", @class => @class.Type, (dotNetClass, type, classes) =>
            {
                var @class = new Class(classes) { Type = type };

                tmpType = dotNetClass.Type;

                setClassModifier(@class);
                updateInheritance(@class, classes);

                return @class;
            },
            (type, items) => new ClassInterfaceImplementation(items) { Class = type }, (DotNetClass type, Class @class, DBEntityCollection<Class> classes) =>
            {
                tmpType = type.Type;

                setClassModifier(@class);
                updateInheritance(@class, classes);
            },
            DotNetClasses, CreateClassFile, type => DeleteClassMetadata(type, null));
#if !CS8
            }
#endif
        }
    }
}
