using PlutoData.Interface;
using PlutoData.Interface.Base;
using System;

namespace PlutoData
{
	public interface IDapperUnitOfWork: IDisposable
	{
		/// <summary>
		/// 获取基本的仓储
		/// </summary>
		/// <typeparam name="TEntity"></typeparam>
		/// <remarks>
		/// 获取到的仓储，仅具有IDapperRepository中的操作
		/// </remarks>
		/// <returns></returns>
		IDapperRepository<TEntity> GetBaseRepository<TEntity>() where TEntity : class, new();


		/// <summary>
		/// 获取基本的仓储
		/// </summary>
		/// <remarks>
		/// 获取到的仓储，仅具有IDapperRepository中的操作
		/// </remarks>
		/// <returns></returns>
		TRepository GetRepository<TRepository>() where TRepository : IDapperRepository;
	}
}