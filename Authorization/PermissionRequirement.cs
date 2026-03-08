using Microsoft.AspNetCore.Authorization;

namespace Poseidon.Authorization
{
    public class PermissionRequirement : IAuthorizationRequirement
    {
        public string PermissionCode { get; }
        public PermissionRequirement(string permissionCode)
        {
            PermissionCode = permissionCode;
        }
    }
}
