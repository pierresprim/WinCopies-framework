using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

using WinCopies.Data.SQL;
using WinCopies.EntityFramework;
using WinCopies.Reflection.DotNetParser;
using WinCopies.Util;

namespace WinCopies.Reflection.DotNetDocBuilder
{
    public partial class Writer
    {
        public void UpdateEnums()
        {
#if CS8
            static
#endif
                string getUnderlyingTypeCSName(in DotNetEnum @enum) => @enum.UnderlyingType.ToCSName();

            using
#if !CS8
                (
#endif
                var utColl = new DBEntityCollection<UnderlyingType>(Connection)
#if CS8
                ;
#else
                )
            {
#endif
            UnderlyingType getUnderlyingType(DotNetEnum @enum) => new
#if !CS9
                UnderlyingType
#endif
                (utColl)
            { Name = getUnderlyingTypeCSName(@enum) };

            Member docMember = null;
            string constantName = null;
            Action action;
            System.Type type;
            uint tables;
            FieldInfo f = null;
            (ActionIn<IConst> onDBParsing, ActionIn<FieldInfo> onDotNetTypeParsing) _action;

            UpdateItems("Enum", false, @enum => true, (Enum @enum, DotNetEnum dotNetEnum) =>
                {
                    void log(in string msg, in bool increment) => Logger($"Updat{msg} constants for {dotNetEnum}.", increment);

                    log("ing", true);

                    var constants = dotNetEnum.Type.GetConstants().Where(c => c.IsPublic || c.IsFamily || c.IsFamilyOrAssembly);

                    using ISQLConnection connection = GetConnection();

                    type = dotNetEnum.Type.GetEnumUnderlyingType();

                    Logger("Parsing DB.", true);

                    (ActionIn<IConst>, ActionIn<FieldInfo>) getTypesEqualAction(ConverterIn<object, object> converter)
                    {
                        Logger($"Underlying types have the same byte length. The constants will be updated or removed, if any, depending on whether they are still in {dotNetEnum}.", null);

                        return ((in IConst docConst) =>
                        {
                            Logger($"Searching {constantName}.", true, ConsoleColor.DarkYellow);

                            if (constants.Any(c => (f = c).Name == constantName))
                            {
                                Logger($"{constantName} found in {dotNetEnum}. Updating", null);

                                docConst.Value = converter(f.GetValue(null));

                                Logger($"Updated {docConst.Update(out tables)} rows in {tables} {nameof(tables)}.", false, ConsoleColor.DarkGreen);

                                return;
                            }

                            Logger($"{constantName} not found in {dotNetEnum}. Removed {docConst.Remove(out tables)} rows in {tables} {nameof(tables)}.", false, ConsoleColor.Red);
                        }, (in FieldInfo _f) =>
                        {

                        }
                        );
                    }

                    (ActionIn<IConst>, ActionIn<FieldInfo>) getTypesNotEqualAction()
                    {
                        Logger($"Underlying types do not have the same byte length. All constants will be removed.", null);

                        return ((in IConst docConst) =>
                        {
                            Logger($"Removing {docConst}.", true, ConsoleColor.Red);

                            Logger($"Removed {docConst.Remove(out tables)} rows in {tables} {nameof(tables)}.", false);
                        }, (in FieldInfo _f) =>
                        {

                        }
                        );
                    }

                    byte getSize()
                    {
                        bool checkTypeEquality(params System.Type[] types) => types.Any(t => t == type);

                        if (checkTypeEquality(typeof(byte), typeof(sbyte), typeof(short), typeof(ushort)))

                            return 2;

                        if (checkTypeEquality(typeof(int), typeof(uint), typeof(long), typeof(ulong)))

                            return 4;

                        Debug.Assert(false);

                        throw new Exception("An unknown exception occurred.");
                    }

                    _action.onDotNetTypeParsing = null;

                    unsafe bool process<TConst, TValue>(ConverterIn<object, object> converter) where TConst : Const<TConst, TValue> where TValue : unmanaged
                    {
                        void updateConstantName() => constantName = docMember.Name;

                        bool parseAgain = true;

                        _action.onDBParsing = (in IConst docConst) => (_action = getSize() == sizeof(TValue) ? getTypesEqualAction(converter) : getTypesNotEqualAction()).onDBParsing(docConst);

                        action = () =>
                        {
                            parseAgain = false;

                            (action = updateConstantName)();
                        };

                        foreach (TConst docConst in new DBEntityCollection<TConst>(connection).Where(docConst => (docMember = docConst.Member).Type.Id == @enum.Type.Id))
                        {
                            action();

                            _action.onDBParsing(docConst);
                        }

                        return parseAgain;
                    }

                    if (process<Const16, ushort>((in object obj) => unchecked((ushort)obj)))

                        process<Const64, ulong>((in object obj) => unchecked((ulong)obj));

                    Logger("Parsing DB completed.", false);

                    Logger($"Parsing {dotNetEnum}.", true);

                    foreach (FieldInfo c in constants)

                        _action.onDotNetTypeParsing(c);

                    Logger($"Parsing {dotNetEnum} completed.", false);

                    log("ed", false);
                }, null, item => item.Type, GetAllEnumsInPackages, (@enum, type, enums) => new
#if !CS9
Enum
#endif
(enums)
                {
                    UnderlyingType = getUnderlyingType(@enum),

                    Type = type
                },
            (DotNetEnum type, Enum @enum, DBEntityCollection<Enum> enums) => @enum.UnderlyingType = getUnderlyingType(type), CreateEnumFile, null, false);
#if !CS8
            }
#endif
        }
    }
}
