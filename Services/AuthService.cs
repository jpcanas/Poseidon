using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Poseidon.Configurations;
using Poseidon.Data.Interfaces;
using Poseidon.Data.Repositories;
using Poseidon.Models.Entities;
using Poseidon.Models.ViewModels;
using System.Security.Claims;

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

            bool validPassword = BCrypt.Net.BCrypt.Verify(loginCreds.Password, user.Password);
            if (!validPassword)
                return null;

            return user;
        }
    }
}
