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
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.Data.SqlClient;
using PlutoData.Interface.Base;
using PlutoData.Test.models;
using PlutoData.Test.Repositorys.Ef;

namespace PlutoData.Test
{
    
    public enum Flag
    {
        Both,
        EfCore,
        Dapper
    }


	[TestFixture]
    public class BaseTest
    {
		private readonly Flag _flag;
        private readonly string connStr = @"Server =0.0.0.0,3433;Database = PlutoDataDemo;User ID = pluto_admin;Password = 970307Lbx;Trusted_Connection = False;";

	    public BaseTest(Flag flag)
	    {
		    _flag=flag;
	    }

        internal IServiceProvider _provider;
        internal IEfUnitOfWork<BloggingContext> _uow;
        internal IDapperUnitOfWork<DapperDbContext> _dapperUnitOfWork;
        internal Random r=new Random();
        [SetUp]
        public void SetUp()
        {
            var service=new ServiceCollection();
            service.AddControllers();
            service.AddScoped<Repositorys.Ef.ICustomBlogRepository, Repositorys.Ef.CustomBlogRepository>();
            if (_flag==Flag.EfCore) // onlyEf
            {

                service.AddEfUnitOfWork<BloggingContext>(opt =>
                {
                    opt.UseSqlServer(connStr);
                    opt.UseLoggerFactory(new LoggerFactory(new[] { new EFLoggerProvider() }));
                });

                service.AddEfUnitOfWork<BloggingContext>(opt =>
	                                                            {
		                                                            opt.UseSqlServer(connStr);
		                                                            opt.UseLoggerFactory(new LoggerFactory(new[] { new EFLoggerProvider() }));
	                                                            });
            }

            if (_flag==Flag.Dapper)  // only dapper
            {
                service.AddScoped<DapperDbContext>(sp => new DapperDbContext(sp, connStr,SqlClientFactory.Instance.CreateConnection));
                service.AddDapperUnitOfWork<DapperDbContext>();
            }

            if (_flag==Flag.Both) // dapper & efcore
            {

                //service.AddHybridUnitOfWork<BloggingContext>(opt =>
                //{
                //    opt.UseSqlServer(connStr);
                //    opt.UseLoggerFactory(new LoggerFactory(new[] { new EFLoggerProvider() }));
                //});

                service.AddHybridUnitOfWork<BloggingContext>(opt =>
                {
                    opt.UseSqlServer(connStr);
                    opt.UseLoggerFactory(new LoggerFactory(new[] { new EFLoggerProvider() }));
                });
            }
            service.AddRepository(Assembly.GetExecutingAssembly());

            service.AddScoped(typeof(IBloggingEfCoreRepository<>),typeof(BloggingEfCoreRepository<>));

            _provider = service.BuildServiceProvider();
            _uow=_provider.GetService<IEfUnitOfWork<BloggingContext>>();
            _dapperUnitOfWork=_provider.GetService<IDapperUnitOfWork<DapperDbContext>>();
        }

  
    }
}
