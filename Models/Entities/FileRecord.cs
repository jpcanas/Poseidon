using System.ComponentModel.DataAnnotations;

namespace Poseidon.Models.Entities
{
    public class FileRecord
    {
        public int Id { get; set; }

        [Required]
        public string FileKey { get; set; } = string.Empty;
        public string? ThumbnailKey{ get; set; }

        [Required]
        public string OriginalFileName { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string ContentType { get; set; } = string.Empty;

        [Required]
        public long FileSizeBytes { get; set; } 
        public int ModuleId { get; set; }
        public int ReferenceId { get; set; }
        public int ModuleDocumentTypeId { get; set; }
        public DateTime? UploadedAt { get; set; }
        public string UploadedBy { get; set; } = string.Empty;
        public Module Module { get; set; } = null!;
        public ModuleDocumentType ModuleDocumentType { get; set; } = null!;
    }
}
