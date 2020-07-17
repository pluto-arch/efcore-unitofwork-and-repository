# pluto-efcore-general-repository
efcore-general-repository

### 注入UOW

```csharp
services.AddUnitOfWorkDbContext<BloggingContext>(opt =>
{
    opt.UseSqlServer(
        "Server =.;Database = PlutoDataDemo;User ID = sa;Password = 123456;Trusted_Connection = False;");
    opt.UseLoggerFactory(new LoggerFactory(new[] { new EFLoggerProvider() }));
});
// 支持注入多个
```

### 仓储
1. 基本仓储
```csharp
// 基本仓储unitofwork中已有，直接使用TEntity
// IRepository<TEntity>
// 使用 uow的GetBaseRepository即可
unitOfWork.GetBaseRepository<Blog>();
```

2. 自定义仓储
```csharp
services.AddRepository(); 
// 默认使用程序集扫描注入仓储，可选，不使用这个的话，请自定使用services.AddScoped<I{customer}Repository,{customer}Repository>()
// 自定义repository，需分别继承自IRepository<TEntity>。Repository<TEntity>
// 使用unitOfWork.GetRepository<ICustomBlogRepository>();
public interface ICustomBlogRepository : IRepository<Blog>
{
  // ...
}
public class CustomBlogRepository : Repository<Blog>, ICustomBlogRepository
{
  // ...
}
```

### 获取仓储
1. 获取基本仓储(仅支持IRepository中的操作)
```csharp
// unitOfWork.GetBaseRepository<TEntity>()
public ctor(IUnitOfWork<BloggingContext> unitOfWork){
  var baseRep = unitOfWork.GetBaseRepository<Blog>();
}
```
2. 获取自定义仓储(支持IRepository中的操作和自定义操作)
```csharp
// unitOfWork.GetRepository<ICustomBlogRepository>()
public ctor(IUnitOfWork<BloggingContext> unitOfWork){
  var baseRep = unitOfWork.GetRepository<ICustomBlogRepository>();
}
```

获取自定义仓储
```csharp
        private readonly IUnitOfWork<BloggingContext> _unitOfWork;
        private readonly ICustomBlogRepository _customBlogRepository;

        public ValuesController(IUnitOfWork<BloggingContext> unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _customBlogRepository = unitOfWork.GetRepository<ICustomBlogRepository>();
        }
```

