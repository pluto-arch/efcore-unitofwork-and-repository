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

### 使用纯dapper模式
> 自定义DapperDbContext需要继承自DapperDbContext
```csharp
service.AddDapperDbContext<BlogDapperDbContext>(op =>
                    {
                        op.DependOnEf = true;
                        op.DbType = EnumDbType.SQLServer;
                        op.EfDbContextType = typeof(BloggingContext);
                    })
                    .AddDapperUnitOfWork<BlogDapperDbContext>();

```

### 使用包含dapper的模式时，需要新建上下文，类似于efcore 的
```csharp
 public class DemoDapperDbContext:DapperDbContext
    {
        /// <inheritdoc />
        public DemoDapperDbContext(IServiceProvider service, DapperDbContextOption<DemoDapperDbContext> options) : base(service, options)
        {
        }
    }
```


### 使用混合模式
```csharp
services.AddDbContext<EfCoreDbContext>(options)
    .AddEfUnitOfWork<EfCoreDbContext>();

services.AddDapperDbContext<PlutoNetCoreDapperDbContext>(op =>
    {
        op.DependOnEf = true;
        op.DbType = EnumDbType.SQLServer;
        op.EfDbContextType = typeof(EfCoreDbContext);
    })
    .AddDapperUnitOfWork<PlutoNetCoreDapperDbContext>();

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




2. dapper 仓储
```csharp
//1. 基础仓储
public interface IBloggingDapperRepository<TEntity> : IBaseDapperRepository<TEntity> where TEntity :  BaseEntity { }

public class BloggingDapperRepository<TEntity> : BaseDapperRepository<DapperDbContext, TEntity> , IBloggingDapperRepository<TEntity>
   where TEntity : BaseEntity
{
   public BloggingDapperRepository(DapperDbContextdbContext) : base(dbContext)
   {
   }
}


//2. 自定义仓储
public interface ICustomerDapperRepository : IBaseDapperRepository<Blog>
{

}

public class CustomerDapperRepository : BaseDapperRepository<BlogDapperDbContext, Blog>, ICustomerDapperRepository
{
    /// <inheritdoc />
    public CustomerDapperRepository(BlogDapperDbContext dapperDb) : base(dapperDb)
    {
    }
}
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


## 仓储的使用

ef仓储使用可以直接在仓储对象上进行linq查询，其余同普通用法。

dapper仓储，执行操作需要使用 ExecuteAsync 或者 Execute 中进行操作。
```csharp
public class SystemDomainService : ISystemDomainService
{
    private readonly IPlutoNetCoreTemplateDapperRepository<UserEntity> _userDapperRep;

    public SystemDomainService(
        IPlutoNetCoreTemplateDapperRepository<UserEntity> userDapperRep)
    {
        _userDapperRep = userDapperRep;
    }

    public async Task<int?> InsertAsync(UserEntity user)
    {
        return await _userDapperRep.ExecuteAsync(async conn => await conn.InsertAsync(user));
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

2. 纯dapper模式
```csharp
var dapperUow=services.GetService<IDapperUnitOfWork<DapperDbContext>>();

var rep1= dapperUow.GetRepository<IBloggingDapperRepository<Blog>>();
var rep2 = dapperUow.GetRepository<IBloggingDapperRepository<Post>>();


using (var tran = _dapperUnitOfWork.BeginTransaction())
using (rep1.UseTransaction(tran))
using (rep1.UseTransaction(tran)) // dapper仓储每个都需要切换到当前事务的链接对象。
{
    var res = rep1.Execute(conn =>
    {
        return conn.Insert(new Blog
        {
            Url = "ExecuteInTransactionAsync_Success_Test",
            Title = "ExecuteInTransactionAsync_Success_Test",
            Sort = 0,
            Posts = null
        },tran);
    });

    var res2= rep1.Execute(conn =>
    {
        return conn.Insert(new Post
        {
            BlogId = res.Value,
            Title = "ExecuteInTransactionAsync_Success_Test",
            Content = "ExecuteInTransactionAsync_Success_Test",
        },tran);
    });
    _dapperUnitOfWork.CommitTransaction(tran);

}
```

3. 混合模式
```csharp
var efUow=services.GetService<IEfUnitOfWork<EfDbContext>>();
var dapperUow=services.GetService<IDapperUnitOfWork<DapperDbContext>>();

// 从daooer uow获取仓储，或者从efuow获取dapper仓储，或者直接注入仓储
var rep1= dapperUow.GetRepository<IBloggingDapperRepository<Blog>>();
var rep2 = dapperUow.GetRepository<IBloggingDapperRepository<Post>>();

// efcore 开启事务
await using (var ddsd = await efUow.BeginTransactionAsync())
{
    // ef的仓储 操作

    var tran = ddsd.GetDbTransaction();
    using (rep1.UseTransaction(tran))  // dapper仓储每个都需要切换到当前事务的链接对象。
    using (rep2 .UseTransaction(tran))
    {
        // dapper 仓储 rep1和rep2  的操作
    }
    
    // ef的仓储 操作
}
```

