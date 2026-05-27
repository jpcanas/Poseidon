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
                UserId = u.UserId,
                Email = u.Email,
                FullName = $"{u.FirstName} {u.MiddleName} {u.LastName}",
                UserName = u.UserName,
                FirstName = u.FirstName,
                MiddleName = u.MiddleName,
                LastName = u.LastName,
                BirthDate = u.BirthDate,
                BiologicalSex = u.BiologicalSex,
                Address = u.Address,
                CreatedDate = u.CreatedDate,
                UpdatedDate = u.UpdatedDate,
                RoleId = u.RoleId,
                RoleName = u.Role?.RoleName,
                UserStatusId = u.UserStatusId,
                Status = u.UserStatus?.Name,
                MobileNumber = u.MobileNumber,
                StatusColor = u.UserStatus?.Color,
                ProfilePictureFileRecordId = u.ProfilePictureFileRecordId,
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
        public async Task<User?> AddUser(UserVM userVM)
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
                RequiredPasswordChange = true,
                BiologicalSex = userVM.BiologicalSex,
                BirthDate = userVM.BirthDate,
                Address = userVM.Address,
                CreatedBy = userVM.CreatedBy,
            };

            return await _userRepository.AddUser(newUser);
        }

        public async Task<User?> GetUserByEmailorUsername(string? email = null, string? username = null)
        {
            return await _userRepository.GetUserByEmailOrUsername(email, username);
        }

        public async Task<UserVM> UpdateUserData(UserVM userModel)
        {
            var userEntity = await _userRepository.UpdateUserData(userModel);
            UserVM updatedUser = new UserVM();
            if (userEntity != null)
            {
                updatedUser.UserId = userEntity.UserId;
                updatedUser.Email = userEntity.Email;
                updatedUser.UserName = userEntity.UserName;
                updatedUser.RoleId = userEntity.Role.RoleId;
                updatedUser.RoleName = userEntity.Role.RoleName;
                updatedUser.UserStatusId = userEntity.UserStatusId;
                updatedUser.StatusName = userEntity.UserStatus?.Name;
                updatedUser.FirstName = userEntity.FirstName;
                updatedUser.LastName = userEntity.LastName;
                updatedUser.MiddleName = userEntity.MiddleName;
                updatedUser.BiologicalSex = userEntity.BiologicalSex;
                updatedUser.BirthDate = userEntity.BirthDate;
                updatedUser.MobileNumber = userEntity.MobileNumber;
                updatedUser.Address = userEntity.Address;
            }
            return updatedUser;
        }
        public async Task<RolePermissionVM> GetRolePermissions(int roleId)
        {
            return await _roleRepository.GetRolePermissions(roleId);
        }
        public async Task<int> SaveRolePermissions(SaveRoleRequestVM request)
        {
            Role? role = await _roleRepository.GetSingleRole(request.RoleId);
            int resultId = 0;
            if (role == null) // add new role
            {
                role = new Role
                {
                    RoleName = request.RoleName,
                    Description = request.Description,
                    CreatedBy = request.CreatedBy ?? string.Empty,
                };

                resultId = await _roleRepository.AddRole(role);
                request.RoleId = resultId;
            }
            else // update existing role
            {
                role.RoleName = request.RoleName;
                role.Description = request.Description;
                role.UpdatedBy = request.UpdatedBy ?? string.Empty;
                role.UpdatedDate = DateTime.UtcNow;

                resultId = await _roleRepository.UpdateRole(role);
            }

            await _roleRepository.DeleteRolePermission(role.RoleId);
            await _roleRepository.AddRolePermissions(request);

            return resultId;
        }
        public async Task<int> UpdateUserDatabyAdmin(UserVM userModel)
        {
            return await _userRepository.UpdateUserDatabyAdmin(userModel);
        }
        public async Task<int> UpdateUserProfilePictureId(int userId, int fileRecordId)
        {
            return await _userRepository.UpdateUserProfilePicId(userId, fileRecordId);
        }
    }
}
