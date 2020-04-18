# pluto-efcore-general-repository
efcore-general-repository

### how to use

```csharp
services.AddDbContext<BloggingContext>(opt =>
                    {
                        opt.UseSqlServer(
                            "Server =.;Database = PlutoDataDemo;User ID = sa;Password = 123456;Trusted_Connection = False;");
                        opt.UseLoggerFactory(new LoggerFactory(new[] {new EFLoggerProvider()}));
                    })
                .AddUnitOfWork<BloggingContext>().AddRepository();
```
`AddRepository()  可选。默认使用程序集扫描`
```csharp
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
```


