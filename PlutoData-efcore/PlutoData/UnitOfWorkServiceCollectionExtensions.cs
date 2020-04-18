using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PlutoData.Interface;


namespace PlutoData
{
    public static class UnitOfWorkServiceCollectionExtensions
    {
        public static IServiceCollection AddUnitOfWork<TContext>(this IServiceCollection services)
            where TContext : DbContext
        {
            services.AddScoped<IUnitOfWork<TContext>, UnitOfWork<TContext>>();
            services.AddScoped(typeof(IRepository<>),typeof(Repository<>));
            return services;
        }



        public static IServiceCollection AddRepository(this IServiceCollection services)
        {
            var assembly = Assembly.GetEntryAssembly();
            var implTypes = assembly.GetTypes().Where(c => !c.IsInterface && c.Name.EndsWith("Repository")).ToList();
            foreach (var impltype in implTypes)
            {
                var interfaces = impltype.GetInterfaces().Where(c => c.Name.StartsWith("I") && c.Name.EndsWith("Repository"));
                if (interfaces.Count() <= 0)
                    continue;
                foreach (var inter in interfaces)
                    services.AddScoped(inter, impltype);
            }
            return services;
        }

    }
}