using Poseidon.Configurations;
using Poseidon.Models.Entities;
using Poseidon.Models.ViewModels;

namespace Poseidon.Services
{
    public interface IAuthService
    {
        Task<User?> LoginUser(LoginViewModel loginCreds);
        CookieClaims SetupClaims(LoginViewModel loginCreds, User loguser);
    }
}