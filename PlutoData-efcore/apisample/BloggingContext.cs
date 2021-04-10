using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using PlutoData.Models;


namespace apisample
{
    public class BloggingContext : DbContext
    {
        public BloggingContext(DbContextOptions<BloggingContext> options)
            : base(options)
        {
        }

        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Post> Posts { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        }

    }


    #region entitys

    [Table("Blogs")]
    public class Blog:BaseEntity<int>
    {
        public string Url { get; set; }
        public string Title { get; set; }

        public int Sort { get; set; }

        public List<Post> Posts { get; set; }
    }

    [Table("Posts")]
    public class Post:BaseEntity<int>
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }

        public List<Comment> Comments { get; set; }
    }

    public class Comment:BaseEntity<int>
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
    }
    #endregion
}