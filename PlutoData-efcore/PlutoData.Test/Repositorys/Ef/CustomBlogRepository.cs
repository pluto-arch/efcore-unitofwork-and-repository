using System;

using apisample;

using Microsoft.EntityFrameworkCore;
using PlutoData;
using PlutoData.Interface;
using Blog = apisample.Blog;

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


    public interface IBloggingEfCoreRepository<TEntity> : IEfRepository<TEntity> where TEntity : class { }

    public class BloggingEfCoreRepository<TEntity> : EfRepository<BloggingContext, TEntity> , IBloggingEfCoreRepository<TEntity>
        where TEntity : class
    {
        public BloggingEfCoreRepository(BloggingContext dbContext) : base(dbContext)
        {
        }
    }



}