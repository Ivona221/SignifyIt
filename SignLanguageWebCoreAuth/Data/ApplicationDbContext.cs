using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SignLanguageWebCoreAuth.Models;

namespace SignLanguageWebCoreAuth.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        IConfiguration configuration;
        public ApplicationDbContext()
        {
        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IConfiguration _configuration)
            : base(options)
        {
            configuration = _configuration;
        }

        public static string GetConnectionString()
        {
            return Startup.ConnectionString;
        }

        public DbSet<Synonyms> Synonyms { get; set; }
        public DbSet<Tags> Tags { get; set; }
        public DbSet<Users> Users { get; set; }
        public DbSet<Role> Role { get; set; }
        //public DbSet<IdentityUser> User { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var con = GetConnectionString();
            optionsBuilder.UseMySQL(con);
            //optionsBuilder.UseMySQL(configuration["ConnectionStrings:DefaultConnection"]);
            //optionsBuilder.UseSqlServer(
            // @"Data Source=.\SQLEXPRESS;Initial Catalog=SignLanguage;Integrated Security=True;MultipleActiveResultSets=True");
        }

    }
}
