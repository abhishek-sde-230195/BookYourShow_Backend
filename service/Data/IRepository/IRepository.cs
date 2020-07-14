using Data.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Data.IRepository
{
    public interface IRepository<TEntity>
    {
        IQueryable<TEntity> Get(System.Linq.Expressions.Expression<Func<TEntity, bool>> predicate);
        IQueryable<TEntity> GetAll();
        Task<int> Delete(TEntity entity);
        Task<int> Update(TEntity entity);
        Task<int> Add(TEntity entity);
        ProjectContext GetContext();
        Task<TEntity> ExecuteProcedure(string procName, TEntity modal, List<KeyValuePair<string, string>> param);
    }
}
