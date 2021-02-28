using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

using Dapper;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

using PlutoData.Collections;
using PlutoData.Extensions;
using PlutoData.Interface;
using PlutoData.Models;

namespace PlutoData
{
    /// <summary>
    /// dapper仓储
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public partial class DapperRepository<TEntity> : IDapperRepository<TEntity> where TEntity : class, new()
    {
        private IDbTransaction? _dbTransaction;

        /// <summary>
        /// 
        /// </summary>
        public IDbConnection DbConnection => DbContext!.GetDbConnection();

        /// <summary>
        /// 事务对象
        /// </summary>
        public IDbTransaction? DbTransaction
        {
            get
            {
                if (_dbTransaction != null)
                {
                    return _dbTransaction;
                }
                if (DependOnEf)
                {
                    if (DbContext?._dbContext == null)
                        throw new InvalidOperationException("未配置efcore 上下文");
                    return DbContext._dbContext.Database?.CurrentTransaction?.GetDbTransaction();
                }
                return DbConnection.BeginTransaction();
            }
            set
            {
                if (DependOnEf)
                    throw new InvalidOperationException("由efcore托管，无法更改");
                _dbTransaction = value;
            }
        }

        /// <summary>
		/// dapper 上下文
		/// IdbConnection和IDbTranstration
		/// </summary>
		public DapperDbContext? DbContext { get; set; }


        public DapperRepository(DapperDbContext dapperDb)
        {
            DbContext = dapperDb;
        }



        /// <inheritdoc />
        public string EntityMapName
        {
            get
            {
                if (DependOnEf)
                {
                    if (DbContext?._dbContext == null)
                        throw new InvalidOperationException("未配置efcore 上下文");
                    var entityType = DbContext._dbContext.Model.FindEntityType(typeof(TEntity));
                    return entityType.GetTableName();
                }
                return typeof(TEntity).GetMainTableName();
            }
        }

        /// <summary>
        /// 执行
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        /// <remarks>
        ///	当使用纯dapper时 ，使用此执行T-SQL语句
        /// </remarks>
        protected async Task<TResult> ExecuteAsync<TResult>(Func<IDbConnection, IDbTransaction?, Task<TResult>> func)
        {
            if (DependOnEf)
                return await func(DbConnection, null);
            if (IsInTran)
                return await func(DbConnection, DbTransaction);
            using var conn = DbConnection;
            return await func(conn, null);
        }


        /// <summary>
        /// 执行
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        /// <remarks>
        ///	当使用纯dapper时 ，使用此执行T-SQL语句
        /// </remarks>
        protected TResult Execute<TResult>(Func<IDbConnection, IDbTransaction?, TResult> func)
        {
            if (DependOnEf)
                return func(DbConnection, DbTransaction);
            if (IsInTran)
                return func(DbConnection, DbTransaction);
            using var conn = DbConnection;
            return func(conn, null);
        }


        private bool IsInTran = false;

        /// <inheritdoc />
        public T BeginTransaction<T>(Func<IDbTransaction?, T> func)
        {
            _dbTransaction = null;
            using var conn = DbConnection;
            if (conn.State != ConnectionState.Open)
                conn.Open();
            using (_dbTransaction = DbTransaction)
            {
                IsInTran = true;
                try
                {
                    var res = func(_dbTransaction);
                    _dbTransaction!.Commit();
                    return res;
                }
                finally
                {
                    IsInTran = false;
                }
            }
        }

        private bool DependOnEf => DbContext?._dbContext != null;

    }


    /// <summary>
    /// 
    /// </summary>
    public partial class DapperRepository<TEntity>
    {
        /// <summary>
        /// insert
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int Insert(TEntity entity)
        {
            return Execute((conn, tran) => conn.Insert(entity, tran) ?? 0);
        }

        /// <summary>
        /// insert
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public Task<int?> InsertAsync(TEntity entity)
        {
            return ExecuteAsync((conn, tran) => conn.InsertAsync(entity, tran));
        }

        /// <summary>
        /// update
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int Update(TEntity entity)
        {
            return Execute((conn, tran) => conn.Update(entity, tran));
        }


        /// <summary>
        /// update
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public Task<int> UpdateAsync(TEntity entity)
        {
            return ExecuteAsync((conn, tran) => conn.UpdateAsync(entity, tran));
        }

        /// <summary>
        /// Delete
        /// </summary>
        /// <param name="entity"></param>
        public int Delete(TEntity entity)
        {
            return Execute((conn, tran) => conn.Delete(entity, tran));
        }

