using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Poseidon.Models.Entities;

namespace Poseidon.Data.Configurations
{
    public class RoleConfig : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder
                .Property(u => u.CreatedDate)
                .HasDefaultValueSql("now()");

            builder.HasData(new Role
            {
                RoleId = 1,
                RoleName = "Admin",
                RoleType = "System",
                Description = "System administrator with full access",
                IsActive = true,
                CreatedBy = "System"
            });

            builder.HasData(new Role
            {
                RoleId = 2,
                RoleName = "User",
                RoleType = "Default",
                Description = "Standard application user",
                IsActive = true,
                CreatedBy = "System"
            });
        }
    }
}
