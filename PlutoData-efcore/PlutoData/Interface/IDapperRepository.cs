using System;
using System.Collections.Generic;
using System.Linq.Expressions;
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


		#region 同步

		/// <summary>
		/// 添加
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="isReturnInentity">是否返回标识</param>
		/// <returns></returns>
		bool Insert(TEntity entity, bool isReturnInentity = false);

		/// <summary>
		/// 插入多个
		/// </summary>
		/// <param name="entities"></param>
		/// <returns></returns>
		bool Insert(params TEntity[] entities);

		/// <summary>
		/// 条件删除
		/// </summary>
		/// <param name="predicate"></param>
		/// <returns></returns>
		bool Delete(Expression<Func<TEntity, bool>> predicate);
		#endregion
		

	}
}