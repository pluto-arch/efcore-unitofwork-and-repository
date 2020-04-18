using System;
using PlutoData;
using PlutoData.Interface;


namespace apisample
{


    public interface ICustomBlogRepository : IRepository<Blog>
    {

    }


    public class CustomBlogRepository : Repository<Blog>, ICustomBlogRepository
    {
        public CustomBlogRepository(BloggingContext dbContext) : base(dbContext)
        {

        }
    }
}