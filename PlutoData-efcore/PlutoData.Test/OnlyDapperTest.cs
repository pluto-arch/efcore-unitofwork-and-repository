using System;

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
            var entity = dapperRep.GetAll();
            var res = dapperRep.Insert(new Blog
            {
                Url = "dapper_DbTransaction",
                Title = "dapper_DbTransaction",
            });

            var ddfd = rep.Insert(new Blog
            {
                Url = "dapper_DbTransaction",
                Title = "dapper_DbTransaction",
            });

            Assert.IsTrue(res);
        }


        [Test]

        public void DbTransaction() 
        {
            var rep = _dapperUnitOfWork.GetRepository<IBlogDapperRepository>();

            var rep2 = _dapperUnitOfWork.GetBaseRepository<Post>();
            var res = rep.BeginTransaction<bool>(transaction =>
                                                 {
                                                     rep2.DbTransaction = transaction;

                                                     var dsdsds= rep.GetDbConnection().GetHashCode();

                                                     var dsdsds222= rep2.GetDbConnection().GetHashCode();

                                                     var res = rep.Insert(new Blog
                                                     {
                                                         Url = "1111111111111111111111",
                                                         Title = "11111111111111111",
                                                     });
                                                     var aaa2 = rep2.GetDbConnection().Execute($@"INSERT INTO [dbo].[Posts]([Title], [Content]) VALUES (N'11111111111111', N'111111111111111111');
	                                                ", transaction: transaction) > 0;
                                                     return aaa2 & res;
                                                 });
            Assert.IsTrue(res);


            var res2 = rep.BeginTransaction<bool>(transaction =>
            {
                rep2.DbTransaction = transaction;
                var res = rep.Insert(new Blog
                {
                    Url = "dapper_DbTransaction2222222222222222222222222",
                    Title = "dapper_DbTransaction22222222222222222222222222222",
                });
                var aaa2 = rep2.GetDbConnection().Execute($@"INSERT INTO [dbo].[Posts]([Id],[Title], [Content]) VALUES (22223,N'22222222222', N'222222222222222222');
	                                                ", transaction: transaction) > 0;
                return aaa2 & res;
            });
            Assert.IsFalse(res2);
            rep.Insert(new Blog
            {
                Url = "dapper_DbTransaction2_dddddddddddddddddddd",
                Title = "dapper_DbTransaction2_dddddddddddddddddddd",
            });

        }
    }
}