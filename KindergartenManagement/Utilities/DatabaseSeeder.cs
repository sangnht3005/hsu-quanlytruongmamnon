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
                new Permission { Name = "ViewDashboard", DisplayName = "Xem tổng quan", Description = "Xem bảng điều khiển" },
                new Permission { Name = "ManageStudents", DisplayName = "Quản lý học sinh", Description = "Quản lý học sinh" },
                new Permission { Name = "ManageClasses", DisplayName = "Quản lý lớp học", Description = "Quản lý lớp học" },
                new Permission { Name = "ManageStaff", DisplayName = "Quản lý nhân viên", Description = "Quản lý nhân viên" },
                new Permission { Name = "ManageParents", DisplayName = "Quản lý phụ huynh", Description = "Quản lý phụ huynh" },
                new Permission { Name = "ManageAccounts", DisplayName = "Quản lý tài khoản", Description = "Quản lý tài khoản" },
                new Permission { Name = "ManageGrades", DisplayName = "Quản lý khối lớp", Description = "Quản lý khối lớp" },
                new Permission { Name = "ManageAttendance", DisplayName = "Quản lý điểm danh", Description = "Quản lý điểm danh" },
                new Permission { Name = "ManageHealth", DisplayName = "Quản lý sức khỏe", Description = "Quản lý sức khỏe" },
                new Permission { Name = "ManageFinance", DisplayName = "Quản lý tài chính", Description = "Quản lý tài chính" },
                new Permission { Name = "ManageMenu", DisplayName = "Quản lý thực đơn", Description = "Quản lý thực đơn" },
                new Permission { Name = "ManageLeave", DisplayName = "Quản lý nghỉ phép", Description = "Quản lý nghỉ phép" },
                new Permission { Name = "ViewReports", DisplayName = "Xem báo cáo", Description = "Xem báo cáo" }
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
