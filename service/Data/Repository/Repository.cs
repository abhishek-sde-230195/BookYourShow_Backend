using Data.DBContext;
using Data.IRepository;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Data.Repository
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        private readonly ProjectContext _context;

        public Repository(ProjectContext context)
        {
            this._context = context;
        }
        public virtual Task<int> Add(TEntity entity)
        {
            this._context.Add<TEntity>(entity);
            return this._context.SaveChangesAsync();
        }
        public virtual Task<int> Delete(TEntity entity)
        {
            this._context.Remove<TEntity>(entity);
            return this._context.SaveChangesAsync();
        }
        public virtual IQueryable<TEntity> Get(System.Linq.Expressions.Expression<Func<TEntity, bool>> predicate)
        {
            IQueryable<TEntity> query = _context.Set<TEntity>().Where(predicate);
            return query;
        }
        public virtual IQueryable<TEntity> GetAll()
        {
            IQueryable<TEntity> query = _context.Set<TEntity>();
            return query;
        }
        public virtual Task<int> Update(TEntity entity)
        {
            _context.Update<TEntity>(entity);
            return _context.SaveChangesAsync();
        }
        public ProjectContext GetContext()
        {
            return this._context;
        }
        public Task<TEntity> ExecuteProcedure(string procedureName, TEntity modal, List<KeyValuePair<string, string>> param)
        {
            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                SqlParameter parameter = null;
                command.CommandText = @"dbo.proceduresd";
                command.CommandType = CommandType.StoredProcedure;
                foreach (var keyval in param)
                {
                    parameter = new SqlParameter(keyval.Key, keyval.Value);
                    command.Parameters.Add(parameter);
                }

                _context.Database.OpenConnection();
                var res = command.ExecuteReader();
                using (IDataReader reader = command.ExecuteReader())
                {
                    var schema = reader.GetSchemaTable();

                    foreach (DataRow row in schema.Rows)
                    {
                        string name = (string)row["ColumnName"];
                        Type type = (Type)row["DataType"];
                    }
                }
            }

            return Task.FromResult(modal);
        }
    }
}
