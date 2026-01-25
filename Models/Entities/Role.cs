using System.ComponentModel.DataAnnotations;

namespace Poseidon.Models.Entities
{
    public class Role
    {
        public int RoleId { get; set; }

        [Required]
        [MaxLength(100)]
        public string RoleName { get; set; }

        [MaxLength(200)]
        public string? Description { get; set; }
        public string? RoleType { get; set; }
        public List<User>? Users { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
