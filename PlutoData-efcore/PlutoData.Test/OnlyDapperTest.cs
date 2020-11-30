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
	public class OnlyDapperTest:BaseTest
	{
        public OnlyDapperTest(): base(Flag.Dapper)
        {
        }

        [Test]
        public void GetRepository()
        {
            var rep = _dapperUnitOfWork.GetRepository<IBlogDapperRepository>();
            Assert.IsTrue(rep!=null&&(rep is IDapperRepository repository));

            var rep2 = _dapperUnitOfWork.GetBaseRepository<Blog>();
            Assert.IsTrue(rep2!=null&&(rep2 is IDapperRepository repository2));

            var dapperRep = _dapperUnitOfWork.GetRepository<IBlogDapperRepository>();
            var entity = dapperRep.GetAll();
            var res= dapperRep.Insert(new Blog
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
            var rep2=_dapperUnitOfWork.GetBaseRepository<Post>();
            var res = rep.BeginTransaction<bool>(transaction =>
                                                 {
	                                                 rep2.DbTransaction=transaction;
	                                                 var res= rep.Insert(new Blog
	                                                            {
		                                                            Url = "dapper_DbTransaction",
		                                                            Title = "dapper_DbTransaction",
	                                                            });
	                                                var aaa=res&(rep2.DbConnection.Execute($@"INSERT INTO [dbo].[Posts]([Title], [Content], [BlogId]) VALUES (N'12312', N's', NULL);
	                                                ")>0);
                                                     return aaa;
                                                 });
            
        }
    }
}