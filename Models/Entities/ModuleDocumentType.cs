using System.ComponentModel.DataAnnotations;

namespace Poseidon.Models.Entities
{
    public class ModuleDocumentType
    {
        public int Id { get; set; }
        public int ModuleId { get; set; }
        [Required]
        [MaxLength(200)]
        public string DocumentTypeName { get; set; } = string.Empty;
        [MaxLength(255)]
        public string? Description { get; set; } = string.Empty;
        public Module Module { get; set; } = null!;
        public List<FileRecord> FileRecords { get; set; } = new();
    }
}
