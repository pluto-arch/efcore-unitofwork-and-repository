using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Query;
using PlutoData.Collections;

namespace PlutoData.Interface
{

    /// <summary>
    /// 
    /// </summary>
    public interface IRepository
    {
        /// <summary>
        /// dbcontext
        /// </summary>
        /// <remarks>
        /// 只能由unitofwork初始化
        /// </remarks>
        DbContext DbContext { get; set; }

    }

    /// <summary>
    /// 泛型仓储接口
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IRepository<TEntity>: IRepository where TEntity : class
    {
        /// <summary>
        /// 实体映射的表名
        /// </summary>
        string EntityMapName { get; }


        /// <summary>
        /// 单个scope内设置不追踪
        /// </summary>
        void SetNoTracking();
        

        /// <summary>
        /// 获取分页数据 <see cref="IPagedList{T}"/> 。默认无追踪
        /// </summary>
        /// <param name="predicate">查询条件</param>
        /// <param name="orderBy">排序表达式</param>
        /// <param name="include">导航属性表达式</param>
        /// <param name="pageIndex">默认1</param>
        /// <param name="pageSize">默认20</param>
        /// <param name="disableTracking">是否关闭追踪 Default to <c>true</c>.</param>
        /// <param name="ignoreQueryFilters">忽略查询过滤器</param>
        /// <returns>返回值: <see cref="IPagedList{TEntity}"/> </returns>
        IPagedList<TEntity> GetPagedList(Expression<Func<TEntity, bool>> predicate = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
            int pageIndex = 1,
            int pageSize = 20,
            bool disableTracking = true,
            bool ignoreQueryFilters = false);


        /// <summary>
        /// 获取分页数据 <see cref="IPagedList{TEntity}"/>
        /// </summary>
        /// <param name="predicate">查询条件</param>
        /// <param name="orderBy">排序表达式</param>
        /// <param name="include">导航属性表达式</param>
        /// <param name="pageIndex">默认1</param>
        /// <param name="pageSize">默认20</param>
        /// <param name="disableTracking">是否关闭追踪. Default to <c>true</c>.</param>
        /// <param name="cancellationToken">
        ///     A <see cref="CancellationToken" /> to observe while waiting for the task to complete.
        /// </param>
        /// <param name="ignoreQueryFilters">Ignore query filters</param>
        /// <returns>返回： <see cref="IPagedList{TEntity}"/></returns>
        /// <remarks>默认不追踪.</remarks>
        Task<IPagedList<TEntity>> GetPagedListAsync(Expression<Func<TEntity, bool>> predicate = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
            int pageIndex = 1,
            int pageSize = 20,
            bool disableTracking = true,
            CancellationToken cancellationToken = default(CancellationToken),
            bool ignoreQueryFilters = false);


        /// <summary>
        /// 获取分页数据  the <see cref="IPagedList{TResult}"/> 
        /// </summary>
        /// <param name="selector">查询映射表达式</param>
        /// <param name="predicate">条件表达式</param>
        /// <param name="orderBy">排序表达式</param>
        /// <param name="include">导航属性</param>
        /// <param name="pageIndex">默认1</param>
        /// <param name="pageSize">默认20</param>
        /// <param name="disableTracking">默认true <c>true</c>.</param>
        /// <param name="ignoreQueryFilters">Ignore query filters</param>
        /// <returns>返回<see cref="IPagedList{TResult}"/> </returns>
        IPagedList<TResult> GetPagedList<TResult>(Expression<Func<TEntity, TResult>> selector,
            Expression<Func<TEntity, bool>> predicate = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
            int pageIndex = 1,
            int pageSize = 20,
            bool disableTracking = true,
            bool ignoreQueryFilters = false) where TResult : class;


        /// <summary>
        /// 获取分页 <see cref="IPagedList{TEntity}"/> 
        /// </summary>
        /// <param name="selector">查询映射表达式</param>
        /// <param name="predicate">条件表达式</param>
        /// <param name="orderBy">排序表达式</param>
        /// <param name="include">导航属性</param>
        /// <param name="pageIndex">默认1</param>
        /// <param name="pageSize">默认20</param>
        /// <param name="disableTracking">默认true <c>true</c>.</param>
        /// <param name="cancellationToken">
        ///     A <see cref="CancellationToken" /> to observe while waiting for the task to complete.
        /// </param>
        /// <param name="ignoreQueryFilters">Ignore query filters</param>
        /// <returns>返回<see cref="IPagedList{TResult}"/> </returns>
        Task<IPagedList<TResult>> GetPagedListAsync<TResult>(Expression<Func<TEntity, TResult>> selector,
                                                             Expression<Func<TEntity, bool>> predicate = null,
                                                             Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
                                                             Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
                                                             int pageIndex = 1,
                                                             int pageSize = 20,
                                                             bool disableTracking = true,
                                                             CancellationToken cancellationToken = default(CancellationToken),
                                                             bool ignoreQueryFilters = false) where TResult : class;


        /// <summary>
        /// 获取第一个或默认实体。此方法默认为只读的无跟踪查询。
        /// </summary>
        /// <param name="predicate">条件表达式</param>
        /// <param name="orderBy">排序表达式</param>
        /// <param name="include">导航属性</param>
        /// <param name="disableTracking">默认true <c>true</c>.</param>
        /// <param name="ignoreQueryFilters">Ignore query filters</param>
        /// <returns>一个 <see cref="TEntity"/></returns>
        TEntity GetFirstOrDefault(Expression<Func<TEntity, bool>> predicate = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
            bool disableTracking = true,
            bool ignoreQueryFilters = false);



        /// <summary>
        /// 获取第一个或默认实体。此方法默认为只读的无跟踪查询
        /// </summary>
        /// <param name="selector">映射表达式</param>
        /// <param name="predicate">条件表达式</param>
        /// <param name="orderBy">排序表达式</param>
        /// <param name="include">导航属性</param>
        /// <param name="disableTracking">默认true <c>true</c>.</param>
        /// <param name="ignoreQueryFilters">Ignore query filters</param>
        /// <returns>一个 <see cref="TEntity"/></returns>
        TResult GetFirstOrDefault<TResult>(Expression<Func<TEntity, TResult>> selector,
            Expression<Func<TEntity, bool>> predicate = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
            bool disableTracking = true,
            bool ignoreQueryFilters = false);


        /// <summary>
        ///  获取第一个或默认实体。此方法默认为只读的无跟踪查询
        /// </summary>
        /// <param name="selector">映射表达式</param>
        /// <param name="predicate">条件表达式</param>
        /// <param name="orderBy">排序表达式</param>
        /// <param name="include">导航属性</param>
        /// <param name="disableTracking">默认true <c>true</c>.</param>
        /// <param name="ignoreQueryFilters">Ignore query filters</param>
        /// <param name="cancellationToken"></param>
        /// <returns>一个 <see cref="TEntity"/></returns>
        Task<TResult> GetFirstOrDefaultAsync<TResult>(Expression<Func<TEntity, TResult>> selector,
            Expression<Func<TEntity, bool>> predicate = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
            bool disableTracking = true,
            bool ignoreQueryFilters = false,
            CancellationToken cancellationToken = default);


        /// <summary>
        ///  获取第一个或默认实体。此方法默认为只读的无跟踪查询
        /// </summary>
        /// <param name="predicate">条件表达式</param>
        /// <param name="orderBy">排序表达式</param>
        /// <param name="include">导航属性</param>
        /// <param name="disableTracking">默认true <c>true</c>.</param>
        /// <param name="ignoreQueryFilters">Ignore query filters</param>
        /// <param name="cancellationToken"></param>
        /// <returns>一个 <see cref="TEntity"/></returns>
        Task<TEntity> GetFirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
            bool disableTracking = true,
            bool ignoreQueryFilters = false,
            CancellationToken cancellationToken = default);



