using System.ComponentModel.DataAnnotations;

namespace Poseidon.Models.Entities
{
    public class SubModule
    {
        public int Id { get; set; }
        public int ModuleId { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string Code { get; set; } = string.Empty;
        public Module Module { get; set; } = null!;
        public List<RolePermission> RolePermissions { get; set; } = new();

    }
}
