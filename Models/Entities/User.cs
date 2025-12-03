using System.ComponentModel.DataAnnotations;

namespace Poseidon.Models.Entities
{
    public class User
    {
        [Required]
        public int UserId { get; set; }
        [Required]
        public Guid UserIdentifier { get; set; }
        public string? UserName { get; set; }

        [Required]
        public string? Password { get; set; }

        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        [Required]
        public int RoleId { get; set; }
        public Role? Role { get; set; }

        public List<User> SampleUser()
        {
            List<User> sampleUser = new List<User>
            {
                new User
                {
                    UserId = 1,
                    UserIdentifier = new Guid(),
                    UserName = "admin123",
                    Email = "jpcanas@westpac-sfi.com",
                    Password = "Password1",
                    RoleId = 1,
                },

                  new User
                {
                    UserId = 2,
                    UserIdentifier = new Guid(),
                    UserName = "jpcanas",
                    Email = "junecanas18@gmail.com",
                    Password = "Password1",
                     RoleId = 2,
                }
            };
            return sampleUser;
        }
    }
}
