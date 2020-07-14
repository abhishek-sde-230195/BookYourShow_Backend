using Data.DBContext;
using Microsoft.EntityFrameworkCore.Storage;
using System;

namespace Data.Transaction
{
    public class DBTransaction : IDBTransaction, IDisposable
    {
        private IDbContextTransaction _transaction;
        private ProjectContext _context;
        public DBTransaction(ProjectContext context)
        {
            _context = context;
        }
        public void BeginTransaction()
        {
            _transaction = _context.Database.BeginTransaction();
        }

        public void Commit()
        {
            _transaction.Commit();
        }

        public void Rollback()
        {
            _transaction.Rollback();
        }

        public void Dispose()
        {
            if (_transaction != null)
                _transaction.Dispose();
            _context.Dispose();
        }
    }
}
