using System;
using Microsoft.EntityFrameworkCore;
using PlutoData;
using PlutoData.Interface;


namespace apisample
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


}