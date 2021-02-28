using PlutoData.Models;

using System;
using System.Collections.Generic;
using System.Text;

namespace PlutoData.Test.models
{
    public class Demo: AbstractQuerySqlBuild
    {
        public string Name { get; set; }


        public override string BuildWhere()
        {
            var where = "";
            if (!string.IsNullOrEmpty(Name))
            {
                AddSqlParam(nameof(Name), Name);
                where += $" {nameof(Name)}=@{nameof(Name)}";
            }
            return where;
        }
    }
}
