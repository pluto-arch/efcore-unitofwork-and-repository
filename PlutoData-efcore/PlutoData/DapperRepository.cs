using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using PlutoData.Extensions;
using PlutoData.Interface;

namespace PlutoData
{
	/// <summary>
	/// dapper仓储
	/// </summary>
	/// <typeparam name="TEntity"></typeparam>
	public class DapperRepository<TEntity> : IDapperRepository<TEntity> where TEntity : class, new()
    {
        private IDbTransaction _dbTransaction;

        /// <summary>
		/// 链接对象
		/// </summary>
        public IDbConnection DbConnection
		{
			get
			{
				if (IsShareEfCoreDbContext)
					return DbContext._dbContext.Database.GetDbConnection();
                if (DbContext._dbConnection != null)
                {
                    DbContext._dbConnection.ConnectionString = DbContext._connectionString;
                    if (DbContext._dbConnection.State!=ConnectionState.Open)
                    {
                            DbContext._dbConnection.Open();
                    }
                    return DbContext._dbConnection;
                }
				throw new InvalidOperationException("初始化DbConnection异常，请检查设置");
			}
		}

		/// <summary>
		/// efcore 共享时使用
		/// </summary>
		public IDbTransaction DbTransaction
		{
			get
			{
                if (IsShareEfCoreDbContext)
                {
                    if (DbContext._dbContext==null)
                        throw new InvalidOperationException("未配置efcore 上下文");
                    return DbContext._dbContext.Database?.CurrentTransaction?.GetDbTransaction();
                }
                else
                {
                    if (_dbTransaction!=null)
                    {
                        return _dbTransaction;
                    }
                    throw new InvalidOperationException("未配置dbTransaction");
                }
			}
            set
            {
                if (IsShareEfCoreDbContext)
                {
                    throw new InvalidOperationException("与efcore 共享，不可修改");
                }
                _dbTransaction = value;
            }
        }

        public bool BeginTransaction(Func<IDbTransaction, bool> func)
        {
            IDbTransaction tr = null;
            if (IsShareEfCoreDbContext)
            {
                throw new InvalidOperationException("与efcore共享，不可使用此方法");
            }
            else if(_dbTransaction==null)
            {
                throw new ArgumentNullException(nameof(_dbTransaction));
            }
            using (tr=_dbTransaction)
            {
                return func.Invoke(tr);
            }
        }

        public Task<bool> BeginTransactionAsync(Func<IDbTransaction, Task<bool>> func)
        {
            IDbTransaction tr = null;
            if (IsShareEfCoreDbContext)
            {
                throw new InvalidOperationException("与efcore共享，不可使用此方法");
            }
            else if(_dbTransaction==null)
            {
                throw new ArgumentNullException(nameof(_dbTransaction));
            }
            using (tr=_dbTransaction)
            {
                return func.Invoke(tr);
            }
        }

        /// <summary>
		/// dapper 上下文
		/// IdbConnection和IDbTranstration
		/// </summary>
		public DapperDbContext DbContext{get;set;}

        /// <inheritdoc />
        public string EntityMapName
        {
            get
            {
                if (IsShareEfCoreDbContext)
                {
                    if (DbContext._dbContext==null)
                        throw new InvalidOperationException("未配置efcore 上下文");
                    var entityType = DbContext._dbContext.Model.FindEntityType(typeof(TEntity));
                    return entityType.GetTableName();
                }
                else
                {
                    return typeof(TEntity).GetMainTableName();
                }
            }
        }

		/// <summary>
		/// 执行
		/// </summary>
		/// <typeparam name="TResult"></typeparam>
		/// <param name="func"></param>
		/// <returns></returns>
		/// <remarks>
		///	当使用纯dapper时 ，使用此执行T-SQL语句
		/// </remarks>
		protected async Task<TResult> Execute<TResult>(Func<IDbConnection, Task<TResult>> func)
		{
			if (IsShareEfCoreDbContext)
				return await func(DbConnection);
			using (var connection = DbConnection)
			{
				return await func(connection);
			}
		}


		private bool IsShareEfCoreDbContext=>DbContext._dbContext!=null;

	}
}