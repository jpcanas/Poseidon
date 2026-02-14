using Poseidon.Models.Entities;
using Poseidon.Models.ViewModels;

namespace Poseidon.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task AddPasswordResetToken(PasswordResetToken passToken);
        Task<User?> AddUser(User newUser);
        Task<PasswordResetToken?> GetActivePasswordResetToken(int userId);
        Task<PasswordResetToken?> GetPasswordResetTokenByIdAndToken(int userId, string token);
        Task<List<UserStatusVM>> GetStatus();
        Task<User?> GetUser(string? email);
        Task<User?> GetUserByEmailOrUsername(string? email = null, string? username = null);
        Task<User?> GetUserByGuid(string userId);
        Task<User?> GetUserById(int userId);
        Task<List<User>> GetUsers(string? status = null);
        Task<int> MarkPasswordResetTokenAsUsed(int tokenId);
        Task SetLastLoginDateTime(int userId);
        Task<User?> UpdateUserData(UserVM userModel);
        Task<int> UpdateUserPassword(int userId, string newHashedPassword);
        Task<int> UpdateUserRequirePasswordChange(int userId, bool requireChange);
    }
}
