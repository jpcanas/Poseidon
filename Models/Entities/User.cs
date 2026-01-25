using Poseidon.Enums;
using System.ComponentModel.DataAnnotations;

namespace Poseidon.Models.Entities
{
    public class User
    {
        public int UserId { get; set; }

        [Required]
        public Guid UserIdentifier { get; set; }

        [MaxLength(50)]
        public string? UserName { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(255)]
        public string Email { get; set; }

        [Required]
        public int RoleId { get; set; }
        public Role? Role { get; set; }

        [MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? MiddleName { get; set; }
        public DateTime? BirthDate { get; set; }
        public DateTime? LastLoginDatetime { get; set; }

        [MaxLength(20)]
        public string? Gender { get; set; }

        public BiologicalSexType? BiologicalSex { get; set; }

        [MaxLength(20)]
        public string? MobileNumber { get; set; }

        [MaxLength(1000)]
        public string? Address { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

        [Required]
        public int UserStatusId { get; set; }
        public UserStatus? UserStatus { get; set; }

        [Required]
        public bool RequiredPasswordChange { get; set; }

    }
}
