namespace PlutoData.Interface.Base
{
	/// <summary>
	/// dapper 仓储 基本接口
	/// </summary>
	public interface IDapperRepository
	{
		/// <summary>
		/// dbcontext
		/// </summary>
		/// <remarks>
		/// 只能由unitofwork初始化
		/// </remarks>
		DapperDbContext DbContext { set; }
	}
}