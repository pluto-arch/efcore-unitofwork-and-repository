using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        private ILogger<ValuesController> _logger;
        private readonly ICustomBlogRepository _customBlogRepository;

        public ValuesController(IUnitOfWork<BloggingContext> unitOfWork, ILogger<ValuesController> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _customBlogRepository = unitOfWork.GetRepository<ICustomBlogRepository>();
        }

        // GET api/values
        [HttpGet]
        public async Task<IList<Blog>> Get()
        {

            //_unitOfWork.ChangeDatabase("PlutoDataDemo_2020");


            //userRepo.ChangeTable("Blogs_10086");

            var blog10086 = new Blog
            {
                Id = (int)DateTime.Now.Ticks % 100,
                Url = "1212",
                Title = "12312"
            };
            _customBlogRepository.Insert(blog10086);

            await _unitOfWork.SaveChangesAsync(true);


            //userRepo.ChangeTable("Blogs_10087");
            var blog10087 = new Blog
            {
                Id = (int)DateTime.Now.Ticks % 10,
                Url = "1212",
                Title = "12312"
            };
            _customBlogRepository.Insert(blog10087);

            await _unitOfWork.SaveChangesAsync(true);


            return await _customBlogRepository.GetAllAsync(include: source => source.Include(blog => blog.Posts).ThenInclude(post => post.Comments));
        }



        // GET api/values/Page/5/10
        [HttpGet("Page/{pageIndex}/{pageSize}")]
        public async Task<IPagedList<Blog>> Get(int pageIndex, int pageSize)
        {
            // projection
            var items = _customBlogRepository.GetPagedList(b => new { Name = b.Title, Link = b.Url });

            return await _customBlogRepository.GetPagedListAsync(pageIndex: pageIndex, pageSize: pageSize);
        }


        // GET api/values/Search/a1
        [HttpGet("Search/{term}")]
        public async Task<IPagedList<Blog>> Get(string term)
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
            return await _customBlogRepository.FindAsync(id);
        }


        // POST api/values
        [HttpPost]
        public async Task Post([FromBody]Blog value)
        {
            _customBlogRepository.Insert(value);
            await _unitOfWork.SaveChangesAsync();
        }



    }
}