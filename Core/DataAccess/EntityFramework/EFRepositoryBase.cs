using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq.Expressions;

namespace Core.DataAccess.EntityFramework
{
    public class EFRepositoryBase<TEntity, TContext> : IRepositoryBase<TEntity>
        where TEntity : class, IEntity
        where TContext : DbContext, new()
    {
        public void Add(TEntity entity)
        {
            using TContext context = new();
            EntityEntry addEntity = context.Entry(entity);
            addEntity.State = EntityState.Added;
            context.SaveChanges();
        }

        public async Task AddAsync(TEntity entity)
        {
            using TContext context = new();
            EntityEntry addEntity = context.Entry(entity);
            addEntity.State = EntityState.Added;
            await context.SaveChangesAsync();
        }

        public void Remove(TEntity entity)
        {
            using TContext context = new();
            EntityEntry deletedEntity = context.Entry(entity);
            deletedEntity.State = EntityState.Deleted;
            context.SaveChanges();
        }

        public async Task RemoveAsync(TEntity entity)
        {
            using TContext context = new();
            EntityEntry deletedEntity = context.Entry(entity);
            deletedEntity.State = EntityState.Deleted;
            await context.SaveChangesAsync();
        }

        public TEntity Get(Expression<Func<TEntity, bool>> expression, bool tracking = true)
        {
            using TContext context = new();
            if (!tracking)
                return context.Set<TEntity>().AsNoTracking().FirstOrDefault(expression);
            return context.Set<TEntity>().FirstOrDefault(expression);

        }

        public List<TEntity> GetAll(bool tracking = true, Expression<Func<TEntity, bool>>? expression = null)
        {
            using TContext context = new();
            if (!tracking)
                return expression == null ? context.Set<TEntity>().AsNoTracking().ToList() :
                    context.Set<TEntity>().AsNoTracking().Where(expression).ToList();
            return expression == null ? context.Set<TEntity>().ToList() :
                context.Set<TEntity>().Where(expression).ToList();
        }

        public async Task<List<TEntity>> GetAllAsync(bool tracking = true, Expression<Func<TEntity, bool>>? expression = null)
        {
            using TContext context = new();
            if (!tracking)
                return expression == null ? await context.Set<TEntity>().AsNoTracking().ToListAsync() :
                    await context.Set<TEntity>().AsNoTracking().Where(expression).ToListAsync();
            return expression == null ? await context.Set<TEntity>().ToListAsync() :
                await context.Set<TEntity>().Where(expression).ToListAsync();
        }

        public async Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> expression, bool tracking = true)
        {
            using TContext context = new();
            if (!tracking)
                return await context.Set<TEntity>().AsNoTracking().FirstOrDefaultAsync(expression);
            return await context.Set<TEntity>().FirstOrDefaultAsync(expression);
        }

        public TEntity GetById<T>(T id)
        {
            using TContext context = new();
            return context.Set<TEntity>().Find(id);
        }
        public async Task<TEntity> GetByIdAsync<T>(T id)
        {
            using TContext context = new();
            return await context.Set<TEntity>().FindAsync(id);
        }

        public void Update(TEntity entity)
        {
            using TContext context = new();
            EntityEntry updatedEntity = context.Entry(entity);
            updatedEntity.State = EntityState.Modified;
            context.SaveChanges();
        }

        public async Task UpdateAsync(TEntity entity)
        {
            using TContext context = new();
            EntityEntry updatedEntity = context.Entry(entity);
            updatedEntity.State = EntityState.Modified;
            await context.SaveChangesAsync();
        }
    }
}
