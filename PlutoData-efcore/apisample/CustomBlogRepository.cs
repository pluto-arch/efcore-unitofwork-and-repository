using System;
using Microsoft.EntityFrameworkCore;
using PlutoData;
using PlutoData.Interface;


namespace apisample
{


    public interface ICustomBlogRepository : IRepository<Blog>
    {

    }


    public class CustomBlogRepository : Repository<Blog>, ICustomBlogRepository
    {
    }



    public interface ICustomBlog2Repository : IRepository<Blog2>
    {

    }


    public class CustomBlog2Repository : Repository<Blog2>, ICustomBlog2Repository
    {
    }


}