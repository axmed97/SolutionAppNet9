using Core.Entities;
using System.Linq.Expressions;

namespace Core.DataAccess
{
    public interface IRepositoryBase<TEntity>
        where TEntity : class, IEntity
    {
        void Add(TEntity entity);
        Task AddAsync(TEntity entity);
        void Update(TEntity entity);
        Task UpdateAsync(TEntity entity);
        void Remove(TEntity entity);
        Task RemoveAsync(TEntity entity);
        TEntity Get(Expression<Func<TEntity, bool>> expression, bool tracking = true);
        Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> expression, bool tracking = true);
        TEntity GetById<T>(T id);
        Task<TEntity> GetByIdAsync<T>(T id);
        List<TEntity> GetAll(bool tracking = true, Expression<Func<TEntity, bool>>? expression = null);
        Task<List<TEntity>> GetAllAsync(bool tracking = true, Expression<Func<TEntity, bool>>? expression = null);
    }
}
