﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using PlutoData.Extensions;
using PlutoData.Interface;

namespace PlutoData
{
	/// <summary>
	/// dapper仓储
	/// </summary>
	/// <typeparam name="TEntity"></typeparam>
	public class DapperRepository<TEntity> : IDapperRepository<TEntity> where TEntity : class, new()
    {
        private IDbTransaction _dbTransaction;
		private IDbConnection _dbConnection;


        public IDbConnection GetDbConnection()
        {
            _dbConnection= DbContext.GetDbConnection();
            return _dbConnection;
        }


		/// <summary>
		/// 事务对象
		/// </summary>
		public IDbTransaction DbTransaction
		{
			get
			{
                if (_dbTransaction!=null)
                    return _dbTransaction;
                if (IsShareEfCoreDbContext)
                {
                    if (DbContext._dbContext==null)
                        throw new InvalidOperationException("未配置efcore 上下文");
                    _dbTransaction= DbContext._dbContext.Database?.CurrentTransaction?.GetDbTransaction();
                }
                else
                {
                    if (_dbTransaction==null)
                        _dbTransaction = _dbConnection.BeginTransaction();
                }
                return _dbTransaction;
			}
            set
            {
                if (IsShareEfCoreDbContext)
	                throw new InvalidOperationException("由efcore托管，无法更改");
                _dbTransaction = value;
            }
        }

        /// <summary>
		/// dapper 上下文
		/// IdbConnection和IDbTranstration
		/// </summary>
		public DapperDbContext DbContext{get;set;}

        /// <inheritdoc />
        public string EntityMapName
        {
            get
            {
                if (IsShareEfCoreDbContext)
                {
                    if (DbContext._dbContext==null)
                        throw new InvalidOperationException("未配置efcore 上下文");
                    var entityType = DbContext._dbContext.Model.FindEntityType(typeof(TEntity));
                    return entityType.GetTableName();
                }
                else
                {
                    return typeof(TEntity).GetMainTableName();
                }
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
		protected async Task<TResult> Execute<TResult>(Func<IDbConnection,IDbTransaction, Task<TResult>> func)
		{
			if (IsShareEfCoreDbContext)
				return await func(GetDbConnection(),null);
            if (IsInTran)
                return await func(GetDbConnection(),_dbTransaction);
			using (var conn=GetDbConnection())
			{
				return await func(conn,null);
			}
		}

        private bool IsInTran = false;

		/// <inheritdoc />
		public T BeginTransaction<T>(Func<IDbTransaction, T> func)
		{
			using (var conn=GetDbConnection())
			{
				using (DbTransaction)
                {
                    IsInTran = true;
                    try
                    {
                        var res= func(_dbTransaction);
                        _dbTransaction.Commit();
                        return res;
                    }
                    catch (Exception e)
                    {
                        IsInTran = false;
                        return default;
                    }
				}
			}
		}

		private bool IsShareEfCoreDbContext=>DbContext._dbContext!=null;

	}
}