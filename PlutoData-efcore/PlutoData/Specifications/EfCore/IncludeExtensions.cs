using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace PlutoData.Specifications.EfCore
{
    /// IncludeExtensions
    public static class IncludeExtensions
    {
        /// <summary>
        /// 加载导航属性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        public static IQueryable<T> Include<T>(this IQueryable<T> source, IncludeExpressionInfo info)
        {
            _ = info ?? throw new ArgumentNullException(nameof(info));

            var queryExpr = Expression.Call(
                typeof(EntityFrameworkQueryableExtensions),
                "Include",
                new Type[] {
                    info.EntityType,
                    info.PropertyType
                },
                source.Expression,
                info.LambdaExpression
                );

            return source.Provider.CreateQuery<T>(queryExpr);
        }

        /// <summary>
        /// 加载导航属性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        public static IQueryable<T> ThenInclude<T>(this IQueryable<T> source, IncludeExpressionInfo info)
        {
            _ = info ?? throw new ArgumentNullException(nameof(info));
            _ = info.PreviousPropertyType ?? throw new ArgumentNullException("PreviousPropertyType", nameof(info.PreviousPropertyType));
            var queryExpr = Expression.Call(
                typeof(EntityFrameworkQueryableExtensions),
                "ThenInclude",
                new Type[] {
                    info.EntityType,
                    info.PreviousPropertyType,
                    info.PropertyType
                },
                source.Expression,
                info.LambdaExpression
                );

            return source.Provider.CreateQuery<T>(queryExpr);
        }
    }
}
