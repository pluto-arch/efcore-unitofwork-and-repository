using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;

namespace PlutoData.Interface
{
    public interface IUnitOfWork<TContext> : IDisposable where TContext : DbContext
    {
        /// <summary>
        /// 是否有活动的事务对象
        /// </summary>
        bool HasActiveTransaction { get; }


        /// <summary>
        /// 获取仓储
        /// </summary>
        /// <typeparam name="TRepository"></typeparam>
        /// <remarks>
        /// 获取到的仓储，具有IRepository中的操作，和自定义操作
        /// </remarks> 
        /// <returns></returns>
        TRepository GetRepository<TRepository>() where TRepository :IRepository;


        /// <summary>
        /// 获取基本的仓储
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <remarks>
        /// 获取到的仓储，仅具有IRepository中的操作
        /// </remarks>
        /// <returns></returns>
        IRepository<TEntity> GetBaseRepository<TEntity>() where TEntity : class, new();


        /// <summary>
        /// 返回一个数据库执行策略
        /// </summary>
        /// <returns></returns>
        IExecutionStrategy CreateExecutionStrategy();

        /// <summary>
        /// 保存更改
        /// </summary>
        /// <returns></returns>
        int SaveChanges();
        /// <summary>
        /// 保存更改
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// 执行SQL脚本--返回受影响的行数
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        int ExecuteSqlCommand(string sql, params object[] parameters);

        /// <summary>
        /// SQL查询 返回实体--延迟加载
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        IQueryable<TEntity> FromSql<TEntity>(string sql, params object[] parameters) where TEntity : class;

        /// <summary>
        /// 使用TrakGrap Api附加断开连接的实体
        /// </summary>
        /// <param name="rootEntity"></param>
        /// <param name="callback"></param>
        void TrackGraph(object rootEntity, Action<EntityEntryGraphNode> callback);


        /// <summary>
        /// 获取数据库上下文。
        /// </summary>
        /// <returns>The instance of type <typeparamref name="TContext"/>.</returns>
        TContext DbContext { get; }

        /// <summary>
        /// 获取当前的事务对象
        /// </summary>
        /// <returns></returns>
        IDbContextTransaction GetCurrentTransaction();


        /// <summary>
        /// 保存更改
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <param name="unitOfWorks"></param>
        /// <returns></returns>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default, params IUnitOfWork<TContext>[] unitOfWorks);


        Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);

        Task CommitTransactionAsync(IDbContextTransaction transaction, CancellationToken cancellationToken = default);
    }
}