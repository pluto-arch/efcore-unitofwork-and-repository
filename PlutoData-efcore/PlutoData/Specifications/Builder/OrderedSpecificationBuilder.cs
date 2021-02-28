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
    public class OrderedSpecificationBuilder<T> : IOrderedSpecificationBuilder<T>
    {
        /// <summary>
        /// 
        /// </summary>
        public Specification<T> Specification { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="specification"></param>
        public OrderedSpecificationBuilder(Specification<T> specification) => Specification = specification;
    }
}
