using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace WinCopies.IO.Reflection
{
    public static class ReflectionHelper
    {
        public static AccessModifier GetAccessModifier(in Type t) => t.IsPublic || t.IsNestedPublic ? AccessModifier.Public
            : t.IsNestedFamily ? AccessModifier.Protected
            : t.IsNestedFamORAssem ? AccessModifier.ProtectedInternal
            : t.IsNestedAssembly ? AccessModifier.Internal
            : t.IsNestedFamANDAssem ? AccessModifier.PrivateProtected
            : t.IsNestedPrivate ? AccessModifier.Private
            : (AccessModifier)0;

        public static AccessModifier GetAccessModifier(in MethodBase method) => method.IsPublic ? AccessModifier.Public
            : method.IsFamily ? AccessModifier.Protected
            : method.IsFamilyOrAssembly ? AccessModifier.ProtectedInternal
            : method.IsAssembly ? AccessModifier.Internal
            : method.IsFamilyAndAssembly ? AccessModifier.PrivateProtected
            : method.IsPrivate ? AccessModifier.Private
            : (AccessModifier)0;

        public static AccessModifier GetAccessModifier(in FieldInfo field) => field.IsPublic ? AccessModifier.Public
            : field.IsFamily ? AccessModifier.Protected
            : field.IsFamilyOrAssembly ? AccessModifier.ProtectedInternal
            : field.IsAssembly ? AccessModifier.Internal
            : field.IsFamilyAndAssembly ? AccessModifier.PrivateProtected
            : field.IsPrivate ? AccessModifier.Private
            : (AccessModifier)0;
    }
}
