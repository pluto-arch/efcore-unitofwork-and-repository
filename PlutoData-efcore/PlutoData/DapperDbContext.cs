using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
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

		/// <summary>
		/// 初始化
		/// </summary>
		/// <param name="service"></param>
		/// <param name="connectionString">链接字符串</param>
		public DapperDbContext(IServiceProvider service,string connectionString)
		{
			_service=service;
			// 打开连接
			_dbConnection = SqlClientFactory.Instance.CreateConnection();
			if (_dbConnection != null)
			{
				_dbConnection.ConnectionString = connectionString;
				if (_dbConnection.State != ConnectionState.Open)
					_dbConnection.Open();
			}
		}

		/// <inheritdoc />
		public void Dispose()
		{
			_dbConnection?.Dispose();
		}
	}
}