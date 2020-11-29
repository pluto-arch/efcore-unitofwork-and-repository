using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Transactions;
using apisample;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

using PlutoData.Interface;
using PlutoData.Interface.Base;
using PlutoData.Test.Repositorys.Dapper;

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


		private void EfInsertRange()
		{
			var entities=new List<Blog>();
            entities.Add(new Blog
            {
                Url = $"{r.Next(1,99999)}_efefefef",
                Title = $"{r.Next(1,99999)}_efefefef",
            });
			var efRep = _uow.GetBaseRepository<Blog>();
			efRep.Insert(entities);
			_uow.SaveChanges();
		}

		private void EfInsert()
		{
			var efRep = _uow.GetBaseRepository<Blog>();
			efRep.Insert(new Blog
			             {
				             Url = $"{r.Next(1,99999)}_efefefef",
				             Title = $"{r.Next(1,99999)}_efefefef",
			             });
			_uow.SaveChanges();
		}
		private void EfUpdate()
		{
			var efRep = _uow.GetBaseRepository<Blog>();
			var blog=efRep.GetFirstOrDefault(predicate:x=>x.Id>0,disableTracking:true);
			blog.Title=$"ef_update_{r.Next(1,888888)}";
			_uow.SaveChanges();
		}

		private void DapperInsert()
		{
			var dapperRep = _uow.GetDapperRepository<IBlogDapperRepository>();
			dapperRep.Insert(new
			                 {
				                 Url = $"{r.Next(1,99999)}_dapper",
				                 Title = $"{r.Next(1,99999)}_dapper",
			                 });
		}

	}

}