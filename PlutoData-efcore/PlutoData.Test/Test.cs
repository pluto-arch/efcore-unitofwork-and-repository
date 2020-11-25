using apisample;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using PlutoData.Test.Repositorys.Dapper;
using PlutoData.Uows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlutoData.Test
{
	[TestFixture]
    public class Test
    {

        private IServiceProvider _provider;
        private IEfUnitOfWork<BloggingContext> _uow;
        private IDapperUnitOfWork _dapperUnitOfWork;
        [SetUp]
        public void SetUp()
        {
            var service=new ServiceCollection();
            service.AddControllers();
            service.AddUnitOfWorkDbContext<BloggingContext>(opt =>
                                                            {
	                                                            opt.UseSqlServer(
	                                                             "Server =.;Database = PlutoDataDemo;User ID = sa;Password = 123456;Trusted_Connection = False;");
	                                                            opt.UseLoggerFactory(new LoggerFactory(new[] { new EFLoggerProvider() }));
                                                            });
            service.AddRepository();
            service.AddDapperUnitOfWork("Server =.;Database = PlutoDataDemo;User ID = sa;Password = 123456;Trusted_Connection = False;");
            service.AddScoped<IBlogDapperRepository,BlogDapperRepository>();
            _provider = service.BuildServiceProvider();
            _uow=_provider.GetService<IEfUnitOfWork<BloggingContext>>();
            _dapperUnitOfWork=_provider.GetService<IDapperUnitOfWork>();
        }

        [Test]
        public async Task EfBaseRepository()
        {
			var repository = _uow.GetBaseRepository<Blog>();
			var model = await repository.GetFirstOrDefaultAsync(predicate: t => t.Title=="123");
			Assert.IsNull(model);
		}


        [Test]
        public void DapperBaseRepository()
        {
	        var repository = _dapperUnitOfWork.GetBaseRepository<Blog>();
	        var entity=new List<Blog>
	                   {
                           new Blog
                           {
	                           Url = "null2223",
	                           Title = "asdasdasdas",
                           },
                           new Blog
                           {
	                           Url = "null3444444",
	                           Title = "asdasdasdas",
                           }
	                   };
	        var res=repository.Insert(entity.ToArray());
            Assert.IsTrue(res);
        }

        [Test]
        public void DapperRepository()
        {
	        var repository = _dapperUnitOfWork.GetRepository<IBlogDapperRepository>();
            var entity=new Blog
                       {
	                       Url = "null",
	                       Title = "asdasdasdas",
                       };
	        var res=repository.Insert(entity,true);
	        Assert.IsTrue(res);
        }

    }
}
