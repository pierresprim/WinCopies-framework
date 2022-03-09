using WinCopies.Temp;

namespace WinCopies.EntityFramework
{
    public interface IEntityCollectionUpdater<TParameter, TResult>
    {
        void AddValue(string column, TParameter parameter, bool isId);

        uint ExecuteRequest(string table, out TResult result);

        uint Delete(string table, string foreignKeyIdColumn, object foreignKeyId);

        void Delete(string table, string idColumn, string foreignKeyIdColumn, object foreignKeyId, Predicate<object> predicate);
    }

    public interface IEntityCollectionDeleter : System.IDisposable
    {
        uint Delete(string table, string idColumn, object id);
    }

    public abstract class EntityCollectionUpdater<TParameter, TResult> : IEntityCollectionUpdater<TParameter, TResult>
    {
        protected abstract IEnumerable<IPopable<string, object?>> GetValues(string table, string idColumn, string foreignKeyIdColumn, object foreignKeyId);
        protected abstract IEntityCollectionDeleter GetDeleter();

        public void Delete(string table, string idColumn, string foreignKeyIdColumn, object foreignKeyId, Predicate<object> predicate)
        {
            using IEntityCollectionDeleter deleter = GetDeleter();

            void tryDelete(in object id)
            {
                if (id != null && predicate(id))

                    _ = deleter.Delete(table, idColumn, id);
            }

            foreach (IPopable<string, object?> popable in GetValues(table, idColumn, foreignKeyIdColumn, foreignKeyId))

                tryDelete(idColumn);
        }

        public abstract void AddValue(string column, TParameter parameter, bool isId);
        public abstract uint ExecuteRequest(string table, out TResult result);
        public abstract uint Delete(string table, string foreignKeyIdColumn, object foreignKeyId);
    }
}
