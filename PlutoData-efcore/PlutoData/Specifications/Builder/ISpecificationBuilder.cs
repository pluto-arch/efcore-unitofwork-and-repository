using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
namespace PlutoData.Specifications.Builder
{
    public interface ISpecificationBuilder<T, TResult> : ISpecificationBuilder<T>
    {
        new Specification<T, TResult> Specification { get; }
    }

    public interface ISpecificationBuilder<T>
    {
        Specification<T> Specification { get; }
    }
}
#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释
