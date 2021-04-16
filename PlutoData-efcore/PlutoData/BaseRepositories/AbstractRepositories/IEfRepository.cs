using PlutoData.Collections;
using PlutoData.Specifications;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace PlutoData
{
    /// <summary>
    /// ef仓储接口
    /// </summary>
    public interface IEfRepository
    {
        DbContext Uow { get; }
    }

    /// <summary>
    /// 泛型ef仓储接口
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IEfRepository<TEntity> : IEfRepository, IQueryable<TEntity> where TEntity : class,new()
    {
        /// <summary>
        /// 实体映射的表名
        /// </summary>
        string EntityMapName { get; }

        /// <summary>
        /// 查询对象
        /// </summary>
        IQueryable<TEntity> Query { get; }

        /// <summary>
        /// Insert an entity
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="autoSave"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<TEntity> InsertAsync(TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default);


        /// <summary>
        /// Insert entities
        /// </summary>
        /// <param name="entities"></param>
        /// <param name="autoSave"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task InsertAsync(IEnumerable<TEntity> entities, bool autoSave = false, CancellationToken cancellationToken = default);

        /// <summary>
        /// update an entity
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="autoSave"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<TEntity> UpdateAsync(TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default);

        /// <summary>
        /// delete an entity
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="autoSave"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task DeleteAsync(TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default);

        /// <summary>
        /// delete entities by predicate
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="autoSave"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task DeleteAsync([NotNull] Expression<Func<TEntity, bool>> predicate, bool autoSave = false, CancellationToken cancellationToken = default);

        /// <summary>
        /// entity count
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<long> GetCountAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// find an entity
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<TEntity> FindAsync( Expression<Func<TEntity, bool>> predicate,CancellationToken cancellationToken = default);

        /// <summary>
        /// get list
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<List<TEntity>> GetListAsync(CancellationToken cancellationToken = default);

        #region Specification
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="specification"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<List<TEntity>> GetListAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default);
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="specification"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<List<TResult>> GetListAsync<TResult>(ISpecification<TEntity, TResult> specification, CancellationToken cancellationToken = default);

        /// <summary>
        /// 分页
        /// </summary>
        /// <param name="specification"></param>
        /// <param name="pageNo"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        IPagedList<TEntity> GetPageList(ISpecification<TEntity> specification, int pageNo, int pageSize);

        /// <summary>
        /// 分页
        /// </summary>
        /// <param name="specification"></param>
        /// <param name="pageNo"></param>
        /// <param name="pageSize"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<IPagedList<TEntity>> GetPageListAsync(ISpecification<TEntity> specification, int pageNo, int pageSize, CancellationToken cancellationToken = default);

        /// <summary>
        /// 分页
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="specification"></param>
        /// <param name="pageNo"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        IPagedList<TResult> GetPageList<TResult>(ISpecification<TEntity, TResult> specification, int pageNo, int pageSize);

        /// <summary>
        /// 分页
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="specification"></param>
        /// <param name="pageNo"></param>
        /// <param name="pageSize"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<IPagedList<TResult>> GetPageListAsync<TResult>(ISpecification<TEntity, TResult> specification, int pageNo, int pageSize, CancellationToken cancellationToken = default);
        #endregion
    }
}