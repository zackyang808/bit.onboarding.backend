using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace bit.common.Contracts
{
    public interface IRepository<T> where T : IMongoCommon
    {
        T Create();
        IEnumerable<T> Get(Expression<Func<T, bool>> predicate);
        T GetByIdAsync(object id);
        Task Add(T entity);
        Task Update(T entity);
        Task Delete(T entity);
        IQueryable<T> Collection { get; }
        IQueryable<T> GetQueryable(bool includeDeleted = false);
    }
}
