using System;
using System.Collections.Generic;
using Dapper;

namespace PlutoData.Test.Repositorys.Dapper
{

    public interface ICustomerDapperRepository : IBaseDapperRepository<Blog>
    {

    }

    public class CustomerDapperRepository : BaseDapperRepository<BlogDapperDbContext, Blog>, ICustomerDapperRepository
    {
        /// <inheritdoc />
        public CustomerDapperRepository(BlogDapperDbContext dapperDb) : base(dapperDb)
        {
        }
    }

    
    public interface IBloggingDapperRepository<TEntity> : IBaseDapperRepository<TEntity> where TEntity :  class,new() { }

    public class BloggingDapperRepository<TEntity> : BaseDapperRepository<BlogDapperDbContext, TEntity> , IBloggingDapperRepository<TEntity>
        where TEntity : class,new()
    {
        public BloggingDapperRepository(BlogDapperDbContext dbContext) : base(dbContext)
        {
        }
    }
    
    
    
}