using Poseidon.Models.Entities;
using Poseidon.Models.ViewModels;

namespace Poseidon.Repositories.Interfaces
{
    public interface IRoleRepository
    {
        Task<int> AddRole(Role role);
        Task AddRolePermissions(SaveRoleRequestVM rolePermissions);
        Task<bool> CheckUserHasPermission(int userId, string permissionCode);
        Task DeleteRolePermission(int roleId);
        Task<List<RoleVM>> GetRoleList();
        Task<RolePermissionVM> GetRolePermissions(int roleId);
        Task<Role?> GetSingleRole(int roleId);
        Task<List<string>> GetUserPermissions(int userId);
        Task<int> UpdateRole(Role role);
    }
}