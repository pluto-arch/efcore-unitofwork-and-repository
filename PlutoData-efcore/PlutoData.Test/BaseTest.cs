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
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.Data.SqlClient;
using PlutoData.Interface.Base;

namespace PlutoData.Test
{
    



	[TestFixture]
    public class BaseTest
    {
		private int _flag;


	    public BaseTest(int flag)
	    {
		    _flag=flag;
	    }

        internal IServiceProvider _provider;
        internal IEfUnitOfWork<BloggingContext> _uow;
        internal IDapperUnitOfWork _dapperUnitOfWork;
        internal Random r=new Random();
        [SetUp]
        public void SetUp()
        {
            var service=new ServiceCollection();
            service.AddControllers();
            if (_flag==1) // onlyEf
            {
	            service.AddUnitOfWorkDbContext<BloggingContext>(opt =>
	                                                            {
		                                                            opt.UseSqlServer(
		                                                             "Server =.;Database = PlutoDataDemo;User ID = sa;Password = 123456;Trusted_Connection = False;");
		                                                            opt.UseLoggerFactory(new LoggerFactory(new[] { new EFLoggerProvider() }));
	                                                            });
            }

            if (_flag==2)
            {
	            service.AddDapperUnitOfWork("Server =.;Database = PlutoDataDemo;User ID = sa;Password = 123456;Trusted_Connection = False;");
            }

            if (_flag==3)
            {
	            service.AddDapperUnitOfWork<BloggingContext>(opt =>
	                                                         {
		                                                         opt.UseSqlServer(
		                                                          "Server =.;Database = PlutoDataDemo;User ID = sa;Password = 123456;Trusted_Connection = False;");
		                                                         opt.UseLoggerFactory(new LoggerFactory(new[] { new EFLoggerProvider() }));
	                                                         });
            }
            service.AddRepository();
            _provider = service.BuildServiceProvider();
            _uow=_provider.GetService<IEfUnitOfWork<BloggingContext>>();
            _dapperUnitOfWork=_provider.GetService<IDapperUnitOfWork>();
        }

  
    }
}
