using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using PlutoData.Collections;
using PlutoData.Interface;
using PlutoData.Uows;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using apisample.Dapper;
using PlutoData;

namespace apisample.Controllers
{
	[Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly IEfUnitOfWork<BloggingContext> _unitOfWork;
        private ILogger<ValuesController> _logger;
        private readonly IEfRepository<Blog> _customBlogRepository;
        private Random r=new Random();
        public ValuesController(
            IEfUnitOfWork<BloggingContext> unitOfWork,
            ILogger<ValuesController> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _customBlogRepository = unitOfWork.GetRepository<ICustomBlogRepository>();
        }

        //[HttpGet("like")]
        //public async Task<IActionResult> GetList()
        //{
        //    Expression<Func<Blog, bool>> p = x => x.Id > 0;
        //    //p = p.And(x => EF.Functions.Like(x.Title, "9%"));
        //    //var aaa = await _customBlogRepository.GetListAsync(p,pageIndex:1,pageSize:20);
        //    return Ok("");
        //}



        //[HttpPost("Create")]
        //public async Task<IActionResult> Create()
        //{
        //    var rep1 = _unitOfWork.GetRepository<ICustomBlogRepository>();
        //    rep1.Insert(new Blog
        //    {
        //        Url = "_unitOfWork2",
        //        Title = Guid.NewGuid().ToString("N"),
        //    });
        //    await _unitOfWork.SaveChangesAsync();
        //    return Ok("11");
        //}


        //// GET api/values/Search/a1
        //[HttpGet("QueryDemo")]
        //public async Task<IPagedList<Blog>> QueryDemo(string term)
        //{
        //    _logger.LogInformation("demo about first or default with include");

        //    var item = _customBlogRepository.GetFirstOrDefault(predicate: x => x.Title.Contains(term), include: source => source.Include(blog => blog.Posts).ThenInclude(post => post.Comments));

        //    _logger.LogInformation("demo about first or default without include");

        //    item = _customBlogRepository.GetFirstOrDefault(predicate: x => x.Title.Contains(term), orderBy: source => source.OrderByDescending(b => b.Id));

        //    _logger.LogInformation("demo about first or default with projection");

        //    var projection = _customBlogRepository.GetFirstOrDefault(b => new { Name = b.Title, Link = b.Url }, predicate: x => x.Title.Contains(term));

        //    return await _customBlogRepository.GetPagedListAsync(predicate: x => x.Title.Contains(term));
        //}


        //// GET api/values/4
        //[HttpGet("{id}")]
        //public async Task<Blog> Get(int id)
        //{
        //    return await _customBlogRepository.FindAsync(new object[] { id });
        //}

        //[HttpPost("GetRepository")]
        //public async Task<IActionResult> GetRepository()
        //{

        //    var first = _unitOfWork.GetRepository<ICustomBlogRepository>(); // 单独获取


        //    var second = _unitOfWork.GetRepository<ICustomBlogRepository>(); // 单独获取


        //    var blog2 = new Blog
        //    {
        //        Url = "Normal_" + new Random().Next(100, 999),
        //        Title = "12312"
        //    };
        //    _customBlogRepository.Insert(blog2); 


        //    var blog222 = new Blog
        //    {
        //        Url = "Normal_" + new Random().Next(100, 999),
        //        Title = "12312"
        //    };
        //    first.Insert(blog222);


        //    var blog312 = new Blog
        //    {
        //        Url = "Normal_" + new Random().Next(100, 999),
        //        Title = "12312"
        //    };
        //    second.Insert(blog312);

        //    #region MyRegion

        //    //var ad = _customBlogRepository.DbContext;
        //    //second.DbContext = ad; //will throw exception, 仓储的dbcontext 只能由unitofwork设置

        //    #endregion



        //    await _unitOfWork.SaveChangesAsync();
        //    return Ok("123");
        //}

        //// POST api/values
        //[HttpPost]
        //public async Task Post([FromBody] Blog value)
        //{
        //    try
        //    {
        //        var strategy = _unitOfWork.CreateExecutionStrategy();
        //        await strategy.ExecuteAsync(async () =>
        //        {
        //            using (var transaction = await _unitOfWork.BeginTransactionAsync())
        //            {
        //                var blog2 = new Blog
        //                {
        //                    Id = (int)DateTime.Now.Ticks,
        //                    Url = "1212",
        //                    Title = "12312"
        //                };
        //                _customBlogRepository.Insert(blog2);
        //                await _unitOfWork.SaveChangesAsync();
        //                Thread.Sleep(1000);

        //                _customBlogRepository.Insert(value);
        //                await _unitOfWork.SaveChangesAsync();
        //                await _unitOfWork.CommitTransactionAsync(transaction);
        //            }
        //        });
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine(e);
        //    }
        //    await _unitOfWork.SaveChangesAsync();
        //}


        //[HttpGet("test")]
        //public string Test()
        //{
	       // EfInsert();
	       // EfUpdate();
	       // DapperInsert();
        //    return "123123";
        //}

        
        //private void EfInsertRange()
        //{
	       // var entities=new List<Blog>();
	       // for (int i = 0; i < 4000; i++)
	       // {
		      //  entities.Add(new Blog
		      //               {
			     //                Url = $"{r.Next(1,99999)}_efefefef",
			     //                Title = $"{r.Next(1,99999)}_efefefef",
		      //               });
	       // }
	       // var efRep = _unitOfWork.GetRepository<ICustomBlogRepository>();
	       // efRep.Insert(entities);
	       // _unitOfWork.SaveChanges();
        //}


        //private void EfInsert()
        //{
	       // var efRep = _unitOfWork.GetRepository<ICustomBlogRepository>();
        //    efRep.Insert(new Blog
	       //              {
		      //               Url = $"{r.Next(1,99999)}_efefefef",
		      //               Title = $"{r.Next(1,99999)}_efefefef",
	       //              });
	       // _unitOfWork.SaveChanges();
        //}
        //private void EfUpdate()
        //{
	       // var efRep = _unitOfWork.GetRepository<ICustomBlogRepository>();
        //    var blog=efRep.GetFirstOrDefault(predicate:x=>x.Id>0);
	       // blog.Title=$"ef_update_{r.Next(1,888888)}";
	       // _unitOfWork.SaveChanges();
        //}

        //private void DapperInsert()
        //{
	       // var dapperRep = _unitOfWork.GetDapperRepository<IBlogDapperRepository,DapperDbContext>();
	       // dapperRep.Insert(new
	       //                  {
		      //                   Url = $"{r.Next(1,99999)}_dapper",
		      //                   Title = $"{r.Next(1,99999)}_dapper",
	       //                  });
        //}



        /*
         * 

            //_unitOfWork.ChangeDatabase("PlutoDataDemo_2020");


            //userRepo.ChangeTable("Blogs_10086");
            var blog10086 = new Blog
            {
                Id = (int)DateTime.Now.Ticks % 100,
                Url = "1212",
                Title = "12312"
            };
            _customBlogRepository.Insert(blog10086);

            _unitOfWork.SaveChanges();


            //userRepo.ChangeTable("Blogs_10087");
            var blog10087 = new Blog
            {
                Id = (int)DateTime.Now.Ticks % 10,
                Url = "1212",
                Title = "12312"
            };
            _customBlogRepository.Insert(blog10087);

            await _unitOfWork.SaveChangesAsync();
         */


    }
}