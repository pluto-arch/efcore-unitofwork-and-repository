using System;
using System.Data;
using PlutoData.Enums;

namespace PlutoData
{
    public class DapperDbContextOption
    {
        public EnumDbType DbType { get; set; }

        public bool DependOnEf { get; set; }

        public Func<IDbConnection> DbConnCreateFunc  { get; set; }

        public string ConnectionString { get; set; }

        public Type EfDbContextType { get;set; }
    }


    public class DapperDbContextOption<TDapperDbContext>:DapperDbContextOption where TDapperDbContext : DapperDbContext
    {

    }
}