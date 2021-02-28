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
    public class IncludableSpecificationBuilder<T, TProperty> : IIncludableSpecificationBuilder<T, TProperty>
    {
        /// <summary>
        /// 
        /// </summary>
        public Specification<T> Specification { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="specification"></param>
        public IncludableSpecificationBuilder(Specification<T> specification)
        {
            Specification = specification;
        }
    }
}
