using System;
using System.Collections.Concurrent;
using System.Data;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PlutoData.Extensions;
using Dapper;
using System.Linq;
#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释

namespace PlutoData
{

    public partial class BaseDapperRepository<TDapperDbContext, TEntity>
    {
        public int Count()
        {
            //return this.Execute<int>(conn =>
            //{
            //    return conn.Count<int>(predicate:"");
            //});
            return 0;
        }
    }
}