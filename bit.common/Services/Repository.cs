using bit.common.Contracts;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace bit.common.Services
{
    public class Repository<T> : IRepository<T> where T : IMongoCommon, new()
    {
        private readonly IMongoDatabase _database;
        private IMongoCollection<T> _collection => _database.GetCollection<T>(typeof(T).Name);

        public Repository(IMongoDatabase database)
        {
            _database = database;
        }

        public IQueryable<T> Collection => _collection.AsQueryable();

        public async Task Add(T entity)
        {
            try
            {
                if (entity == null)
                {
                    throw new ArgumentNullException("entity");
                }
                await _collection.InsertOneAsync(entity);
            }
            catch (Exception dbEx)
            {
                throw dbEx;
            }
        }

        public T Create()
        {
            var entity = new T();
            return entity;
        }

        public async Task Delete(T entity)
        {
            try
            {
                if (entity == null)
                {
                    throw new ArgumentNullException("entity");
                }

                await _collection.DeleteOneAsync(w => w.Id.Equals(entity.Id));
            }
            catch (Exception dbEx)
            {
                throw dbEx;
            }

        }

        public IEnumerable<T> Get(Expression<Func<T, bool>> predicate)
        {
            return _collection.AsQueryable().Where(predicate).AsEnumerable();
        }

        public IQueryable<T> GetQueryable(bool includeDeleted = false) => _collection.AsQueryable();

        public async Task Update(T entity)
        {
            try
            {
                if (entity == null)
                {
                    throw new ArgumentNullException("entity");
                }
                await _collection.ReplaceOneAsync(w => w.Id.Equals(entity.Id),
                    entity, new UpdateOptions { IsUpsert = true });
            }
            catch (Exception dbEx)
            {
                throw dbEx;
            }
        }

        public T GetByIdAsync(object id)
        {
            var foundById = _collection.Find(x => x.Id == id.ToString()).FirstOrDefault();
            return foundById;
        }
    }
}
