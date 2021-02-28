using Dapper;

using System;

namespace PlutoData.Models
{
    /// <summary>
    /// dapper 抽线查询model
    /// </summary>
    public abstract class AbstractQuerySqlBuild
    {

        private readonly DynamicParameters _pars;

        /// <summary>
        /// 
        /// </summary>
        public AbstractQuerySqlBuild()
        {
            _pars = new DynamicParameters();
        }

        /// <summary>
        /// where 条件
        /// </summary>
        /// <returns></returns>
        public virtual string BuildWhere() => "";

        /// <summary>
        /// 排序
        /// </summary>
        /// <returns></returns>
        public virtual string BuildOrderBy() => "";

        /// <summary>
        /// 添加SQL参数
        /// </summary>
        public void AddSqlParam<TType>(string paramName,TType value)
        {
            _pars.Add(paramName, value);
        }

        /// <summary>
        /// 获取查询sql 参数
        /// </summary>
        /// <returns></returns>
        public DynamicParameters QueryParam => _pars;
    }
}