        /// <summary>
        /// Delete
        /// </summary>
        /// <param name="entity"></param>
        public Task<int> DeleteAsync(TEntity entity)
        {
            return ExecuteAsync((conn, tran) => conn.DeleteAsync(entity, tran));
        }

        /// <summary>
        /// 查询单个
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public TEntity Get(object key)
        {
            return Execute((conn, tran) => conn.Get<TEntity>(key));
        }

        /// <summary>
        /// 查询单个
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public Task<TEntity> GetAsync(object key)
        {
            return ExecuteAsync((conn, tran) => conn.GetAsync<TEntity>(key));
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TEntity> GetList(AbstractQuerySqlBuild? queryBuild)
        {
            var where = "";
            var orderBy = "";
            if (queryBuild != null)
            {
                where = queryBuild.BuildWhere();
                orderBy = queryBuild.BuildOrderBy();
            }
            return Execute((conn, tran) => conn.GetList<TEntity>($"{where} {orderBy}", queryBuild?.QueryParam));
        }


        /// <summary>
        /// 查询
        /// </summary>
        /// <returns></returns>
        public IPagedList<TEntity> GetPageList(AbstractQuerySqlBuild? queryBuild, int pageNo, int pageSize)
        {
            var where = "";
            var orderBy = "";
            if (queryBuild != null)
            {
                where = queryBuild.BuildWhere();
                orderBy = queryBuild.BuildOrderBy();
            }
            var count = Execute((conn, tran) => conn.RecordCount<TEntity>($"{where}", queryBuild?.QueryParam));
            if (count <= 0)
            {
                return PagedList.Empty<TEntity>();
            }
            var res = Execute((conn, tran) => conn.GetListPaged<TEntity>(pageNo, pageSize, where, orderBy, queryBuild?.QueryParam));
            return res.ToPagedList<TEntity>(pageNo, pageSize, count);
        }


        /// <summary>
        /// 查询
        /// </summary>
        /// <returns></returns>
        public async Task<IPagedList<TEntity>> GetPageListAsync(AbstractQuerySqlBuild? queryBuild, int pageNo, int pageSize)
        {
            var where = "";
            var orderBy = "";
            if (queryBuild != null)
            {
                where = queryBuild.BuildWhere();
                orderBy = queryBuild.BuildOrderBy();
            }
            var count = await ExecuteAsync(async (conn, tran) => await conn.RecordCountAsync<TEntity>($"{where}", queryBuild?.QueryParam));
            if (count <= 0)
            {
                return PagedList.Empty<TEntity>();
            }

            var res = await ExecuteAsync(async (conn, tran) => await conn.GetListPagedAsync<TEntity>(pageNo, pageSize, where, orderBy, queryBuild?.QueryParam));
            return res.ToPagedList<TEntity>(pageNo, pageSize, count);
        }


        /// <summary>
        /// 查询
        /// </summary>
        /// <returns></returns>
        public Task<IEnumerable<TEntity>> GetListAsync(AbstractQuerySqlBuild? queryBuild)
        {
            var where = "";
            var orderBy = "";
            if (queryBuild != null)
            {
                where = queryBuild.BuildWhere();
                orderBy = queryBuild.BuildOrderBy();
            }
            return ExecuteAsync((conn, tran) => conn.GetListAsync<TEntity>($"{where} {orderBy}", queryBuild?.QueryParam));
        }

        /// <summary>
        /// 数量
        /// </summary>
        /// <returns></returns>
        public int RecordCount(AbstractQuerySqlBuild? queryBuild)
        {
            var where = "";
            var orderBy = "";
            if (queryBuild != null)
            {
                where = queryBuild.BuildWhere();
                orderBy = queryBuild.BuildOrderBy();
            }
            return Execute((conn, tran) => conn.RecordCount<TEntity>($"{where} {orderBy}", queryBuild?.QueryParam));
        }

        /// <summary>
        /// 数量
        /// </summary>
        /// <returns></returns>
        public Task<int> RecordCountAsync(AbstractQuerySqlBuild? queryBuild)
        {
            var where = "";
            var orderBy = "";
            if (queryBuild != null)
            {
                where = queryBuild.BuildWhere();
                orderBy = queryBuild.BuildOrderBy();
            }
            return ExecuteAsync((conn, tran) => conn.RecordCountAsync<TEntity>($"{where} {orderBy}", queryBuild?.QueryParam));
        }
    }
}