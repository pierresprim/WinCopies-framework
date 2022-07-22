#region Usings
#region System
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
#endregion System

#region WinCopies
using WinCopies.Data.SQL;
using WinCopies.EntityFramework;
using WinCopies.Reflection.DotNetParser;
using WinCopies.Util;
#endregion WinCopies
#endregion Usings

namespace WinCopies.Reflection.DotNetDocBuilder
{
    public partial class Writer
    {
        protected partial class ConstantUpdater<T, TDotNetType> : IWriterCallback<T, TDotNetType> where T : IEntity where TDotNetType : DotNetType
        {
            private struct RemoveActionProvider
            {
                private readonly Writer _writer;

                public RemoveActionProvider(in Writer writer) => _writer = writer;

                public ConverterIn<Predicate<U>, ulong> GetRemoveAction<U>() where U : IConst => _writer.RemovePredicate;
            }

            #region Fields
            //private T tmp;
            private Member docMember;
            private string constantName;
            private Action action;
            private System.Type type;
            private System.Type constType;
            private uint tables;
            private FieldInfo f;
            private string fName;
            private (Action onDBParsing, Func<Action> onDotNetTypeParsing) _action;
            private IEnumerable<FieldInfo> constants;
            private Collections.Generic.IDisposableEnumerable<IConst> dbConsts;
            private Type enumType;
            private ulong enumId;
            private long? id;
            private Func<IConst> func;
            private IConst c;
            private ActionIn<bool> _add;
            private DBEntityCollection<Member> mColl;
            private DBEntityCollection<AccessModifier> amColl;
            private ConverterIn<object, object> converter;
            private Converter<T, Type> _converter;
            private readonly Converter<TDotNetType, System.Type>
#if CS8
                ?
#endif
                _underlyingTypeConverter;
            private readonly ISQLConnection connection;
            private readonly Logger logger;
            private readonly RemoveActionProvider removeActionProvider;
            #endregion

            public ConstantUpdater(in Writer writer, in Converter<T, Type> converter, in Converter<TDotNetType, System.Type>
#if CS8
                ?
#endif
                underlyingTypeConverter)
            {
                connection = writer.Connection;
                logger = writer.Logger;
                removeActionProvider = new RemoveActionProvider(writer);
                amColl = new DBEntityCollection<AccessModifier>(connection.Clone());
                _converter = converter;
                _underlyingTypeConverter = underlyingTypeConverter;
            }

            #region Methods
            private unsafe bool AreByteLengthsEqual<U>() where U : unmanaged
            {
                byte getSize()
                {
                    bool checkTypeEquality(params System.Type[] types) => types.Any(t => t == type);

                    if (checkTypeEquality(typeof(byte), typeof(sbyte), typeof(short), typeof(ushort), typeof(bool), typeof(char)))

                        return 2;

                    if (checkTypeEquality(typeof(int), typeof(uint), typeof(long), typeof(ulong)
#if CS9
                        , typeof(nint), typeof(nuint)
#endif
                        ))

                        return 4;

                    Debug.Assert(false);

                    throw new Exception("An unknown exception occurred.");
                }

                constType = typeof(U);

                bool checkTypes(params System.Type[] types) => types.Any(t => constType == t && constType == type);

                return checkTypes(typeof(float), typeof(double), typeof(decimal), typeof(string)) || getSize() == sizeof(U);
            }

            private void _SetFunc<U>(Collections.Generic.IDisposableEnumerable<IConst> _dbConsts, ConverterIn<DBEntityCollection<U>, IConst> _func) where U : IConst => func = () => _func((DBEntityCollection<U>)_dbConsts);

            private static DBEntityCollection<U> SetFunc<U>(in ISQLConnection connection, in ConverterIn<DBEntityCollection<U>, IConst> _func, in Action<Collections.Generic.IDisposableEnumerable<IConst>, ConverterIn<DBEntityCollection<U>, IConst>> __action) where U : IConst
            {
                var _dbConstants = new DBEntityCollection<U>(connection);

                __action(new Collections.Generic.DisposableEnumerable<U, IConst>(_dbConstants), _func);

                return _dbConstants;
            }

            private void UpdateValue()
            {
                c.Value = converter(f.GetValue(null));

                SetAccessModifier(amColl, f.IsPublic, c.Member);
            }

            private void Add(in bool refresh, in bool decrement)
            {
                c = func();

                c.Member = new Member(mColl) { Name = fName, Type = enumType };

                UpdateValue();

                if (refresh)

                    c.MarkForRefresh();

                _add(decrement);
            }

            private void SetConstants(in DotNetType dotNetEnum) => constants = dotNetEnum.Type.GetConstants().Where(_c => _c.IsPublic || _c.IsFamily || _c.IsFamilyOrAssembly);

