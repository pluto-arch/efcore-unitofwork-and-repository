using Microsoft.EntityFrameworkCore;

using PlutoData.Collections;
using PlutoData.Specifications;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PlutoData
{
    public partial class EfRepository<TContext, TEntity>
    {
        private readonly ISpecificationEvaluator<TEntity> _specification = new SpecificationEvaluatorBase<TEntity>();

        /// <inheritdoc/>
        public IPagedList<TEntity> GetPageList(ISpecification<TEntity> specification,int pageNo,int pageSize)
        {
            return ApplySpecification(specification).ToPagedList(pageNo, pageSize);
        }

        /// <inheritdoc/>
        public Task<IPagedList<TEntity>> GetPageListAsync(ISpecification<TEntity> specification, int pageNo, int pageSize, CancellationToken cancellationToken = default)
        {
            return ApplySpecification(specification).ToPagedListAsync(pageNo, pageSize, cancellationToken);
        }


        /// <inheritdoc/>
        public IPagedList<TResult> GetPageList<TResult>(ISpecification<TEntity, TResult> specification, int pageNo, int pageSize)
        {
            return ApplySpecification(specification).ToPagedList(pageNo, pageSize);
        }

        /// <inheritdoc/>
        public Task<IPagedList<TResult>> GetPageListAsync<TResult>(ISpecification<TEntity, TResult> specification, int pageNo, int pageSize, CancellationToken cancellationToken = default)
        {
            return ApplySpecification(specification).ToPagedListAsync(pageNo, pageSize, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<List<TEntity>> GetListAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default)
        {
            return ApplySpecification(specification).ToListAsync(cancellationToken);
        }

        /// <inheritdoc/>
        public Task<List<TResult>> GetListAsync<TResult>(ISpecification<TEntity, TResult> specification, CancellationToken cancellationToken = default)
        {
            return ApplySpecification(specification).ToListAsync(cancellationToken);
        }

        #region protected
        /// <summary>
        /// 
        /// </summary>
        /// <param name="specification"></param>
        /// <returns></returns>
        protected IQueryable<TEntity> ApplySpecification(ISpecification<TEntity> specification)
        {
            return _specification.GetQuery(_dbSet, specification);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="specification"></param>
        /// <returns></returns>
        protected IQueryable<TResult> ApplySpecification<TResult>(ISpecification<TEntity, TResult> specification)
        {
            if (specification is null) throw new ArgumentNullException(nameof(specification));
            if (specification.Selector is null) throw new Exception();
            return _specification.GetQuery(_dbSet, specification);
        }
        #endregion


    }
}
