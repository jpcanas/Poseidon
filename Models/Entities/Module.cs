using System.ComponentModel.DataAnnotations;

namespace Poseidon.Models.Entities
{
    public class Module
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string Description { get; set; } = string.Empty;
        public List<SubModule> SubModules { get; set; } = new();
    }
}
