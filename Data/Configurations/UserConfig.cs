using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Poseidon.Models.Entities;
using System.Reflection.Emit;

namespace Poseidon.Data.Configurations
{
    public class UserConfig : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder
               .HasOne(u => u.Role) // join with Role
               .WithMany(r => r.Users)
               .HasForeignKey(u => u.RoleId);

            builder
                .Property(u => u.UserIdentifier)
                .HasDefaultValueSql("NEWID()");

            builder
                .Property(u => u.CreatedDate)
                .HasDefaultValueSql("GETDATE()");

            builder
                .HasIndex(u => u.UserIdentifier)
                .IsUnique(true);

            builder
                .HasIndex(u => u.Email)
                .IsUnique(true);

            builder
                .HasIndex(u => u.UserName)
                .IsUnique(true);

            builder
              .HasOne(u => u.UserStatus) // join with UserStatus
              .WithMany(s => s.Users)
              .HasForeignKey(u => u.UserStatusId);
        }
    }
}
