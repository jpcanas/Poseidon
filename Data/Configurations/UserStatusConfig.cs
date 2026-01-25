using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Poseidon.Models.Entities;

namespace Poseidon.Data.Configurations
{
    public class UserStatusConfig : IEntityTypeConfiguration<UserStatus>
    {
        public void Configure(EntityTypeBuilder<UserStatus> builder)
        {
            builder.Property(s => s.CreatedDate)
               .HasDefaultValueSql("GETDATE()");

            builder.HasData(
                new UserStatus
                {
                    UserStatusId = 1,
                    Name = "Active",
                }
            );
        }
    }
}
