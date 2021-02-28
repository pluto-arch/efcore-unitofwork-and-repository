using PlutoData.Enums;
using PlutoData.Specifications.Builder;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
namespace PlutoData.Specifications
{
    public class Specification<T, TResult> : Specification<T>, ISpecification<T, TResult>
    {
        protected new virtual ISpecificationBuilder<T, TResult> Query { get; }

        protected Specification() : base()
        {
            Query = new SpecificationBuilder<T, TResult>(this);
        }

        public Expression<Func<T, TResult>>? Selector { get; internal set; }
    }

    public abstract class Specification<T> : ISpecification<T>
    {
        protected virtual ISpecificationBuilder<T> Query { get; }

        protected Specification() => Query = new SpecificationBuilder<T>(this);

        public IEnumerable<Expression<Func<T, bool>>> WhereExpressions { get; } = new List<Expression<Func<T, bool>>>();

        public IEnumerable<(Expression<Func<T, object>> KeySelector, OrderTypeEnum OrderType)> OrderExpressions { get; } = new List<(Expression<Func<T, object>> KeySelector, OrderTypeEnum OrderType)>();

        public IEnumerable<IncludeExpressionInfo> IncludeExpressions { get; } = new List<IncludeExpressionInfo>();

        public IEnumerable<string> IncludeStrings { get; } = new List<string>();

        public IEnumerable<(Expression<Func<T, string>> selector, string searchTerm, int searchGroup)> SearchCriterias { get; } = new List<(Expression<Func<T, string>> Selector, string SearchTerm, int SearchGroup)>();

        /// <inheritdoc/>
        public string? CacheKey { get; internal set; }

        /// <inheritdoc/>
        public bool CacheEnabled { get; internal set; }

        /// <inheritdoc/>
        public bool AsNoTracking { get; internal set; } = false;

    }
}
#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释