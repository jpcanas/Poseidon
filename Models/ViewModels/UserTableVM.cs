using Poseidon.Enums;
using System.ComponentModel.DataAnnotations;

namespace Poseidon.Models.ViewModels
{
    public class UserTableVM
    {
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public DateTime? BirthDate { get; set; }
        public string? BiologicalSex { get; set; }
        public string? Address { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? Status { get; set; }
        public string? RoleName { get; set; }
    }
}
