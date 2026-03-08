namespace Poseidon.Models.Entities
{
    public class RolePermission
    {
        public int Id { get; set; }
        public int RoleId { get; set; }
        public int SubModuleId { get; set; }
        public Role Role { get; set; } = null!;
        public SubModule SubModule { get; set; } = null!;
    }
}
