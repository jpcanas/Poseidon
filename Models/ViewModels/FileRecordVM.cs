using System.ComponentModel.DataAnnotations;

namespace Poseidon.Models.ViewModels
{
    public class FileRecordVM
    {
        public int Id { get; set; }
        public string FileKey { get; set; } = string.Empty;
        public string? ThumbnailKey { get; set; }
        public string OriginalFileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public long FileSizeBytes { get; set; }
        public int ModuleId { get; set; }
        public int ReferenceId { get; set; }
        public int ModuleDocumentTypeId { get; set; }
    }
}
