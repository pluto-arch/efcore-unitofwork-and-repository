using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage;
using PlutoData.Interface;


namespace PlutoData
{
    public class UnitOfWork<TContext> : IUnitOfWork<TContext> where TContext : DbContext
    {
        private readonly TContext _context;
        private bool disposed = false;

        private IDbContextTransaction _currentTransaction;
        public IDbContextTransaction GetCurrentTransaction() => _currentTransaction;


        /// <summary>
        /// 初始化的新实例 <see cref="UnitOfWork{TContext}"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public UnitOfWork(TContext context)
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
        public TRepository GetRepository<TRepository>() where TRepository: IRepository
        {
            var repository = _context.GetService<TRepository>();
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



        #region 触发领域事件的savechange
        /// <inheritdoc />
        public void SaveEntityChanges(Action dispatchDomainEvent = null)
        {
            dispatchDomainEvent?.Invoke();
            _context.SaveChanges();
        }

        /// <inheritdoc />
        public async Task<int> SaveEntityChangesAsync(Action dispatchDomainEvent = null,CancellationToken cancellationToken=default)
        {
            dispatchDomainEvent?.Invoke();
            return await _context.SaveChangesAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default, params IUnitOfWork<TContext>[] unitOfWorks)
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

        #endregion


        /// <inheritdoc />
        public int SaveChanges()
        {
            return _context.SaveChanges();
        }

        /// <inheritdoc />
        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken=default)
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
        public async Task CommitTransactionAsync(IDbContextTransaction transaction,CancellationToken cancellationToken = default)
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