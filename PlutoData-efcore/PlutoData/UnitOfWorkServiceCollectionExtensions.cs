using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using PlutoData.Interface;
using PlutoData.Uows;


namespace PlutoData
{
    /// <summary>
    /// 
    /// </summary>
    public static class UnitOfWorkServiceCollectionExtensions
    {
        /// <summary>
        /// 添加unitofwork和dbcontext
        /// </summary>
        /// <typeparam name="TContext"></typeparam>
        /// <param name="services"></param>
        /// <param name="optionBuilder"></param>
        /// <returns></returns>
        public static IServiceCollection AddUnitOfWorkDbContext<TContext>(
            this IServiceCollection services, 
            Action<DbContextOptionsBuilder> optionBuilder)
            where TContext : DbContext
        {
	        if(optionBuilder==null)
		        throw new ArgumentNullException(nameof(TContext));
	        services
                .AddDbContext<TContext>(optionBuilder,ServiceLifetime.Scoped)
                .AddEfUnitOfWork<TContext>();
            return services;
        }

        /// <summary>
        /// 添加单个unitofwork
        /// 需要单独添加dbcontext
        /// <see>
        ///     <cref>services.AddDbContext</cref>
        /// </see>
        /// </summary>
        /// <typeparam name="TContext"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        private static IServiceCollection AddEfUnitOfWork<TContext>(this IServiceCollection services)
            where TContext : DbContext
        {
            services.AddScoped<IEfUnitOfWork<TContext>, EfUnitOfWork<TContext>>();
            services.TryAddScoped(typeof(IEfRepository<>), typeof(EfRepository<>));
            return services;
        }

        /// <summary>
        /// 添加仓储
        /// </summary>
        /// <param name="services"></param>
        /// <param name="assembly">入口程序集</param>
        public static void AddRepository(this IServiceCollection services, Assembly assembly = null)
        {
            assembly = assembly ?? Assembly.GetEntryAssembly();
            var implTypes = assembly.GetTypes().Where(c => !c.IsInterface && c.Name.EndsWith("Repository")).ToList();
            foreach (var impltype in implTypes)
            {
                var interfaces = impltype.GetInterfaces().Where(c => c.Name.StartsWith("I") && c.Name.EndsWith("Repository"));
                if (!interfaces.Any())
                    continue;
                foreach (var inter in interfaces)
                    services.AddScoped(inter, impltype);
            }
        }



        /// <summary>
        /// 添加dapper 单元工作
        /// </summary>
        /// <param name="service"></param>
        /// <param name="connStr"></param>
        /// <returns></returns>
        public static IServiceCollection AddDapperUnitOfWork(this IServiceCollection service,string connStr)
        {
	        service.AddScoped(typeof(IDapperRepository<>), typeof(DapperRepository<>));
	        service.AddScoped<DapperDbContext>(sp=>
	                                           {
		                                           return new DapperDbContext(sp,connStr);
	                                           });
	        service.AddScoped<IDapperUnitOfWork, DapperUnitOfWork>();
	        return service;
        }


        /// <summary>
        /// 添加dapper 单元工作
        /// </summary>
        /// <returns></returns>
        public static IServiceCollection AddDapperUnitOfWork<TDbContext>(
		        this IServiceCollection service,
		        Action<DbContextOptionsBuilder> optionBuilder) where TDbContext:DbContext
        {
            
            if(optionBuilder==null)
				throw new ArgumentNullException(nameof(TDbContext));
	        service
			       .AddDbContext<TDbContext>(optionBuilder,ServiceLifetime.Scoped) 
			       .AddEfUnitOfWork<TDbContext>();
			       
	        service.AddScoped(typeof(IDapperRepository<>), typeof(DapperRepository<>));
	        service.AddScoped<DapperDbContext>(sp=>
	                                           {
		                                           var efUow=sp.GetService<IEfUnitOfWork<TDbContext>>();
		                                           if (efUow!=null)
		                                           {
			                                           return new DapperDbContext(sp,efUow.DbContext);
		                                           }
                                                   throw new ArgumentNullException(nameof(TDbContext));
	                                           });
	        service.AddScoped<IDapperUnitOfWork, DapperUnitOfWork>();
	        return service;
        }

    }
}