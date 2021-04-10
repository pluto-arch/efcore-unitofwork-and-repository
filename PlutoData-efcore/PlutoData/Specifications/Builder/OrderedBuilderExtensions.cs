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
    public static class OrderedBuilderExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="orderedBuilder"></param>
        /// <param name="orderExpression"></param>
        /// <returns></returns>
        public static IOrderedSpecificationBuilder<T> ThenBy<T>(this IOrderedSpecificationBuilder<T> orderedBuilder, Expression<Func<T, object?>> orderExpression)
        {
            ((List<(Expression<Func<T, object?>> OrderExpression, OrderByTypeEnum OrderType)>)orderedBuilder.Specification.OrderExpressions).Add((orderExpression, OrderByTypeEnum.ThenBy));

            return orderedBuilder;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="orderedBuilder"></param>
        /// <param name="orderExpression"></param>
        /// <returns></returns>
        public static IOrderedSpecificationBuilder<T> ThenByDescending<T>(this IOrderedSpecificationBuilder<T> orderedBuilder, Expression<Func<T, object?>> orderExpression)
        {
            ((List<(Expression<Func<T, object?>> OrderExpression, OrderByTypeEnum OrderType)>)orderedBuilder.Specification.OrderExpressions).Add((orderExpression, OrderByTypeEnum.ThenByDescending));

            return orderedBuilder;
        }
    }
}
