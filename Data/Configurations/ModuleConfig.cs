using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Poseidon.Models.Entities;

namespace Poseidon.Data.Configurations
{
    public class ModuleConfig : IEntityTypeConfiguration<Module>
    {
        public void Configure(EntityTypeBuilder<Module> builder)
        {
            builder.HasMany(m => m.SubModules)
                .WithOne(sm => sm.Module)
                .HasForeignKey(sm => sm.ModuleId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasData(new Module
            {
                Id = 1,
                Name = "User and Access Control",
                Description = "Manage people and their access levels to ensure secure and compliant usage of the app"
            });
        }
    }
}
