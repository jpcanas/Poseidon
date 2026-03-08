using Poseidon.Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace Poseidon.Models.ViewModels
{
    public class RolePermissionVM
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? RoleType { get; set; }
        public bool IsSystemRole { get; set; }
        public List<ModulePermissionVM> Permissions { get; set; } = new();
    }

    public class ModulePermissionVM
    {
        public int ModuleId { get; set; }
        public string ModuleName { get; set; } = string.Empty;
        public string ModuleDescription { get; set; } = string.Empty;
        public List<SubModulePermissionVM> SubModules { get; set; } = new();
        public bool Enabled { get; set; }
    }
    public class SubModulePermissionVM
    {
        public int SubModuleId { get; set; }
        public string SubModuleName { get; set; } = string.Empty;
        public string SubModuleDescription { get; set; } = string.Empty;
        public bool IsAssigned { get; set; }
    }
    public class SaveRoleRequestVM
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<int> SubModuleIds { get; set; } = new();
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
