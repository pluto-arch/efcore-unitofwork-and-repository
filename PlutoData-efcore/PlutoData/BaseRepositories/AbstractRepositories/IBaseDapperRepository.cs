using System;
using System.Data;
using System.Threading.Tasks;

namespace PlutoData
{

    /// <summary>
    /// dapper base repository
    /// </summary>
    public interface IDapperRepository
    {
        /// <summary>
        /// 使用第三方事务
        /// </summary>
        /// <param name="tran"></param>
        /// <returns></returns>
        IDisposable UseTransaction(IDbTransaction tran);
    }

    /// <summary>
    /// dapper base repository
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IBaseDapperRepository<TEntity> : IDapperRepository where TEntity : class,new()
    {

        /// <summary>
        /// 是否有活动的事务
        /// </summary>
        bool HasActiveTransaction { get; }

        #region 普通执行
        /// <summary>
        /// 普通执行
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="execSqlFunc"></param>
        /// <returns></returns>
        TResult Execute<TResult>(Func<IDbConnection, TResult> execSqlFunc);


        /// <summary>
        /// 普通执行
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="execSqlFunc"></param>
        /// <returns></returns>
        Task<TResult> ExecuteAsync<TResult>(Func<IDbConnection, Task<TResult>> execSqlFunc);


        /// <summary>
        /// 普通执行
        /// </summary>
        /// <param name="execSqlFunc"></param>
        void Execute(Action<IDbConnection> execSqlFunc);

        /// <summary>
        /// 普通执行
        /// </summary>
        /// <param name="execSqlFunc"></param>
        /// <returns></returns>
        Task ExecuteAsync(Func<IDbConnection, Task> execSqlFunc);
        #endregion
    }
}
