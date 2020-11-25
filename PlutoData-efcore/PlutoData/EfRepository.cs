using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Query;
using PlutoData.Collections;
using PlutoData.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;


namespace PlutoData
{
	/// <summary>
	/// ef core 仓储
	/// </summary>
	/// <typeparam name="TEntity"></typeparam>
	public class EfRepository<TEntity> : IEfRepository<TEntity> where TEntity : class
    {
        protected DbContext _dbContext;
        protected DbSet<TEntity> _dbSet;


        /// <inheritdoc />
        public string EntityMapName {
            get
            {
                var entityType = _dbContext.Model.FindEntityType(typeof(TEntity));
                return entityType.GetTableName();
            }
        }
        
        /// <inheritdoc />
        public DbContext DbContext
        {
            get => _dbContext;
            set
            {
                if (_dbContext!=null)
                    throw new InvalidOperationException();
                _dbContext = value;
                _dbSet = _dbContext.Set<TEntity>();
            }
        }


        /// <inheritdoc />
        public virtual IQueryable<TEntity> GetAll(bool disableTracking=false)
        {
            return disableTracking ? _dbSet.AsNoTracking() : _dbSet;
        }

        /// <inheritdoc />
        public virtual IQueryable<TEntity> GetAll(
            Expression<Func<TEntity, bool>> predicate = null,
            Func<IQueryable<TEntity>,
                IOrderedQueryable<TEntity>> orderBy = null,
            Func<IQueryable<TEntity>,
                IIncludableQueryable<TEntity, object>> include = null,
            bool disableTracking = false,
            bool ignoreQueryFilters = false)
        {
            IQueryable<TEntity> query = _dbSet;

            if (disableTracking)
            {
                query = query.AsNoTracking();
            }

            if (include != null)
            {
                query = include(query);
            }

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            if (ignoreQueryFilters)
            {
                query = query.IgnoreQueryFilters();
            }

            if (orderBy != null)
            {
                return orderBy(query);
            }
            else
            {
                return query;
            }
        }


        /// <summary>
        /// 单个scope内设置不追踪
        /// </summary>
        public virtual void SetNoTracking()=>this._dbSet.AsNoTracking();
        
        
        /// <inheritdoc />
        public virtual IPagedList<TEntity> GetPagedList(Expression<Func<TEntity, bool>> predicate = null,
                                                Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
                                                Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
                                                int pageIndex = 1,
                                                int pageSize = 20,
                                                bool disableTracking = false,
                                                bool ignoreQueryFilters = false)
        {
            IQueryable<TEntity> query = _dbSet;

            if (disableTracking)
            {
                query = query.AsNoTracking();
            }

            if (include != null)
            {
                query = include(query);
            }

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            if (ignoreQueryFilters)
            {
                query = query.IgnoreQueryFilters();
            }

            if (orderBy != null)
            {
                return orderBy(query).ToPagedList(pageIndex, pageSize);
            }
            else
            {
                return query.ToPagedList(pageIndex, pageSize);
            }
        }


        /// <inheritdoc />
        public virtual Task<IPagedList<TEntity>> GetPagedListAsync(Expression<Func<TEntity, bool>> predicate = null,
                                                           Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
                                                           Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
                                                           int pageIndex = 1,
                                                           int pageSize = 20,
                                                           bool disableTracking = false,
                                                           CancellationToken cancellationToken = default(CancellationToken),
                                                           bool ignoreQueryFilters = false)
        {
            IQueryable<TEntity> query = _dbSet;

            if (disableTracking)
            {
                query = query.AsNoTracking();
            }

            if (include != null)
            {
                query = include(query);
            }

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            if (ignoreQueryFilters)
            {
                query = query.IgnoreQueryFilters();
            }

            if (orderBy != null)
            {
                return orderBy(query).ToPagedListAsync(pageIndex, pageSize, cancellationToken);
            }
            else
            {
                return query.ToPagedListAsync(pageIndex, pageSize, cancellationToken);
            }
        }

        /// <inheritdoc />
        public virtual IPagedList<TResult> GetPagedList<TResult>(Expression<Func<TEntity, TResult>> selector,
                                                         Expression<Func<TEntity, bool>> predicate = null,
                                                         Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
                                                         Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
                                                         int pageIndex = 1,
                                                         int pageSize = 20,
                                                         bool disableTracking = false,
                                                         bool ignoreQueryFilters = false)
            where TResult : class
        {
            IQueryable<TEntity> query = _dbSet;

            if (disableTracking)
            {
                query = query.AsNoTracking();
            }

            if (include != null)
            {
                query = include(query);
            }

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            if (ignoreQueryFilters)
            {
                query = query.IgnoreQueryFilters();
            }

            if (orderBy != null)
            {
                return orderBy(query).Select(selector).ToPagedList(pageIndex, pageSize);
            }
            else
            {
                return query.Select(selector).ToPagedList(pageIndex, pageSize);
            }
        }

