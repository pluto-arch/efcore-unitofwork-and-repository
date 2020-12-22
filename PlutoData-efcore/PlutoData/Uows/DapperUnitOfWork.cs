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
	public class DapperUnitOfWork:IDapperUnitOfWork
	{
		private ConcurrentDictionary<Type, object> repositories;
		private readonly DapperDbContext _context;
		private bool disposed = false;

		/// <summary>
		/// 初始化
		/// </summary>
		/// <param name="context"></param>
		public DapperUnitOfWork(DapperDbContext context)
		{
			_context = context ?? throw new ArgumentNullException(nameof(context));
		}

		/// <inheritdoc />
		public DapperDbContext DapperDbContext =>_context;

		/// <inheritdoc />
		public IDapperRepository<TEntity> GetBaseRepository<TEntity>() where TEntity : class, new()
		{
			if (repositories==null)
			{
				repositories=new ConcurrentDictionary<Type, object>();
			}
			var type = typeof(IDapperRepository<TEntity>);
			if (repositories.ContainsKey(type))
			{
				return (IDapperRepository<TEntity>)repositories[type];
			}
			var repository = _context._service.GetService<IDapperRepository<TEntity>>();
			if (repository == null)
			{
				throw new NullReferenceException($"{typeof(IDapperRepository<TEntity>)} not register");
			}
			repository.SetDbContext(_context);
			repositories[type] = repository;
			return repository;
		}

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
			var repository = _context._service.GetService<TRepository>();
			if (repository == null)
			{
				throw new NullReferenceException($"{typeof(TRepository)} not register");
			}
			repository.SetDbContext(_context);
			repositories[type] = repository;
			return repository;
		}


		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		/// <param name="disposing">The disposing.</param>
		protected virtual void Dispose(bool disposing)
		{
			disposed = true;
		}

	}
}