using Poseidon.Models.Entities;
using Poseidon.Models.ViewModels;
using Poseidon.Repositories.Interfaces;
using Poseidon.Services.Interfaces;

namespace Poseidon.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;

        public UserService(IUserRepository userRepository, IRoleRepository roleRepository)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
        }

        public async Task<List<UserTableVM>> GetUserTable(string? status = null)
        {
            var users =  await _userRepository.GetUsers();

            List<UserTableVM> userList = users.Select(u => new UserTableVM
            {
                Email = u.Email,
                FullName = $"{u.FirstName} {u.MiddleName} {u.LastName}",
                BirthDate = u.BirthDate,
                BiologicalSex = u.BiologicalSex?.ToString(),
                Address = u.Address,
                CreatedDate = u.CreatedDate,
                UpdatedDate = u.UpdatedDate,
                RoleName = u.Role?.RoleName,
                Status = u.UserStatus?.Name

            }).ToList();

            return userList;
        }

        public async Task<List<RoleVM>> GetRoleList()
        {
            return await _roleRepository.GetRoleList();
        }
        public async Task<List<UserStatusVM>> GetStatusList()
        {
            return await _userRepository.GetStatus();
        }
        public async Task<int> AddUser(UserVM userVM)
        {
            var newUser = new User
            {
                Email = userVM.Email,
                UserName = userVM.UserName,
                Password = BCrypt.Net.BCrypt.HashPassword("Password1"),
                FirstName = userVM.FirstName,
                LastName = userVM.LastName,
                RoleId = userVM.RoleId,
                UserStatusId = userVM.UserStatusId,
                CreatedDate = DateTime.UtcNow,
                RequiredPasswordChange = true,
                BiologicalSex = userVM.BiologicalSex,
                BirthDate = userVM.BirthDate,
                Address = userVM.Address,
            };

            return await _userRepository.AddUser(newUser);
        }

        public async Task<User?> GetUserByEmailorUsername(string? email = null, string? username = null)
        {
            return await _userRepository.GetUserByEmailOrUsername(email, username);
        }
    }
}
