using Microsoft.Extensions.DependencyInjection;
using System;
using System.Data;
#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
namespace PlutoData.Uows
{
	/// <summary>
	/// dapper 单元
	/// </summary>
	public class DapperUnitOfWork<TDapperDbContext> : IDapperUnitOfWork<TDapperDbContext>
		where TDapperDbContext : DapperDbContext
	{
        private bool disposedValue;
        private readonly TDapperDbContext _context;

        public DapperUnitOfWork(TDapperDbContext context)
        {
			_context = context ?? throw new ArgumentNullException(nameof(context));
		}

		/// <inheritdoc />
		public TDapperDbContext DapperDbContext =>_context;

		/// <inheritdoc />
		public TRepository GetRepository<TRepository>() where TRepository : IDapperRepository
		{
            if (_context==null)
            {
				throw new NullReferenceException($"DapperDbContext not register");
			}
			var repository = _context._service.GetService<TRepository>();
			if (repository == null)
			{
				throw new NullReferenceException($"{typeof(TRepository)} not register");
			}
			return repository;
		}

        private IDbTransaction _currentTransaction;

        /// <inheritdoc />
        public IDbTransaction GetCurrentTransaction() => _currentTransaction;

        /// <inheritdoc />
        public IDbTransaction BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            if (_currentTransaction != null)
            {
                return null;
            }
            _currentTransaction = _context.BeginTransaction(isolationLevel);
            return _currentTransaction;
        }

        /// <inheritdoc />
        public void CommitTransaction(IDbTransaction transaction)
        {
            if (transaction == null) throw new ArgumentNullException(nameof(transaction));
            if (_currentTransaction!= transaction) throw new InvalidOperationException($"Transaction is not current");
            try
            {
                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
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


        #region dispose
        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)
                    _currentTransaction?.Dispose();
                    _context?.Dispose();
                }

                // TODO: 释放未托管的资源(未托管的对象)并替代终结器
                // TODO: 将大型字段设置为 null
                disposedValue = true;
            }
        }

        // // TODO: 仅当“Dispose(bool disposing)”拥有用于释放未托管资源的代码时才替代终结器
        // ~DapperUnitOfWork()
        // {
        //     // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
        //     Dispose(disposing: false);
        // }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion

    }
}