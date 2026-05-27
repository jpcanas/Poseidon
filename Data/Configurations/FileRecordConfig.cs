using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Poseidon.Models.Entities;

namespace Poseidon.Data.Configurations
{
    public class FileRecordConfig : IEntityTypeConfiguration<FileRecord>
    {
        public void Configure(EntityTypeBuilder<FileRecord> builder)
        {
            builder
               .Property(u => u.UploadedAt)
               .HasDefaultValueSql("now()");

            builder.HasIndex(f => new { f.ModuleId, f.ReferenceId });
        }
    }
}
