using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlutoData.Specifications.Builder
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TProperty"></typeparam>
    public interface IIncludableSpecificationBuilder<T, out TProperty> : ISpecificationBuilder<T>
    {
    }
}
