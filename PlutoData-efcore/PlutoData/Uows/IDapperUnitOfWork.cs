using System;
using System.Data;

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
		/// 上下文
		/// </summary>
		TDapperDbContext DapperDbContext {get;}

		/// <summary>
		/// 获取当前事务对象
		/// </summary>
		/// <returns></returns>
		IDbTransaction GetCurrentTransaction();

		/// <summary>
		/// 获取基本的仓储
		/// </summary>
		/// <remarks>
		/// 获取到的仓储，仅具有IDapperRepository中的操作
		/// </remarks>
		/// <returns></returns>
		TRepository GetRepository<TRepository>() where TRepository : IDapperRepository;

		/// <summary>
		/// 开启事务
		/// </summary>
		/// <param name="isolationLevel"></param>
		/// <returns></returns>
		IDbTransaction BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);

		/// <summary>
		/// 提交事务
		/// </summary>
		/// <param name="transaction"></param>
		void CommitTransaction(IDbTransaction transaction);
	}
}