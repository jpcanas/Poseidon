using Poseidon.Enums;
using System.ComponentModel.DataAnnotations;

namespace Poseidon.Models.ViewModels
{
    public class UserTableVM
    {
        public int UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? UserName { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? MiddleName { get; set; }
        public DateTime? BirthDate { get; set; }
        public string? BirthDateInput
        {
            get
            {
                return BirthDate?.ToString("MM/dd/yyyy");
            }
        }
        public BiologicalSexType? BiologicalSex { get; set; }
        public string? BiologicalSexStr
        {
            get
            {
                return BiologicalSex?.ToString();
            }
        }
        public string? Address { get; set; }
        public string? MobileNumber { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int RoleId { get; set; }
        public int UserStatusId { get; set; }
        public string? Status { get; set; }
        public string? RoleName { get; set; }
        public string? StatusColor { get; set; }
        public int? ProfilePictureFileRecordId { get; set; }
    }
}
