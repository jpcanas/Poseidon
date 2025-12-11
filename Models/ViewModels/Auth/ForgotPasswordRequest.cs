using System.ComponentModel.DataAnnotations;

namespace Poseidon.Models.ViewModels.Auth
{
    public class ForgotPasswordRequest
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string? Email { get; set; }
    }
}
