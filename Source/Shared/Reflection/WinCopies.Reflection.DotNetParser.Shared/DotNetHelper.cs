using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

using EUT = WinCopies.Reflection.DotNetParser.DotNetEnumUnderlyingType;

namespace WinCopies.Reflection.DotNetParser
{
    public static class DotNetHelper
    {
        public static string
#if CS8
            ?
#endif
            GetCSAccessModifier(Type type) => type.IsPublic
                ? "public"
                : type.IsNestedFamily
                ? type.IsNestedFamANDAssem ? "private protected" : type.IsNestedFamORAssem ? "protected internal" : "protected"
                : type.IsNestedAssembly ? "internal" : type.IsNestedPrivate ? "private" : null;

        public static string GetCSAccessModifier<T>() => GetCSAccessModifier(typeof(T));

        public static IEnumerable<Type
#if CS8
            ?
#endif
            >
#if CS8
            ?
#endif
             GetDefinedTypes(this Assembly assembly)
        {
            try
            {
                return (assembly ?? throw ThrowHelper.GetArgumentNullException(nameof(assembly))).DefinedTypes;
            }

            catch (ReflectionTypeLoadException ex)
            {
                return ex.Types;
            }
        }

        public static ArgumentException GetTypeIsNotEnumException(in string paramName) => new
#if !CS9
            ArgumentException
#endif
            ($"The current type does not represent an {nameof(Enum)} type.", paramName);

        public static ArgumentException GetTypeHasWrongTypeTypeException(in string paramName, in string typeType) => new
#if !CS9
            ArgumentException
#endif
            ($"The current type does not represent {typeType} type.", paramName);

        public static EUT ToDotNetEnumUnderlyingType(this Type type)
        {
            if (type.IsEnum)
            {
                Type t = type.GetEnumUnderlyingType();

#if CS8
                static
#endif
                KeyValuePair<Type, EUT> getType<T>(in EUT enumType) => new
#if !CS9
                    KeyValuePair<Type, EUT>
#endif
                    (typeof(T), enumType);

                KeyValuePair<Type, EUT>[] types = UtilHelpers.GetArray(getType<int>(EUT.Int32), getType<uint>(EUT.UInt32), getType<short>(EUT.Int16), getType<ushort>(EUT.UInt16), getType<long>(EUT.Int64), getType<ulong>(EUT.UInt64), getType<byte>(EUT.Byte), getType<sbyte>(EUT.SByte));

                foreach (KeyValuePair<Type, EUT> underlyingType in types)

                    if (t == underlyingType.Key)

                        return underlyingType.Value;
            }

            throw GetTypeIsNotEnumException(nameof(type));
        }

        public static string ToCSName(this EUT value)
#if CS8
            =>
#else
        {
#endif
#if !CS8
            switch (
#endif
                value
#if CS8
                switch
#else
                )
#endif
                {
#if !CS8
                case
#endif
                    EUT.Int32
#if CS8
                    =>
#else
                    :
                    return
#endif
                    "int"
#if CS8
                    ,
#else
                    ;
                case
#endif
                    EUT.UInt32
#if CS8
                    =>
#else
                    :
                    return
#endif
                    "uint"
#if CS8
                    ,
#else
                    ;
                case
#endif
                    EUT.Int16
#if CS8
                    =>
#else
                    :
                    return
#endif
                    "short"
#if CS8
                    ,
#else
                    ;
                case
#endif
                    EUT.UInt16
#if CS8
                    =>
#else
                    :
                    return
#endif
                    "ushort"
#if CS8
                    ,
#else
                    ;
                case
#endif
                    EUT.Int64
#if CS8
                    =>
#else
                    :
                    return
#endif
                    "long"
#if CS8
                    ,
#else
                    ;
                case
#endif
                    EUT.UInt64
#if CS8
                    =>
#else
                    :
                    return
#endif
                    "ulong"
#if CS8
                    ,
#else
                    ;
                case
#endif
                    EUT.Byte
#if CS8
                    =>
#else
                    :
                    return
#endif
                    "byte"
#if CS8
                    ,
#else
                    ;
                case
#endif
                    EUT.SByte
#if CS8
                    =>
#else
                    :
                    return
#endif
                    "sbyte"
#if CS8
                    ,
                    _ =>
#else
                    ;
                default:
#endif
                    throw new ArgumentOutOfRangeException(nameof(value))
#if CS8
                }
        ;
#else
            ;
            }
        }
#endif
    }
}
