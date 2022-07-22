using System;
using System.Linq;

using WinCopies.Collections.Generic;
using WinCopies.Data.SQL;
using WinCopies.Extensions;
using WinCopies.Reflection.DotNetParser;
using WinCopies.Util;

namespace WinCopies.Reflection.DotNetDocBuilder
{
    public partial class Writer
    {
        public void UpdateClasses(in ArrayBuilder<IWriterCallback<Class, DotNetClass>> callbacks)
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
                    if (DotNetNamespace.DefaultTypePredicate(tmpType) && ValidPackages.Contains(tmpType.Assembly.GetName().Name))
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

                        return false;
                    }

                    else if (tmpType != typeof(object))

                        @class.InheritsFrom = tmpType.FullName;

                    return true;
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

                while (!((tmpType = tmpType.BaseType) == null || func())) { /* Left empty. */ }
            }

            callbacks.AddFirst(GetConstantUpdater<Class, DotNetClass>(c => c.Type, DefaultConverter<DotNetClass>));
            callbacks.AddFirst(new WriterDelegateCallback<Class, DotNetClass>(
                null, null, (Class @class, DotNetClass type, DBEntityCollection<Class> classes) =>
                {
                    tmpType = type.Type;

                    setClassModifier(@class);
                    updateInheritance(@class, classes);
                },

                (type, __type) => DeleteClassMetadata(type, null)));

            UpdateTypes<Class, DotNetClass, ClassInterfaceImplementation>("Class", @class => @class.Type, (type, dotNetClass, classes) =>
            {
                var @class = new Class(classes) { Type = type };

                tmpType = dotNetClass.Type;

                setClassModifier(@class);
                updateInheritance(@class, classes);

                return @class;
            }, (type, items) => new ClassInterfaceImplementation(items) { Class = type }, DotNetClasses, CreateClassFile, callbacks);
#if !CS8
            }
#endif
        }

        public void UpdateClasses(in System.Collections.Generic.IEnumerable<IWriterCallback<Class, DotNetClass>> callbacks) => UpdateClasses(new ArrayBuilder<IWriterCallback<Class, DotNetClass>>(callbacks));

        public void UpdateClasses(params IWriterCallback<Class, DotNetClass>[] callbacks) => UpdateClasses(callbacks.AsEnumerable());
    }
}
