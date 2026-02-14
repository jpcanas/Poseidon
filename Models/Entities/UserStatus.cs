using System.ComponentModel.DataAnnotations;

namespace Poseidon.Models.Entities
{
    public class UserStatus
    {
        public int UserStatusId { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(20)]
        public string Color { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;

        public bool AffectsAccess { get; set; } = false;
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime? UpdatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public List<User>? Users { get; set; }
    }
}
