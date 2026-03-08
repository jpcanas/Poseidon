using Microsoft.EntityFrameworkCore;
using Poseidon.Data;
using Poseidon.Models.Entities;
using Poseidon.Models.ViewModels;
using Poseidon.Repositories.Interfaces;
using System.Reflection;

namespace Poseidon.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly PoseidonDbContext _context;
        public RoleRepository(PoseidonDbContext context)
        {
            _context = context;
        }
        public async Task<List<RoleVM>> GetRoleList()
        {
            var role = await _context.Roles
                .Where(r => r.IsSystemRole == false) //do not include system roles in the list (Admin)
                .Select(r => new RoleVM
                {
                    RoleId = r.RoleId,
                    RoleName = r.RoleName,
                    Description = r.Description,
                    RoleType = r.RoleType,
                }).ToListAsync();

            return role;
        }
        public async Task<RolePermissionVM> GetRolePermissions(int roleId)
        {
            try
            {
                var roleData = await _context.Roles
                .Where(r => r.RoleId == roleId)
                .Select(r => new
                {
                    RoleId = r.RoleId,
                    RoleName = r.RoleName,
                    Description = r.Description,
                    IsSystemRole = r.IsSystemRole,
                    AssignedSubModuleIds = r.RolePermissions.Select(rp => rp.SubModuleId).ToList()
                }).FirstOrDefaultAsync();

                var modules = await _context.Modules
                    .Include(m => m.SubModules)
                    .Select(m => new ModulePermissionVM
                    {
                        ModuleId = m.Id,
                        ModuleName = m.Name,
                        ModuleDescription = m.Description,
                        SubModules = m.SubModules.Select(sm => new SubModulePermissionVM
                        {
                            SubModuleId = sm.Id,
                            SubModuleName = sm.Name,
                            SubModuleDescription = sm.Description,
                        }).ToList()
                    }).ToListAsync();

                var rolePermission = new RolePermissionVM();

                if (roleId == 0 && roleData == null)
                {
                    rolePermission = new RolePermissionVM
                    {
                        RoleId = 0,
                        RoleName = string.Empty,
                        Description = string.Empty,
                        IsSystemRole = false,
                        Permissions = modules.Select(module => new ModulePermissionVM
                        {
                            ModuleId = module.ModuleId,
                            ModuleName = module.ModuleName,
                            ModuleDescription = module.ModuleDescription,
                            SubModules = module.SubModules.Select(subModule => new SubModulePermissionVM
                            {
                                SubModuleId = subModule.SubModuleId,
                                SubModuleName = subModule.SubModuleName,
                                SubModuleDescription = subModule.SubModuleDescription,
                                IsAssigned = false,
                            }).ToList(),

                            Enabled = false

                        }).ToList()
                    };
                }
                else
                {
                    rolePermission = new RolePermissionVM
                    {
                        RoleId = roleData.RoleId,
                        RoleName = roleData.RoleName,
                        Description = roleData.Description,
                        IsSystemRole = roleData.IsSystemRole,
                        Permissions = modules.Select(module => new ModulePermissionVM
                        {
                            ModuleId = module.ModuleId,
                            ModuleName = module.ModuleName,
                            ModuleDescription = module.ModuleDescription,
                            SubModules = module.SubModules.Select(subModule => new SubModulePermissionVM
                            {
                                SubModuleId = subModule.SubModuleId,
                                SubModuleName = subModule.SubModuleName,
                                SubModuleDescription = subModule.SubModuleDescription,
                                IsAssigned = roleData.AssignedSubModuleIds.Contains(subModule.SubModuleId)
                            }).ToList(),

                            Enabled = module.SubModules.Any(sm => roleData.AssignedSubModuleIds.Contains(sm.SubModuleId))

                        }).ToList()
                    };
                }

                return rolePermission;

            }
            catch (Exception ex)
            {
                return new RolePermissionVM();
            }

        }
        public async Task<Role?> GetSingleRole(int roleId)
        {
            return await _context.Roles.FindAsync(roleId);
        }
        public async Task<int> AddRole(Role role)
        {
            try
            {
                _context.Roles.Add(role);
                await _context.SaveChangesAsync();
                return role.RoleId;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
        public async Task<int> UpdateRole(Role role)
        {
            try
            {
                var existingRole = await _context.Roles.FindAsync(role.RoleId);
                if (existingRole != null)
                {
                    existingRole.RoleName = role.RoleName;
                    existingRole.Description = role.Description;
                    existingRole.RoleType = role.RoleType;
                    existingRole.UpdatedBy = role.UpdatedBy;
                    existingRole.UpdatedDate = DateTime.UtcNow;
                    return await _context.SaveChangesAsync();
                }

                return 0;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public async Task AddRolePermissions(SaveRoleRequestVM rolePermissions)
        {
            try
            {
                var newPermissions = rolePermissions.SubModuleIds.Select(submodule => new RolePermission
                {
                    RoleId = rolePermissions.RoleId,
                    SubModuleId = submodule
                });

                if (newPermissions != null)
                {
                    await _context.RolePermissions.AddRangeAsync(newPermissions);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                // Handle exception (e.g., log it)
            }
        }

        public async Task DeleteRolePermission(int roleId)
        {
            try
            {
                await _context.RolePermissions
                     .Where(rp => rp.RoleId == roleId)
                     .ExecuteDeleteAsync();
            }
            catch (Exception ex)
            {
                // Handle exception (e.g., log it)
            }
        }

        public async Task<bool> CheckUserHasPermission(int userId, string permissionCode)
        {
            try
            {
                var hasPermission = await _context.Users
                    .Where(u => u.UserId == userId)
                    .SelectMany(u => u.Role.RolePermissions)
                    .AnyAsync(rp => rp.SubModule.Code == permissionCode);
                return hasPermission;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<List<string>> GetUserPermissions(int userId)
        {
            try
            {
                var permissions = await _context.Users
                    .Where(u => u.UserId == userId)
                    .SelectMany(u => u.Role.RolePermissions)
                    .Select(rp => rp.SubModule.Code)
                    .Distinct()
                    .ToListAsync();
                return permissions;
            }
            catch (Exception ex)
            {
                return new List<string>();
            }
        }
    }
}
