using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Poseidon.Models.Entities;

namespace Poseidon.Data.Configurations
{
    public class SubModuleConfig : IEntityTypeConfiguration<SubModule>
    {
        public void Configure(EntityTypeBuilder<SubModule> builder)
        {
            builder.HasMany(s => s.RolePermissions)
                .WithOne(rp => rp.SubModule)
                .HasForeignKey(rp => rp.SubModuleId);

            builder.HasData(
                 new SubModule { Id = 1, ModuleId = 1, Name = "Assign Roles", Code = "UAC_ASSIGN_ROLES" },
                 new SubModule { Id = 2, ModuleId = 1, Name = "Add User", Code = "UAC_ADD_USER" },
                 new SubModule { Id = 3, ModuleId = 1, Name = "Remove User", Code = "UAC_REMOVE_USER" },
                 new SubModule { Id = 4, ModuleId = 1, Name = "View User list", Code = "UAC_VIEW_USERLIST" },
                 new SubModule { Id = 5, ModuleId = 1, Name = "View Roles", Code = "UAC_VIEW_ROLES" }
            );
        }
    }
}
