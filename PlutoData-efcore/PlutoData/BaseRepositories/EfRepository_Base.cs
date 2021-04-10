using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace PlutoData
{
    public partial class EfRepository<TContext, TEntity>
    {
        /// <inheritdoc/>
        public virtual async Task<TEntity> InsertAsync(TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default)
        {
            var savedEntity = (await DbSet.AddAsync(entity, cancellationToken)).Entity;
            if (autoSave)
            {
                await _dbContext.SaveChangesAsync(cancellationToken);
            }
            return savedEntity;
        }

        /// <inheritdoc/>
        public virtual async Task InsertAsync(IEnumerable<TEntity> entities, bool autoSave = false, CancellationToken cancellationToken = default)
        {
            await DbSet.AddRangeAsync(entities, cancellationToken);
            if (autoSave)
            {
                await _dbContext.SaveChangesAsync(cancellationToken);
            }
        }

        /// <inheritdoc/>
        public virtual async Task<TEntity> UpdateAsync(TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default)
        {
            var savedEntity = DbSet.Update(entity).Entity;
            if (autoSave)
            {
                await _dbContext.SaveChangesAsync(cancellationToken);
            }
            return savedEntity;
        }

        /// <inheritdoc/>
        public virtual async Task DeleteAsync(TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default)
        {
            DbSet.Remove(entity);

            if (autoSave)
            {
                await _dbContext.SaveChangesAsync(cancellationToken);
            }
        }


        /// <inheritdoc/>
        public virtual async Task<long> GetCountAsync(CancellationToken cancellationToken = default)
        {
            return await DbSet.CountAsync(cancellationToken);
        }

        /// <inheritdoc/>
        public virtual async Task DeleteAsync([NotNull] Expression<Func<TEntity, bool>> predicate, bool autoSave = false, CancellationToken cancellationToken = default)
        {
            var entities = await DbSet.Where(predicate).ToListAsync(cancellationToken);

            foreach (var entity in entities)
            {
                DbSet.Remove(entity);
            }

            if (autoSave)
            {
                await _dbContext.SaveChangesAsync(cancellationToken);
            }
        }


        /// <inheritdoc/>
        public async Task<TEntity> FindAsync(Expression<Func<TEntity, bool>> predicate,  CancellationToken cancellationToken = default)
        {
            return await Query.Where(predicate).SingleOrDefaultAsync(cancellationToken);
        }

        /// <inheritdoc/>
        public virtual async Task<List<TEntity>> GetListAsync( CancellationToken cancellationToken = default)
        {
            return await DbSet.ToListAsync(cancellationToken);
        }


    }
}
