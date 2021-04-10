using System;
using System.Threading.Tasks;
using Dapper;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using NUnit.Framework;
using PlutoData.Enums;
using PlutoData.Test.Repositorys.Dapper;
using PlutoData.Test.Repositorys.Ef;

namespace PlutoData.Test
{
	[TestFixture]
	public class MixTest:BaseTest
	{
		/// <inheritdoc />
		public MixTest() : base(Flag.Both)
		{
            
		}

        [Test]  
        public async Task Transaction()
        {
            var ef = _provider.GetService<BloggingContext>();
            var ddd = _dapperUnitOfWork.GetRepository<IBloggingDapperRepository<Blog>>();
            var ddd2 = _dapperUnitOfWork.GetRepository<IBloggingDapperRepository<Post>>();
            var efRep = _uow.GetRepository<IBloggingEfCoreRepository<Blog>>();
            await using (var ddsd = await _uow.BeginTransactionAsync())
            {
                var tran = ddsd.GetDbTransaction();
                using (ddd.UseTransaction(tran))
                using (ddd2.UseTransaction(tran))
                {
                    var repository = _uow.GetRepository<Repositorys.Ef.ICustomBlogRepository>();
                    await repository.DeleteAsync(x => x.Id > 0, true);
                    var res1 = await ddd.ExecuteAsync(async conn =>
                    {
                        return await conn.InsertAsync(new Blog
                        {
                            Url = "ExecuteInTran_Test  dapper",
                            Title = "ExecuteInTran_Test  dapper",
                            Sort = 0,
                        },tran);
                    });

                    if (res1 > 0)
                    {
                        var res2 = await ddd2.ExecuteAsync(async conn =>
                        {
                            return await conn.InsertAsync(new Post
                            {
                                BlogId = res1.Value,
                                Title = "ExecuteInTran_Test  dapper",
                                Content = "ExecuteInTran_Test  dapper",
                            },tran);
                        });
                    }
                    else
                    {
                        _uow.RollbackTransaction();
                        return;
                    }
                }

                await efRep.InsertAsync(new Blog
                {
                    Url = "ExecuteInTran_Test  ef",
                    Title = "ExecuteInTran_Test  ef",
                    Sort = 0,
                });

                await _uow.CommitTransactionAsync(ddsd);
            }

        }


        [Test]  
        public async Task Specification()
        {
            var spec = new BlogSpecification();
            var rep = _uow.GetRepository<IBloggingEfCoreRepository<Blog>>();

            var res = await rep.GetListAsync(spec);

            var page = await rep.GetPageListAsync(spec, 1, 20);

            Console.WriteLine(JsonConvert.SerializeObject(res));
            Console.WriteLine(JsonConvert.SerializeObject(page));

            var spec2 = new Blog2Specification();
            var res2 = await rep.GetListAsync(spec2);
            var page2 = await rep.GetPageListAsync(spec2,1,20);
            Console.WriteLine(JsonConvert.SerializeObject(res2));
            Console.WriteLine(JsonConvert.SerializeObject(page2));
        }
       
    }

}