
namespace Poseidon.Services.Interfaces
{
    public interface IPermissionService
    {
        Task<bool> CheckUserHasPermission(int userId, string permissionCode);
        Task<List<string>> GetUserPermissions(int userId);
    }
}
