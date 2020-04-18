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
        /// 切换数据库
        /// <para>待完善</para>
        /// </summary>
        /// <param name="database"></param>
        void ChangeDatabase(string database);

        /// <summary>
        /// 获取仓储
        /// </summary>
        /// <typeparam name="IRepository"></typeparam>
        /// <returns></returns>
        IRepository GetRepository<IRepository>();

        /// <summary>
        /// 返回一个数据库执行策略
        /// </summary>
        /// <returns></returns>
        IExecutionStrategy CreateExecutionStrategy();

        /// <summary>
        /// 保存更改
        /// </summary>
        /// <param name="ensureAutoHistory">请确保自动记录更改历史记录: modelBuilder。EnableAutoHistory（）;</param>
        /// <returns></returns>
        int SaveChanges(bool ensureAutoHistory = false);
        /// <summary>
        /// 保存更改
        /// </summary>
        /// <param name="ensureAutoHistory">请确保自动记录更改历史记录: modelBuilder。EnableAutoHistory（）;</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<int> SaveChangesAsync(bool ensureAutoHistory = false, CancellationToken cancellationToken = default);

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
        /// 触发领域事件的保存更改
        /// </summary>
        /// <param name="dispatchDomainEvent"></param>
        /// <param name="ensureAutoHistory">请确保自动记录更改历史记录: modelBuilder。EnableAutoHistory（）;</param>
        void SaveEntityChanges(Action dispatchDomainEvent = null, bool ensureAutoHistory = false);
        /// <summary>
        /// 触发领域事件的保存更改--异步
        /// </summary>
        /// <param name="dispatchDomainEvent"></param>
        /// <param name="ensureAutoHistory">请确保自动记录更改历史记录: modelBuilder。EnableAutoHistory（）;</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<int> SaveEntityChangesAsync(Action dispatchDomainEvent = null, bool ensureAutoHistory = false, CancellationToken cancellationToken = default);



        /// <summary>
        /// 保存更改。
        /// </summary>
        /// <param name="ensureAutoHistory"><c>True</c> 请确保自动记录更改历史记录: modelBuilder。EnableAutoHistory（）; </param>
        /// <param name="unitOfWorks">An optional <see cref="IUnitOfWork"/> array.</param>
        /// <returns>A <see cref="Task{TResult}"/> 代表异步保存操作。任务结果包含写入数据库的状态实体的数量.</returns>
        Task<int> SaveChangesAsync(bool ensureAutoHistory = false, CancellationToken cancellationToken = default, params IUnitOfWork<TContext>[] unitOfWorks);


        Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);

        Task CommitTransactionAsync(IDbContextTransaction transaction, CancellationToken cancellationToken = default);
    }
}