using System;
using System.Collections.Generic;
using apisample;
using Dapper;
using Microsoft.EntityFrameworkCore.Storage;
using PlutoData.Interface;

namespace PlutoData.Test.Repositorys.Dapper
{
	public class BlogDapperRepository:DapperRepository<Blog>,IBlogDapperRepository
	{
		/// <inheritdoc />
		public IEnumerable<Blog> GetAll()
		{

			//return DbConnection.Query<Blog>($"SELECT * FROM {EntityMapName}");
			return Execute(async conn=>
			        {
						return await conn.QueryAsync<Blog>($"SELECT * FROM {EntityMapName}",transaction:DbTransaction);
			        }).Result;

			//return DbConnection.Query<Blog>($"SELECT * FROM {EntityMapName}");
		}
	}
}