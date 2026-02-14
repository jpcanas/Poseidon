using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Poseidon.Configurations;
using Poseidon.Enums;
using Poseidon.Models.Entities;
using Poseidon.Models.ViewModels;
using Poseidon.Models.ViewModels.Auth;
using Poseidon.Repositories.Interfaces;
using Poseidon.Services.Interfaces;
using System;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.AspNetCore.WebUtilities;
using System.Net;

namespace Poseidon.Services
{
    public class AuthService : IAuthService
    {
        private readonly AuthSetting _authSetting;
        private readonly IUserRepository _userRepository;
        private readonly IEmailService _emailService;
        public AuthService(IOptions<AuthSetting> authOptions, IEmailService emailService, IUserRepository userRepository)
        {
            _authSetting = authOptions.Value;
            _userRepository = userRepository;
            _emailService = emailService;
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

        public async Task<UserVM?> GetUserByGuid(string userId)
        {
            var userEntity = await _userRepository.GetUserByGuid(userId);
            UserVM? userVM = new UserVM();

            if (userEntity != null)
            {
                userVM = new UserVM
                {
                    UserId = userEntity.UserId,
                    UserIdentifier = userEntity.UserIdentifier,
                    FirstName = userEntity.FirstName,
                    LastName = userEntity.LastName,
                    MiddleName = userEntity.MiddleName,
                    Email = userEntity.Email,
                    UserName = userEntity.UserName,
                    BirthDate = userEntity.BirthDate,
                    Gender = userEntity.Gender,
                    BiologicalSex = userEntity.BiologicalSex,
                    MobileNumber = userEntity.MobileNumber,
                    Address = userEntity.Address,
                    RoleId = userEntity.Role.RoleId,
                    RoleName = userEntity.Role.RoleName,
                    UserStatusId = userEntity.UserStatusId,
                    StatusName = userEntity.UserStatus.Name
                };
            }
            return userVM;
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
        public async Task<string> GeneratePasswordResetLink(User logUser, DateTime expiry)
        {
            var existingToken = await _userRepository.GetActivePasswordResetToken(logUser.UserId);
            if (existingToken != null)
            {
                return $"{_authSetting.BaseUrl}/Auth/NewPasswordSetup/{logUser.UserIdentifier}?resetToken={existingToken.Token}";
            }

            var tokenBytes = RandomNumberGenerator.GetBytes(64);
            var newToken = WebEncoders.Base64UrlEncode(tokenBytes);

            var passwordResetEntry = new PasswordResetToken
            {
                UserId = logUser.UserId,
                Token = newToken,
                ExpiresAt = expiry,
            };

            await _userRepository.AddPasswordResetToken(passwordResetEntry);

            return $"{_authSetting.BaseUrl}/Auth/NewPasswordSetup/{logUser.UserIdentifier}?resetToken={newToken}";
        }

        public async Task SendEmailResetPassword(User user)
        {
            var resetExpiry = DateTime.UtcNow.AddMinutes(_authSetting.PasswordResetTokenExpiryMinutes);
            string resetLink = await GeneratePasswordResetLink(user, resetExpiry);
            var variables = new Dictionary<string, object>
            {
                { "PASSWORD_RESET_LINK", resetLink },
                 { "EXPIRY_TIME", $"{_authSetting.PasswordResetTokenExpiryMinutes} minutes" },
            };

            await _emailService.SendEmailAsync(EmailTemplateName.PasswordReset.ToString(), user.Email, variables);
        }
        public async Task SendWelcomeEmail(User user)
        {
            var resetExpiry = DateTime.UtcNow.AddHours(_authSetting.PasswordResetTokenExpiryHours);
            string setupLink = await GeneratePasswordResetLink(user, resetExpiry);
            var variables = new Dictionary<string, object>
            {
                  { "USER_EMAIL", user.Email },
                  { "PASSWORD_RESET_LINK", setupLink },
                  { "EXPIRY_TIME", $"{_authSetting.PasswordResetTokenExpiryHours} hours" },
            };

            await _emailService.SendEmailAsync(EmailTemplateName.NewUserWelcome.ToString(), user.Email, variables);
        }
        public async Task<User?> GetUserById(int userId)
        {
            return await _userRepository.GetUserById(userId);
        }
        public async Task<object?> CheckExistingPasswordForUpdate(UserPasswordVM userPassword)
        {
            User? user = await _userRepository.GetUserById(userPassword.UserId);
            if (user == null)
                return new { General = new string[] { "Cannot update password. Unexpected Error happen." } };

            bool validCurrentPassword = BCrypt.Net.BCrypt.Verify(userPassword.CurrentPassword, user.Password);
            if (!validCurrentPassword)
                return new { CurrentPassword = new string[] { "Current password is incorrect" } };

            bool sameAsCurrent = BCrypt.Net.BCrypt.Verify(userPassword.NewPassword, user.Password);
            if (sameAsCurrent)
                return new { NewPassword = new string[] { "New password cannot be the same as the current password" } };

            return null;
        }

    }
}
