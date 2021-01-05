using System;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace PlutoData.Models
{
    /// <summary>
    /// 实体查询
    /// </summary>
    public class EntityQueryBase<TEntity> where TEntity:class
    {
        /// <summary>
        ///  关闭追踪
        /// </summary>
        public bool DisableTracking { get; set; } = false;

        /// <summary>
        /// 忽略查询过滤器
        /// </summary>
        public bool IgnoreQueryFilters { get; set; } = false;

        /// <summary>
        /// 查询条件
        /// </summary>
        public Expression<Func<TEntity, bool>> Predicate { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> OrderBy { get; set; }

        /// <summary>
        /// 关联导航属性
        /// </summary>
        public Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> Include { get; set; }

        /// <summary>
        /// 获取查询
        /// </summary>
        /// <returns></returns>
        public virtual IQueryable<TEntity> GetQuery(DbSet<TEntity> init)
        {
            IQueryable<TEntity> query = init;

            if (DisableTracking)
            {
                query = query.AsNoTracking();
            }

            if (Include != null)
            {
                query = Include(query);
            }

            if (Predicate != null)
            {
                query = query.Where(Predicate);
            }

            if (IgnoreQueryFilters)
            {
                query = query.IgnoreQueryFilters();
            }

            if (OrderBy != null)
            {
                return OrderBy(query);
            }
            else
            {
                return query;
            }
        }
    }
}