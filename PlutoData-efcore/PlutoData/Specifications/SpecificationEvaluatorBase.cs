using Microsoft.EntityFrameworkCore;

using PlutoData.Collections;
using PlutoData.Enums;
using PlutoData.Specifications.EfCore;
using System;
using System.Linq;

namespace PlutoData.Specifications
{
    /// <summary>
    /// 解析
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SpecificationEvaluatorBase<T> : ISpecificationEvaluator<T> where T : class
    {
        /// <summary>
        /// 获取最终的Queryable对象
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="inputQuery"></param>
        /// <param name="specification"></param>
        /// <returns></returns>
        public IQueryable<TResult> GetQuery<TResult>(IQueryable<T> inputQuery, ISpecification<T, TResult> specification)
        {
            var query = GetQuery(inputQuery, (ISpecification<T>)specification);
            var selectQuery = query.Select(specification.Selector!);
            return selectQuery;
        }

        /// <summary>
        ///  获取最终的Queryable对象
        /// </summary>
        /// <param name="inputQuery"></param>
        /// <param name="specification"></param>
        /// <returns></returns>
        public IQueryable<T> GetQuery(IQueryable<T> inputQuery, ISpecification<T> specification)
        {
            var query = inputQuery;
            foreach (var criteria in specification.WhereExpressions)
            {
                query = query.Where(criteria);
            }

            foreach (var searchCriteria in specification.SearchCriterias.GroupBy(x => x.searchGroup))
            {
                var criterias = searchCriteria.Select(x => (x.selector, x.searchTerm));
                query = query.Search(criterias);
            }

            foreach (var includeString in specification.IncludeStrings)
            {
                query = query.Include(includeString);
            }

            foreach (var includeInfo in specification.IncludeExpressions)
            {
                if (includeInfo.Type == IncludeTypeEnum.Include)
                {
                    query = query.Include(includeInfo);
                }
                else if (includeInfo.Type == IncludeTypeEnum.ThenInclude)
                {
                    query = query.ThenInclude(includeInfo);
                }
            }

            // Need to check for null if <Nullable> is enabled.
            if (specification.OrderExpressions != null) 
            {
                if (specification.OrderExpressions.Where(x => x.OrderType is OrderByTypeEnum.OrderBy or OrderByTypeEnum.OrderByDescending).Count() > 1)
                {
                    throw new Exception();
                }
                IOrderedQueryable<T>? orderedQuery = null;
                foreach (var (keySelector, orderType) in specification.OrderExpressions)
                {
                    switch (orderType)
                    {
                        case OrderByTypeEnum.OrderBy:
                            orderedQuery = query.OrderBy(keySelector);
                            break;
                        case OrderByTypeEnum.OrderByDescending:
                            orderedQuery = query.OrderByDescending(keySelector);
                            break;
                        case OrderByTypeEnum.ThenBy:
                            orderedQuery = orderedQuery!.ThenBy(keySelector);
                            break;
                        case OrderByTypeEnum.ThenByDescending:
                            orderedQuery = orderedQuery!.ThenByDescending(keySelector);
                            break;
                    }
                }

                if (orderedQuery != null)
                {
                    query = orderedQuery;
                }
            }

            if (specification.AsNoTracking)
            {
                query = query.AsNoTracking();
            }
            return query;
        }
    }
}
