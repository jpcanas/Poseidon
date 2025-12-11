using Poseidon.Models.Entities;

namespace Poseidon.Data.Interfaces
{
    public interface IUserRepository
    {
        Task AddPasswordResetToken(PasswordResetToken passToken);
        public Task<User?> GetUser(string email);
    }
}
