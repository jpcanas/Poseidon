using Poseidon.Models.Entities;
using Poseidon.Models.ViewModels;

namespace Poseidon.Services.Interfaces
{
    public interface IUserService
    {
        Task<User?> AddUser(UserVM userVM);
        Task<List<RoleVM>> GetRoleList();
        Task<RolePermissionVM> GetRolePermissions(int roleId);
        Task<List<UserStatusVM>> GetStatusList();
        Task<User?> GetUserByEmailorUsername(string? email = null, string? username = null);
        Task<List<UserTableVM>> GetUserTable(string? status = null);
        Task<int> SaveRolePermissions(SaveRoleRequestVM request);
        Task<UserVM> UpdateUserData(UserVM userModel);
        Task<int> UpdateUserDatabyAdmin(UserVM userModel);
    }
}
