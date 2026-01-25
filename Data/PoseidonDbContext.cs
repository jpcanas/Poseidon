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

            //modelBuilder.Entity<User>()
            //    .Property(u => u.UserIdentifier)
            //    .HasDefaultValueSql("NEWID()");

            //modelBuilder.Entity<Role>()
            //    .Property(r => r.CreatedDate)
            //    .HasDefaultValueSql("GETDATE()");

            //modelBuilder.Entity<User>()
            //    .Property(u => u.CreatedDate)
            //    .HasDefaultValueSql("GETDATE()");

            //modelBuilder.Entity<User>()
            //    .HasIndex(u => u.UserIdentifier)
            //    .IsUnique(true);

            //modelBuilder.Entity<User>()
            //    .HasIndex(u => u.Email)
            //    .IsUnique(true);

            //modelBuilder.Entity<User>()
            //    .HasIndex(u => u.UserName)
            //    .IsUnique(true);

            //modelBuilder.Entity<User>()
            //  .HasOne(u => u.UserStatus)
            //  .WithMany(s => s.Users)
            //  .HasForeignKey(u => u.UserStatusId);

            //modelBuilder.Entity<UserStatus>()
            //   .Property(s => s.CreatedDate)
            //   .HasDefaultValueSql("GETDATE()");

            //modelBuilder.Entity<UserStatus>().HasData(
            //    new UserStatus
            //    {
            //        UserStatusId = 1,
            //        Name = "Active",
            //    }
            // );
        }
    }
}
