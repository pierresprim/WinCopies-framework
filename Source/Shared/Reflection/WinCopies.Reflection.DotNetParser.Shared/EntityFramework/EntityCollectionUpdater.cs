using System.Collections.Generic;

namespace WinCopies.EntityFramework
{
    public interface IEntityCollectionUpdater<TParameter, TResult>
    {
        void AddValue(string column, TParameter parameter, bool isId);

        uint ExecuteRequest(string table, out TResult result);

        uint Delete(string table, string foreignKeyIdColumn, object foreignKeyId);

        void Delete(string table, string idColumn, string foreignKeyIdColumn, object foreignKeyId, Predicate predicate);
    }

    public interface IEntityCollectionDeleter : System.IDisposable
    {
        uint Delete(string table, string idColumn, object id);
    }

    public abstract class EntityCollectionUpdater<TParameter, TResult> : IEntityCollectionUpdater<TParameter, TResult>
    {
        protected abstract IEnumerable<IPopable<string, object
#if CS8
            ?
#endif
            >> GetValues(string table, string idColumn, string foreignKeyIdColumn, object foreignKeyId);
        protected abstract IEntityCollectionDeleter GetDeleter();

        public void Delete(string table, string idColumn, string foreignKeyIdColumn, object foreignKeyId, Predicate predicate)
        {
            using
#if !CS8
                (
#endif
                IEntityCollectionDeleter deleter = GetDeleter()
#if CS8
                ;
#else
                )
            {
#endif

                void tryDelete(in object id)
                {
                    if (id != null && predicate(id))

                        _ = deleter.Delete(table, idColumn, id);
                }

                foreach (IPopable<string, object
#if CS8
            ?
#endif
            > popable in GetValues(table, idColumn, foreignKeyIdColumn, foreignKeyId))

                    tryDelete(idColumn);
#if !CS8
            }
#endif
        }

        public abstract void AddValue(string column, TParameter parameter, bool isId);
        public abstract uint ExecuteRequest(string table, out TResult result);
        public abstract uint Delete(string table, string foreignKeyIdColumn, object foreignKeyId);
    }
}
