using System;
using apisample;
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
            
        }
    }
}