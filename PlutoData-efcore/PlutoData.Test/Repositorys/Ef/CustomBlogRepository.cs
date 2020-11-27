using System;
using Microsoft.EntityFrameworkCore;
using PlutoData;
using PlutoData.Interface;
using Blog = apisample.Blog;

namespace PlutoData.Test.Repositorys.Ef
{


    public interface ICustomBlogRepository : IEfRepository<Blog>
    {

    }


    public class CustomBlogRepository : EfRepository<Blog>, ICustomBlogRepository
    {
    }


}