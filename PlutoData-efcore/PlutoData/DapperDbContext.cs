﻿using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using PlutoData.Extensions;
using PlutoData.Interface;

namespace PlutoData
{
	/// <summary>
	/// dapper 上下文
	/// </summary>
	public class DapperDbContext:IDisposable
	{
		/// <summary>
		/// </summary>
		internal readonly IServiceProvider _service;
		internal IDbConnection _dbConnection;
        internal string _connectionString;
		private bool isShareEfDbContext=false;
		internal readonly DbContext _dbContext;

		/// <summary>
		/// 纯dapper
		/// </summary>
		/// <param name="service"></param>
		/// <param name="connectionString">链接字符串</param>
		public DapperDbContext(IServiceProvider service,string connectionString)
		{
			_service=service??throw new ArgumentNullException(nameof(service));
            _connectionString = connectionString??throw new ArgumentNullException(nameof(connectionString));
			var dbConnection = SqlClientFactory.Instance.CreateConnection();
			if (dbConnection != null)
			{
                dbConnection.ConnectionString = connectionString;
			}
            _dbConnection = dbConnection;
        }

		/// <summary>
		/// 和ef共用
		/// </summary>
		public DapperDbContext(IServiceProvider service,DbContext efDbContext)
		{
			if (efDbContext==null)
			{
				throw new ArgumentNullException(nameof(efDbContext));
			}
			_service=service;
			_dbConnection = efDbContext.Database.GetDbConnection();
			_dbContext=efDbContext;
			isShareEfDbContext=true;
		}


		/// <inheritdoc />
		public void Dispose()
		{
			try
			{
				if (isShareEfDbContext)
				{
					_dbContext?.Dispose();
				}else
				{
					_dbConnection?.Dispose();
				}
			}
			catch
			{
				// ignored
			}
		}
	}
}