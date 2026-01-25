using Poseidon.Models.ViewModels;

namespace Poseidon.Repositories.Interfaces
{
    public interface IRoleRepository
    {
        Task<List<RoleVM>> GetRoleList();
    }
}