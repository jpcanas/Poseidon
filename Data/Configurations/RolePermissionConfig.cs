using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Poseidon.Models.Entities;

namespace Poseidon.Data.Configurations
{
    public class RolePermissionConfig : IEntityTypeConfiguration<RolePermission>
    {
        public void Configure(EntityTypeBuilder<RolePermission> builder)
        {
            builder.HasData(
                    new RolePermission { Id = 1, RoleId = 1, SubModuleId = 1 },
                    new RolePermission { Id = 2, RoleId = 1, SubModuleId = 2 },
                    new RolePermission { Id = 3, RoleId = 1, SubModuleId = 3 },
                    new RolePermission { Id = 4, RoleId = 1, SubModuleId = 4 },
                    new RolePermission { Id = 5, RoleId = 1, SubModuleId = 5 }
                );
        }
    }
}
