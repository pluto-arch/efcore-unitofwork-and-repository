using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

using Dapper;

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
		private IDbConnection _dbConnection;
		public DapperDbContext DbContext
		{
			set
			{
				if (_dbConnection != null)
					throw new InvalidOperationException();
				_dbConnection = value._dbConnection;
			}
		}


		/// <inheritdoc />
		public string EntityMapName => typeof(TEntity).GetMainTableName();

		/// <inheritdoc />
		public virtual bool Insert(TEntity entity, bool isReturnInentity = false)
		{
			return InsertAsync(entity, isReturnInentity).Result;
		}

		/// <inheritdoc />
		public virtual bool Insert(params TEntity[] entities)
		{
			return InsertAsync(entities).Result;
		}

		/// <inheritdoc />
		public bool Delete(Expression<Func<TEntity, bool>> predicate)
		{
			throw new NotImplementedException();
		}


		/// <summary>
		/// 添加
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="isReturnInentity"></param>
		/// <returns></returns>
		public virtual async Task<bool> InsertAsync(TEntity entity, bool isReturnInentity)
		{
			if (entity == null)
				return false;
			return await Execute(async (connection) =>
														{
															var tableName = EntityMapName;
															var res = await connection.InsertAsync(tableName, entity, isReturnInentity);
															return res > 0;
														});
		}


		/// <summary>
		/// 插入多个
		/// </summary>
		/// <param name="entities"></param>
		/// <returns></returns>
		public virtual async Task<bool> InsertAsync(params TEntity[] entities)
		{
			if ((entities?.Count() ?? 0) <= 0)
				return false;
			return await Execute(async (connection) =>
			                     {
				                     var tableName = this.EntityMapName;
				                     var result = await connection.InsertAsync(tableName, entities);
				                     return result > 0;
			                     });
		}



		/// <summary>
		/// 执行
		/// </summary>
		/// <typeparam name="TResult"></typeparam>
		/// <param name="func"></param>
		/// <param name="isMaster"></param>
		/// <returns></returns>
		protected async Task<TResult> Execute<TResult>(Func<IDbConnection, Task<TResult>> func)
		{
			using (var connection = _dbConnection)
			{
				return await func(connection);
			}
		}

	}
}