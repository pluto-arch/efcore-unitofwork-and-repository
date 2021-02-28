using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Threading.Tasks;

using PlutoData.Collections;
using PlutoData.Interface.Base;
using PlutoData.Models;

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

		/// <summary>
		/// 链接对象
		/// </summary>
        IDbConnection DbConnection{ get; }

		/// <summary>
		/// 事务对象
		/// </summary>
        IDbTransaction? DbTransaction { get; set;}

		/// <summary>
		/// 事务中执行
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="func"></param>
		/// <returns></returns>
		T BeginTransaction<T>(Func<IDbTransaction?, T> func);


		/// <summary>
		/// 新增
		/// </summary>
		/// <param name="entity"></param>
		/// <returns></returns>
		int Insert(TEntity entity);

		/// <summary>
		/// 新增
		/// </summary>
		/// <param name="entity"></param>
		/// <returns></returns>
		Task<int?> InsertAsync(TEntity entity);

		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="entity"></param>
		/// <returns></returns>
		int Update(TEntity entity);

		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="entity"></param>
		/// <returns></returns>
		Task<int> UpdateAsync(TEntity entity);


		/// <summary>
		/// 删除
		/// </summary>
		/// <param name="entity"></param>
		/// <returns></returns>
		int Delete(TEntity entity);

		/// <summary>
		/// 删除
		/// </summary>
		/// <param name="entity"></param>
		/// <returns></returns>
		Task<int> DeleteAsync(TEntity entity);


		/// <summary>
		/// 查询
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		TEntity Get(object key);


		/// <summary>
		/// 查询
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		Task<TEntity> GetAsync(object key);


		/// <summary>
		/// 查询全部
		/// </summary>
		/// <returns></returns>
		IEnumerable<TEntity> GetList(AbstractQuerySqlBuild sqBuilder);

		/// <summary>
		/// 查询全部
		/// </summary>
		/// <returns></returns>
		Task<IEnumerable<TEntity>> GetListAsync(AbstractQuerySqlBuild queryBuild);

		/// <summary>
		/// 分页
		/// </summary>
		/// <param name="queryBuild"></param>
		/// <param name="pageNo"></param>
		/// <param name="pageSize"></param>
		/// <returns></returns>
		IPagedList<TEntity> GetPageList(AbstractQuerySqlBuild queryBuild, int pageNo, int pageSize);

		/// <summary>
		/// 分页
		/// </summary>
		/// <param name="queryBuild"></param>
		/// <param name="pageNo"></param>
		/// <param name="pageSize"></param>
		/// <returns></returns>
		Task<IPagedList<TEntity>> GetPageListAsync(AbstractQuerySqlBuild queryBuild, int pageNo, int pageSize);
	}
}