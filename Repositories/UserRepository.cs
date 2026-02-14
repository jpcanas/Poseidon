using Microsoft.EntityFrameworkCore;
using Poseidon.Data;
using Poseidon.Models.Entities;
using Poseidon.Models.ViewModels;
using Poseidon.Repositories.Interfaces;

namespace Poseidon.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly PoseidonDbContext _context;
        public UserRepository(PoseidonDbContext context)
        {
            _context = context;
        }
        public async Task<User?> GetUser(string? email)
        {
            if(string.IsNullOrWhiteSpace(email))
                return null;

            var user = await _context.Users
                .Include(u => u.Role)
                .Include(s => s.UserStatus)
                .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());

            return user;
        }
        public async Task<User?> GetUserByEmailOrUsername(string? email = null, string? username = null)
        {
            var user = await _context.Users
                .Include(u => u.Role)
                .Include(s => s.UserStatus)
                .FirstOrDefaultAsync(u =>
                            (!string.IsNullOrWhiteSpace(email) && u.Email.ToLower() == email.ToLower()) ||
                            (!string.IsNullOrWhiteSpace(username) && u.UserName.ToLower() == username.ToLower()));

            return user;
        }

        public async Task<PasswordResetToken?> GetActivePasswordResetToken(int userId)
        {
            return await _context.PasswordResetTokens
                .Where(t => t.UserId == userId
                            && !t.IsUsed
                            && t.ExpiresAt > DateTime.UtcNow)
                .OrderByDescending(t => t.CreatedDate)
                .FirstOrDefaultAsync();
        }

        public async Task AddPasswordResetToken(PasswordResetToken passToken)
        {
            _context.PasswordResetTokens.Add(passToken);
            await _context.SaveChangesAsync();
        }
        public async Task<List<User>> GetUsers(string? status = null)
        {
            return await _context.Users
                .Include(u => u.Role)
                .Include(s => s.UserStatus)
                .ToListAsync();

        }
        public async Task<List<UserStatusVM>> GetStatus()
        {
            return await _context.UserStatuses
                .Select(s => new UserStatusVM
                {
                    UserStatusId = s.UserStatusId,
                    Name = s.Name,
                    Color = s.Color,
                    Description = s.Description
                }).ToListAsync();
        }
        public async Task<User?> AddUser(User newUser)
        {
            try
            {
                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();
                return newUser;
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        public async Task<User?> GetUserByGuid(string userId)
        {
            try
            {
                var user = await _context.Users
                .Include(u => u.Role)
                .Include(s => s.UserStatus)
                .FirstOrDefaultAsync(u => u.UserIdentifier.ToString().ToLower() == userId.ToLower());

                return user;
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        public async Task<PasswordResetToken?> GetPasswordResetTokenByIdAndToken(int userId, string token)
        {
            try
            {
                return await _context.PasswordResetTokens
                .FirstOrDefaultAsync(t => t.UserId == userId && t.Token == token);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<int> MarkPasswordResetTokenAsUsed(int tokenId)
        {
            try
            {
                var token = await _context.PasswordResetTokens.FindAsync(tokenId);
                if (token != null)
                {
                    token.IsUsed = true;
                    return await _context.SaveChangesAsync();
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public async Task<int> UpdateUserRequirePasswordChange(int userId, bool requireChange)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user != null)
                {
                    user.RequiredPasswordChange = requireChange;
                    return await _context.SaveChangesAsync();
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                return 0;
            }

        }

        public async Task<int> UpdateUserPassword(int userId, string newHashedPassword)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user != null)
                {
                    user.Password = newHashedPassword;
                    user.UpdatedDate = DateTime.UtcNow;
                    return await _context.SaveChangesAsync();
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
        public async Task<User?> UpdateUserData(UserVM userModel)
        {
            try
            {
                var user = await _context.Users
                        .Include(u => u.Role)
                        .Include(s => s.UserStatus)
                        .FirstOrDefaultAsync(u => u.UserId == userModel.UserId);

                if (user != null)
                {
                    user.UserName = userModel.UserName;
                    user.FirstName = userModel.FirstName;
                    user.LastName = userModel.LastName;
                    user.MiddleName = userModel.MiddleName;
                    user.BiologicalSex = userModel.BiologicalSex;
                    user.MobileNumber = userModel.MobileNumber;
                    user.Address = userModel.Address;
                    user.BirthDate = userModel.BirthDate;
                    await _context.SaveChangesAsync();
                }

                return user;

            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<User?> GetUserById(int userId)
        {
            try
            {
                var user = await _context.Users
                .Include(u => u.Role)
                .Include(s => s.UserStatus)
                .FirstOrDefaultAsync(u => u.UserId == userId);

                return user;
            }
            catch (Exception ex)
            {
                return null;
            }

        }
    }
}
