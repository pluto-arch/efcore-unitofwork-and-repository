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

        /// <summary>
        /// 获取数据库上下文
        /// </summary>
        /// <returns>The instance of type <typeparamref name="TContext"/>.</returns>
        public TContext DbContext => _context;


        /// <summary>
        /// 是否有活动的事务对象
        /// </summary>
        public bool HasActiveTransaction
        {
            get { return _currentTransaction != null; }
        }

        /// <summary>
        /// 切换数据库(还不完善)
        /// </summary>
        /// <param name="database"></param>
        public void ChangeDatabase(string database)
        {
            var connection = _context.Database.GetDbConnection();
            if (connection.State.HasFlag(ConnectionState.Open))
            {
                connection.ChangeDatabase(database);
            }
            else
            {
                var connectionString = Regex.Replace(connection.ConnectionString, @"(?<=[Dd]atabase=)\w+(?=;)", database, RegexOptions.Singleline);
                connection.ConnectionString = connectionString;
            }
            // mysql schema 就是数据库
            //var items = _context.Model.GetEntityTypes();
            //foreach (var item in items)
            //{
            //    if (item is IConventionEntityType entityType)
            //    {
            //        entityType.SetSchema(database);
            //    }
            //}
        }


        /// <summary>
        /// 获取仓储(和请求周期一致)
        /// </summary>
        /// <typeparam name="IRepository"></typeparam>
        /// <returns></returns>
        public IRepository GetRepository<IRepository>()
        {
            var repository = _context.GetService<IRepository>();
            return repository;
        }
        
        /// <summary>
        /// 执行策略
        /// </summary>
        /// <returns></returns>
        public IExecutionStrategy CreateExecutionStrategy()
        {
            return _context.Database.CreateExecutionStrategy();
        }


        /// <summary>
        /// 执行sql
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public int ExecuteSqlCommand(string sql, params object[] parameters) => _context.Database.ExecuteSqlRaw(sql, parameters);


        /// <summary>
        /// 执行SQL查询，返回实体，不支持直接返回非实体对象，如果有需要请使用 select 映射
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public IQueryable<TEntity> FromSql<TEntity>(string sql, params object[] parameters) where TEntity : class => _context.Set<TEntity>().FromSqlRaw(sql, parameters);



        #region 触发领域事件的savechange
        public void SaveEntityChanges(Action dispatchDomainEvent = null, bool ensureAutoHistory = false)
        {
            dispatchDomainEvent?.Invoke();
            if (ensureAutoHistory)
            {
                _context.EnsureAutoHistory();
            }
            _context.SaveChanges();
        }
        public async Task<int> SaveEntityChangesAsync(Action dispatchDomainEvent = null, bool ensureAutoHistory = false,CancellationToken cancellationToken=default)
        {
            dispatchDomainEvent?.Invoke();
            if (ensureAutoHistory)
            {
                _context.EnsureAutoHistory();
            }
            return await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<int> SaveChangesAsync(bool ensureAutoHistory = false, CancellationToken cancellationToken = default, params IUnitOfWork<TContext>[] unitOfWorks)
        {
            using (var ts = new TransactionScope())
            {
                var count = 0;
                foreach (var unitOfWork in unitOfWorks)
                {
                    count += await unitOfWork.SaveChangesAsync(ensureAutoHistory, cancellationToken);
                }

                count += await SaveChangesAsync(ensureAutoHistory, cancellationToken);

                ts.Complete();

                return count;
            }
        }

        #endregion


        /// <summary>
        /// 
        /// </summary>
        /// <param name="ensureAutoHistory">是否自动记录数据更改历史记录</param>
        /// <returns></returns>
        public int SaveChanges(bool ensureAutoHistory = false)
        {
            if (ensureAutoHistory)
            {
                _context.EnsureAutoHistory();
            }

            return _context.SaveChanges();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ensureAutoHistory">是否自动记录数据更改历史记录</param>
        /// <returns></returns>
        public async Task<int> SaveChangesAsync(bool ensureAutoHistory = false,CancellationToken cancellationToken=default)
        {
            if (ensureAutoHistory)
            {
                _context.EnsureAutoHistory();
            }
            return await _context.SaveChangesAsync(cancellationToken);
        }


        /// <summary>
        /// 更改追踪
        /// </summary>
        /// <param name="rootEntity"></param>
        /// <param name="callback"></param>
        public void TrackGraph(object rootEntity, Action<EntityEntryGraphNode> callback)
        {
            _context.ChangeTracker.TrackGraph(rootEntity, callback);
        }

        /// <summary>
        /// 开始事务--异步
        /// </summary>
        /// <returns></returns>
        public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_currentTransaction != null) return null;
            _currentTransaction = await DbContext.Database.BeginTransactionAsync(System.Data.IsolationLevel.ReadCommitted, cancellationToken);
            return _currentTransaction;
        }

        /// <summary>
        /// 提交事务--异步
        /// </summary>
        /// <param name="transaction"></param>
        /// <returns></returns>
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