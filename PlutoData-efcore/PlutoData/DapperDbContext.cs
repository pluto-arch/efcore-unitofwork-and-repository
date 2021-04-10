using System;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PlutoData.Enums;
using ConnectionState = System.Data.ConnectionState;
#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
namespace PlutoData
{
    /// <summary>
    /// dapper 上下文
    /// </summary>
    public class DapperDbContext : IDisposable
    {
        internal readonly IServiceProvider _service;
        internal string _connectionString;
        private bool disposedValue;
        private bool _dependOnEf;
        private Type _efDbContextType;
        internal readonly Func<IDbConnection> _dbConnCreateFunc;
        internal readonly EnumDbType _dbType;

        public DapperDbContext(IServiceProvider service,DapperDbContextOption options)
        {
            var option = options??throw new Exception("no DapperDbContext options found");
            if (!option.DependOnEf)
            {
                _connectionString = option.ConnectionString;
                _dbConnCreateFunc = option.DbConnCreateFunc ?? throw new ArgumentNullException(nameof(option.DbConnCreateFunc),$"no DapperDbContext connection Create fun found");
            }
            else
            {
                _dependOnEf = option.DependOnEf;
                _efDbContextType = option.EfDbContextType??throw new ArgumentNullException(nameof(option.EfDbContextType),"ef db context type can not be null");;
            }
            _dbType = option.DbType;
            _service = service;
        }



        #region dispose
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
                    if (!DependOnEf)
                    {
                        this.Close();
                    }
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
        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion


        #region 基础

        internal bool DependOnEf => _dependOnEf;


        public IDbConnection Connection { private set; get; }

        public IDbTransaction Transaction { private set; get; }

        public bool HasActiveTransaction => this.Transaction != null;

        #endregion

        #region 事务操作
        public IDbTransaction BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            if (DependOnEf)
            {
                throw new InvalidOperationException("can not BeginTransaction with entity framework core");
            }
            OpenConnection();
            this.Transaction = this.Connection.BeginTransaction(isolationLevel);
            return this.Transaction;
        }
        #endregion

        #region 事务、链接对象
        /// <summary>
        /// 打开链接
        /// </summary>
        public void OpenConnection()
        {
            if (this.Connection != null)
            {
                if (this.Connection.State != ConnectionState.Open)
                {
                    this.Connection.Open();
                }
            }
            else
            {
                this.Connection = GetConnection();
                if (this.Connection.State != ConnectionState.Open)
                {
                    this.Connection.Open();
                }
            }
        }

        /// <summary>
        /// 获取链接
        /// </summary>
        /// <returns></returns>
        public IDbConnection GetConnection()
        {
            if (this.Transaction != null)
            {
                return this.Transaction.Connection;
            }
            if (DependOnEf)
            {
                var dbContext = _service.GetService(_efDbContextType) as DbContext;
                if (dbContext==null)
                {
                    throw new InvalidOperationException("no ef db context found ！");
                }
                Connection = dbContext.Database.GetDbConnection();
            }
            else
            {
                Connection = this._dbConnCreateFunc();
                Connection.ConnectionString = this._connectionString;
            }
            return Connection;
        }

        public DbContext GetEfDbContext()
        {
            var dbContext = _service.GetService(_efDbContextType) as DbContext;
            return dbContext;
        }


        /// <summary>
        /// 关闭对象，事务中使用
        /// </summary>
        private void Close()
        {
            CloseTransaction();
            CloseConnection(false);
        }

        private void CloseTransaction(bool isRollback = true)
        {
            if (HasActiveTransaction)
            {
                if (isRollback)
                {
                    if (Transaction.Connection != null && Transaction.Connection.State != ConnectionState.Closed)
                    {
                        Transaction.Rollback();
                    }
                }

                Transaction.Dispose();
                Transaction = null;
            }
        }

        private void CloseConnection(bool disposed = true)
        {
            if (this.Connection != null)
            {
                if (this.Connection.State != ConnectionState.Closed)
                {
                    this.Connection.Close();
                }
                if (disposed)
                {
                    this.Connection.Dispose();
                    this.Connection = null;
                }
            }
        }
        #endregion
    }
}