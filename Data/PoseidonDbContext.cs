using Microsoft.EntityFrameworkCore;
using Poseidon.Data.Configurations;
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
        public DbSet<PasswordResetToken> PasswordResetTokens { get; set; }
        public DbSet<UserStatus> UserStatuses { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //modelBuilder.ApplyConfiguration(new RoleConfig());
            //new UserConfig().Configure(modelBuilder.Entity<User>());

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(PoseidonDbContext).Assembly); // include all config

            //modelBuilder.Entity<User>()
            //    .HasOne(u => u.Role)
            //    .WithMany(r => r.Users)
            //    .HasForeignKey(u => u.RoleId);
        }
    }
}
