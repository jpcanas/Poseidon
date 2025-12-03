using Microsoft.EntityFrameworkCore;
using Poseidon.Models.Entities;

namespace Poseidon.Data
{
    public class PoseidonDbContext : DbContext
    {
        public PoseidonDbContext(DbContextOptions options) : base(options) 
        {
            
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId);

            modelBuilder.Entity<User>()
                .Property(u => u.UserIdentifier)
                .HasDefaultValueSql("NEWID()");
        }
    }
}
