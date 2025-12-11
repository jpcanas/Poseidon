using Microsoft.EntityFrameworkCore;
using Poseidon.Data.Interfaces;
using Poseidon.Models.Entities;

namespace Poseidon.Data.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly PoseidonDbContext _context;
        public UserRepository(PoseidonDbContext context )
        {
            _context = context;
        }
        public async Task<User?> GetUser(string email)
        {
            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());

            return user;
        }

        public async Task AddPasswordResetToken(PasswordResetToken passToken)
        {
            _context.PasswordResetTokens.Add(passToken);
            await _context.SaveChangesAsync();
        }
    }
}
