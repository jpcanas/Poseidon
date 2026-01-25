using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using Poseidon.Enums;
using Poseidon.Models.ViewModels;
using Poseidon.Services.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Threading.Tasks;

namespace Poseidon.Controllers
{
    [Authorize]
    public class SettingController : Controller
    {
        private readonly IUserService _userService;

        public SettingController(IUserService userService)
        {
            _userService = userService;
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

        [HttpGet("Setting/Users")]
        public async Task<IActionResult> GetUsers()
        {
            var users =  await _userService.GetUserTable();
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

            var userId = await _userService.AddUser(user);
            string msg = userId > 0 ? "User successfully added" : "Something went wrong. Cannot insert user";
            return Ok(new
            {
                isUserAdded = userId > 0,
                message = new { general = msg }
            });
        }
    }
}
