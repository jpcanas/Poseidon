using Poseidon.Models.Entities;
using Poseidon.Models.ViewModels;

namespace Poseidon.Services.Interfaces
{
    public interface IUserService
    {
        Task<int> AddUser(UserVM userVM);
        Task<List<RoleVM>> GetRoleList();
        Task<List<UserStatusVM>> GetStatusList();
        Task<User?> GetUserByEmailorUsername(string? email = null, string? username = null);
        Task<List<UserTableVM>> GetUserTable(string? status = null);
    }
}