        /// <summary>
        /// 使用原始SQL查询来获取指定的 <typeparamref name="TEntity" /> data.
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters">parameters.</param>
        /// <returns>An <see cref="IQueryable{TEntity}" /> </returns>
        IQueryable<TEntity> FromSql(string sql, params object[] parameters);

        /// <summary>
        /// 查找具有给定主键值的实体。如果找到，则附加到上下文并返回。如果未找到实体，则返回null。
        /// </summary>
        /// <param name="keyValues">要找到的实体的主键值</param>
        /// <returns>找到的实体或为null</returns>
        TEntity Find(params object[] keyValues);


        /// <summary>
        /// Finds an entity with the given primary key values. If found, is attached to the context and returned. If no entity is found, then null is returned.
        /// </summary>
        /// <param name="keyValues">The values of the primary key for the entity to be found.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A <see cref="Task{TEntity}"/> that represents the asynchronous find operation. The task result contains the found entity or null.</returns>
        ValueTask<TEntity> FindAsync(object[] keyValues, CancellationToken cancellationToken = default);

        /// <summary>
        /// 获取所有实体
        /// </summary>
        /// <returns>The <see cref="IQueryable{TEntity}"/>.</returns>
        IQueryable<TEntity> GetAll(bool disableTracking=false);

