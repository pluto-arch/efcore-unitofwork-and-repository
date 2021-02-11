namespace PlutoData.Models
{
    /// <summary>
    /// dapper 抽线查询model
    /// </summary>
    public abstract class AbstractQueryModel
    {
        /// <summary>
        /// where 条件
        /// </summary>
        /// <returns></returns>
        public abstract string Where();

        /// <summary>
        /// 排序
        /// </summary>
        /// <returns></returns>
        public abstract string OrderBy();
    }
}