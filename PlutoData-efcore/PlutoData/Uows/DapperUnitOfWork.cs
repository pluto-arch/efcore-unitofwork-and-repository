using Microsoft.Extensions.DependencyInjection;
using PlutoData.Extensions;
using PlutoData.Interface;
using PlutoData.Interface.Base;
using System;
using System.Collections.Concurrent;
using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace PlutoData.Uows
{
	/// <summary>
	/// dapper 单元
	/// </summary>
	public class DapperUnitOfWork<TDapperDbContext> : IDapperUnitOfWork<TDapperDbContext>
		where TDapperDbContext : DapperDbContext
	{
		private ConcurrentDictionary<Type, object>? repositories;
        private bool disposedValue;
        private readonly TDapperDbContext _context;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="context"></param>
        public DapperUnitOfWork(TDapperDbContext context)
		{
			_context = context ?? throw new ArgumentNullException(nameof(context));
		}

		/// <inheritdoc />
		public TDapperDbContext DapperDbContext =>_context;

		/// <inheritdoc />
		public TRepository GetRepository<TRepository>() where TRepository : IDapperRepository
		{
			if (repositories==null)
			{
				repositories=new ConcurrentDictionary<Type, object>();
			}
			var type = typeof(TRepository);
			if (repositories.ContainsKey(type))
			{
				return (TRepository)repositories[type];
			}
            if (_context==null)
            {
				throw new NullReferenceException($"DapperDbContext not register");
			}
			var repository = _context._service.GetService<TRepository>();
			if (repository == null)
			{
				throw new NullReferenceException($"{typeof(TRepository)} not register");
			}
			repositories[type] = repository;
			return repository;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)
                }

                // TODO: 释放未托管的资源(未托管的对象)并替代终结器
                // TODO: 将大型字段设置为 null
                disposedValue = true;
            }
        }

        // // TODO: 仅当“Dispose(bool disposing)”拥有用于释放未托管资源的代码时才替代终结器
        // ~DapperUnitOfWork()
        // {
        //     // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
        //     Dispose(disposing: false);
        // }

		/// <summary>
		/// 
		/// </summary>
        public void Dispose()
        {
            // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}