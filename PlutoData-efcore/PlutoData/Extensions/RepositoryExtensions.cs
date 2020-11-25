using Microsoft.EntityFrameworkCore;
using PlutoData.Interface.Base;

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
        public static void SetDbContext(this IEfRepository @this,DbContext context)
        {
            @this.DbContext = context;
        } 


        /// <summary>
        /// setting dbcontext
        /// </summary>
        /// <param name="this"></param>
        /// <param name="context"></param>
        public static void SetDbContext(this IDapperRepository @this,DapperDbContext context)
        {
	        @this.DbContext = context;
        } 


    }
}