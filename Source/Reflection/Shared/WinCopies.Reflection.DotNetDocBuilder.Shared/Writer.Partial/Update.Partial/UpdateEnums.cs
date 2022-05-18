using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

using WinCopies.Data.SQL;
using WinCopies.EntityFramework;
using WinCopies.Linq;
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
            void setUnderlyingType(DotNetEnum dotNetEnum, Enum @enum) => @enum.UnderlyingType = new UnderlyingType(utColl) { Name = getUnderlyingTypeCSName(dotNetEnum) };

            Enum tmp;
            Member docMember = null;
            string constantName = null;
            Action action;
            System.Type type;
            uint tables;
            FieldInfo f = null;
            string fName = null;
            (Action onDBParsing, Func<Action> onDotNetTypeParsing) _action;
            IEnumerable<FieldInfo> constants;
            Collections.Generic.IDisposableEnumerable<IConst> dbConsts = null;
            Type enumType;
            ulong enumId;
            long? id;
            Func<IConst> func = null;
            IConst c = null;
            ActionIn<bool> _add = null;
            DBEntityCollection<Member> mColl = null;
            ConverterIn<object, object> converter;

            unsafe bool areByteLengthsEqual<T>() where T : unmanaged
            {
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

                return getSize() == sizeof(T);
            }

            bool areIdEqual(in ulong _id) => _id == enumId;

            void setConstants(in DotNetEnum dotNetEnum) => constants = dotNetEnum.Type.GetConstants().Where(_c => _c.IsPublic || _c.IsFamily || _c.IsFamilyOrAssembly);

            DBEntityCollection<T> _getCollection<T>(in ISQLConnection connection) where T : IEntity => new
#if !CS9
                    DBEntityCollection<T>
