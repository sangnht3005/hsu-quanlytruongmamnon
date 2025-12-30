using KindergartenManagement.DAO;
using KindergartenManagement.DTO;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace KindergartenManagement.Utilities
{
    public class DatabaseSeeder
    {
        private readonly KindergartenDbContext _context;

        public DatabaseSeeder(KindergartenDbContext context)
        {
            _context = context;
        }

        public async Task SeedAsync()
        {
            // Check if data already exists
            if (_context.Accounts.Any())
            {
                return; // Database already seeded
            }

            // 1. Create Admin Staff User
            var adminUser = new User
            {
                FullName = "Sang Nguyen",
                Email = "admin@kindergarten.com",
                PhoneNumber = "0123456789",
                Position = "Quản trị viên",
                Salary = 0,
                Allowance = 0,
                Specialization = "Quản lý hệ thống"
            };
            _context.Users.Add(adminUser);
            await _context.SaveChangesAsync();

            // 2. Create Admin Role
            var adminRole = new Role
            {
                Name = "Admin",
                Description = "Quản trị viên - Toàn quyền truy cập hệ thống"
            };
            _context.Roles.Add(adminRole);
            await _context.SaveChangesAsync();

            // 3. Create Permissions
            var permissions = new[]
            {
                new Permission { Name = "ViewDashboard", Description = "Xem bảng điều khiển" },
                new Permission { Name = "ManageStudents", Description = "Quản lý học sinh" },
                new Permission { Name = "ManageClasses", Description = "Quản lý lớp học" },
                new Permission { Name = "ManageStaff", Description = "Quản lý nhân viên" },
                new Permission { Name = "ManageParents", Description = "Quản lý phụ huynh" },
                new Permission { Name = "ManageAccounts", Description = "Quản lý tài khoản" },
                new Permission { Name = "ManageGrades", Description = "Quản lý khối lớp" },
                new Permission { Name = "ManageAttendance", Description = "Quản lý điểm danh" },
                new Permission { Name = "ManageHealth", Description = "Quản lý sức khỏe" },
                new Permission { Name = "ManageFinance", Description = "Quản lý tài chính" },
                new Permission { Name = "ManageMenu", Description = "Quản lý thực đơn" },
                new Permission { Name = "ManageLeave", Description = "Quản lý nghỉ phép" },
                new Permission { Name = "ViewReports", Description = "Xem báo cáo" }
            };

            _context.Permissions.AddRange(permissions);
            await _context.SaveChangesAsync();

            // 4. Assign all permissions to Admin role
            foreach (var permission in permissions)
            {
                _context.RolePermissions.Add(new RolePermission
                {
                    RoleId = adminRole.Id,
                    PermissionId = permission.Id
                });
            }
            await _context.SaveChangesAsync();

            // 5. Create Admin Account
            var adminAccount = new Account
            {
                Username = "admin",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                UserId = adminUser.Id,
                RoleId = adminRole.Id,
                IsActive = true
            };
            _context.Accounts.Add(adminAccount);
            await _context.SaveChangesAsync();

            System.Diagnostics.Debug.WriteLine("✅ Database seeded successfully!");
            System.Diagnostics.Debug.WriteLine($"Admin account created:");
            System.Diagnostics.Debug.WriteLine($"  - Name: {adminUser.FullName}");
            System.Diagnostics.Debug.WriteLine($"  - Username: {adminAccount.Username}");
            System.Diagnostics.Debug.WriteLine($"  - Password: admin123");
        }
    }
}
