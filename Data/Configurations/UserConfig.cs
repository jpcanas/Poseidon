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
               .HasOne(u => u.Role)
               .WithMany(r => r.Users)
               .HasForeignKey(u => u.RoleId);

            builder
                .Property(u => u.UserIdentifier)
                .HasDefaultValueSql("gen_random_uuid()"); //postgresql

            builder
                .Property(u => u.CreatedDate)
                .HasDefaultValueSql("now()");

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

            // Seed initial admin user
            builder.HasData(
                new User
                {
                    UserId = 1,
                    UserIdentifier = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                    UserName = "admin",
                    Password = "$2a$12$MZ9oNNn/RfLnn2ipec5FzeeXuTCiHAZDw44Hd3rD8zomXZjOV2Fe6",
                    Email = "admin@example.com",
                    RoleId = 1,
                    FirstName = "System",
                    LastName = "Administrator",
                    UserStatusId = 1,
                    RequiredPasswordChange = false,
                    CreatedBy = "System"

                }
            );

            builder           
            .Property(u => u.BirthDate)       
            .HasColumnType("date");           
        }
    }
}
