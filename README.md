# efcore+dapper

> nuget : Install-Package PlutoData -Version 1.7.0-alpha

## uow
### 使用纯efcore模式

```csharp
service.AddDbContext<EFDbContext>(opt =>
                {
                    opt.UseSqlServer(connStr);
                    opt.UseLoggerFactory(new LoggerFactory(new[] { new EFLoggerProvider() }));
                }).AddEfUnitOfWork<EFDbContext>();
```



## 仓储
1. ef 仓储 
```csharp
//1. 基础仓储：
// 定义接口
public interface IBloggingEfCoreRepository<TEntity> : IEfRepository<TEntity> where TEntity :  BaseEntity { }
// 实现接口
public class BloggingEfCoreRepository<TEntity> : EfRepository<EFDbContext, TEntity> , IBloggingEfCoreRepository<TEntity>
     where TEntity : BaseEntity
{
    public BloggingEfCoreRepository(EFDbContext dbContext) : base(dbContext)
    {
    }
}

// 注入：
service.AddTransient(typeof(IBloggingEfCoreRepository<>),typeof(BloggingEfCoreRepository<>));

//2. 自定义仓储
public interface ICustomBlogRepository : IEfRepository<Blog>
{}
public class CustomBlogRepository : EfRepository<BloggingContext, Blog>, ICustomBlogRepository
{
    public CustomBlogRepository(BloggingContext dbContext) : base(dbContext)
    {
    }
}

// 获取仓储
var _uow=_provider.GetService<IEfUnitOfWork<BloggingContext>>();
var efRep = _uow.GetRepository<IBloggingEfCoreRepository<Blog>>();
或者直接在构造函数中注入IBloggingEfCoreRepository<Blog>。
```


## EFcore Specification 模式

新建自定义的Specification，继承自Specification<EntityType>
然后就可以在构造函数中进行查询，再配合仓储的方法。

```csharp
public class BlogSpecification: Specification<Blog>
{
    public BlogSpecification()
    {
        Query.Where(x=>x.Id>0)
            .OrderByDescending(x=>x.Id);
    }
}

// 指定tresult时 Select(x => x.Title) 必填
public class Blog2Specification: Specification<Blog,string>
{
    public Blog2Specification()
    {
        Query.Select(x => x.Title).Where(x=>x.Id>0);
    }
}

```



## 事务
1. 纯efcore模式
```csharp
var efUow=services.GetService<IEfUnitOfWork<EfDbContext>>();
await  using(var transaction = await efUow.BeginTransactionAsync(cancellationToken))
{
    transactionId = transaction.TransactionId;
    // rep 操作1
    // rep 操作2
    // ....
    await _unitOfWork.CommitTransactionAsync(transaction,cancellationToken);
}
```

