using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Poseidon.Configurations;
using Poseidon.Models.Entities;
using Poseidon.Models.ViewModels.Auth;
using Poseidon.Repositories.Interfaces;
using Poseidon.Services.Interfaces;
using System.Security.Claims;
using System.Security.Cryptography;

namespace Poseidon.Services
{
    public class AuthService : IAuthService
    {
        private readonly AuthSetting _authSetting;
        private readonly IUserRepository _userRepository;
        public AuthService(IOptions<AuthSetting> authOptions, IUserRepository userRepository)
        {
            _authSetting = authOptions.Value;
            _userRepository = userRepository;
        }
        public CookieClaims SetupClaims(LoginViewModel loginCreds, User loguser)
        {
            var claims = new List<Claim>
            {
               new Claim(ClaimTypes.NameIdentifier, loguser.UserIdentifier.ToString()),
                new Claim("FirstName", loguser.FirstName),
                new Claim("LastName", loguser.LastName),
                new Claim(ClaimTypes.Email, loguser.Email),
               new Claim(ClaimTypes.Role, loguser.Role.RoleName),
            };

            var identity = new ClaimsIdentity(claims, _authSetting.CookieName);
            var principal = new ClaimsPrincipal(identity);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = loginCreds.RememberMe,
                ExpiresUtc = loginCreds.RememberMe ? DateTimeOffset.UtcNow.AddHours(_authSetting.AbsoluteExpireHours)
                : DateTimeOffset.UtcNow.AddMinutes(_authSetting.CookieExpireMinutes)
            };

            return new CookieClaims
            {
                Principal = principal,
                AuthProperties = authProperties
            };

        }
        public async Task<User?> LoginUser(LoginViewModel loginCreds)
        {
            User? user = await _userRepository.GetUser(loginCreds.Email);

            if (user == null) return null;

            bool validPassword = BCrypt.Net.BCrypt.Verify(loginCreds.Password, user.Password);
            if (!validPassword)
                return null;

            return user;
        }

        public async Task<User?> GetUserByEmail(string? email)
        {
            return await _userRepository.GetUser(email);
        }

        public async Task<string?> GenerateResetToken(string email)
        {
            var user = await _userRepository.GetUser(email);

            if (user == null)
                return null;

            var tokenBytes = RandomNumberGenerator.GetBytes(64);
            var token = Convert.ToBase64String(tokenBytes);

            var passwordResetEntry = new PasswordResetToken
            {
                UserId = user.UserId,
                Token = token,
                ExpiresAt = DateTime.UtcNow.AddMinutes(_authSetting.PasswordResetTokenExpiryMinutes)
            };

            await _userRepository.AddPasswordResetToken(passwordResetEntry);

            return token;
        }

        public async Task<PasswordResetToken?> GetActiveResetToken(int userId)
        {
            return await _userRepository.GetActivePasswordResetToken(userId);
        }

        public async Task<User?> GetUserByGuid(string userId)
        {
            return await _userRepository.GetUserByGuid(userId);
        }
        public async Task<PasswordResetToken?> ValidateResetToken(string userId, string token)
        {
            var user = await _userRepository.GetUserByGuid(userId);
            if (user == null)
                return null;

            var resetToken = await _userRepository.GetPasswordResetTokenByIdAndToken(user.UserId, token);

            if (resetToken == null || resetToken.IsUsed || resetToken.ExpiresAt <= DateTime.UtcNow)
                return null;

            return resetToken;
        }

        public async Task<int> UpdateUserPassword(int userId, string newPassword)
        {
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(newPassword);
            return await _userRepository.UpdateUserPassword(userId, hashedPassword);
        }

        public async Task<bool> CompletePasswordReset(int userId, int tokenId)
        {
            var token = await _userRepository.MarkPasswordResetTokenAsUsed(tokenId);
            var userUpdate = await _userRepository.UpdateUserRequirePasswordChange(userId, false);

            return token > 0 && userUpdate > 0;
        }
    }
}