            private void SetConverter<U>() => converter = (in object obj) => unchecked((U)obj);
            private void SetConverter16() => SetConverter<ushort>();
            private void SetConverter64() => SetConverter<ulong>();

            private void _Add<U>(DBEntityCollection<U> _dbConstants, in bool decrement) where U : IConst
            {
                id = _dbConstants.Add((U)c, out tables, out ulong rows);

                logger($"Added {rows} {nameof(rows)} in {tables} {nameof(tables)}. Last inserted id: {id}.", decrement ?
#if !CS9
                    (bool?)
#endif
                    false : null);
            }

            private void SetAddDelegate<U>(DBEntityCollection<U> _dbConstants) where U : IConst => _add = (in bool d) => _Add(_dbConstants, d);

            private bool AreIdsEqual(in ulong _id) => _id == enumId;

            private void SetAddDelegate<U>(in ConverterIn<DBEntityCollection<U>, IConst> _converter) where U : IConst
            {
                DBEntityCollection<U> _dbConstants = SetFunc(connection, _converter, _SetFunc);

                SetAddDelegate(_dbConstants);
            }

            private void Add(in Action add)
            {
                amColl = new DBEntityCollection<AccessModifier>(connection);

                foreach (FieldInfo _c in constants)
                {
                    type = (f = _c).FieldType;
                    fName = _c.Name;

                    logger($"Processing {fName}.", true, ConsoleColor.DarkYellow);

                    if (AreByteLengthsEqual<short>())
                    {
                        SetConverter16();

                        SetAddDelegate<Const16>(Const16.GetNewConstant);
                    }

                    else if (AreByteLengthsEqual<long>())
                    {
                        SetConverter64();

                        SetAddDelegate<Const64>(Const64.GetNewConstant);
                    }

                    else
                    {
                        converter = Delegates.SelfIn;

                        if (type == typeof(float))

                            SetAddDelegate<ConstFP32>(ConstFP32.GetNewConstant);

                        else if (type == typeof(double))

                            SetAddDelegate<ConstFP64>(ConstFP64.GetNewConstant);

                        else if (type == typeof(decimal))

                            SetAddDelegate<ConstFP128>(ConstFP128.GetNewConstant);

                        else if (type == typeof(string))

                            SetAddDelegate<ConstString>(ConstString.GetNewConstant);
                    }

                    add();

                    logger($"Processing {fName} completed.", false);
                }
            }

            public void OnGetItem(T item, Type type, TDotNetType @enum, DBEntityCollection<T> enums) { /* Left empty. */ } //=> tmp = item;

            public void OnAdded(T item, Type _type, TDotNetType dotNetType)
            {
                void add() => Add(false, false);

                bool process<TConst, TValue>(in ConverterIn<DBEntityCollection<TConst>, IConst> _converter) where TConst : IConst where TValue : unmanaged
                {
                    if (AreByteLengthsEqual<TValue>())
                    {
                        SetAddDelegate(_converter);

                        foreach (FieldInfo _c in constants)
                        {
                            fName = (f = _c).Name;

                            add();
                        }

                        return false;
                    }

                    return true;
                }

                SetConstants(dotNetType);

                enumType = _type;

                if (_underlyingTypeConverter == null)

                    Add(add);

                else
                {
                    type = _underlyingTypeConverter(dotNetType);

                    SetConverter16();

                    if (process<Const16, short>(Const16.GetNewConstant))
                    {
                        SetConverter64();

                        _ = process<Const64, long>(Const64.GetNewConstant);
                    }
                }
            }

            public void OnDeleting(T item, Type @enum)
            {
                enumId = @enum.Id;

                PredicateIn<ulong> predicate = AreIdsEqual;

                ulong remove<V>() where V : IConst => removeActionProvider.GetRemoveAction<V>()(_c => predicate(_c.Member.Type.Id));

                if (remove<Const16>() == 0ul)

                    _ = remove<Const64>();
            }
            #endregion Methods
        }

        protected ConstantUpdater<T, U> GetConstantUpdater<T, U>(in Converter<T, Type> converter, in Converter<U, System.Type>
#if CS8
            ?
#endif
            underlyingTypeConverter) where T : IEntity where U : DotNetType => new
#if !CS9
            ConstantUpdater<T, U>
#endif
            (this, converter, underlyingTypeConverter);

        protected System.Type DefaultConverter<T>(T value) => null;

        protected static AccessModifier GetAccessModifier(in DBEntityCollection<AccessModifier> amColl, in bool isPublic) => new
#if !CS9
            AccessModifier
#endif
            (amColl)
        { Name = isPublic ? "public" : "protected" };

        protected static void SetAccessModifier(in DBEntityCollection<AccessModifier> amColl, in bool isPublic, in Member member) => member.AccessModifier = GetAccessModifier(amColl, isPublic);
    }
}
