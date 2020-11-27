using apisample;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using PlutoData.Test.Repositorys.Dapper;
using PlutoData.Uows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.Data.SqlClient;
using PlutoData.Interface.Base;

namespace PlutoData.Test
{
	[TestFixture]
    public class MixTest
    {

        private IServiceProvider _provider;
        private IEfUnitOfWork<BloggingContext> _uow;
        private IDapperUnitOfWork _dapperUnitOfWork;
        [SetUp]
        public void SetUp()
        {
            var service=new ServiceCollection();
            service.AddControllers();
            service.AddUnitOfWorkDbContext<BloggingContext>(opt =>
                                                            {
	                                                            opt.UseSqlServer(
	                                                             "Server =.;Database = PlutoDataDemo;User ID = sa;Password = 123456;Trusted_Connection = False;");
	                                                            opt.UseLoggerFactory(new LoggerFactory(new[] { new EFLoggerProvider() }));
                                                            });
            service.AddRepository();
            //service.AddDapperUnitOfWork("Server =.;Database = PlutoDataDemo;User ID = sa;Password = 123456;Trusted_Connection = False;");
            service.AddDapperUnitOfWork<BloggingContext>();
            service.AddScoped<IBlogDapperRepository,BlogDapperRepository>();
            _provider = service.BuildServiceProvider();
            _uow=_provider.GetService<IEfUnitOfWork<BloggingContext>>();
            _dapperUnitOfWork=_provider.GetService<IDapperUnitOfWork>();
        }

        [Test]
        public async Task EfBaseRepository()
        {
			var repository = _uow.GetBaseRepository<Blog>();
			var model = await repository.GetFirstOrDefaultAsync(predicate: t => t.Title=="123");
			Assert.IsNull(model);
		}

        [Test]
        public void DapperBaseRepository()
        {
	        var repository = _dapperUnitOfWork.GetBaseRepository<Blog>();
			Assert.IsTrue(repository!=null&&(repository is IDapperRepository));
        }

        [Test]
		public async Task Mix_EfTransaction_With_Error()
		{
			var dapperRep = _dapperUnitOfWork.GetRepository<IBlogDapperRepository>();
			var efRep = _uow.GetBaseRepository<Blog>();
			var tr=await _uow.BeginTransactionAsync();
			try
			{
				using (tr)
				{
					var entity = dapperRep.GetAll();
					dapperRep.Insert(new Blog
					                 {
						                 Url = "Mix_EfTransaction_With_Error",
						                 Title = "Mix_EfTransaction_With_Error",
					                 });
					efRep.Insert(new Blog
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
	        var efRep = _uow.GetBaseRepository<Blog>();
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
	        catch
	        {
		        await tr.RollbackAsync();
	        }
        }


        [Test]
        public void Mix_TransactionScope_With_Error()
        {
	        var dapperRep = _dapperUnitOfWork.GetRepository<IBlogDapperRepository>();
	        var efRep = _uow.GetBaseRepository<Blog>();
            Assert.Throws<DbUpdateException>(()=>
                                             {
	                                             var transactionOption = new TransactionOptions();
	                                             transactionOption.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted;
	                                             transactionOption.Timeout = new TimeSpan(0, 0, 120);
	                                             using (var scope=new TransactionScope(TransactionScopeOption.Required, transactionOption))
	                                             {
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
	                                             }
                                             });

        }

		/// <summary>
		/// 混合 
		/// </summary>
        [Test]
        public void Mix_TransactionScope_With_Success()
        {
	        var transactionOption = new TransactionOptions();
	        transactionOption.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted;
	        transactionOption.Timeout = new TimeSpan(0, 0, 120);
	        
	        var dapperRep = _dapperUnitOfWork.GetRepository<IBlogDapperRepository>();
	        var efRep = _uow.GetBaseRepository<Blog>();

	        using (var scope=new TransactionScope(TransactionScopeOption.Required, transactionOption))
	        {
		        #region dapper
		        var entity = dapperRep.GetAll();
		        var res= dapperRep.Insert(new Blog
		                                  {
			                                  Url = "Mix_TransactionScope_With_Success",
			                                  Title = "Mix_TransactionScope_With_Success",
		                                  });
		        #endregion

		        #region ef
		        var res2= efRep.Insert(new Blog
		                               {
			                               Url = "Mix_TransactionScope_With_Success",
			                               Title = "Mix_TransactionScope_With_Success",
		                               });
		        _uow.SaveChanges();
		        #endregion

		        scope.Complete();
		        Assert.IsTrue(res&&(res2.Id>0));
	        }
        }


    }
}
