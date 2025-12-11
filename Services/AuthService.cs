using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Poseidon.Configurations;
using Poseidon.Data.Interfaces;
using Poseidon.Data.Repositories;
using Poseidon.Models.Entities;
using Poseidon.Models.ViewModels.Auth;
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
                new Claim(ClaimTypes.Name, loguser.UserName),
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

            if(user == null) return null;

            bool validPassword = BCrypt.Net.BCrypt.Verify(loginCreds.Password, user.Password);
            if (!validPassword)
                return null;

            return user;
        }

        public async Task<User?> GetUserByEmail(string email)
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
                ExpiresAt = DateTime.UtcNow.AddMinutes(30)
            };

            await _userRepository.AddPasswordResetToken(passwordResetEntry);

            return token;
        }
    }
}
