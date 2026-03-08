using Poseidon.Repositories.Interfaces;
using Poseidon.Services.Interfaces;

namespace Poseidon.Services
{
    public class PermissionService : IPermissionService
    {
        private readonly IRoleRepository _roleRepository;
        public PermissionService(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }
        public async Task<bool> CheckUserHasPermission(int userId, string permissionCode)
        {
            return await _roleRepository.CheckUserHasPermission(userId, permissionCode);
        }
        public async Task<List<string>> GetUserPermissions(int userId)
        {
            return await _roleRepository.GetUserPermissions(userId);
        }
    }
}
