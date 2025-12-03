using System.ComponentModel.DataAnnotations;

namespace Poseidon.Models.ViewModels
{
    public class LoginViewModel
    {
        public string? UserName { get; set; }
        [Required]
        [EmailAddress]
        public string? Email { get; set; }
        [Required]
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }
}
