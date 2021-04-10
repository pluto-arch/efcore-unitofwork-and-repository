using Microsoft.EntityFrameworkCore;
using PlutoData;

namespace PlutoData.Test.Repositorys.Ef
{


    public interface ICustomBlogRepository : IEfRepository<Blog>
    {

    }


    public class CustomBlogRepository : EfRepository<BloggingContext, Blog>, ICustomBlogRepository
    {
        public CustomBlogRepository(BloggingContext dbContext) : base(dbContext)
        {
        }
    }


    public interface IBloggingEfCoreRepository<TEntity> : IEfRepository<TEntity> where TEntity :  class,new() { }

    public class BloggingEfCoreRepository<TEntity> : EfRepository<BloggingContext, TEntity> , IBloggingEfCoreRepository<TEntity>
        where TEntity : class,new()
    {
        public BloggingEfCoreRepository(BloggingContext dbContext) : base(dbContext)
        {
        }
    }



}