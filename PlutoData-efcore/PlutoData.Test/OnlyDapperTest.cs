using System;
using System.Threading;
using apisample;

using Dapper;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using NUnit.Framework;

using PlutoData.Interface.Base;
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

		[Test]
		public void GetRepository()
		{
			var rep = _dapperUnitOfWork.GetRepository<IBlogDapperRepository>();
			Assert.IsTrue(rep != null && (rep is IDapperRepository repository));

			var rep2 = _dapperUnitOfWork.GetBaseRepository<Blog>();
			Assert.IsTrue(rep2 != null && (rep2 is IDapperRepository repository2));

			var dapperRep = _dapperUnitOfWork.GetRepository<IBlogDapperRepository>();

            var res = dapperRep.Insert(new Blog
            {
                Url = "dapper_GetRepository",
                Title = "dapper_GetRepository",
            });

            var ddfd = rep.Insert(new Blog
            {
                Url = "dapper_GetRepository",
                Title = "dapper_GetRepository",
            });
        }


		[Test]

		public void DbTransaction()
		{
			var rep = _dapperUnitOfWork.GetRepository<IBlogDapperRepository>();

			var rep2 = _dapperUnitOfWork.GetBaseRepository<Post>();
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


            try
            {
                var res2 = rep.BeginTransaction<bool>(transaction =>
                {
                    rep2.DbTransaction = transaction;
                    var res = rep.Insert(new Blog
                    {
                        Url = $"{transaction.GetHashCode()}_22222222",
                        Title = $"{rep2.DbTransaction.GetHashCode()}_2222222",
                    });
                    var aaa2 = rep2.DbConnection.Execute($@"INSERT INTO [dbo].[Posts]([Id],[Title], [Content]) VALUES (22223,N'fffff', N'fffffff'); ", transaction: transaction) > 0;
                    return aaa2 & res;
                });
                Assert.IsFalse(res2);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
		}
	}
}