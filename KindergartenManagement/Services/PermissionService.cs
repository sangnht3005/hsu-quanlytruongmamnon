using KindergartenManagement.DAO;
using KindergartenManagement.DTO;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KindergartenManagement.Services
{
    public class PermissionService : IPermissionService
    {
        private readonly KindergartenDbContext _context;
        private HashSet<string> _currentPermissions = new();

        public PermissionService(KindergartenDbContext context)
        {
            _context = context;
        }

        public async Task<bool> HasPermissionAsync(Account account, string permissionName)
        {
            if (account?.RoleId == null) return false;

            var hasPermission = await _context.RolePermissions
                .Include(rp => rp.Permission)
                .AnyAsync(rp => rp.RoleId == account.RoleId && 
                               rp.Permission != null &&
                               rp.Permission.Name == permissionName);

            return hasPermission;
        }

        public async Task LoadPermissionsAsync(Account account)
        {
            _currentPermissions.Clear();

            if (account?.RoleId == null) return;

            var permissions = await _context.RolePermissions
                .Include(rp => rp.Permission)
                .Where(rp => rp.RoleId == account.RoleId && rp.Permission != null)
                .Select(rp => rp.Permission!.Name)
                .ToListAsync();

            _currentPermissions = new HashSet<string>(permissions);
        }

        public bool HasPermission(string permissionName)
        {
            return _currentPermissions.Contains(permissionName);
        }
    }
}
