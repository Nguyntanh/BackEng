using Microsoft.EntityFrameworkCore;
using signup.Models;
using System.Collections.Generic;

namespace signup.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Account { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Đặt Email làm khóa chính và unique
            modelBuilder.Entity<User>()
                .HasKey(u => u.Email);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();
        }
    }
}