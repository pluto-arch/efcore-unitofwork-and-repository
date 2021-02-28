using PlutoData.Enums;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace PlutoData.Specifications.Builder
{
    /// <summary>
    /// 
    /// </summary>
    public static class SpecificationBuilderExtensions
    {
        /// <summary>
        /// Where
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="specificationBuilder"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public static ISpecificationBuilder<T> Where<T>(this ISpecificationBuilder<T> specificationBuilder, Expression<Func<T, bool>> criteria)
        {
            ((List<Expression<Func<T, bool>>>)specificationBuilder.Specification.WhereExpressions).Add(criteria);

            return specificationBuilder;
        }

        /// <summary>
        /// OrderBy
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="specificationBuilder"></param>
        /// <param name="orderExpression"></param>
        /// <returns></returns>
        public static IOrderedSpecificationBuilder<T> OrderBy<T>(this ISpecificationBuilder<T> specificationBuilder, Expression<Func<T, object>> orderExpression)
        {
            ((List<(Expression<Func<T, object>> OrderExpression, OrderTypeEnum OrderType)>)specificationBuilder.Specification.OrderExpressions).Add((orderExpression, OrderTypeEnum.OrderBy));

            var orderedSpecificationBuilder = new OrderedSpecificationBuilder<T>(specificationBuilder.Specification);

            return orderedSpecificationBuilder;
        }

        /// <summary>
        /// OrderByDescending
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="specificationBuilder"></param>
        /// <param name="orderExpression"></param>
        /// <returns></returns>
        public static IOrderedSpecificationBuilder<T> OrderByDescending<T>(this ISpecificationBuilder<T> specificationBuilder, Expression<Func<T, object>> orderExpression)
        {
            ((List<(Expression<Func<T, object>> OrderExpression, OrderTypeEnum OrderType)>)specificationBuilder.Specification.OrderExpressions).Add((orderExpression, OrderTypeEnum.OrderByDescending));

            var orderedSpecificationBuilder = new OrderedSpecificationBuilder<T>(specificationBuilder.Specification);

            return orderedSpecificationBuilder;
        }

        /// <summary>
        /// Include
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="specificationBuilder"></param>
        /// <param name="includeExpression"></param>
        /// <returns></returns>
        public static IIncludableSpecificationBuilder<T, TProperty> Include<T, TProperty>(this ISpecificationBuilder<T> specificationBuilder, Expression<Func<T, TProperty>> includeExpression)
        {
            var info = new IncludeExpressionInfo(includeExpression, typeof(T), typeof(TProperty));

            ((List<IncludeExpressionInfo>)specificationBuilder.Specification.IncludeExpressions).Add(info);

            var includeBuilder = new IncludableSpecificationBuilder<T, TProperty>(specificationBuilder.Specification);

            return includeBuilder;
        }

        /// <summary>
        /// Include
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="specificationBuilder"></param>
        /// <param name="includeString"></param>
        /// <returns></returns>
        public static ISpecificationBuilder<T> Include<T>(this ISpecificationBuilder<T> specificationBuilder, string includeString)
        {
            ((List<string>)specificationBuilder.Specification.IncludeStrings).Add(includeString);
            return specificationBuilder;
        }

        /// <summary>
        /// Search
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="specificationBuilder"></param>
        /// <param name="selector"></param>
        /// <param name="searchTerm"></param>
        /// <param name="searchGroup"></param>
        /// <returns></returns>
        public static ISpecificationBuilder<T> Search<T>(this ISpecificationBuilder<T> specificationBuilder, Expression<Func<T, string>> selector, string searchTerm, int searchGroup = 1)
        {
            ((List<(Expression<Func<T, string>> Selector, string SearchTerm, int SearchGroup)>)specificationBuilder.Specification.SearchCriterias).Add((selector, searchTerm, searchGroup));

            return specificationBuilder;
        }

        /// <summary>
        /// Select
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="specificationBuilder"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static ISpecificationBuilder<T, TResult> Select<T, TResult>(this ISpecificationBuilder<T, TResult> specificationBuilder, Expression<Func<T, TResult>> selector)
        {
            specificationBuilder.Specification.Selector = selector;

            return specificationBuilder;
        }
    }
}
