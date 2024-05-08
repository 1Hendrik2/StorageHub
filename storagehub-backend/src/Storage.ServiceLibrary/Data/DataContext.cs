using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Storage.ServiceLibrary.Entities;
using StorageHub.ServiceLibrary.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorageHub.ServiceLibrary.Data
{
    public class DataContext: IdentityDbContext<UserEntity>
    {
        public DataContext(DbContextOptions<DataContext> options): base(options) 
        {

        }

        public DbSet<StorageEntity> Storages { get; set; }
        public DbSet<UserEntity> AppUsers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            List<IdentityRole> roles = new List<IdentityRole>
            {
                new IdentityRole
                {
                    Name = "Admin",
                    NormalizedName = "ADMIN"
                },
                new IdentityRole
                {
                    Name = "User",
                    NormalizedName = "USER"
                },
            };
            modelBuilder.Entity<IdentityRole>().HasData(roles);
            modelBuilder.Entity<UserEntity>()
                .HasMany(u => u.Storages)
                .WithOne(s => s.User)
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<StorageEntity>()
                .HasOne(s => s.ParentStorage)
                .WithMany(s => s.ChildStorages)
                .HasForeignKey(s => s.ParentStorageId)
                .IsRequired(false);
                
        }
    }
}
