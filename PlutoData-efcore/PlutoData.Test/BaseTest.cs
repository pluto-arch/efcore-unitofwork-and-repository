using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using PlutoData.Uows;
using System;
using System.Collections.Generic;
using System.Reflection;
using PlutoData.Test.Repositorys.Ef;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using PlutoData.Test.Repositorys.Dapper;
using System.ComponentModel.DataAnnotations.Schema;
using PlutoData.Enums;

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
        // 970307lBX  970307Lbx$
        public readonly string connStr = @"Server =127.0.0.1,1433;Database = PlutoDataDemo;User ID = sa;Password = 970307lBX;Trusted_Connection = False;";

	    public BaseTest(Flag flag)
	    {
		    _flag=flag;
	    }

        internal IServiceProvider _provider;
        internal IEfUnitOfWork<BloggingContext> _uow;
        internal IDapperUnitOfWork<BlogDapperDbContext> _dapperUnitOfWork;
        internal Random r=new();
        [SetUp]
        public void SetUp()
        {
            var service=new ServiceCollection();
            service.AddScoped<Repositorys.Ef.ICustomBlogRepository, Repositorys.Ef.CustomBlogRepository>();
            if (_flag==Flag.EfCore) // onlyEf
            {

                service.AddDbContext<BloggingContext>(opt =>
                {
                    opt.UseSqlServer(connStr);
                    opt.UseLoggerFactory(new LoggerFactory(new[] { new EFLoggerProvider() }));
                }).AddEfUnitOfWork<BloggingContext>();
            }

            if (_flag==Flag.Dapper)  // only dapper
            {
                service.AddDapperDbContext<BlogDapperDbContext>(op =>
                    {
                        op.DependOnEf = true;
                        op.DbType = EnumDbType.SQLServer;
                        op.EfDbContextType = typeof(BloggingContext);
                    })
                    .AddDapperUnitOfWork<BlogDapperDbContext>();
            }

            if (_flag==Flag.Both) // dapper & efcore
            {
                service.AddDbContext<BloggingContext>(opt =>
                {
                    opt.UseSqlServer(connStr);
                    opt.UseLoggerFactory(new LoggerFactory(new[] { new EFLoggerProvider() }));
                }).AddEfUnitOfWork<BloggingContext>();

                service.AddDapperDbContext<BlogDapperDbContext>(op =>
                    {
                        op.DependOnEf = true;
                        op.DbType = EnumDbType.SQLServer;
                        op.EfDbContextType = typeof(BloggingContext);
                    })
                    .AddDapperUnitOfWork<BlogDapperDbContext>();
            }


            service.AddRepository(Assembly.GetExecutingAssembly());
            service.AddTransient(typeof(IBloggingDapperRepository<>), typeof(BloggingDapperRepository<>));
            service.AddTransient(typeof(IBloggingEfCoreRepository<>),typeof(BloggingEfCoreRepository<>));
            _provider = service.BuildServiceProvider();
            _uow=_provider.GetService<IEfUnitOfWork<BloggingContext>>();
            _dapperUnitOfWork=_provider.GetService<IDapperUnitOfWork<BlogDapperDbContext>>();
        }

  
    }


    [Dapper.Table("Blogs")]
    public class Blog
    {
        [Dapper.Key]
        public int Id { get; set; }
        public string Url { get; set; }
        public string Title { get; set; }

        public int Sort { get; set; }

        [Dapper.IgnoreInsert,Dapper.IgnoreSelect,Dapper.IgnoreUpdate]
        public List<Post> Posts { get; set; }
    }

    [Dapper.Table("Posts")]
    public class Post 
    {
        [Dapper.Key]
        public int Id { get; set; }

        public long BlogId { get; set; }

        [Dapper.IgnoreInsert,Dapper.IgnoreSelect,Dapper.IgnoreUpdate]
        public Blog Blog { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }

        [Dapper.IgnoreInsert,Dapper.IgnoreSelect,Dapper.IgnoreUpdate]
        public List<Comment> Comments { get; set; }
    }

    public class Comment
    {
        [Dapper.Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
    }



    public class EFLogger : ILogger
    {
        private readonly string categoryName;

        public EFLogger(string categoryName) => this.categoryName = categoryName;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            //ef core?????????????categoryName?Microsoft.EntityFrameworkCore.Database.Command,????????Information
            if (categoryName == "Microsoft.EntityFrameworkCore.Database.Command"
                && logLevel == LogLevel.Information)
            {
                var logContent = formatter(state, exception);
                //TODO: ????????????????????????
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(logContent);
                Console.ResetColor();
            }
        }

        public IDisposable BeginScope<TState>(TState state) => null;
    }

    public class EFLoggerProvider : ILoggerProvider
    {
        public ILogger CreateLogger(string categoryName) => new EFLogger(categoryName);
#pragma warning disable CA1816 // Dispose 方法应调用 SuppressFinalize
        public void Dispose() { }
#pragma warning restore CA1816 // Dispose 方法应调用 SuppressFinalize
    }
}