        /// <inheritdoc />
        public virtual Task<IPagedList<TResult>> GetPagedListAsync<TResult>(Expression<Func<TEntity, TResult>> selector,
                                                                    Expression<Func<TEntity, bool>> predicate = null,
                                                                    Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
                                                                    Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
                                                                    int pageIndex = 1,
                                                                    int pageSize = 20,
                                                                    bool disableTracking = false,
                                                                    CancellationToken cancellationToken = default(CancellationToken),
                                                                    bool ignoreQueryFilters = false)
            where TResult : class
        {
            IQueryable<TEntity> query = _dbSet;

            if (disableTracking)
            {
                query = query.AsNoTracking();
            }

            if (include != null)
            {
                query = include(query);
            }

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            if (ignoreQueryFilters)
            {
                query = query.IgnoreQueryFilters();
            }

            if (orderBy != null)
            {
                return orderBy(query).Select(selector).ToPagedListAsync(pageIndex, pageSize, cancellationToken);
            }
            else
            {
                return query.Select(selector).ToPagedListAsync(pageIndex, pageSize, cancellationToken);
            }
        }

        /// <inheritdoc />
        public virtual TEntity GetFirstOrDefault(Expression<Func<TEntity, bool>> predicate = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
            bool disableTracking = false,
            bool ignoreQueryFilters = false)
        {
            IQueryable<TEntity> query = _dbSet;

            if (disableTracking)
            {
                query = query.AsNoTracking();
            }

            if (include != null)
            {
                query = include(query);
            }

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            if (ignoreQueryFilters)
            {
                query = query.IgnoreQueryFilters();
            }

            if (orderBy != null)
            {
                return orderBy(query).FirstOrDefault();
            }
            else
            {
                return query.FirstOrDefault();
            }
        }


        /// <inheritdoc />
        public virtual async Task<TEntity> GetFirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
            bool disableTracking = false,
            bool ignoreQueryFilters = false,
            CancellationToken cancellationToken = default)
        {
            IQueryable<TEntity> query = _dbSet;

            if (disableTracking)
            {
                query = query.AsNoTracking();
            }

            if (include != null)
            {
                query = include(query);
            }

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            if (ignoreQueryFilters)
            {
                query = query.IgnoreQueryFilters();
            }

            if (orderBy != null)
            {
                return await orderBy(query).FirstOrDefaultAsync();
            }
            else
            {
                return await query.FirstOrDefaultAsync();
            }
        }

        /// <inheritdoc />
        public virtual TResult GetFirstOrDefault<TResult>(Expression<Func<TEntity, TResult>> selector,
            Expression<Func<TEntity, bool>> predicate = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
            bool disableTracking = false,
            bool ignoreQueryFilters = false)
        {
            IQueryable<TEntity> query = _dbSet;

            if (disableTracking)
            {
                query = query.AsNoTracking();
            }

            if (include != null)
            {
                query = include(query);
            }

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            if (ignoreQueryFilters)
            {
                query = query.IgnoreQueryFilters();
            }

            if (orderBy != null)
            {
                return orderBy(query).Select(selector).FirstOrDefault();
            }
            else
            {
                return query.Select(selector).FirstOrDefault();
            }
        }

        /// <inheritdoc />
        public virtual async Task<TResult> GetFirstOrDefaultAsync<TResult>(Expression<Func<TEntity, TResult>> selector,
            Expression<Func<TEntity, bool>> predicate = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
            bool disableTracking = false, bool ignoreQueryFilters = false, CancellationToken cancellationToken = default)
        {
            IQueryable<TEntity> query = _dbSet;

            if (disableTracking)
            {
                query = query.AsNoTracking();
            }

            if (include != null)
            {
                query = include(query);
            }

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            if (ignoreQueryFilters)
            {
                query = query.IgnoreQueryFilters();
            }

            if (orderBy != null)
            {
                return await orderBy(query).Select(selector).FirstOrDefaultAsync(cancellationToken);
            }
            else
            {
                return await query.Select(selector).FirstOrDefaultAsync(cancellationToken);
            }
        }


        /// <inheritdoc />
        public virtual IQueryable<TEntity> FromSql(string sql, params object[] parameters) => _dbSet.FromSqlRaw(sql, parameters);

        /// <inheritdoc />
        public virtual TEntity Find(params object[] keyValues) => _dbSet.Find(keyValues);


