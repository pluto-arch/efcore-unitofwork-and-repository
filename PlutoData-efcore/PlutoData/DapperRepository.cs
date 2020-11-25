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
		/// <summary>
		/// 链接对象
		/// </summary>
		protected IDbConnection DbConnection
		{
			get
			{
				if (DbContext._dbContext!=null)
				{
					return DbContext._dbContext.Database.GetDbConnection();
				}
				if (DbContext._dbConnection!=null)
				{
					return DbContext._dbConnection;
				}
				throw new InvalidOperationException("初始化DbConnection异常，请检查设置");
			}
		}

		/// <summary>
		/// efcore 共享时使用
		/// </summary>
		protected IDbTransaction DbTransaction
		{
			get
			{
				if (DbContext._dbContext!=null)
				{
					return DbContext._dbContext.Database.CurrentTransaction.GetDbTransaction();
				}
				throw new InvalidOperationException("未配置efcore 上下文");
			}
		}

		/// <summary>
		/// dapper 上下文
		/// IdbConnection和IDbTranstration
		/// </summary>
		public DapperDbContext DbContext{get;set;}

		/// <inheritdoc />
		public string EntityMapName => typeof(TEntity).GetMainTableName();


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
			{
				return await func(DbConnection);
			}
			using (var connection = DbConnection)
			{
				return await func(connection);
			}
		}


		private bool IsShareEfCoreDbContext=>DbContext._dbContext!=null;

	}
}