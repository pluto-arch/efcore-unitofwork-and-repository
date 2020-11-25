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

	}
}