using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Threading.Tasks;
using PlutoData.Interface.Base;

namespace PlutoData.Interface
{
	/// <summary>
	/// dapper 仓储接口
	/// </summary>
	/// <typeparam name="TEntity"></typeparam>
	public interface IDapperRepository<TEntity> : IDapperRepository where TEntity : class
	{
		/// <summary>
		/// 实体映射的表名
		/// </summary>
		string EntityMapName { get; }


        IDbConnection GetDbConnection();

		/// <summary>
		/// 事务对象
		/// </summary>
        IDbTransaction DbTransaction { get;set; }

		/// <summary>
		/// 事务中执行
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="func"></param>
		/// <returns></returns>
		T BeginTransaction<T>(Func<IDbTransaction, T> func);

    }
}