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
    /// <typeparam name="TResult"></typeparam>
    public class SpecificationBuilder<T, TResult> : SpecificationBuilder<T>, ISpecificationBuilder<T, TResult>
    {
        /// <summary>
        /// 
        /// </summary>
        public new Specification<T, TResult> Specification { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="specification"></param>
        public SpecificationBuilder(Specification<T, TResult> specification) : base(specification) => Specification = specification;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SpecificationBuilder<T> : ISpecificationBuilder<T>
    {
        /// <summary>
        /// 
        /// </summary>
        public Specification<T> Specification { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="specification"></param>
        public SpecificationBuilder(Specification<T> specification) => Specification = specification;
    }
}
