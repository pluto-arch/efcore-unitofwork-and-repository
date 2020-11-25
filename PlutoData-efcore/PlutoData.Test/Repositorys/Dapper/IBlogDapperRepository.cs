using System.Collections.Generic;
using apisample;
using PlutoData.Interface;

namespace PlutoData.Test.Repositorys.Dapper
{
	public interface IBlogDapperRepository:IDapperRepository<Blog>
	{
		IEnumerable<Blog> GetAll();
	}
}