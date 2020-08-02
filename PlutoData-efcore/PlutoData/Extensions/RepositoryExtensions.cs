using Microsoft.EntityFrameworkCore;
using PlutoData.Interface;

namespace PlutoData.Extensions
{
    /// <summary>
    /// Repository Extensions
    /// </summary>
    public static class RepositoryExtensions
    {
        /// <summary>
        /// setting dbcontext
        /// </summary>
        /// <param name="this"></param>
        /// <param name="context"></param>
        public static void SetDbContext(this IRepository @this,DbContext context)
        {
            @this.DbContext = context;
        } 
    }
}