using Poseidon.Configurations;
using Poseidon.Models.Entities;
using Poseidon.Models.ViewModels.Auth;

namespace Poseidon.Services.Interfaces
{
    public interface IAuthService
    {
        Task<User?> LoginUser(LoginViewModel loginCreds);
        CookieClaims SetupClaims(LoginViewModel loginCreds, User loguser);
        Task<User?> GetUserByEmail(string? email);
        Task<string?> GenerateResetToken(string email);
        Task<PasswordResetToken?> GetActiveResetToken(int userId);
        Task<User?> GetUserByGuid(string userId);
        Task<PasswordResetToken?> ValidateResetToken(string userId, string token);
        Task<int> UpdateUserPassword(int userId, string newPassword);
        Task<bool> CompletePasswordReset(int userId, int tokenId);
    }
}