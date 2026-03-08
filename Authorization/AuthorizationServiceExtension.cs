using Microsoft.AspNetCore.Authorization;

namespace Poseidon.Authorization
{
    public static class AuthorizationServiceExtension
    {
        public static IServiceCollection AddPermissionAuthorization(this IServiceCollection services)
        {
            // Register the handler
            services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();

            // Register the custom policy provider
            services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();

            // Enable authorization
            services.AddAuthorization();

            return services;
        }
    }
} 
