using Microsoft.EntityFrameworkCore;
using Poseidon.Data;
using Poseidon.Models.Entities;
using Poseidon.Models.ViewModels;
using Poseidon.Repositories.Interfaces;

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
                .Select(r => new RoleVM
                {
                    RoleId = r.RoleId,
                    RoleName = r.RoleName,
                    Description = r.Description,
                    RoleType = r.RoleType,
                }).ToListAsync();

            return role;
        }

    }
}
