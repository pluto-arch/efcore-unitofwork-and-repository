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


}