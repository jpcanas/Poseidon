using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Poseidon.Models.Entities;

namespace Poseidon.Data.Configurations
{
    public class ModuleDocumentTypeConfig : IEntityTypeConfiguration<ModuleDocumentType>
    {
        public void Configure(EntityTypeBuilder<ModuleDocumentType> builder)
        {
            builder.HasIndex(dt => new { dt.ModuleId, dt.DocumentTypeName })
                .IsUnique();

            builder.HasMany(dt => dt.FileRecords)
                .WithOne(fr => fr.ModuleDocumentType)
                .HasForeignKey(fr => fr.ModuleDocumentTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasData(new ModuleDocumentType
            {
                Id = 1,
                ModuleId = 1,
                DocumentTypeName = "Profile Picture",
                Description = "Profile pictures uploaded by users for their accounts"
            });
        }
    }
}
