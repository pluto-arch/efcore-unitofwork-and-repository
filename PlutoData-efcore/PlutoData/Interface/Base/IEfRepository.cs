using Microsoft.EntityFrameworkCore;

namespace PlutoData.Interface.Base
{
	/// <summary>
	/// 
	/// </summary>
	public interface IEfRepository
	{
		/// <summary>
		/// dbcontext
		/// </summary>
		/// <remarks>
		/// 只能由unitofwork初始化
		/// </remarks>
		DbContext DbContext { get; set; }
	}
}