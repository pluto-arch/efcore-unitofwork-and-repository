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
        /// 添加ef单元工作
        /// </summary>
        /// <typeparam name="TContext"></typeparam>
        /// <param name="services"></param>
        /// <param name="optionBuilder"></param>
        /// <returns></returns>
        public static IServiceCollection AddEfUnitOfWork<TContext>(
            this IServiceCollection services,
            Action<DbContextOptionsBuilder> optionBuilder) where TContext : DbContext
        {
            if (optionBuilder == null)
                throw new ArgumentNullException(nameof(optionBuilder), "缺少初始化参数：DbContextOptionsBuilder");
            services
                .AddDbContext<TContext>(optionBuilder, ServiceLifetime.Scoped)
                .AddEfUnitOfWork<TContext>();
            return services;
        }

        /// <summary>
        /// 添加ef单元工作
        /// </summary>
        /// <returns></returns>
        public static IServiceCollection AddEfUnitOfWorkUsingPool<TContext>(
            this IServiceCollection services,
            Action<DbContextOptionsBuilder> optionBuilder,
            int poolSize=128) where TContext : DbContext
        {
            if (optionBuilder == null)
                throw new ArgumentNullException("missing DbContextOptionsBuilder Action");
            services
                .AddDbContextPool<TContext>(optionBuilder, poolSize)
                .AddEfUnitOfWork<TContext>();
            return services;
        }



        /// <summary>
        /// 添加仓储
        /// </summary>
        /// <param name="services"></param>
        /// <param name="assembly">入口程序集</param>
        public static void AddRepository(this IServiceCollection services, Assembly? assembly = null)
        {
            assembly ??= Assembly.GetEntryAssembly();
            var implTypes = assembly?.GetTypes().Where(c => !c.IsInterface && c.Name.EndsWith("Repository")).ToList();
            if (implTypes==null)
            {
                return;
            }
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
        /// <returns></returns>
        public static IServiceCollection AddDapperUnitOfWork<TDapperDbContext>(this IServiceCollection service)
            where TDapperDbContext: DapperDbContext
        {
            service.AddScoped<IDapperUnitOfWork<TDapperDbContext>, DapperUnitOfWork<TDapperDbContext>>();
            service.AddScoped(typeof(IDapperRepository<>), typeof(DapperRepository<>));
            return service;
        }


        /// <summary>
        /// 添加ef/dapper混合单元工作
        /// </summary>
        /// <returns></returns>
        public static IServiceCollection AddHybridUnitOfWork<TDbContext>(
                this IServiceCollection service,
                Action<DbContextOptionsBuilder> optionBuilder) where TDbContext : DbContext
        {

            if (optionBuilder == null)
                throw new ArgumentNullException(nameof(TDbContext));
            service
                   .AddDbContext<TDbContext>(optionBuilder, ServiceLifetime.Scoped)
                   .AddEfUnitOfWork<TDbContext>();

            service.AddScoped(typeof(IDapperRepository<>), typeof(DapperRepository<>));
            service.AddScoped<DapperDbContext>(sp =>
                                               {
                                                   var efUow = sp.GetService<IEfUnitOfWork<TDbContext>>();
                                                   if (efUow != null)
                                                   {
                                                       return new DapperDbContext(sp, efUow.DbContext);
                                                   }
                                                   throw new ArgumentNullException(nameof(TDbContext));
                                               });
            service.AddScoped<IDapperUnitOfWork<DapperDbContext>, DapperUnitOfWork<DapperDbContext>>();
            return service;
        }


        /// <summary>
        /// 添加ef/dapper混合单元工作，使用dbcontextpool
        /// </summary>
        /// <returns></returns>
        public static IServiceCollection AddHybridUnitOfWorkUsingPool<TDbContext>(
            this IServiceCollection service,
            Action<DbContextOptionsBuilder> optionBuilder,
            int poolSize = 128) where TDbContext : DbContext
        {

            if (optionBuilder == null)
                throw new ArgumentNullException(nameof(TDbContext));
            service
                .AddDbContextPool<TDbContext>(optionBuilder, poolSize)
                .AddEfUnitOfWork<TDbContext>();

            service.AddScoped(typeof(IDapperRepository<>), typeof(DapperRepository<>));
            service.AddScoped<DapperDbContext>(sp =>
            {
                var efUow = sp.GetService<IEfUnitOfWork<TDbContext>>();
                if (efUow != null)
                {
                    return new DapperDbContext(sp, efUow.DbContext);
                }
                throw new ArgumentNullException(nameof(TDbContext));
            });
            service.AddScoped<IDapperUnitOfWork<DapperDbContext>, DapperUnitOfWork<DapperDbContext>>();
            return service;
        }


        #region private 
        /// <summary>
        /// 添加unitofwork
        /// </summary>
        /// <typeparam name="TContext"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        private static IServiceCollection AddEfUnitOfWork<TContext>(this IServiceCollection services)
            where TContext : DbContext
        {
            services.AddScoped<IEfUnitOfWork<TContext>, EfUnitOfWork<TContext>>();
            return services;
        }

        #endregion
    }
}