using Poseidon.Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace Poseidon.Models.ViewModels
{
    public class RoleVM
    {
        public int RoleId { get; set; }

        [Required]
        [MaxLength(100)]
        public string RoleName { get; set; }

        [MaxLength(200)]
        public string? Description { get; set; }
        public string? RoleType { get; set; }

    }
}
