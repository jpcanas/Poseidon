using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Poseidon.Configurations;
using Poseidon.Models.Entities;
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

        public AuthController(IAuthService authService, IEmailService emailService)
        {
            _authService = authService;
            _emailService = emailService;
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

            await HttpContext.SignInAsync("PoseidonAuth", cookieClaims.Principal, cookieClaims.AuthProperties);

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
        [Authorize]
        [HttpGet]
        public IActionResult GetCookieExpiry()
        {
            var cookie = Request.Cookies["PoseidonCookie"];
            if (cookie == null)
                return Unauthorized();

            var ticket = HttpContext.AuthenticateAsync("PoseidonAuth").Result;
            if (ticket == null || !ticket.Succeeded)
                return Unauthorized();

            var props = ticket.Properties;

            if (props.IssuedUtc.HasValue && props.ExpiresUtc.HasValue)
            {
                var now = DateTimeOffset.UtcNow;
                var remaining = props.ExpiresUtc.Value - now;

                var rem = now - props.IssuedUtc.Value;

                var lifetime = TimeSpan.FromMinutes(1);

                return Ok(new
                {
                    status = "active",
                    issuedUtc = props.IssuedUtc.Value.ToLocalTime(),
                    expiresUtc = props.ExpiresUtc.Value.ToLocalTime(),
                    remainingSeconds = (int)remaining.TotalSeconds
                });
            }

            // If expiration timestamps are not available
            return Ok(new { status = "active", message = "No expiration info available" });

        }
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();

            return Ok();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AutoLogout()
        {
            await HttpContext.SignOutAsync();
            return Ok();
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
        [Route("Auth/forgot-password")]
        public IActionResult ForgotPassword()
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

            string msg = "If the address you entered is registered, we’ve sent instructions to reset your password." +
                    "\r\n Please check your inbox (and your spam folder) for further steps.";

            if (logUser != null)
            {
                // check for the existence of token for user and if not exire
               string? token = await _authService.GenerateResetToken(logUser.Email);
               await _emailService.SendEmail(logUser, token);
            }         

            return Ok(msg);
        }


    }
}
