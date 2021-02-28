using PlutoData.Interface;
using PlutoData.Interface.Base;
using System;

namespace PlutoData
{
	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="TDapperDbContext"></typeparam>
	public interface IDapperUnitOfWork<TDapperDbContext> : IDisposable 
		where TDapperDbContext: DapperDbContext
	{
		/// <summary>
		/// 
		/// </summary>
		TDapperDbContext DapperDbContext {get;}


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