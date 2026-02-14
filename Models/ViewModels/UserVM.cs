using Poseidon.Enums;
using Poseidon.Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace Poseidon.Models.ViewModels
{
    public class UserVM
    {
        public int UserId { get; set; }
        public Guid? UserIdentifier { get; set; }

        [MaxLength(50)]
        public string? UserName { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(255)]
        public string Email { get; set; }

        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? MiddleName { get; set; }
        public DateTime? BirthDate { get; set; }

        public string? BirthDateString
        {
            get
            {
                return BirthDate?.ToString("MMMM dd, yyyy");
            }
        }

        [MaxLength(20)]
        public string? Gender { get; set; }

        public BiologicalSexType? BiologicalSex { get; set; }
        public string? BiologicalSexStr
        {
            get
            {
                return BiologicalSex?.ToString();
            }
        }
            
        [MaxLength(20)]
        public string? MobileNumber { get; set; }

        [MaxLength(1000)]
        public string? Address { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime? UpdatedDate { get; set; }
        public string? UpdatedBy { get; set; }

        [Required]
        public int RoleId { get; set; }
        public string RoleName { get; set; } = string.Empty;

        [Required]
        public int UserStatusId { get; set; }
        public string StatusName { get; set; } = string.Empty;
    }
}
