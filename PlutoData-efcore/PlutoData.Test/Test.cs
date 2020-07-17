using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Newtonsoft.Json;
using NUnit.Framework;
using PlutoData.Collections;

namespace PlutoData.Test
{
    [TestFixture]
    public class Test
    {

        private InMemoryContext db;

        private InMemory2Context db2;

        [SetUp]
        public void SetUp()
        {
            db = new InMemoryContext();
            if (db.Countries.Any() == false)
            {
                db.AddRange(TestCountries);
                db.AddRange(TestCities);
                db.AddRange(TestTowns);
                db.AddRange(TestItems());
                db.SaveChanges();
            }


            db2 = new InMemory2Context();
            if (db2.Countries.Any() == false)
            {
                db2.AddRange(TestCountries);
                db2.AddRange(TestCities);
                db2.AddRange(TestTowns);
                db2.AddRange(TestItems());
                db2.SaveChanges();
            }

        }

        [Test]
        public async Task TestGetFirstOrDefaultAsyncGetsCorrectItem()
        {
            var repository = new Repository<Country>();
            var city = await repository.GetFirstOrDefaultAsync(predicate: t => t.Name == "A");
            Assert.NotNull(city);
            Assert.AreEqual(1, city.Id);
        }


        [Test]
        public async Task TestGetFirstOrDefaultAsyncReturnsNullValue()
        {
            var repository = new Repository<Country>();
            var city = await repository.GetFirstOrDefaultAsync(predicate: t => t.Name == "Easy-E");
            Assert.Null(city);
        }

        [Test]
        public async Task TestGetFirstOrDefaultAsyncCanInclude()
        {
            var repository = new Repository<City>();
            var city = await repository.GetFirstOrDefaultAsync(
                predicate: c => c.Name == "A",
                include: source => source.Include(t => t.Towns));
            Assert.NotNull(city);
            Assert.NotNull(city.Towns);
        }





        [Test]
        public void GetPagedList()
        {
            var repository = new Repository<City>();
            var page = repository.GetPagedList(predicate:null, include: source => source.Include(t => t.Country));

            Assert.NotNull(page.TotalCount==6);
        }



        [Test]
        public async Task ToPagedListAsyncTest()
        {
            var items = db.Customers.Where(t => t.Age > 1);

            var page = await items.ToPagedListAsync(1, 2);

            Assert.NotNull(page);

        }







        #region 数据


        public List<Customer> TestItems()
        {
            return new List<Customer>()
            {
                new Customer(){Name="A", Age=1},
                new Customer(){Name="B", Age=1},
                new Customer(){Name="C", Age=2},
                new Customer(){Name="D", Age=3},
                new Customer(){Name="E", Age=4},
                new Customer(){Name="F", Age=5},
            };
        }

        protected static List<Country> TestCountries => new List<Country>
        {
            new Country {Id = 1, Name = "A"},
            new Country {Id = 2, Name = "B"}
        };

        public static List<City> TestCities => new List<City>
        {
            new City { Id = 1, Name = "A", CountryId = 1},
            new City { Id = 2, Name = "B", CountryId = 2},
            new City { Id = 3, Name = "C", CountryId = 1},
            new City { Id = 4, Name = "D", CountryId = 2},
            new City { Id = 5, Name = "E", CountryId = 1},
            new City { Id = 6, Name = "F", CountryId = 2},
        };

        public static List<Town> TestTowns => new List<Town>
        {
            new Town { Id = 1, Name="TownA", CityId = 1 },
            new Town { Id = 2, Name="TownB", CityId = 2 },
            new Town { Id = 3, Name="TownC", CityId = 3 },
            new Town { Id = 4, Name="TownD", CityId = 4 },
            new Town { Id = 5, Name="TownE", CityId = 5 },
            new Town { Id = 6, Name="TownF", CityId = 6 },
        };

        #endregion


    }
}
