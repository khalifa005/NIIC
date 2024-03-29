﻿using System;
using Domains;
using Domains.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Persistence
{
    public class DataContext : IdentityDbContext<AppUser>
    {
        public DataContext(DbContextOptions options)
        : base(options)
        {

        }

        public DbSet<Value> Values { get; set; }
        public DbSet<Activity> Activities { get; set; }
        public DbSet<UserActivity> UserActivities { get; set; }
        public DbSet<Photo> Photos { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<UserFollowing> Followings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //without this line app will break , gives us the ability when creating migration to set id of identity user as string
            base.OnModelCreating(modelBuilder);


            //define composite key
            modelBuilder.Entity<UserActivity>().HasKey(ua => new { ua.AppUserId, ua.ActivityId });


            modelBuilder.Entity<UserActivity>()
                .HasOne(a => a.AppUser)
                .WithMany(u => u.UserActivities)
                .HasForeignKey(a => a.AppUserId);

            modelBuilder.Entity<UserActivity>()
                .HasOne(a => a.Activity)
                .WithMany(ua => ua.UserActivities)
                .HasForeignKey(a => a.ActivityId);

            modelBuilder.Entity<Comment>(entity =>
            {
                entity
                .HasMany(c => c.Replies)
                .WithOne(p => p.ParentComment)
                .HasForeignKey(f => f.ParentCommentId);
            });

            modelBuilder.Entity<UserFollowing>(x =>
            {
                x.HasKey(u=> new { u.ObserverId, u.TargetId });

                x.HasOne(u=> u.Observer)
                .WithMany(f=> f.Followings)
                .HasForeignKey(u=> u.ObserverId)
                .OnDelete(DeleteBehavior.Restrict);

                //important to make ef diffrentiate between the relation because both refrecing to the same class 
                x.HasOne(u=> u.Target)
                .WithMany(f=> f.Followers)
                .HasForeignKey(u=> u.TargetId)
                .OnDelete(DeleteBehavior.Restrict);

                
            });

            //seed data without repeating
            modelBuilder.Entity<Value>().HasData(
                new Value() { Id = 1, Name = "khalifa1" },
                new Value() { Id = 2, Name = "khalifa2" },
                new Value() { Id = 3, Name = "khalifa3" });

            modelBuilder.Entity<IdentityUser>(entity =>
            {
                entity.ToTable(name: "User");
            });
            modelBuilder.Entity<IdentityRole>(entity =>
            {
                entity.ToTable(name: "Role");
            });
            modelBuilder.Entity<IdentityUserRole<string>>(entity =>
            {
                entity.ToTable("UserRoles");
            });
            modelBuilder.Entity<IdentityUserClaim<string>>(entity =>
            {
                entity.ToTable("UserClaims");
            });
            modelBuilder.Entity<IdentityUserLogin<string>>(entity =>
            {
                entity.ToTable("UserLogins");
            });
            modelBuilder.Entity<IdentityRoleClaim<string>>(entity =>
            {
                entity.ToTable("RoleClaims");
            });
            modelBuilder.Entity<IdentityUserToken<string>>(entity =>
            {
                entity.ToTable("UserTokens");
            });

        }
    }

}
