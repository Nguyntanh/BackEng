using Microsoft.EntityFrameworkCore;
using register_login.Models;
using System.Collections.Generic;

namespace register_login.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<User> Users { get; set; }
    }
}
