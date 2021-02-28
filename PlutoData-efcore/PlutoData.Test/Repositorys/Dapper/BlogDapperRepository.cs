using System;
using System.Collections.Generic;
using apisample;
using Dapper;
using Microsoft.EntityFrameworkCore.Storage;
using PlutoData.Interface;
using Blog = apisample.Blog;

namespace PlutoData.Test.Repositorys.Dapper
{
	public class BlogDapperRepository:DapperRepository<Blog>,IBlogDapperRepository
	{
        public BlogDapperRepository(DapperDbContext dapperDb) : base(dapperDb)
        {
        }

        /// <inheritdoc />
        public IEnumerable<Blog> GetAll()
		{
			//return DbConnection.Query<Blog>($"SELECT * FROM {EntityMapName}");
			return Execute((conn,tran)=>
			        {
						return conn.Query<Blog>($"SELECT * FROM {EntityMapName}",transaction: tran);
			        });

		}

		/// <inheritdoc />
		public bool Insert(object entity)
		{
			return Execute((conn,tran)=>
			               {
							   var sql=$@"INSERT INTO {EntityMapName}({nameof(Blog.Url)},{nameof(Blog.Title)}) 
										  VALUES (@{nameof(Blog.Url)},@{nameof(Blog.Title)})";
							   return (conn.Execute(sql,entity,tran))>0;
			               });
		}
	}
}