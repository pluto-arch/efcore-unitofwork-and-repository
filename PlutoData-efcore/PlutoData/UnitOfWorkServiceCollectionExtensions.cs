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
        /// 添加ef上下文和单元
        /// </summary>
        /// <typeparam name="TContext"></typeparam>
        /// <param name="services"></param>
        /// <param name="optionBuilder"></param>
        /// <param name="contextLifeTime"></param>
        /// <param name="optionLifeTime"></param>
        /// <returns></returns>
        public static IServiceCollection AddEfUnitOfWork<TContext>(this IServiceCollection services,
                                                                   Action<DbContextOptionsBuilder> optionBuilder,
                                                                   ServiceLifetime contextLifeTime =
                                                                       ServiceLifetime.Scoped,
                                                                   ServiceLifetime optionLifeTime =
                                                                       ServiceLifetime.Scoped)
            where TContext : DbContext
        {
            if (optionBuilder == null)
                throw new ArgumentNullException(nameof(optionBuilder));
            services.AddDbContext<TContext>(optionBuilder, contextLifeTime, optionLifeTime).AddEfUnitOfWork<TContext>();
            return services;
        }


        /// <summary>
        /// 添加dapper 上下文和单元
        /// </summary>
        /// <param name="service"></param>
        /// <returns></returns>
        public static IServiceCollection AddDapperUnitOfWork<TDapperDbContext>(this IServiceCollection service)
            where TDapperDbContext : DapperDbContext
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
            Action<DbContextOptionsBuilder> optionBuilder,
            ServiceLifetime contextLifeTime = ServiceLifetime.Scoped,
            ServiceLifetime optionLifeTime = ServiceLifetime.Scoped) where TDbContext : DbContext
        {
            if (optionBuilder == null)
                throw new ArgumentNullException(nameof(TDbContext));
            service.AddDbContext<TDbContext>(optionBuilder, contextLifeTime, optionLifeTime)
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
        /// 添加仓储
        /// </summary>
        /// <param name="services"></param>
        /// <param name="assembly">入口程序集</param>
        public static void AddRepository(this IServiceCollection services, Assembly? assembly = null)
        {
            assembly ??= Assembly.GetEntryAssembly();
            var implTypes = assembly?.GetTypes().Where(c => !c.IsInterface && c.Name.EndsWith("Repository")).ToList();
            if (implTypes == null)
            {
                return;
            }

            foreach (var impltype in implTypes)
            {
                var interfaces = impltype.GetInterfaces()
                                         .Where(c => c.Name.StartsWith("I") && c.Name.EndsWith("Repository"));
                if (!interfaces.Any())
                    continue;
                foreach (var inter in interfaces)
                    services.AddScoped(inter, impltype);
            }
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