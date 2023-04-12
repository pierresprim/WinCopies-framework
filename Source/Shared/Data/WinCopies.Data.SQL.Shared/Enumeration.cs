using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace WinCopies.Data.SQL
{
    public abstract class SQLEnumerator<T> : Collections.Generic.Enumerator<ISQLGetter> where T : ISQLConnection
    {
        private readonly bool _closeReader;
        private DbDataReader _reader;
        private SQLGetter
#if CS8
            ?
#endif
            _current;
        private Action
#if CS8
            ?
#endif
            _moveNextAction;
        private Func<bool>
#if CS8
            ?
#endif
            _moveNext;
        //ulong i = 0;
        T _connection;
        DbCommand _command;
        bool _dispose;

        protected internal DbDataReader Reader => GetOrThrowIfDisposed(_reader);

        protected override ISQLGetter CurrentOverride => _current;

        public override bool? IsResetSupported => false;

        protected SQLEnumerator(in DbCommand command, in DbDataReader reader, in T connection, in bool closeReader)
        {
            _connection = connection;
            _command = command;
            _closeReader = closeReader;
            _reader = reader;
            _moveNextAction = () => _moveNextAction = () => _current.Dispose(false);

            void updateCurrent() => _current = new SQLGetter(Reader);

            Action onMoveNext = _closeReader ? () =>
            {
                _moveNextAction();

                updateCurrent();
            }
            : (Action)updateCurrent;

            bool moveNext()
            {
                /*if (_reader.IsClosed)
                {
                    //command.Connection = func();

                    if (_connection.IsDisposed)

                        initCollection();

                    _reader = command.ExecuteReader();

                    for (ulong _i = 0; _i < i; _i++)

                        _reader.Read();
                }*/

                if (Reader.Read())
                {
                    //i++;

                    onMoveNext();

                    return true;
                }

                //ResetOverride2();

                return false;
            }

            _moveNext = () =>
            {
                if (Reader == null)

                    return false;

                ResetConnection();

                return (_moveNext = moveNext)();
            };
        }

        protected SQLEnumerator(in DbCommand command, in T connection, in bool closeReader) : this(command ?? throw ThrowHelper.GetArgumentNullException(nameof(command)), command.ExecuteReader(), connection, closeReader) { /* Left empty. */ }

        protected abstract T CloneConnection(T connection);

        protected abstract DbConnection GetConnection(T connection);

        protected void ResetConnection()
        {
            if (_connection.IsDisposed && !_connection.Open())
            {
                _command.Connection = GetConnection(_connection = CloneConnection(_connection));

                _dispose = true;
            }
        }

        protected override void ResetCurrent()
        {
            base.ResetCurrent();

            if (_closeReader && _current != null)
            {
                _current.Dispose();
                _current = null;
            }
        }

        protected override void ResetOverride2()
        {
            _moveNextAction = null;
            _moveNext = null;

            //i = 0;
        }

        protected override bool MoveNextOverride() => _moveNext();

        protected override void DisposeUnmanaged()
        {
            ResetCurrent();

            if (_reader != null)
            {
                void closeReader()
                {
                    _command.Dispose();
                    _command = null;

                    if (_dispose)

                        _connection.Dispose();

                    _connection = default;
                }

                if (_closeReader)
                {
                    _reader.Close();
                    _reader.Dispose();

                    _reader = null;

                    closeReader();
                }

                else
                {
                    _reader = null;

                    if (_current != null)

                        _current.OnDispose = closeReader;
                }
            }

            base.DisposeUnmanaged();
        }
    }

    public abstract class SQLEnumerable<T> : Collections.DotNetFix.Generic.IDisposableEnumerable<ISQLGetter> where T : ISQLConnection
    {
        private struct Data
        {
            private readonly SQLEnumerable<T> _enumerable;
            private SQLEnumerator<T>
#if CS8
                ?
#endif
                _enumerator;

#if CS8
            [MaybeNull]
#endif
            public SQLEnumerator<T> Enumerator
            {
                get => _enumerator;

                set
                {
                    if ((_enumerator = value) != null)

                        _enumerable.DataReader = value.Reader;
                }
            }

            public Data(in SQLEnumerable<T> enumerable)
            {
                _enumerable = enumerable;
                _enumerator = null;
            }
        }

        private Data _data;

#if CS8
        [MaybeNull]
#endif
        protected SQLEnumerator<T> Enumerator { get => _data.Enumerator; private set => _data.Enumerator = value; }

        protected DbDataReader
#if CS8
            ?
#endif
            DataReader
        { get; private set; }

        public T Connection { get; }

        protected DbCommand Command { get; }

        public bool CloseReader { get; }

        public bool IsDisposed => DataReader == null;

        public SQLEnumerable(in DbCommand command, in T connection, in bool closeReader)
        {
            _data = new Data(this);
            Command = command;
            Connection = connection;
            CloseReader = closeReader;
        }

        protected abstract SQLEnumerator<T> GetSQLEnumerator();
        protected abstract Task<SQLEnumerator<T>> GetSQLEnumeratorAsync();

        public void Dispose()
        {
            if (IsDisposed)

                return;

            Enumerator.Dispose();
            Enumerator = null;

            if (!DataReader.IsClosed)
            {
                DataReader.Close();
                DataReader.Dispose();
            }

            DataReader = null;
        }

        protected InvalidOperationException GetEnumerationAlreadyStartedException() => new
#if !CS9
            InvalidOperationException
#endif
            ("An enumeration has already started.");

        public IEnumerator<ISQLGetter> GetEnumerator() => IsDisposed ? (Enumerator = GetSQLEnumerator()) : throw GetEnumerationAlreadyStartedException();
        public async Task<IEnumerator<ISQLGetter>> GetEnumeratorAsync() => IsDisposed ? (Enumerator = await GetSQLEnumeratorAsync()) : throw GetEnumerationAlreadyStartedException();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
