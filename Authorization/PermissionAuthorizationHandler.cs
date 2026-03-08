using Microsoft.AspNetCore.Authorization;
using Poseidon.Services.Interfaces;
using System.Security.Claims;

namespace Poseidon.Authorization
{
    public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
    {
        private readonly IPermissionService _permissionService;
        public PermissionAuthorizationHandler(IPermissionService permissionService)
        {
            _permissionService = permissionService;
        }
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return Task.CompletedTask;
            }
            var hasPermission = _permissionService.CheckUserHasPermission(userId, requirement.PermissionCode).Result;
            if (hasPermission)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;

        }
    }
}
