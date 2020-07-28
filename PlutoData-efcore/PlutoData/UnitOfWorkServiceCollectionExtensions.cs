using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using PlutoData.Interface;


namespace PlutoData
{
    public static class UnitOfWorkServiceCollectionExtensions
    {
        /// <summary>
        /// 添加unitofwork和dbcontext
        /// </summary>
        /// <typeparam name="TContext"></typeparam>
        /// <param name="services"></param>
        /// <param name="optionBuilder"></param>
        /// <param name="liftLifetime"></param>
        /// <returns></returns>
        public static IServiceCollection AddUnitOfWorkDbContext<TContext>(
            this IServiceCollection services, 
            Action<DbContextOptionsBuilder> optionBuilder,
            ServiceLifetime liftLifetime=ServiceLifetime.Scoped)
            where TContext : DbContext
        {
            services
                .AddDbContext<TContext>(optionBuilder,liftLifetime)
                .AddUnitOfWork<TContext>();
            return services;
        }

        /// <summary>
        /// 添加单个unitofwork
        /// 需要单独添加dbcontext<see cref="services.AddDbContext"/>
        /// </summary>
        /// <typeparam name="TContext"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddUnitOfWork<TContext>(this IServiceCollection services)
            where TContext : DbContext
        {
            services.AddScoped<IUnitOfWork<TContext>, UnitOfWork<TContext>>();
            services.TryAddScoped(typeof(IRepository<>), typeof(Repository<>));
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
                if (interfaces.Count() <= 0)
                    continue;
                foreach (var inter in interfaces)
                    services.AddScoped(inter, impltype);
            }
        }

    }
}