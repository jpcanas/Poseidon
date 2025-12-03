namespace Poseidon.Models.Entities
{
    public class Role
    {
        public int RoleId { get; set; }
        public string? RoleName { get; set; }
        public string? RoleType { get; set; }
        public List<User>? Users { get; set; }
    }
}
