using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Poseidon.Models.Entities;

namespace Poseidon.Data.Configurations
{
    public class UserStatusConfig : IEntityTypeConfiguration<UserStatus>
    {
        public void Configure(EntityTypeBuilder<UserStatus> builder)
        {

            builder
                .Property(u => u.CreatedDate)
                .HasDefaultValueSql("now()");

            builder.HasData(
                new UserStatus
                {
                    UserStatusId = 1,
                    Name = "Active",
                    Color = "",
                    IsActive = true,
                    AffectsAccess = false,
                    Description = "Default status for active users",
                    CreatedBy = "System"
                }
            );
            builder.HasData(
                new UserStatus
                {
                    UserStatusId = 2,
                    Name = "Inactive",
                    Color = "",
                    IsActive = true,
                    AffectsAccess = false,
                    Description = "Default status for inactive users",
                    CreatedBy = "System"
                }
            );
        }
    }
}
