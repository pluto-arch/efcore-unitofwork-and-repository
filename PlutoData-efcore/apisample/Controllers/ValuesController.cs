using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using Microsoft.Extensions.Logging;

using PlutoData.Collections;
using PlutoData.Interface;

namespace apisample.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly IUnitOfWork<BloggingContext> _unitOfWork;
        private readonly IUnitOfWork<Blogging2Context> _unitOfWork2;
        private ILogger<ValuesController> _logger;
        private readonly ICustomBlogRepository _customBlogRepository;

        public ValuesController(
            IUnitOfWork<BloggingContext> unitOfWork,
            ILogger<ValuesController> logger,
            IUnitOfWork<Blogging2Context> unitOfWork2)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _unitOfWork2 = unitOfWork2;
            _customBlogRepository = unitOfWork.GetRepository<ICustomBlogRepository>();


            var a_customBlogRepository = unitOfWork.GetBaseRepository<Blog>();
        }

        // GET api/values
        [HttpGet("BeginTransactionAsync")]
        public async Task<IList<Blog>> BeginTransactionAsync()
        {
            using (var tran = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    var commandText = @$"INSERT INTO {_customBlogRepository.EntityMapName}([Url], [Title]) VALUES (N'Normal_55222', N'1231222');";
                    _unitOfWork.ExecuteSqlCommand(commandText);
                    await tran.CommitAsync();
                }
                catch (Exception e)
                {
                    _logger.LogError(e, e.Message);
                    await tran.RollbackAsync();
                }
            }

            return await _customBlogRepository.GetAllAsync(include: source => source.Include(blog => blog.Posts).ThenInclude(post => post.Comments));
        }



        // GET api/values/Page/5/10
        [HttpGet("MultipleDbContext")]
        public IActionResult MultipleDbContext()
        {

            var rep1 = _unitOfWork.GetRepository<ICustomBlogRepository>();
            rep1.Insert(new Blog
            {
                Url = "_unitOfWork2",
                Title = "_unitOfWork2",
            });


            var rep2 = _unitOfWork2.GetRepository<ICustomBlog2Repository>();
            rep2.Insert(new Blog2
            {
                Url = "_unitOfWork2",
                Title = "_unitOfWork2",
            });

            _unitOfWork.SaveChanges();
            _unitOfWork2.SaveChanges();
            return Ok("12312");
        }


        // GET api/values/Search/a1
        [HttpGet("QueryDemo")]
        public async Task<IPagedList<Blog>> QueryDemo(string term)
        {
            _logger.LogInformation("demo about first or default with include");

            var item = _customBlogRepository.GetFirstOrDefault(predicate: x => x.Title.Contains(term), include: source => source.Include(blog => blog.Posts).ThenInclude(post => post.Comments));

            _logger.LogInformation("demo about first or default without include");

            item = _customBlogRepository.GetFirstOrDefault(predicate: x => x.Title.Contains(term), orderBy: source => source.OrderByDescending(b => b.Id));

            _logger.LogInformation("demo about first or default with projection");

            var projection = _customBlogRepository.GetFirstOrDefault(b => new { Name = b.Title, Link = b.Url }, predicate: x => x.Title.Contains(term));

            return await _customBlogRepository.GetPagedListAsync(predicate: x => x.Title.Contains(term));
        }


        // GET api/values/4
        [HttpGet("{id}")]
        public async Task<Blog> Get(int id)
        {
            return await _customBlogRepository.FindAsync(new object[] { id });
        }

        [HttpPost("GetRepository")]
        public async Task<IActionResult> GetRepository()
        {

            var first= _unitOfWork.GetRepository<ICustomBlogRepository>(); // 单独获取


            var second = _unitOfWork.GetRepository<ICustomBlogRepository>(); // 单独获取


            var blog2 = new Blog
            {
                Url = "Normal_" + new Random().Next(100, 999),
                Title = "12312"
            };
            _customBlogRepository.Insert(blog2); // 从构造函数中获取


            var blog222 = new Blog
            {
                Url = "Normal_" + new Random().Next(100, 999),
                Title = "12312"
            };
            first.Insert(blog222);


            var blog312 = new Blog
            {
                Url = "Normal_" + new Random().Next(100, 999),
                Title = "12312"
            };
            second.Insert(blog312);

            #region MyRegion

            //var ad = _customBlogRepository.DbContext;
            //second.DbContext = ad; //will throw exception, 仓储的dbcontext 只能由unitofwork设置

            #endregion



            await _unitOfWork.SaveChangesAsync();
            return Ok("123");
        }

        // POST api/values
        [HttpPost]
        public async Task Post([FromBody] Blog value)
        {
            try
            {
                var strategy = _unitOfWork.CreateExecutionStrategy();
                await strategy.ExecuteAsync(async () =>
                {
                    Guid transactionId;
                    using (var transaction = await _unitOfWork.BeginTransactionAsync())
                    {
                        var blog2 = new Blog
                        {
                            Id = (int)DateTime.Now.Ticks,
                            Url = "1212",
                            Title = "12312"
                        };
                        _customBlogRepository.Insert(blog2);
                        await _unitOfWork.SaveChangesAsync();
                        Thread.Sleep(1000);

                        _customBlogRepository.Insert(value);
                        await _unitOfWork.SaveChangesAsync();
                        await _unitOfWork.CommitTransactionAsync(transaction);
                    }
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            await _unitOfWork.SaveChangesAsync();
        }



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