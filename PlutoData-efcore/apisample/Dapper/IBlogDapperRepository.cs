using System.Collections.Generic;
using apisample;
using PlutoData.Interface;

namespace apisample.Dapper
{
	public interface IBlogDapperRepository:IDapperRepository<Blog>
	{
		IEnumerable<Blog> GetAll();

		bool Insert(object entity);

	}
}