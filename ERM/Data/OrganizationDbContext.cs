using Microsoft.EntityFrameworkCore;
using Organization.DataModels;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Organization.Data
{
    // ✅ DbContext class used to interact with the database
    public class OrganizationDbContext : DbContext
    {
        // ✅ Constructor — gets connection info from Program.cs
        public OrganizationDbContext(DbContextOptions<OrganizationDbContext> options)
            : base(options)
        {
            //Removed OnConfiguring()
            //Added constructor injection(ASP.NET Core style)
        }
        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    // ✅ Seeding 10 employee records
        //    modelBuilder.Entity<employee>().HasData(
        //        new employee { Sid = 1, SName = "John", SEmail = "john@email.com" },
        //        new employee { Sid = 2, SName = "Alice", SEmail = "alice@email.com" },
        //        new employee { Sid = 3, SName = "Bob", SEmail = "bob@email.com" },
        //        new employee { Sid = 4, SName = "David", SEmail = "david@email.com" },
        //        new employee { Sid = 5, SName = "Emma", SEmail = "emma@email.com" },
        //        new employee { Sid = 6, SName = "Frank", SEmail = "frank@email.com" },
        //        new employee { Sid = 7, SName = "Grace", SEmail = "grace@email.com" },
        //        new employee { Sid = 8, SName = "Henry", SEmail = "henry@email.com" },
        //        new employee { Sid = 9, SName = "Ivy", SEmail = "ivy@email.com" },
        //        new employee { Sid = 10, SName = "Jack", SEmail = "jack@email.com" }
        //    );
        //}


        // ✅ DbSets represent your database tables
        public DbSet<Employee> Emploees {get; set;}
    }
}
