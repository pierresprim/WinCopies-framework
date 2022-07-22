using System;
using System.Linq;
using System.Reflection;

using WinCopies.Data.SQL;
using WinCopies.EntityFramework;
using WinCopies.Reflection.DotNetParser;

namespace WinCopies.Reflection.DotNetDocBuilder
{
    public partial class Writer
    {
        protected partial class ConstantUpdater<T, TDotNetType> : IWriterCallback<T, TDotNetType> where T : IEntity where TDotNetType : DotNetType
        {
            public void OnUpdated(T @enum, TDotNetType dotNetEnum, DBEntityCollection<T> collection)
            {
                void log(in string msg, in bool increment) => logger($"Updat{msg} constants for {dotNetEnum}.", increment);

                log("ing", true);

                enumId = (enumType = _converter(@enum)).Id;
                SetConstants(dotNetEnum);

                using
#if !CS8
                (
#endif
                ISQLConnection _connection = connection.Clone()
#if CS8
                ;
#else
                )
                {
#endif
                DBEntityCollection<V> getCollection<V>() where V : IEntity => new
#if !CS9
                DBEntityCollection<V>
#endif
                (_connection);

                void parseDB<V>(in Action _action) where V : IConst
                {
                    using
#if !CS8
                    (
#endif
                    DBEntityCollection<V> dbConstants = getCollection<V>()
#if CS8
                    ;
#else
                    )
#endif
                    foreach (V docConst in dbConstants.Where(docConst => (docMember = docConst.Member).Type.Id == enumId))
                    {
                        c = docConst;

                        _action();
                    }
                }

                void updateValue()
                {
                    UpdateValue();

                    logger($"Updated {c.Update(out tables)} rows in {tables} {nameof(tables)}.", false, ConsoleColor.DarkGreen);
                }

                void logParsingDB() => logger("Parsing DB.", true);
                void logSwitchParsing()
                {
                    logger("Parsing DB completed.", false);
                    logger($"Parsing {dotNetEnum}.", true);
                }
                void logParsingCompleted() => logger($"Parsing {dotNetEnum} completed.", false);
                void logSearching() => logger($"Searching {constantName}.", true, ConsoleColor.DarkYellow);
                void logUpdating() => logger($"{constantName} found in {dotNetEnum}. Updating", null);
                void logAndRemove(in string msg)
                {
                    logger($"{msg}. Removing.", null, ConsoleColor.Red);
                    logger($"Removed {c.Remove(out tables)} rows in {tables} {nameof(tables)}.", false);
                }

                if (_underlyingTypeConverter == null)
                {
                    logger($"{dotNetEnum} is not an enumeration. Parsing constants as constant fields.", null);

                    void update(in Func<bool> _func)
                    {
                        constantName = c.Member.Name;

                        logSearching();

                        type = (f = constants.FirstOrDefault(_c => _c.Name == constantName)).FieldType;

                        if (f == null)

                            logAndRemove($"{c.Member.Name} not found in {f}");

                        else if (_func())

                            updateValue();

                        else

                            logAndRemove("The byte length of the constant's type has changed");
                    }

                    void _parseDB<V, W>() where V : IConst where W : unmanaged => parseDB<V>(() => update(AreByteLengthsEqual<W>));

                    logParsingDB();

                    SetConverter16();
                    _parseDB<Const16, short>();

                    SetConverter64();
                    _parseDB<Const64, long>();

                    this.converter = Delegates.SelfIn;
                    _parseDB<ConstFP32, float>();
                    _parseDB<ConstFP64, double>();
                    _parseDB<ConstFP128, decimal>();

                    parseDB<ConstString>(() => update(() => type == typeof(string)));

                    logSwitchParsing();

                    Add(() => Add(true, false));

                    logParsingCompleted();
                }

                else
                {
                    logger($"{dotNetEnum} is an enumeration. Parsing constants as enumeration values.", null);

                    type = _underlyingTypeConverter(dotNetEnum);

                    logParsingDB();

                    void _log(in string
#if CS8
                        ?
#endif
                        msg, in string _msg) => logger($"Underlying types{msg} have the same byte length. {_msg}.", null);
                    void __log(in string msg) => _log(null, msg);
                    void ___log(in string msg) => _log(" do not", msg);

                    (Action, Func<Action>) getTypesNotEqualAction()
                    {
                        ___log("All constants will be removed");

                        return (() =>
                        {
                            logger($"Removing {c}.", true, ConsoleColor.Red);

                            logger($"Removed {c.Remove(out tables)} rows in {tables} {nameof(tables)}.", false);
                        }, () =>
                        {
                            ___log($"All constants in {dotNetEnum} will be added, if any");

                            return () => Add(false, false);
                        }
                        );
                    }

                    _action.onDotNetTypeParsing = null;

                    unsafe bool process<TConst, TValue>(ConverterIn<DBEntityCollection<TConst>, IConst> _func) where TConst : Const<TConst, TValue> where TValue : unmanaged
                    {
                        void updateConstantName() => constantName = docMember.Name;

                        bool parseAgain;

                        (Action, Func<Action>) __func()
                        {
                            __log($"The constants will be updated or removed, if any, depending on whether they are still in {dotNetEnum}");

                            DBEntityCollection<TConst> _dbConstants = SetFunc<TConst>(_connection, _func, (Collections.Generic.IDisposableEnumerable<IConst> _dbConsts, ConverterIn<DBEntityCollection<TConst>, IConst> __converter) =>
                            {
                                _SetFunc(_dbConsts, __converter);

                                dbConsts = _dbConsts;
                            });

                            SetAddDelegate<TConst>(_dbConstants);

                            return (() =>
                            {
                                logSearching();

                                if (constants.Any(_c => (f = _c).Name == constantName))
                                {
                                    logUpdating();

                                    updateValue();

                                    return;
                                }

                                logAndRemove($"{constantName} not found in {dotNetEnum}");
                            }, () =>
                            {
                                __log("The new constants will be added, if any");

                                return () =>
                                {
                                    logger($"Searching {fName = f.Name}.", true);

                                    foreach (IConst dbConst in dbConsts.Where(_c => AreIdsEqual((docMember = _c.Member).Type.Id) && docMember.Name == fName))

                                        return;

                                    Add(false, true);
                                };
                            }
                            );
                        }

                        _action.onDBParsing = () => (_action = UtilHelpers.GetValue(AreByteLengthsEqual<TValue>, __func, getTypesNotEqualAction)).onDBParsing();

                        action = () =>
                        {
                            parseAgain = false;

                            (action = updateConstantName)();
                        };

                        parseAgain = true;

                        parseDB<TConst>(() =>
                        {
                            action();

                            _action.onDBParsing();
                        });

                        return parseAgain;
                    }

                    SetConverter16();

                    if (process<Const16, ushort>(Const16.GetNewConstant))
                    {
                        SetConverter64();

                        process<Const64, ulong>(Const64.GetNewConstant);
                    }

                    logSwitchParsing();

                    action = _action.onDotNetTypeParsing();

                    mColl = getCollection<Member>();

                    foreach (FieldInfo _c in constants)
                    {
                        f = _c;

                        action();
                    }

                    mColl.Dispose();
                    dbConsts.Dispose();

                    logParsingCompleted();
                }
#if !CS8
            }
#endif

                log("ed", false);
            }
        }
    }
}
