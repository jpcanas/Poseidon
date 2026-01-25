using System.ComponentModel.DataAnnotations;

namespace Poseidon.Models.ViewModels.Auth
{
    public class UpdatePasswordRequestVM
    {
        [Required]
        public string UserGuid { get; set; } = string.Empty;

        [Required]
        public string ResetToken { get; set; } = string.Empty;

        [Required]
        [MinLength(8)]
        public string NewPassword { get; set; } = string.Empty;

        [Required]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
