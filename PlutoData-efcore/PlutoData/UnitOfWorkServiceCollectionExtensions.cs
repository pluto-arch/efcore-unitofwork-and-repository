using System;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using PlutoData.Enums;
using PlutoData.Uows;


namespace PlutoData
{
    /// <summary>
    /// 
    /// </summary>
    public static class UnitOfWorkServiceCollectionExtensions
    {

        /// <summary>
        /// 添加dapper dbcontext
        /// </summary>
        /// <typeparam name="TDapperDbContext"></typeparam>
        /// <param name="service"></param>
        /// <param name="option"></param>
        /// <returns></returns>
        public static IServiceCollection AddDapperDbContext<TDapperDbContext>(this IServiceCollection service,Action<DapperDbContextOption> option)
            where TDapperDbContext : DapperDbContext
        {
            service.TryAdd(new ServiceDescriptor(typeof(DapperDbContextOption<TDapperDbContext>),p => CreateDbContextOptions<TDapperDbContext>(p, option),ServiceLifetime.Scoped));
            service.AddScoped<TDapperDbContext>();
            return service;
        }


        /// <summary>
        /// 添加dapper 上下文和单元
        /// </summary>
        /// <returns></returns>
        public static IServiceCollection AddDapperUnitOfWork<TDapperDbContext>(this IServiceCollection service)
            where TDapperDbContext : DapperDbContext
        {
            service.AddScoped<IDapperUnitOfWork<TDapperDbContext>, DapperUnitOfWork<TDapperDbContext>>();
            return service;
        }


        /// <summary>
        /// 添加仓储
        /// </summary>
        public static void AddRepository(this IServiceCollection services, Assembly assembly = null,bool repositoryScoped=false)
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
                {
                    if (repositoryScoped)
                    {
                        services.AddScoped(inter, impltype);
                    }
                    else
                    {
                        services.AddTransient(inter, impltype);
                    }
                }
            }
        }

        #region private

        /// <summary>
        /// 添加unitofwork
        /// </summary>
        /// <typeparam name="TContext"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddEfUnitOfWork<TContext>(this IServiceCollection services)
            where TContext : DbContext
        {
            services.AddScoped<IEfUnitOfWork<TContext>, EfUnitOfWork<TContext>>();
            return services;
        }


        private static object CreateDbContextOptions<TDapperDbContext>(IServiceProvider serviceProvider, Action<DapperDbContextOption> option) 
            where TDapperDbContext : DapperDbContext
        {
            var opt = new DapperDbContextOption<TDapperDbContext>();
            option.Invoke(opt);
            return opt;
        }
        #endregion
    }
}