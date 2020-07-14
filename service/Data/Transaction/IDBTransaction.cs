namespace Data.Transaction
{
    public interface IDBTransaction
    {
        void BeginTransaction();
        void Commit();
        void Rollback();
    }
}
