using Poseidon.Configurations;
using Poseidon.Models.Entities;
using Poseidon.Models.ViewModels.Auth;

namespace Poseidon.Services.Interfaces
{
    public interface IAuthService
    {
        Task<User?> LoginUser(LoginViewModel loginCreds);
        CookieClaims SetupClaims(LoginViewModel loginCreds, User loguser);
        Task<User?> GetUserByEmail(string email);
        Task<string?> GenerateResetToken(string email);
    }
}