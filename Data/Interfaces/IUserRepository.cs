using Poseidon.Models.Entities;

namespace Poseidon.Data.Interfaces
{
    public interface IUserRepository
    {
        public Task<User?> GetUser(string email);
    }
}
