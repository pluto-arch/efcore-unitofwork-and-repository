using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace PlutoData.Test
{
    public class InMemoryContext: DbContext
    {
        public DbSet<Country> Countries { get; set; }
        public DbSet<Customer> Customers { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase("test");
        }

    }









    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
    }







    public class Country
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public List<City> Cities { get; set; }
    }

    public class City
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public int CountryId { get; set; }

        public Country Country { get; set; }

        public List<Town> Towns { get; set; }
    }

    public class Town
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public int CityId { get; set; }

        public City City { get; set; }
    }
}