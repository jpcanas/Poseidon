using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Poseidon.Configurations;
using Poseidon.Models.Entities;
using Poseidon.Models.ViewModels;
using Poseidon.Services;
using System.Security.Claims;

namespace Poseidon.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost("Auth/Login")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel loginCreds, string? redirectUrl = null)
        {
            if(!ModelState.IsValid || loginCreds == null)
                return View(loginCreds);

            User? logUser = await _authService.LoginUser(loginCreds);

            if (logUser == null)
            {
                ModelState.AddModelError("", "Email or Password is incorrect");
                return View(loginCreds);
            }

            CookieClaims cookieClaims = _authService.SetupClaims(loginCreds, logUser);

            await HttpContext.SignInAsync("PoseidonAuth", cookieClaims.Principal, cookieClaims.AuthProperties);

            if(!string.IsNullOrEmpty(redirectUrl) && Url.IsLocalUrl(redirectUrl))
                return Redirect(redirectUrl);

            return RedirectToAction("Index", "Home");

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
            //return Ok(new { status = "active" });

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


    }
}
