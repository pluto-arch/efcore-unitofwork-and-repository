using System;
using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PlutoData.Enums;

namespace PlutoData.Test.Repositorys.Dapper
{
    public class BlogDapperDbContext:DapperDbContext
    {
        /// <inheritdoc />
        public BlogDapperDbContext(IServiceProvider service, DapperDbContextOption<BlogDapperDbContext> options) : base(service, options)
        {
        }
    }
}