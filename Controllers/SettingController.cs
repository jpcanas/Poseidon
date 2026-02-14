using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using Poseidon.Enums;
using Poseidon.Models.Entities;
using Poseidon.Models.ViewModels;
using Poseidon.Models.ViewModels.Auth;
using Poseidon.Services.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Poseidon.Controllers
{
    [Authorize]
    public class SettingController : Controller
    {
        private readonly IUserService _userService;
        private readonly IAuthService _authService;

        public SettingController(IUserService userService, IAuthService authService)
        {
            _userService = userService;
            _authService = authService;
        }

        public async Task<IActionResult> UserSetting()
        {
            ViewBag.Roles = await _userService.GetRoleList();
            ViewBag.StatusList = await _userService.GetStatusList();
            ViewBag.BiologicalSexOptions = Enum.GetValues<BiologicalSexType>()
                .Select(e => new
                {
                    value = (int)e,
                    text = typeof(BiologicalSexType)
                            .GetField(e.ToString())
                            .GetCustomAttribute<DisplayAttribute>()?.Name ?? e.ToString()
                }).ToList();
            return View();
        }
        [Authorize]
        public async Task<IActionResult> ProfileSetting()
        {
            var currentUserGuid = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            UserVM? userModel = new UserVM();

            ViewBag.SexOptions = Enum.GetValues<BiologicalSexType>()
               .Select(e => new
               {
                   value = (int)e,
                   text = typeof(BiologicalSexType)
                           .GetField(e.ToString())
                           .GetCustomAttribute<DisplayAttribute>()?.Name ?? e.ToString()
               }).ToList();

            if (!string.IsNullOrEmpty(currentUserGuid))
            {
                userModel = await _authService.GetUserByGuid(currentUserGuid);    
            }

            return View(userModel);
        }

        [HttpGet("Setting/GetCurrentUser")]
        public async Task<IActionResult> GetCurrentUser()
        {
            var currentUserGuid = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            UserVM? userModel = new UserVM();

            if (!string.IsNullOrEmpty(currentUserGuid))
            {
                userModel = await _authService.GetUserByGuid(currentUserGuid);
            }

            return Ok(userModel);
        }

        [HttpGet("Setting/Users")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _userService.GetUserTable();
            return Ok(users);
        }

        [HttpPost("Setting/AddUser")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddUser([FromBody] UserVM user)
        {
            if (user is null)
                return BadRequest(new
                {
                    isUserAdded = false,
                    message = new { general = "Something went wrong. Cannot insert user" }
                });

            var duplicateEmail = await _userService.GetUserByEmailorUsername(email: user.Email);
            if (duplicateEmail != null)
                return BadRequest(new
                {
                    isUserAdded = false,
                    message = new { email = "Email already exists" }
                });

            if (!string.IsNullOrWhiteSpace(user.UserName))
            {
                var duplicateUsername = await _userService.GetUserByEmailorUsername(username: user.UserName);
                if (duplicateUsername != null)
                    return BadRequest(new
                    {
                        isUserAdded = false,
                        message = new { username = "Username already exists" }
                    });
            }

            string msg = "Something went wrong. Cannot insert user";

            user.CreatedBy = User.FindFirst(ClaimTypes.Email)?.Value ?? string.Empty;

            var newUser = await _userService.AddUser(user);
            if (newUser != null)
            {
                msg = "User successfully added";
                await _authService.SendWelcomeEmail(newUser);
            }

            return Ok(new
            {
                isUserAdded = newUser != null,
                message = new { general = msg }
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateUser([FromBody] UserVM userUpdate)
        {

            if (userUpdate is null)
                return BadRequest(new
                {
                    user = new UserVM(),
                    message = new { general = "Update Failed" }
                });

            var duplicateUsername = await _userService.GetUserByEmailorUsername(username: userUpdate.UserName);

            if (duplicateUsername != null && duplicateUsername.UserId != userUpdate.UserId)
                return BadRequest(new
                {
                    user = new UserVM(),
                    message = new { username = "Username already exists" }
                });

            userUpdate.UpdatedBy = User.FindFirst(ClaimTypes.Email)?.Value;
            UserVM updatedUser = await _userService.UpdateUserData(userUpdate);

            return Ok(new
            {
                user = updatedUser,
                message = new { general = "User data successfully updated" }
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateUserPassword([FromBody] UserPasswordVM passwordData)
        {
            if (!ModelState.IsValid || passwordData == null)
            {
                Dictionary<string, string[]?>? errors = ModelState
                    .Where(x => x.Value?.Errors.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray()
                        );

                return BadRequest(new { Success = false, Errors = errors });
            }

            object? currentPasswordValidity = await _authService.CheckExistingPasswordForUpdate(passwordData);
            if (currentPasswordValidity != null)
            {
                return BadRequest(new
                {
                    Success = false,
                    Errors = currentPasswordValidity
                });
            }

            int updateResult = await _authService.UpdateUserPassword(passwordData.UserId, passwordData.NewPassword);

            return Ok(new
            {
                Success = updateResult > 0,
                Errors = new { General = new string[] { string.Empty } }
            });
        }
    }
}
