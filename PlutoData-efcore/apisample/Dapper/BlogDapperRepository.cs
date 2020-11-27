using System;
using System.Collections.Generic;
using apisample;
using Dapper;
using Microsoft.EntityFrameworkCore.Storage;
using PlutoData;
using PlutoData.Interface;
using Blog = apisample.Blog;

namespace apisample.Dapper
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

		}

		/// <inheritdoc />
		public bool Insert(object entity)
		{
			return Execute(async conn=>
			               {
							   var sql=$@"INSERT INTO {EntityMapName}({nameof(Blog.Url)},{nameof(Blog.Title)}) 
										  VALUES (@{nameof(Blog.Url)},@{nameof(Blog.Title)})";
							   return (await conn.ExecuteAsync(sql,entity,transaction:DbTransaction))>0;
			               }).Result;
		}
	}
}