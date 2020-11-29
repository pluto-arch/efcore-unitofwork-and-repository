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

        IDbConnection DbConnection { get; }


        IDbTransaction DbTransaction { get; }


        bool BeginTransaction(Func<IDbTransaction,bool> func);


        Task<bool> BeginTransactionAsync(Func<IDbTransaction,Task<bool>> func);
    }
}