#endif
            (connection);

            void _setFunc<T>(Collections.Generic.IDisposableEnumerable<IConst> _dbConsts, ConverterIn<DBEntityCollection<T>, IConst> _func) where T : IConst => func = () => _func((DBEntityCollection<T>)_dbConsts);

            DBEntityCollection<T> setFunc<T>(in ISQLConnection connection, in ConverterIn<DBEntityCollection<T>, IConst> _func, in Action<Collections.Generic.IDisposableEnumerable<IConst>, ConverterIn<DBEntityCollection<T>, IConst>> __action) where T : IConst
            {
                DBEntityCollection<T> _dbConstants = _getCollection<T>(connection);

                __action(new WinCopies.Collections.Generic.DisposableEnumerable<T, IConst>(_dbConstants), _func);

                return _dbConstants;
            }

            void updateValue() => c.Value = converter(f.GetValue(null));

            void setAddDelegate<T>(DBEntityCollection<T> _dbConstants) where T : IConst => _add = (in bool decrement) =>
            {
                id = _dbConstants.Add((T)c, out tables, out ulong rows);

                Logger($"Added {rows} {nameof(rows)} in {tables} {nameof(tables)}. Last inserted id: {id}.", decrement ?
#if !CS9
                                    (bool?)
#endif
                    false : null);
            };

            void add(in bool decrement)
            {
                c = func();

                c.Member = new Member(mColl) { Name = fName, Type = enumType };

                updateValue();

                _add(decrement);
            }

            void setConverter<T>() => converter = (in object obj) => unchecked((T)obj);

            void setConverter16() => setConverter<ushort>();

            void setConverter64 ()=>setConverter<ulong>();

            UpdateItems("Enum", false, @enum => true, (Enum @enum, DotNetEnum dotNetEnum) =>
            {
                bool process<TConst, TValue>(in ConverterIn<DBEntityCollection<TConst>, IConst> _converter) where TConst : IConst where TValue : unmanaged
                {
                    setFunc<TConst>(Connection, _converter, _setFunc);

                    if (areByteLengthsEqual<TValue>())
                    {
                        foreach (FieldInfo _c in constants)
                        {
                            fName = _c.Name;

                            add(false);
                        }

                        return false;
                    }

                    return true;
                }

                setConstants(dotNetEnum);

                type = dotNetEnum.GetEnumUnderlyingType();
                enumType = @enum.Type;

                setConverter16();

                if (process<Const16, ushort>(Const16.GetNewConstant))
                {
                    setConverter64();

                    process<Const64, ulong>(Const64.GetNewConstant);
                }
            },
            @enum =>
            {
                enumId = @enum.Type.Id;

                ulong remove<T>() where T : IConst => Remove<T>(_c => areIdEqual(_c.Member.Type.Id));

                if (Remove<Const16>() == 0ul)

                    Remove<Const64>();
            },
            item => item.Type, GetAllEnumsInPackages, (DotNetEnum @enum, Type _type, DBEntityCollection<Enum> enums) =>
            {
                tmp = new Enum(enums) { Type = _type };

                setUnderlyingType(@enum, tmp);

                return tmp;
            },
                (DotNetEnum dotNetEnum, Enum @enum, DBEntityCollection<Enum> enums) =>
                {
                    setUnderlyingType(dotNetEnum, @enum);

                    void log(in string msg, in bool increment) => Logger($"Updat{msg} constants for {dotNetEnum}.", increment);

                    log("ing", true);

                    enumId = (enumType = @enum.Type).Id;
                    setConstants(dotNetEnum);

                    using
#if !CS8
                    (
#endif
                    ISQLConnection connection = GetConnection()
#if CS8
                    ;
#else
                    )
                    {
#endif
                    type = dotNetEnum.GetEnumUnderlyingType();

                    Logger("Parsing DB.", true);

                    void _log(in string msg, in string _msg) => Logger($"Underlying types{msg} have the same byte length. {_msg}.", null);
                    void __log(in string msg) => _log(null, msg);
                    void ___log(in string msg) => _log(" do not", msg);

                    (Action, Func<Action>) getTypesNotEqualAction()
                    {
                        ___log("All constants will be removed");

                        return (() =>
                        {
                            Logger($"Removing {c}.", true, ConsoleColor.Red);

                            Logger($"Removed {c.Remove(out tables)} rows in {tables} {nameof(tables)}.", false);
                        }, () =>
                        {
                            ___log($"All constants in {dotNetEnum} will be added, if any");

                            return () => add(false);
                        }
                        );
                    }

                    _action.onDotNetTypeParsing = null;

                    DBEntityCollection<T> getCollection<T>() where T : IEntity => _getCollection<T>(connection);

                    unsafe bool process<TConst, TValue>(ConverterIn<DBEntityCollection<TConst>, IConst> _func) where TConst : Const<TConst, TValue> where TValue : unmanaged
                    {
                        void updateConstantName() => constantName = docMember.Name;

                        bool parseAgain;

                        (Action, Func<Action>) __func()
                        {
                            __log($"The constants will be updated or removed, if any, depending on whether they are still in {dotNetEnum}");

                            DBEntityCollection<TConst> _dbConstants = setFunc<TConst>(connection, _func, (Collections.Generic.IDisposableEnumerable<IConst> _dbConsts, ConverterIn<DBEntityCollection<TConst>, IConst> _converter) =>
                            {
                                _setFunc(_dbConsts, _converter);

                                dbConsts = _dbConsts;
                            });

                            setAddDelegate<TConst>(_dbConstants);

                            return (() =>
                            {
                                Logger($"Searching {constantName}.", true, ConsoleColor.DarkYellow);

                                if (constants.Any(_c => (f = _c).Name == constantName))
                                {
                                    Logger($"{constantName} found in {dotNetEnum}. Updating", null);

                                    updateValue();

                                    Logger($"Updated {c.Update(out tables)} rows in {tables} {nameof(tables)}.", false, ConsoleColor.DarkGreen);

                                    return;
                                }

                                Logger($"{constantName} not found in {dotNetEnum}. Removed {c.Remove(out tables)} rows in {tables} {nameof(tables)}.", false, ConsoleColor.Red);
                            }, () =>
                            {
                                __log("The new constants will be added, if any");

                                return () =>
                                {
                                    Logger($"Searching {fName = f.Name}.", true);

                                    foreach (IConst dbConst in dbConsts.Where(_c => areIdEqual((docMember = _c.Member).Type.Id) && docMember.Name == fName))

                                        return;

                                    add(true);
                                };
                            }
                            );
                        }

                        _action.onDBParsing = () => (_action = UtilHelpers.GetValue(areByteLengthsEqual<TValue>, __func, getTypesNotEqualAction)).onDBParsing();

                        action = () =>
                        {
                            parseAgain = false;

                            (action = updateConstantName)();
                        };

                        parseAgain = true;

                        using
#if !CS8
                        (
#endif
                        DBEntityCollection<TConst> dbConstants = getCollection<TConst>()
#if CS8
                        ;
#else
                        )
#endif
                        foreach (TConst docConst in dbConstants.Where(docConst => (docMember = docConst.Member).Type.Id == enumId))
                        {
                            c = docConst;

                            action();

                            _action.onDBParsing();
                        }

                        return parseAgain;
                    }

                    setConverter16();

                    if (process<Const16, ushort>(Const16.GetNewConstant))
                    {
                        setConverter64();

                        process<Const64, ulong>(Const64.GetNewConstant);
                    }

                    Logger("Parsing DB completed.", false);

                    Logger($"Parsing {dotNetEnum}.", true);

                    action = _action.onDotNetTypeParsing();

                    mColl = getCollection<Member>();

                    foreach (FieldInfo _c in constants)
                    {
                        f = _c;

                        action();
                    }

                    mColl.Dispose();
                    dbConsts.Dispose();
#if !CS8
                }
#endif

                    Logger($"Parsing {dotNetEnum} completed.", false);

                    log("ed", false);
                }, CreateEnumFile, null, false);
#if !CS8
            }
#endif
        }
    }
}
