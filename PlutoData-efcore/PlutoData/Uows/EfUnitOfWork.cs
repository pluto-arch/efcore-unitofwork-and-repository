using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using PlutoData.Extensions;
using PlutoData.Interface;
using PlutoData.Interface.Base;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;


namespace PlutoData.Uows
{
	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="TContext"></typeparam>
	public class EfUnitOfWork<TContext> : IEfUnitOfWork<TContext> where TContext : DbContext
    {
        private ConcurrentDictionary<Type, object> repositories;
        private readonly TContext _context;
        private bool disposed = false;

        private IDbContextTransaction _currentTransaction;
        /// <summary>
        /// get Current Transaction
        /// </summary>
        /// <returns></returns>
        public IDbContextTransaction GetCurrentTransaction() => _currentTransaction;


        /// <summary>
        /// 初始化的新实例 <see cref="EfUnitOfWork{TContext}"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public EfUnitOfWork(TContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <inheritdoc />
        public TContext DbContext => _context;


        /// <inheritdoc />
        public bool HasActiveTransaction
        {
            get { return _currentTransaction != null; }
        }



        /// <inheritdoc />
        public TRepository GetRepository<TRepository>() where TRepository : IEfRepository
        {
            if (repositories == null)
            {
                repositories = new ConcurrentDictionary<Type, object>();
            }
            var type = typeof(TRepository);
            if (repositories.ContainsKey(type))
            {
                return (TRepository)repositories[type];
            }
            var repository = _context.GetService<TRepository>();
            if (repository == null)
            {
                throw new NullReferenceException($"{typeof(TRepository)} not register");
            }
            repository.SetDbContext(_context);
            repositories[type] = repository;
            return repository;
        }


        /// <inheritdoc />
        public IEfRepository<TEntity> GetBaseRepository<TEntity>() where TEntity : class, new()
        {
            if (repositories == null)
            {
                repositories = new ConcurrentDictionary<Type, object>();
            }
            var type = typeof(IEfRepository<TEntity>);
            if (repositories.ContainsKey(type))
            {
                return (IEfRepository<TEntity>)repositories[type];
            }
            var repository = _context.GetService<IEfRepository<TEntity>>();
            if (repository == null)
            {
                throw new NullReferenceException($"{typeof(IEfRepository<TEntity>)} not register");
            }
            repository.SetDbContext(_context);
            repositories[type] = repository;
            return repository;
        }

        /// <inheritdoc />
        public IExecutionStrategy CreateExecutionStrategy()
        {
            return _context.Database.CreateExecutionStrategy();
        }


        /// <inheritdoc />
        public int ExecuteSqlCommand(string sql, params object[] parameters) => _context.Database.ExecuteSqlRaw(sql, parameters);


        /// <inheritdoc />
        public IQueryable<TEntity> FromSql<TEntity>(string sql, params object[] parameters) where TEntity : class => _context.Set<TEntity>().FromSqlRaw(sql, parameters);



        /// <inheritdoc />
        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default, params IEfUnitOfWork<TContext>[] unitOfWorks)
        {
            using (var ts = new TransactionScope())
            {
                var count = 0;
                foreach (var unitOfWork in unitOfWorks)
                {
                    count += await unitOfWork.SaveChangesAsync(cancellationToken);
                }

                count += await SaveChangesAsync(cancellationToken);

                ts.Complete();

                return count;
            }
        }


        /// <inheritdoc />
        public int SaveChanges()
        {
            return _context.SaveChanges();
        }

        /// <inheritdoc />
        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }


        /// <inheritdoc />
        public void TrackGraph(object rootEntity, Action<EntityEntryGraphNode> callback)
        {
            _context.ChangeTracker.TrackGraph(rootEntity, callback);
        }

        /// <inheritdoc />
        public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_currentTransaction != null) return null;
            _currentTransaction = await DbContext.Database.BeginTransactionAsync(cancellationToken);
            return _currentTransaction;
        }

        /// <inheritdoc />
        public async Task CommitTransactionAsync(IDbContextTransaction transaction, CancellationToken cancellationToken = default)
        {
            if (transaction == null) throw new ArgumentNullException(nameof(transaction));
            if (transaction != _currentTransaction) throw new InvalidOperationException($"Transaction {transaction.TransactionId} is not current");

            try
            {
                await SaveChangesAsync(cancellationToken: cancellationToken);
                transaction.Commit();
            }
            catch
            {
                RollbackTransaction();
                throw;
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }

        /// <summary>
        /// 回滚
        /// </summary>
        public void RollbackTransaction()
        {
            try
            {
                _currentTransaction?.Rollback();
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }



        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <param name="disposing">The disposing.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    // dispose the db context.
                    _context.Dispose();
                }
            }

            disposed = true;
        }

    }
}