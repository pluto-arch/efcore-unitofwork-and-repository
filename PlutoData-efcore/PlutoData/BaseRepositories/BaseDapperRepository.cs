using System;
using System.Collections.Concurrent;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Dapper;
using Microsoft.EntityFrameworkCore;
using PlutoData.Enums;
using PlutoData.Extensions;
#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释

namespace PlutoData
{

    public partial class BaseDapperRepository<TDapperDbContext, TEntity> : IBaseDapperRepository<TEntity>
     where TEntity : class,new()
     where TDapperDbContext : DapperDbContext
    {

        public BaseDapperRepository(TDapperDbContext dapperDb)
        {
            DbContext = dapperDb ?? throw new ArgumentNullException(nameof(dapperDb));
        }

        private readonly ConcurrentDictionary<Type, string> cacheTableName = new();
        private readonly object getTableNameLock = new ();

        public IDbConnection Connection { private set; get; }

        public IDbTransaction Transaction { private set; get; }

        public TDapperDbContext DbContext { get; set; }

        private bool DependOnEf => DbContext.DependOnEf;

        public bool HasActiveTransaction => this.Transaction != null;

        /// <summary>
        /// 实体对应表名称
        /// </summary>
        public string EntityMapName
        {
            get
            {
                if (DependOnEf)
                {
                    if (DependOnEf == null)
                        throw new InvalidOperationException("no entity frameworkcore dbcontext found!");
                    var ef = DbContext.GetEfDbContext();
                    var entityType = ef.Model.FindEntityType(typeof(TEntity));
                    return entityType.GetTableName();
                }

                if (cacheTableName.ContainsKey(typeof(TEntity)))
                {
                    return cacheTableName[typeof(TEntity)];
                }
                lock (getTableNameLock)
                {
                    var name = typeof(TEntity).GetMainTableName();
                    cacheTableName.AddOrUpdate(typeof(TEntity), t => name, (t, n) => name);
                    return name;
                }
            }
        }


        #region 普通执行

        public TResult Execute<TResult>(Func<IDbConnection, TResult> execSqlFunc)
        {
            try
            {
                OpenConnection();
                var result = execSqlFunc(this.Connection);
                return result;
            }
            finally
            {
                if (!HasActiveTransaction&&DependOnEf)
                {
                    this.Close();
                }
            }
        }
        public async Task<TResult> ExecuteAsync<TResult>(Func<IDbConnection, Task<TResult>> execSqlFunc)
        {
            try
            {
                OpenConnection();
                return await execSqlFunc(this.Connection);
            }
            finally
            {
                if (!HasActiveTransaction&&DependOnEf)
                {
                    this.Close();
                }
            }
        }

        public void Execute(Action<IDbConnection> execSqlFunc)
        {
            try
            {
                OpenConnection();
                execSqlFunc(this.Connection);
            }
            finally
            {
                if (!HasActiveTransaction&&DependOnEf)
                {
                    this.Close();
                }
            }
        }

        public async Task ExecuteAsync(Func<IDbConnection, Task> execSqlFunc)
        {
            try
            {
                OpenConnection();
                await execSqlFunc(this.Connection);
            }
            finally
            {
                if (!HasActiveTransaction&&DependOnEf)
                {
                    this.Close();
                }
            }
        }

        #endregion


        public IDisposable UseTransaction(IDbTransaction tran)
        {
            this.Transaction = tran;
            this.Connection = tran.Connection;
            return new DisposeAction(() =>
            {
                this.Transaction = null;
                this.Connection = null;
            });
        }
        
        #region private

        private void SetDbType()
        {
            switch (DbContext._dbType)
            {
                case EnumDbType.SQLServer:
                    SimpleCRUD.SetDialect(SimpleCRUD.Dialect.SQLServer);
                    break;
                case EnumDbType.PostgreSQL:
                    SimpleCRUD.SetDialect(SimpleCRUD.Dialect.PostgreSQL);
                    break;
                case EnumDbType.SQLite:
                    SimpleCRUD.SetDialect(SimpleCRUD.Dialect.SQLite);
                    break;
                case EnumDbType.MySQL:
                    SimpleCRUD.SetDialect(SimpleCRUD.Dialect.MySQL);
                    break;
                case EnumDbType.Oracle:
                    SimpleCRUD.SetDialect(SimpleCRUD.Dialect.Oracle);
                    break;
                case EnumDbType.DB2:
                    SimpleCRUD.SetDialect(SimpleCRUD.Dialect.DB2);
                    break;
                default:
                    throw new InvalidOperationException("invalid db type,");
            }
        }

        private void OpenConnection()
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
                this.Connection = DbContext.GetConnection();
                if (this.Connection.State != ConnectionState.Open)
                {
                    this.Connection.Open();
                }
            }

            SetDbType();
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
                    this.Connection = null; //这个很重要
                }
            }
        }

        public void Close()
        {
            CloseConnection(false);
        }

        #endregion
    }

    public class DisposeAction : IDisposable
    {
        private readonly Action _action;

        public DisposeAction([NotNull]Action action) => _action = action;

        void IDisposable.Dispose()
        {
            _action();
            GC.SuppressFinalize(this);
        }
    }

}