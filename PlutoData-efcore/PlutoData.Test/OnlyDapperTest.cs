using Dapper;
using Microsoft.Data.SqlClient;
using NUnit.Framework;
using PlutoData.Enums;
using PlutoData.Test.Repositorys.Dapper;

namespace PlutoData.Test
{
	[TestFixture]
	public class OnlyDapperTest : BaseTest
	{
		public OnlyDapperTest() : base(Flag.Dapper)
		{
		}
        
        [Test]
        public void Demo()
        {
            var ddd = _dapperUnitOfWork.GetRepository<IBloggingDapperRepository<Blog>>();
            var ddd2 = _dapperUnitOfWork.GetRepository<IBloggingDapperRepository<Post>>();
        }
        
        

        [Test]
        public void GetRepository()
        {
            //var rep1 = _provider.GetService<IBloggingDapperRepository<Blog>>();
            //Assert.IsTrue(rep1!=null&rep1 is IDapperRepository);
            //var blogCustomerRep = _dapperUnitOfWork.GetRepository<IBlogDapperRepository>();
            //Assert.IsTrue(blogCustomerRep!=null&blogCustomerRep is IDapperRepository);
            //var baseRep = _dapperUnitOfWork.GetRepository<IBloggingDapperRepository<Blog>>();
            //Assert.IsTrue(baseRep!=null&baseRep is IDapperRepository);
        }

        [Test]
        public void ExecuteInTransactionAsync_Success_Test()
        {
            var ddd = _dapperUnitOfWork.GetRepository<IBloggingDapperRepository<Blog>>();
            var ddd2 = _dapperUnitOfWork.GetRepository<IBloggingDapperRepository<Post>>();
            bool result = false;
            using (var tran = _dapperUnitOfWork.BeginTransaction())
            using (ddd.UseTransaction(tran))
            using (ddd2.UseTransaction(tran))
            {
                var res = ddd.Execute(conn =>
                {
                    return conn.Insert(new Blog
                    {
                        Url = "ExecuteInTransactionAsync_Success_Test",
                        Title = "ExecuteInTransactionAsync_Success_Test",
                        Sort = 0,
                        Posts = null
                    },tran);
                });

                var res2= ddd2.Execute(conn =>
                {
                    return conn.Insert(new Post
                    {
                        BlogId = res.Value,
                        Title = "ExecuteInTransactionAsync_Success_Test",
                        Content = "ExecuteInTransactionAsync_Success_Test",
                    },tran);
                });
                _dapperUnitOfWork.CommitTransaction(tran);
                result = res > 0 & res2 > 0;
            }
            Assert.IsTrue(result);

            var repHasTran = ddd.HasActiveTransaction;
            Assert.IsFalse(repHasTran);
        }

        [Test]
        public void ExecuteInTransactionAsync_Fail_Test()
        {
            var ddd = _dapperUnitOfWork.GetRepository<IBloggingDapperRepository<Blog>>();
            var ddd2 = _dapperUnitOfWork.GetRepository<IBloggingDapperRepository<Post>>();
            try
            {
                using (var tran = _dapperUnitOfWork.BeginTransaction())
                using (ddd.UseTransaction(tran))
                using (ddd2.UseTransaction(tran))
                {
                    var res = ddd.Execute(conn =>
                    {
                        return conn.Execute("insert", tran);
                    });

                    ddd2.Execute(conn =>
                    {
                        return conn.Execute("insert", tran);
                    });
                    _dapperUnitOfWork.CommitTransaction(tran);
                }
            }
            catch (SqlException ex)
            {
                Assert.IsNotNull(ex);
                Assert.IsTrue(ex.Message.Contains("FOREIGN KEY"));
            }
        }

    }
}