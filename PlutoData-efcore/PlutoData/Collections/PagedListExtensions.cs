using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;


namespace PlutoData.Collections
{
    /// <summary>
    /// IEnumerable 扩展 <see cref="IEnumerable{T}"/>
    /// </summary>
    public static class IEnumerablePagedListExtensions
    {
        /// <summary>
        /// 转分页： <see cref="IPagedList{T}"/> 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="indexFrom"></param>
        /// <returns></returns>
        public static IPagedList<T> ToPagedList<T>(this IEnumerable<T> source, int pageIndex, int pageSize, int indexFrom = 0) => new PagedList<T>(source, pageIndex, pageSize);

        /// <summary>
        /// 转分页： <see cref="IPagedList{TResult}"/> 
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TResult">结果对象</typeparam>
        /// <param name="source"></param>
        /// <param name="converter"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static IPagedList<TResult> ToPagedList<TSource, TResult>(this IEnumerable<TSource> source, Func<IEnumerable<TSource>, IEnumerable<TResult>> converter, int pageIndex, int pageSize) => new PagedList<TSource, TResult>(source, converter, pageIndex, pageSize);
    }


    /// <summary>
    /// IQueryable to IPageList Extensions
    /// </summary>
    public static class IQueryablePageListExtensions
    {
        /// <summary>
        /// 转分页
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task<IPagedList<T>> ToPagedListAsync<T>(this IQueryable<T> source, int pageIndex, int pageSize, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (pageIndex < 1)
            {
                throw new ArgumentException($"页码不能小于1");
            }

            var count = await source.CountAsync(cancellationToken).ConfigureAwait(false);
            var items = await source.Skip((pageIndex - 1) * pageSize)
                .Take(pageSize).ToListAsync(cancellationToken).ConfigureAwait(false);

            var pagedList = new PagedList<T>()
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalCount = count,
                Items = items,
            };

            return pagedList;
        }
    }
}