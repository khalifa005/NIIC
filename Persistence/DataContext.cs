using System;
using Domains;
using Microsoft.EntityFrameworkCore;

namespace Persistence
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions options)
        :base(options)
        {
            
        }

        public DbSet<Value> Values { get; set; }
        public DbSet<Activity> Activities { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Value>().HasData(
                new Value() { Id = 1, Name = "khalifa1" },
                new Value() { Id = 2, Name = "khalifa2" },
                new Value() { Id = 3, Name = "khalifa3" });

        }
    }

}
