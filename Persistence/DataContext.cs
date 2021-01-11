using System;
using Domains;
using Domains.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Persistence
{
    public class DataContext : IdentityDbContext<AppUser>
    {
        public DataContext(DbContextOptions options)
        :base(options)
        {
            
        }

        public DbSet<Value> Values { get; set; }
        public DbSet<Activity> Activities { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //without this line app will break , gives us the ability when creating migration to set id of identity user as string
            base.OnModelCreating(modelBuilder);

            //seed data without repeating
            modelBuilder.Entity<Value>().HasData(
                new Value() { Id = 1, Name = "khalifa1" },
                new Value() { Id = 2, Name = "khalifa2" },
                new Value() { Id = 3, Name = "khalifa3" });

        }
    }

}
