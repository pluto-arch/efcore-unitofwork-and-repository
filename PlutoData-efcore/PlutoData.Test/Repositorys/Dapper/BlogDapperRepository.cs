using System;
using apisample;
using PlutoData.Interface;

namespace PlutoData.Test.Repositorys.Dapper
{
	public class BlogDapperRepository:DapperRepository<Blog>,IBlogDapperRepository
	{
		
	}
}