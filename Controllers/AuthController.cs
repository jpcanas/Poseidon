using Azure.Core;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Poseidon.Configurations;
using Poseidon.Models.Entities;
using Poseidon.Models.ViewModels;
using Poseidon.Models.ViewModels.Auth;
using Poseidon.Services.Interfaces;
using Resend;
using System.Security.Claims;

namespace Poseidon.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly IEmailService _emailService;
        private readonly AuthSetting _authSetting;

        public AuthController(IAuthService authService, IEmailService emailService, IOptions<AuthSetting> authOptions)
        {
            _authService = authService;
            _emailService = emailService;
            _authSetting = authOptions.Value;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost("Auth/Login")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([FromBody] LoginViewModel loginCreds, string? redirectUrl = null)
        {
            if(!ModelState.IsValid || loginCreds == null)
            {
                Dictionary<string, string[]?>? errors = ModelState
                    .Where(x => x.Value?.Errors.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray()
                        );

                return BadRequest(new { Success = false, Errors = errors });
            }

            User? userByEmail = await _authService.GetUserByEmail(loginCreds.Email);
            if (userByEmail != null && userByEmail.RequiredPasswordChange)
            {
                var existingToken = await _authService.GetActiveResetToken(userByEmail.UserId);
                string? token = existingToken != null ? existingToken.Token :
                    await _authService.GenerateResetToken(userByEmail.Email);

                var url = Url.Action("NewPasswordSetup", "Auth", new
                {
                    userId = userByEmail.UserIdentifier,
                    resetToken = token
                });

                //actual implementation would be to send email with token link and user id
                // await _emailService.SendEmail(logUser, token);
                // var url = Url.Action("NewUserLoginPrompt", "Auth");
                return Ok(new
                {
                    Success = true,
                    RedirectUrl = url,
                });
            }

            User? logUser = await _authService.LoginUser(loginCreds);

            if (logUser == null)
            {
                return BadRequest(new
                {
                    Success = false,
                    Errors = new { General = new string[] { "Invalid email or password" }}
                });
            }

            CookieClaims cookieClaims = _authService.SetupClaims(loginCreds, logUser);

            await HttpContext.SignInAsync(_authSetting.AuthScheme, cookieClaims.Principal, cookieClaims.AuthProperties);

            string? redirect = !string.IsNullOrEmpty(redirectUrl) && Url.IsLocalUrl(redirectUrl) ? redirectUrl : Url.Action("Index", "Home");

            return Ok( new
            {
                Success = true,
                RedirectUrl = redirect,
            });

        }

        [Authorize]
        [HttpGet]
        public IActionResult CheckSession()
        {
            return Ok(new { status = "active" });

        }
        //[Authorize]
        //[HttpGet]
        //public IActionResult GetCookieExpiry()
        //{
        //    var cookie = Request.Cookies["PoseidonCookie"];
        //    if (cookie == null)
        //        return Unauthorized();

        //    var ticket = HttpContext.AuthenticateAsync("PoseidonAuth").Result;
        //    if (ticket == null || !ticket.Succeeded)
        //        return Unauthorized();

        //    var props = ticket.Properties;

        //    if (props.IssuedUtc.HasValue && props.ExpiresUtc.HasValue)
        //    {
        //        var now = DateTimeOffset.UtcNow;
        //        var remaining = props.ExpiresUtc.Value - now;

        //        var rem = now - props.IssuedUtc.Value;

        //        var lifetime = TimeSpan.FromMinutes(1);

        //        return Ok(new
        //        {
        //            status = "active",
        //            issuedUtc = props.IssuedUtc.Value.ToLocalTime(),
        //            expiresUtc = props.ExpiresUtc.Value.ToLocalTime(),
        //            remainingSeconds = (int)remaining.TotalSeconds
        //        });
        //    }

        //    // If expiration timestamps are not available
        //    return Ok(new { status = "active", message = "No expiration info available" });

        //}
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(_authSetting.AuthScheme); //add the scheme

            return Ok();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AutoLogout()
        {
            await HttpContext.SignOutAsync(_authSetting.AuthScheme);
            return Ok();
        }

        [Route("Auth/forgot-password")]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [Route("Auth/new-user-prompt")]
        public IActionResult NewUserLoginPrompt()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword([FromBody] ForgotPasswordRequest forgotPassword)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            User? logUser = await _authService.GetUserByEmail(forgotPassword.Email);

            string message = "If the address you entered is registered, we’ve sent instructions to reset your password." +
                    "\r\n Please check your inbox (and your spam folder) for further steps.";

            string? token = null;
            if (logUser != null)
            {
                // check for the existence of token for user and if not expire
                var existingToken = await _authService.GetActiveResetToken(logUser.UserId);
                token = existingToken != null ? existingToken.Token :
                     await _authService.GenerateResetToken(logUser.Email);

                //await _emailService.SendEmail(logUser, token);
            }

            //temporary reset link
            string userGuid = logUser != null ? logUser.UserIdentifier.ToString() : "";
            string? resetLink = Url.Action("NewPasswordSetup", "Auth", new
            {
                userId = userGuid,
                resetToken = token
            });

            return Ok(new {msg = message , tempResetLink = resetLink }); //tempResetLink is temporRY ONLY FOR TESTING PURPOSES
        }

        [Route("Auth/NewPasswordSetup/{userId}")]
        public async Task<IActionResult> NewPasswordSetup(string userId, string? resetToken)
        {
            if (string.IsNullOrEmpty(resetToken))
            {
                return RedirectToAction("InvalidLink", "Redirect");
            }

            var validToken = await _authService.ValidateResetToken(userId, resetToken);
            if (validToken == null)
            {
                return RedirectToAction("InvalidLink", "Redirect");
            }

            ViewBag.UserGuid = userId;
            ViewBag.ResetToken = resetToken;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdatePassword([FromBody] UpdatePasswordRequestVM resetRequest)
        {
            if (string.IsNullOrWhiteSpace(resetRequest.NewPassword) || string.IsNullOrWhiteSpace(resetRequest.ConfirmPassword))
                return BadRequest("Both password fields are required");

            if (resetRequest.NewPassword != resetRequest.ConfirmPassword)
                return BadRequest("Passwords do not match");

            if (resetRequest.NewPassword.Length < 8)
                return BadRequest("Password must be at least 8 characters long");

            var validToken = await _authService.ValidateResetToken(resetRequest.UserGuid, resetRequest.ResetToken);
            if (validToken == null)
                return BadRequest("Invalid or expired link provided");

            var user = await _authService.GetUserByGuid(resetRequest.UserGuid);
            if (user == null)
                return BadRequest("User not found");

            var userIdPasswordChanged = await _authService.UpdateUserPassword(user.UserId, resetRequest.NewPassword);

            await _authService.CompletePasswordReset(user.UserId, validToken.Id);

            return Ok("Password updated successfully");
        }
    }
}
