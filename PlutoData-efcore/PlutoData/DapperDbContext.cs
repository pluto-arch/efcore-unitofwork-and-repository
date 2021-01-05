using System;
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
using ConnectionState = System.Data.ConnectionState;

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
        private bool disposedValue;
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
        }

		/// <summary>
		/// 和ef共用
		/// </summary>
		public DapperDbContext(IServiceProvider service,DbContext efDbContext)
		{
            _service=service;
			_dbContext=efDbContext ?? throw new ArgumentNullException(nameof(efDbContext));
			isShareEfDbContext=true;
		}

        /// <summary>
        /// 获取链接
        /// </summary>
        /// <returns></returns>
        public IDbConnection GetDbConnection()
        {
            if (_dbConnection != null)
            {
                ResetDapperDbConnectionString();
                return _dbConnection;
            }
            if (isShareEfDbContext)
            {
                if (_dbContext==null)
                    throw new InvalidOperationException("无效的EF CORE配置");
                _dbConnection=_dbContext.Database.GetDbConnection();
            }
            else
            {
                var dbConnection = createDapperDbConnection();
                _dbConnection= dbConnection;
            }

            if (_dbConnection.State!=ConnectionState.Open)
                _dbConnection.Open();
            return _dbConnection;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)
                    _dbConnection?.Dispose();
                    _dbConnection = null;
                }

                // TODO: 释放未托管的资源(未托管的对象)并替代终结器
                // TODO: 将大型字段设置为 null
                disposedValue = true;
            }
        }

        // // TODO: 仅当“Dispose(bool disposing)”拥有用于释放未托管资源的代码时才替代终结器
        // ~DapperDbContext()
        // {
        //     // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }


        #region private
        private void ResetDapperDbConnectionString()
        {
            if (_dbConnection.State != ConnectionState.Open && string.IsNullOrEmpty(_dbConnection.ConnectionString))
            {
                _dbConnection.ConnectionString = _connectionString;
            }
        }


        private IDbConnection createDapperDbConnection()
        {
            var dbConnection = SqlClientFactory.Instance.CreateConnection();
            if (dbConnection != null)
            {
                dbConnection.ConnectionString = _connectionString;
            }

            return dbConnection;
        }
        

        #endregion
    }
}