using Microsoft.EntityFrameworkCore;
using PlutoData.Interface;

namespace PlutoData.Extensions
{
    public static class RepositoryExtensions
    {
        public static void SetDbContext(this IRepository @this,DbContext context)
        {
            @this.DbContext = context;
        } 
    }
}