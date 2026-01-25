using System.ComponentModel.DataAnnotations;

namespace Poseidon.Models.ViewModels
{
    public class UserStatusVM
    {
        public int UserStatusId { get; set; }

        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(20)]
        public string Color { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; } = string.Empty;

    }
}
