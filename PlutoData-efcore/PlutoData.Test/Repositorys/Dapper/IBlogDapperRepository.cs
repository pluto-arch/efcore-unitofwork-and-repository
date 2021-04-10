using System.Collections.Generic;

namespace PlutoData.Test.Repositorys.Dapper
{
	public interface IBlogDapperRepository: IBaseDapperRepository<Blog>
	{
		IEnumerable<Blog> GetAll();

		bool Insert(object entity);

	}
}