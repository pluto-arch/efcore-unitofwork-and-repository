using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NUnit.Framework;
using PlutoData.Collections;
using PlutoData.Test.Repositorys.Ef;

namespace PlutoData.Test
{
	[TestFixture]
	public class EfCoreTest : BaseTest
	{
		/// <inheritdoc />
		public EfCoreTest() : base(Flag.EfCore)
		{

		}


		[Test]
		public async Task Insert()
		{
			var repository = _uow.GetRepository<Repositorys.Ef.ICustomBlogRepository>();
			IList<Blog> d = new List<Blog>();
			for (int i = 0; i < 10; i++)
			{
				d.Add(new Blog
				{
					Url = $"{i}",
					Title = $"{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss:fff}",
					Posts = new List<Post>
					{
						new Post
						{
							Title=$"Blog_{i}_{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss:fff}",
							Content=$"Blog_{i}_{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss:fff}",
						}
					}
				});
			}
			await repository.InsertAsync(d);
			_uow.SaveChanges();
		}


        private async Task SeedAsync()
        {
            var repository = _uow.GetRepository<IBloggingEfCoreRepository<Blog>>();
            var seedData = new List<Blog>();
            for (int i = 0; i < 100; i++)
            {
                seedData.Add(new Blog
                {
                    Url = $"{i}",
                    Sort = i+1,
                    Title = $"{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss:fff}",
                    Posts = new List<Post>
                    {
                        new Post
                        {
                            Title=$"Blog_{i}_{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss:fff}",
                            Content=$"Blog_{i}_{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss:fff}",
                        }
                    }
                });
            }
            await repository.InsertAsync(seedData,true);
        }

        private async Task ClearAsync()
        {
            var repository = _uow.GetRepository<IBloggingEfCoreRepository<Blog>>();
            await repository.DeleteAsync(x => x.Id > 0,true);
        }
        
        
		[Test]
		public async Task Repository()
		{
            await ClearAsync();
            await SeedAsync();
            
            var repository = _uow.GetRepository<IBloggingEfCoreRepository<Blog>>();
			var res = await repository.AsNoTracking().Where(x=>x.Id>0).ToListAsync();
            foreach (var item in res)
            {
                if (item.Id%2==0)
                {
					item.Title = "123123";
                }
            }
			// not workind
		    await _uow.SaveChangesAsync();


            var page = await repository.Where(x=>x.Id>0).ToPagedListAsync(1,20);
            Assert.IsTrue(page.Items.Count==20);
            Console.WriteLine($"paging result : {JsonConvert.SerializeObject(page)} \r\n");


            var sort = await repository.Where(x => x.Id > 0).OrderByDescending(x => x.Id).ToPagedListAsync(1,3);
            Assert.IsTrue(sort.Items.Count>0&sort.Items.First().Id>sort.Items.Last().Id);
            Console.WriteLine($"sorting and paging result : {JsonConvert.SerializeObject(sort)} \r\n");


            var count = await repository.CountAsync();
            Assert.IsTrue(count==100);
            Console.WriteLine($"count result : {JsonConvert.SerializeObject(count)} \r\n");


            var first = await repository.FirstOrDefaultAsync(x=>x.Sort==5);
            Assert.IsTrue(first!=null&&first.Sort==5);
            Console.WriteLine($"first entity id :{first.Id}");


            var exist = repository.Contains(new Blog { Sort = 41 });
            Assert.IsFalse(exist);
            Console.WriteLine($"entity 41 exist :{exist}");


            var last = await repository.OrderByDescending(x=>x.Id).LastOrDefaultAsync();
            Assert.IsTrue(last!=null&&last.Sort==1);
            Console.WriteLine($"last entity id :{last.Id}");
        }


        [Test]
        public async Task Transaction()
        {
            var rep = _uow.GetRepository<IBloggingEfCoreRepository<Blog>>();
            var res = await rep.FindAsync(x=>x.Id==4);


            var ddd=await _uow.BeginTransactionAsync();

            try
            {
                await _uow.CommitTransactionAsync(ddd);
            }
            catch (Exception)
            {
                await ddd.RollbackAsync();
            }

        }
        
	}
}