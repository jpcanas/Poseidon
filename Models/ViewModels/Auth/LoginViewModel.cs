using System.ComponentModel.DataAnnotations;

namespace Poseidon.Models.ViewModels.Auth
{
    public class LoginViewModel
    {
        public string? UserName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be 8-100 characters")]
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }
}