        /// <inheritdoc />
        public virtual ValueTask<TEntity> FindAsync(object[] keyValues, CancellationToken cancellationToken = default) => _dbSet.FindAsync(keyValues, cancellationToken);


        /// <inheritdoc />
        public virtual long LongCount(Expression<Func<TEntity, bool>> predicate = null)
        {
            if (predicate == null)
            {
                return _dbSet.LongCount();
            }
            else
            {
                return _dbSet.LongCount(predicate);
            }
        }

        /// <inheritdoc />
        public virtual async Task<long> LongCountAsync(Expression<Func<TEntity, bool>> predicate = null, CancellationToken cancellationToken = default)
        {
            if (predicate == null)
            {
                return await _dbSet.LongCountAsync(cancellationToken);
            }
            else
            {
                return await _dbSet.LongCountAsync(predicate, cancellationToken);
            }
        }



        /// <inheritdoc />
        public virtual bool Exists(Expression<Func<TEntity, bool>> selector = null)
        {
            if (selector == null)
            {
                return _dbSet.Any();
            }
            else
            {
                return _dbSet.Any(selector);
            }
        }

        /// <inheritdoc />
        public virtual async Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> selector = null, CancellationToken cancellationToken = default)
        {
            if (selector == null)
	            return await _dbSet.AnyAsync(cancellationToken);
            else
	            return await _dbSet.AnyAsync(selector, cancellationToken);
        }

        /// <inheritdoc />
        public virtual TEntity Insert(TEntity entity)
        {
            return _dbSet.Add(entity).Entity;
        }

        /// <inheritdoc />
        public virtual void Insert(params TEntity[] entities) => _dbSet.AddRange(entities);

        /// <inheritdoc />
        public virtual void Insert(IEnumerable<TEntity> entities) => _dbSet.AddRange(entities);

        /// <inheritdoc />
        public virtual ValueTask<EntityEntry<TEntity>> InsertAsync(TEntity entity, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _dbSet.AddAsync(entity, cancellationToken);
        }

        /// <inheritdoc />
        public virtual Task InsertAsync(params TEntity[] entities) => _dbSet.AddRangeAsync(entities);

        /// <inheritdoc />
        public virtual Task InsertAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default(CancellationToken)) => _dbSet.AddRangeAsync(entities, cancellationToken);

        /// <inheritdoc />
        public virtual void Update(TEntity entity)
        {
            _dbSet.Update(entity);
        }


        /// <inheritdoc />
        public virtual void Update(params TEntity[] entities) => _dbSet.UpdateRange(entities);

        /// <inheritdoc />
        public virtual void Update(IEnumerable<TEntity> entities) => _dbSet.UpdateRange(entities);

        /// <inheritdoc />
        public virtual void Delete(TEntity entity) => _dbSet.Remove(entity);

        /// <inheritdoc />
        public virtual void Delete(object id)
        {
            // using a stub entity to mark for deletion
            var typeInfo = typeof(TEntity).GetTypeInfo();
            var key = _dbContext.Model.FindEntityType(typeInfo).FindPrimaryKey().Properties.FirstOrDefault();
            var property = typeInfo.GetProperty(key?.Name);
            if (property != null)
            {
                var entity = Activator.CreateInstance<TEntity>();
                property.SetValue(entity, id);
                _dbContext.Entry(entity).State = EntityState.Deleted;
            }
            else
            {
                var entity = _dbSet.Find(id);
                if (entity != null)
                {
                    Delete(entity);
                }
            }
        }

        /// <inheritdoc />
        public virtual void Delete(params TEntity[] entities) => _dbSet.RemoveRange(entities);

        /// <inheritdoc />
        public virtual void Delete(IEnumerable<TEntity> entities) => _dbSet.RemoveRange(entities);

        /// <inheritdoc />
        public virtual async Task<IList<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _dbSet.ToListAsync(cancellationToken);
        }

        /// <inheritdoc />
        public virtual async Task<IList<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
            bool disableTracking = false, bool ignoreQueryFilters = false, CancellationToken cancellationToken = default)
        {
            IQueryable<TEntity> query = _dbSet;

            if (disableTracking)
            {
                query = query.AsNoTracking();
            }

            if (include != null)       
            {
                query = include(query);
            }

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            if (ignoreQueryFilters)
            {
                query = query.IgnoreQueryFilters();
            }

            if (orderBy != null)
            {
                return await orderBy(query).ToListAsync(cancellationToken);
            }
            else
            {
                return await query.ToListAsync(cancellationToken);
            }
        }

        /// <inheritdoc />
        public virtual void ChangeEntityState(TEntity entity, EntityState state)
        {
            _dbContext.Entry(entity).State = state;
        }

    }
}