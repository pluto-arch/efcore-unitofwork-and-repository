using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using apisample;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using Newtonsoft.Json;

using NUnit.Framework;

using PlutoData.Interface;
using PlutoData.Interface.Base;
using PlutoData.Test.Repositorys;
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
		public void Insert()
		{
			var repository = _uow.GetRepository<Repositorys.Ef.ICustomBlogRepository>();
			IList<Blog> d = new List<Blog>();
            for (int i = 0; i < 1222; i++)
            {
				d.Add(new Blog { 
					Url=$"Blog_{i}",
					Title=$"{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss:fff}",
					Posts=new List<Post>
                    {
						new Post
                        {
							Title=$"Blog_{i}_{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss:fff}",
							Content=$"Blog_{i}_{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss:fff}",
						}
                    }
				});
            }
			repository.Insert(d);
			_uow.SaveChanges();
		}



		[Test]
        public async Task EfBaseRepository()
        {

			var sdsds = _provider.GetService<Repositorys.Ef.ICustomBlogRepository>();

			var ddasd = _uow.GetRepository<Repositorys.Ef.ICustomBlogRepository>();

			var sdsdsc = _uow.GetRepository<IBloggingEfCoreRepository<Blog>>();


			var sdsdss = _provider.GetService<IDapperRepository<Blog>>();


			var sdsdxcsd= sdsdss.GetPageList(null, 1, 20);

			var model = await ddasd.GetPagedListAsync(
				predicate: t => t.Id > 0&&EF.Functions.Like(t.Url, "%g_81%"), 
				include: z => z.Include(d => d.Posts),
				orderBy:z=>z.OrderByDescending(t=>t.Id).ThenBy(d=>d.Title));
            //Assert.IsTrue(model.All(x => x.Posts != null));



            var spec = new ProjectPagingSpecification();
            var ddd = await ddasd.GetPageListAsync(spec,1,20);
            Console.WriteLine(ddd);
        }

        [Test]
        public void DapperBaseRepository()
        {
	        var repository = _dapperUnitOfWork.GetRepository<IBlogDapperRepository>();
			Assert.IsTrue(repository!=null&&(repository is IDapperRepository));
        }

        [Test]
		public async Task Mix_EfTransaction_With_Error()
		{
			var dapperRep = _dapperUnitOfWork.GetRepository<IBlogDapperRepository>();
			var efRep = _uow.GetRepository<Repositorys.Ef.ICustomBlogRepository>();
			var tr=await _uow.BeginTransactionAsync();
			try
			{
				using (tr)
				{
					var entity = dapperRep.GetAll();

					efRep.Insert(new Blog
					             {
						             Url = "Mix_EfTransaction_With_Error",
						             Title = "Mix_EfTransaction_With_Error",
					             });

					dapperRep.Insert(new Blog
					                 {
						                 Id = 32, 
						                 Url = "Mix_EfTransaction_With_Error",
						                 Title = "Mix_EfTransaction_With_Error",
					                 });
				
					await _uow.SaveChangesAsync();
					await tr.CommitAsync();
				}
			}
			catch
			{
				await tr.RollbackAsync();
			}
		}


        [Test]
        public async Task Mix_EfTransaction_With_Success()
        {
	        var dapperRep = _dapperUnitOfWork.GetRepository<IBlogDapperRepository>();
	        var efRep = _uow.GetRepository<Repositorys.Ef.ICustomBlogRepository>();
			var tr=await _uow.BeginTransactionAsync();
	        try
	        {
		        using (tr)
		        {
			        var entity = dapperRep.GetAll();
			        dapperRep.Insert(new Blog
			                         {
				                         Url = "Mix_EfTransaction_With_Success",
				                         Title = "Mix_EfTransaction_With_Success",
			                         });
			        efRep.Insert(new Blog
			                     {
				                     Url = "Mix_EfTransaction_With_Success",
				                     Title = "Mix_EfTransaction_With_Success",
			                     });
			        await _uow.SaveChangesAsync();
			        await tr.CommitAsync();
		        }
	        }
	        catch(Exception)
	        {
		        await tr.RollbackAsync();
	        }
        }


        [Test]
        public void Mix_TransactionScope_With_Error()
        {
	        var dapperRep = _dapperUnitOfWork.GetRepository<IBlogDapperRepository>();
	        var efRep = _uow.GetRepository<Repositorys.Ef.ICustomBlogRepository>(); 
            Assert.Throws<DbUpdateException>(()=>
                                             {
                                                 var transactionOption = new TransactionOptions
                                                 {
                                                     IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted,
                                                     Timeout = new TimeSpan(0, 0, 120)
                                                 };
                                                 using var scope = new TransactionScope(TransactionScopeOption.Required, transactionOption);
                                                 var entity = dapperRep.GetAll();
                                                 dapperRep.Insert(new Blog
                                                 {
                                                     Url = "Mix_TransactionScope_With_Error",
                                                     Title = "Mix_TransactionScope_With_Error",
                                                 });
                                                 efRep.Insert(new Blog
                                                 {
                                                     Id = 32,
                                                     Url = "Mix_TransactionScope_With_Error",
                                                     Title = "Mix_TransactionScope_With_Error",
                                                 });
                                                 _uow.SaveChanges();
                                                 scope.Complete();
                                             });

        }

		/// <summary>
		/// 混合 
		/// </summary>
        [Test]
        public void Mix_TransactionScope_With_Success()
        {
            var transactionOption = new TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted,
                Timeout = new TimeSpan(0, 0, 120)
            };

            var dapperRep = _dapperUnitOfWork.GetRepository<IBlogDapperRepository>();
	        var efRep = _uow.GetRepository<Repositorys.Ef.ICustomBlogRepository>();

			using var scope = new TransactionScope(TransactionScopeOption.Required, transactionOption);
            #region dapper
            var entity = dapperRep.GetAll();
            var res = dapperRep.Insert(new Blog
            {
                Url = "Mix_TransactionScope_With_Success",
                Title = "Mix_TransactionScope_With_Success",
            });
            #endregion

            #region ef
            var res2 = efRep.Insert(new Blog
            {
                Url = "Mix_TransactionScope_With_Success",
                Title = "Mix_TransactionScope_With_Success",
            });
            _uow.SaveChanges();
            #endregion

            scope.Complete();
            Assert.IsTrue(res && (res2.Id > 0));
        }


		[Test]
		public void Multi_Operator()
		{
			for (int i = 0; i < 4000; i++)
			{
				EfInsert();
				EfUpdate();
				DapperInsert();
			}
		}

		[Test]
		public void EfInsertRange()
		{
            var entities = new List<Blog>
            {
                new Blog
                {
                    Url = $"{r.Next(1, 99999)}_efefefef",
                    Title = $"{r.Next(1, 99999)}_efefefef",
                }
            };
            var efRep = _uow.GetRepository<Repositorys.Ef.ICustomBlogRepository>();
			efRep.Insert(entities);
			_uow.SaveChanges();
		}

		private void EfInsert()
		{
			var efRep = _uow.GetRepository<Repositorys.Ef.ICustomBlogRepository>();
			efRep.Insert(new Blog
			             {
				             Url = $"{r.Next(1,99999)}_efefefef",
				             Title = $"{r.Next(1,99999)}_efefefef",
			             });
			_uow.SaveChanges();
		}
		private void EfUpdate()
		{
			var efRep = _uow.GetRepository<Repositorys.Ef.ICustomBlogRepository>();
			var blog=efRep.GetFirstOrDefault(predicate:x=>x.Id>0,disableTracking:true);
			blog.Title=$"ef_update_{r.Next(1,888888)}";
			_uow.SaveChanges();
		}

		private void DapperInsert()
		{
			var dapperRep = _uow.GetDapperRepository<IBlogDapperRepository,DapperDbContext>();
			dapperRep.Insert(new
			                 {
				                 Url = $"{r.Next(1,99999)}_dapper",
				                 Title = $"{r.Next(1,99999)}_dapper",
			                 });
		}

	}

}