        /// <summary>
        /// 获取所有实体
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="orderBy"></param>
        /// <param name="include"></param>
        /// <param name="disableTracking"></param>
        /// <param name="ignoreQueryFilters"></param>
        /// <returns></returns>
        IQueryable<TEntity> GetAll(Expression<Func<TEntity, bool>> predicate = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
            bool disableTracking = true,
            bool ignoreQueryFilters = false);

        /// <summary>
        /// 获取所有
        /// </summary>
        /// <returns></returns>
        Task<IList<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);


        /// <summary>
        /// 获取所有
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="orderBy"></param>
        /// <param name="include"></param>
        /// <param name="disableTracking"></param>
        /// <param name="ignoreQueryFilters"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<IList<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
            bool disableTracking = true,
            bool ignoreQueryFilters = false,
            CancellationToken cancellationToken = default);



        /// <summary>
        /// 计数
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        long LongCount(Expression<Func<TEntity, bool>> predicate = null);

        /// <summary>
        /// 计数
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<long> LongCountAsync(Expression<Func<TEntity, bool>> predicate = null, CancellationToken cancellationToken = default);



        /// <summary>
        /// 是否存在
        /// </summary>
        /// <param name="selector"></param>
        /// <returns></returns>
        bool Exists(Expression<Func<TEntity, bool>> selector = null);

        /// <summary>
        /// 是否存在
        /// </summary>
        /// <param name="selector"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> selector = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="entity">The entity to insert.</param>
        TEntity Insert(TEntity entity);

        /// <summary>
        /// 新增多个
        /// </summary>
        /// <param name="entities">The entities to insert.</param>
        void Insert(params TEntity[] entities);

        /// <summary>
        /// 新增多个
        /// </summary>
        /// <param name="entities">The entities to insert.</param>
        void Insert(IEnumerable<TEntity> entities);

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        ValueTask<EntityEntry<TEntity>> InsertAsync(TEntity entity, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// 新增多个
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        Task InsertAsync(params TEntity[] entities);

        /// <summary>
        /// 新增多个
        /// </summary>
        /// <param name="entities"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task InsertAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="entity"></param>
        void Update(TEntity entity);

        /// <summary>
        /// 更新多个
        /// </summary>
        /// <param name="entities"></param>
        void Update(params TEntity[] entities);

        /// <summary>
        /// 更新多个
        /// </summary>
        /// <param name="entities"></param>
        void Update(IEnumerable<TEntity> entities);

        /// <summary>
        /// 主键删除
        /// </summary>
        /// <param name="id"></param>
        void Delete(object id);

        /// <summary>
        /// 删除一个实体
        /// </summary>
        /// <param name="entity"></param>
        void Delete(TEntity entity);

        /// <summary>
        /// 删除多个
        /// </summary>
        /// <param name="entities"></param>
        void Delete(params TEntity[] entities);

        /// <summary>
        /// 删除多个
        /// </summary>
        /// <param name="entities"></param>
        void Delete(IEnumerable<TEntity> entities);

        /// <summary>
        /// 更改实体状态
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="state"></param>
        void ChangeEntityState(TEntity entity, EntityState state);
    }
}