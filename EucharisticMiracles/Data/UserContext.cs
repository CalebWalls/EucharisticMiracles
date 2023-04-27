using EucharisticMiracles.Models;
using Microsoft.EntityFrameworkCore;

namespace EucharisticMiracles.Data
{
    public class UserContext : DbContext
    {
        public UserContext(DbContextOptions<UserContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Users>().HasKey(x => x.Username);
            modelBuilder.Entity<MiracleLocations>().HasKey(x => x.Id);
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Users> Users { get; set; }
        public DbSet<MiracleLocations> MiracleLocations { get; set; }
    }
}
