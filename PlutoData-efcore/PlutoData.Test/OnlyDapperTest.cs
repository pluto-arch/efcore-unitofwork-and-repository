using System;
using System.Threading;
using apisample;

using Dapper;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using NUnit.Framework;

using PlutoData.Interface.Base;
using PlutoData.Models;
using PlutoData.Test.Repositorys.Dapper;
using PlutoData.Uows;

namespace PlutoData.Test
{
	[TestFixture]
	public class OnlyDapperTest : BaseTest
	{
		public OnlyDapperTest() : base(Flag.Dapper)
		{
		}

        public class Dd: AbstractQuerySqlBuild
		{

            public string Title { get; set; }

            public override string BuildWhere()
            {
				var where = "WHERE 1=1 ";
                if (!string.IsNullOrEmpty(Title))
                {
					where += " AND Title=@Title ";
					AddSqlParam("Title", Title);
				}
				return where;
            }
        }


        [Test]
		public void GetRepository()
		{

			var rep2 = _dapperUnitOfWork.GetRepository<IBlogDapperRepository>();

			var ddaa = rep2.Get(3);

			Assert.IsTrue(ddaa.Id == 3);

			var add = rep2.GetPageList(new Dd { Title= "22222222222222222222222222" }, 1, 20);

			Assert.IsTrue(add.TotalCount == 1);

        }

		public class DemoQuery : AbstractQuerySqlBuild
		{

        }
		[Test]
		public void Query()
        {
			var rep = _dapperUnitOfWork.GetRepository<IBlogDapperRepository>();
			var res= rep.GetList(new DemoQuery());
            Console.WriteLine(res);
		}


		[Test]

		public void DbTransaction()
		{
			var rep = _dapperUnitOfWork.GetRepository<IBlogDapperRepository>();

			var rep2 = _dapperUnitOfWork.GetRepository<IBlogDapperRepository>();
			//var res = rep.BeginTransaction<bool>(transaction =>
			//									 {
			//										 rep2.DbTransaction = transaction;

			//										 var dsdsds = rep.DbConnection.GetHashCode();

			//										 var res = rep.Insert(new Blog
			//										 {
			//											 Url = $"{transaction.GetHashCode()}_111111111",
			//											 Title = $"{rep2.DbTransaction.GetHashCode()}_1111111111",
			//										 });
			//										 var aaa2 = rep2.DbConnection.Execute($@"INSERT INTO [dbo].[Posts]([Title], [Content]) VALUES (N'dsdsds', N'dsdsds');
	  //                                              ", transaction: transaction) > 0;
			//										 return aaa2 & res;
			//									 });
			//Assert.IsTrue(res);


            var res2 = rep.BeginTransaction<bool>(transaction =>
            {
                rep2.DbTransaction = transaction;

                var dsdsds = rep.DbConnection.GetHashCode();

                var dsdsds222 = rep2.DbConnection.GetHashCode();

                var res = rep.Insert(new Blog
                {
                    Url = $"{transaction?.GetHashCode()}_22222222",
                    Title = $"{rep2.DbTransaction?.GetHashCode()}_2222222",
                });
                var aaa2 = rep2.DbConnection.Execute($@"INSERT INTO [dbo].[Posts]([Id],[Title], [Content]) VALUES (22223,N'fffff', N'fffffff'); ", transaction: transaction) > 0;
                return aaa2 & res;
            });


            Assert.IsFalse(res2);
        }
	}
}