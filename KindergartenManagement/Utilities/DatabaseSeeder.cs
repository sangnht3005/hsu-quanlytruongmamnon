using KindergartenManagement.DTO;
using KindergartenManagement.DAO;

namespace KindergartenManagement.Utilities;

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
        if (_context.Roles.Any())
        {
            return; // Database already seeded
        }

        // Seed Permissions
        var permissions = new List<Permission>
        {
            new Permission { Name = "ViewStudents", Description = "Xem danh sách học sinh" },
            new Permission { Name = "ManageStudents", Description = "Quản lý học sinh" },
            new Permission { Name = "ViewClasses", Description = "Xem danh sách lớp học" },
            new Permission { Name = "ManageClasses", Description = "Quản lý lớp học" },
            new Permission { Name = "ViewAttendance", Description = "Xem điểm danh" },
            new Permission { Name = "ManageAttendance", Description = "Quản lý điểm danh" },
            new Permission { Name = "ViewHealth", Description = "Xem sức khỏe" },
            new Permission { Name = "ManageHealth", Description = "Quản lý sức khỏe" },
            new Permission { Name = "ViewMenu", Description = "Xem thực đơn" },
            new Permission { Name = "ManageMenu", Description = "Quản lý thực đơn" },
            new Permission { Name = "ViewInvoices", Description = "Xem hóa đơn" },
            new Permission { Name = "ManageInvoices", Description = "Quản lý hóa đơn" }
        };
        _context.Permissions.AddRange(permissions);
        await _context.SaveChangesAsync();

        // Seed Roles
        var adminRole = new Role { Name = "Admin", Description = "Quản trị viên (Hiệu trưởng)" };
        var managerRole = new Role { Name = "Manager", Description = "Quản lý (Phó hiệu trưởng)" };
        var teacherRole = new Role { Name = "Teacher", Description = "Giáo viên" };
        var accountantRole = new Role { Name = "Accountant", Description = "Kế toán" };
        var nurseRole = new Role { Name = "Nurse", Description = "Y tá" };

        _context.Roles.AddRange(adminRole, managerRole, teacherRole, accountantRole, nurseRole);
        await _context.SaveChangesAsync();

        // Assign all permissions to Admin
        foreach (var permission in permissions)
        {
            _context.RolePermissions.Add(new RolePermission
            {
                RoleId = adminRole.Id,
                PermissionId = permission.Id
            });
        }

        // Assign limited permissions to Teacher
        var teacherPermissions = permissions.Where(p => 
            p.Name.Contains("View") || 
            p.Name == "ManageAttendance" || 
            p.Name == "ManageHealth").ToList();
        
        foreach (var permission in teacherPermissions)
        {
            _context.RolePermissions.Add(new RolePermission
            {
                RoleId = teacherRole.Id,
                PermissionId = permission.Id
            });
        }

        await _context.SaveChangesAsync();

        // Seed Users (Staff) for Phase 1
        var users = new List<User>
        {
            new User 
            { 
                FullName = "Nguyễn Văn Admin", 
                Email = "admin@kindergarten.com", 
                PhoneNumber = "0901111111",
                Position = "Hiệu trưởng",
                Salary = 15000000,
                Allowance = 3000000,
                Specialization = "Quản lý giáo dục"
            },
            new User 
            { 
                FullName = "Trần Thị Mai", 
                Email = "mai.tran@kindergarten.com", 
                PhoneNumber = "0902222222",
                Position = "Giáo viên",
                Salary = 8000000,
                Allowance = 1000000,
                Specialization = "Mầm non"
            },
            new User 
            { 
                FullName = "Lê Văn Phúc", 
                Email = "phuc.le@kindergarten.com", 
                PhoneNumber = "0903333333",
                Position = "Giáo viên",
                Salary = 8500000,
                Allowance = 1000000,
                Specialization = "Mầm non"
            },
            new User 
            { 
                FullName = "Phạm Thị Lan", 
                Email = "lan.pham@kindergarten.com", 
                PhoneNumber = "0904444444",
                Position = "Kế toán",
                Salary = 9000000,
                Allowance = 1500000,
                Specialization = "Kế toán tài chính"
            },
            new User 
            { 
                FullName = "Hoàng Văn Nam", 
                Email = "nam.hoang@kindergarten.com", 
                PhoneNumber = "0905555555",
                Position = "Y tá",
                Salary = 7000000,
                Allowance = 1000000,
                Specialization = "Y tế học đường"
            }
        };
        _context.Users.AddRange(users);
        await _context.SaveChangesAsync();

        // Create accounts for staff (Phase 1)
        var accounts = new List<Account>
        {
            new Account
            {
                Username = "admin",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                UserId = users[0].Id,
                RoleId = adminRole.Id,
                IsActive = true
            },
            new Account
            {
                Username = "mai.tran",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("teacher123"),
                UserId = users[1].Id,
                RoleId = teacherRole.Id,
                IsActive = true
            },
            new Account
            {
                Username = "phuc.le",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("teacher123"),
                UserId = users[2].Id,
                RoleId = teacherRole.Id,
                IsActive = true
            },
            new Account
            {
                Username = "lan.pham",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("accountant123"),
                UserId = users[3].Id,
                RoleId = accountantRole.Id,
                IsActive = true
            }
        };
        _context.Accounts.AddRange(accounts);
        await _context.SaveChangesAsync();

        // Seed Grades
        var grades = new List<Grade>
        {
            new Grade { Name = "Nhà trẻ", Description = "Lớp nhà trẻ", MinAge = 0, MaxAge = 2 },
            new Grade { Name = "Mẫu giáo nhỏ", Description = "Lớp mẫu giáo nhỏ", MinAge = 3, MaxAge = 3 },
            new Grade { Name = "Mẫu giáo vừa", Description = "Lớp mẫu giáo vừa", MinAge = 4, MaxAge = 4 },
            new Grade { Name = "Mẫu giáo lớn", Description = "Lớp mẫu giáo lớn", MinAge = 5, MaxAge = 5 }
        };
        _context.Grades.AddRange(grades);
        await _context.SaveChangesAsync();

        // Seed sample classes with teachers
        var classes = new List<Class>
        {
            new Class { Name = "Lớp Hoa Mai", GradeId = grades[0].Id, Capacity = 20, TeacherId = users[1].Id },
            new Class { Name = "Lớp Hoa Hồng", GradeId = grades[1].Id, Capacity = 25, TeacherId = users[2].Id },
            new Class { Name = "Lớp Hoa Lan", GradeId = grades[2].Id, Capacity = 25, TeacherId = users[1].Id },
            new Class { Name = "Lớp Hoa Cúc", GradeId = grades[3].Id, Capacity = 30, TeacherId = users[2].Id }
        };
        _context.Classes.AddRange(classes);
        await _context.SaveChangesAsync();

        // Seed sample parents (Phase 1)
        var parents = new List<Parent>
        {
            new Parent 
            { 
                FullName = "Nguyễn Văn A", 
                Email = "nguyenvana@gmail.com", 
                PhoneNumber = "0901234567",
                Address = "123 Đường ABC, Quận 1, TP.HCM",
                Occupation = "Kỹ sư phần mềm"
            },
            new Parent 
            { 
                FullName = "Trần Thị B", 
                Email = "tranthib@gmail.com", 
                PhoneNumber = "0912345678",
                Address = "456 Đường XYZ, Quận 2, TP.HCM",
                Occupation = "Giáo viên tiểu học"
            },
            new Parent 
            { 
                FullName = "Lê Văn C", 
                Email = "levanc@gmail.com", 
                PhoneNumber = "0923456789",
                Address = "789 Đường DEF, Quận 3, TP.HCM",
                Occupation = "Bác sĩ"
            },
            new Parent 
            { 
                FullName = "Phạm Thị D", 
                Email = "phamthid@gmail.com", 
                PhoneNumber = "0934567890",
                Address = "101 Đường GHI, Quận 4, TP.HCM",
                Occupation = "Kinh doanh"
            },
            new Parent 
            { 
                FullName = "Hoàng Văn E", 
                Email = "hoangvane@gmail.com", 
                PhoneNumber = "0945678901",
                Address = "202 Đường JKL, Quận 5, TP.HCM",
                Occupation = "Luật sư"
            }
        };
        _context.Parents.AddRange(parents);
        await _context.SaveChangesAsync();

        // Seed sample students
        var students = new List<Student>
        {
            new Student
            {
                FullName = "Nguyễn Văn An",
                DateOfBirth = new DateTime(2021, 5, 15),
                Gender = "Nam",
                Address = "123 Đường ABC, Quận 1, TP.HCM",
                ParentId = parents[0].Id,
                ClassId = classes[0].Id,
                MedicalNotes = "Không có dị ứng"
            },
            new Student
            {
                FullName = "Trần Thị Bảo",
                DateOfBirth = new DateTime(2020, 8, 20),
                Gender = "Nữ",
                Address = "456 Đường XYZ, Quận 2, TP.HCM",
                ParentId = parents[1].Id,
                ClassId = classes[1].Id,
                MedicalNotes = "Dị ứng hải sản"
            },
            new Student
            {
                FullName = "Lê Thị Cẩm",
                DateOfBirth = new DateTime(2019, 3, 10),
                Gender = "Nữ",
                Address = "789 Đường DEF, Quận 3, TP.HCM",
                ParentId = parents[2].Id,
                ClassId = classes[2].Id
            },
            new Student
            {
                FullName = "Phạm Văn Dũng",
                DateOfBirth = new DateTime(2019, 11, 25),
                Gender = "Nam",
                Address = "101 Đường GHI, Quận 4, TP.HCM",
                ParentId = parents[3].Id,
                ClassId = classes[2].Id
            },
            new Student
            {
                FullName = "Hoàng Thị Em",
                DateOfBirth = new DateTime(2018, 7, 8),
                Gender = "Nữ",
                Address = "202 Đường JKL, Quận 5, TP.HCM",
                ParentId = parents[4].Id,
                ClassId = classes[3].Id
            }
        };
        _context.Students.AddRange(students);
        await _context.SaveChangesAsync();

        // Seed StaffLeave data (Phase 1)
        var staffLeaves = new List<StaffLeave>
        {
            new StaffLeave
            {
                UserId = users[1].Id,
                StartDate = DateTime.Now.AddDays(-10),
                EndDate = DateTime.Now.AddDays(-8),
                LeaveType = "Nghỉ phép",
                Reason = "Nghỉ phép thường niên",
                Status = "Đã duyệt",
                ApprovedBy = users[0].Id,
                ApprovalDate = DateTime.Now.AddDays(-12),
                ApprovalNotes = "Đã duyệt"
            },
            new StaffLeave
            {
                UserId = users[2].Id,
                StartDate = DateTime.Now.AddDays(5),
                EndDate = DateTime.Now.AddDays(7),
                LeaveType = "Nghỉ ốm",
                Reason = "Khám bệnh định kỳ",
                Status = "Chờ duyệt"
            },
            new StaffLeave
            {
                UserId = users[1].Id,
                StartDate = DateTime.Now.AddDays(-30),
                EndDate = DateTime.Now.AddDays(-28),
                LeaveType = "Nghỉ phép",
                Reason = "Việc gia đình",
                Status = "Đã duyệt",
                ApprovedBy = users[0].Id,
                ApprovalDate = DateTime.Now.AddDays(-32),
                ApprovalNotes = "Đồng ý cho nghỉ"
            },
            new StaffLeave
            {
                UserId = users[4].Id,
                StartDate = DateTime.Now.AddDays(10),
                EndDate = DateTime.Now.AddDays(12),
                LeaveType = "Nghỉ công tác",
                Reason = "Tham gia hội thảo y tế",
                Status = "Chờ duyệt"
            }
        };
        _context.StaffLeaves.AddRange(staffLeaves);
        await _context.SaveChangesAsync();

        // Seed Vaccines (Phase 4)
        var vaccines = new List<Vaccine>
        {
            new Vaccine
            {
                Name = "BCG (Lao)",
                Description = "Vaccine phòng bệnh lao",
                RequiredAgeMonths = 0,
                DiseasesPrevented = "Bệnh lao",
                IsMandatory = true
            },
            new Vaccine
            {
                Name = "Viêm gan B - Mũi 1",
                Description = "Vaccine phòng viêm gan B - Mũi tiêm thứ nhất",
                RequiredAgeMonths = 0,
                DiseasesPrevented = "Viêm gan B",
                IsMandatory = true
            },
            new Vaccine
            {
                Name = "Viêm gan B - Mũi 2",
                Description = "Vaccine phòng viêm gan B - Mũi tiêm thứ hai",
                RequiredAgeMonths = 1,
                DiseasesPrevented = "Viêm gan B",
                IsMandatory = true
            },
            new Vaccine
            {
                Name = "Viêm gan B - Mũi 3",
                Description = "Vaccine phòng viêm gan B - Mũi tiêm thứ ba",
                RequiredAgeMonths = 2,
                DiseasesPrevented = "Viêm gan B",
                IsMandatory = true
            },
            new Vaccine
            {
                Name = "Bại liệt (OPV) - Mũi 1",
                Description = "Vaccine phòng bệnh bại liệt uống - Mũi 1",
                RequiredAgeMonths = 2,
                DiseasesPrevented = "Bại liệt",
                IsMandatory = true
            },
            new Vaccine
            {
                Name = "Bại liệt (OPV) - Mũi 2",
                Description = "Vaccine phòng bệnh bại liệt uống - Mũi 2",
                RequiredAgeMonths = 3,
                DiseasesPrevented = "Bại liệt",
                IsMandatory = true
            },
            new Vaccine
            {
                Name = "Bại liệt (OPV) - Mũi 3",
                Description = "Vaccine phòng bệnh bại liệt uống - Mũi 3",
                RequiredAgeMonths = 4,
                DiseasesPrevented = "Bại liệt",
                IsMandatory = true
            },
            new Vaccine
            {
                Name = "Bạch hầu - Ho gà - Uốn ván (DPT) - Mũi 1",
                Description = "Vaccine 3 trong 1: Bạch hầu, Ho gà, Uốn ván - Mũi 1",
                RequiredAgeMonths = 2,
                DiseasesPrevented = "Bạch hầu, Ho gà, Uốn ván",
                IsMandatory = true
            },
            new Vaccine
            {
                Name = "Bạch hầu - Ho gà - Uốn ván (DPT) - Mũi 2",
                Description = "Vaccine 3 trong 1: Bạch hầu, Ho gà, Uốn ván - Mũi 2",
                RequiredAgeMonths = 3,
                DiseasesPrevented = "Bạch hầu, Ho gà, Uốn ván",
                IsMandatory = true
            },
            new Vaccine
            {
                Name = "Bạch hầu - Ho gà - Uốn ván (DPT) - Mũi 3",
                Description = "Vaccine 3 trong 1: Bạch hầu, Ho gà, Uốn ván - Mũi 3",
                RequiredAgeMonths = 4,
                DiseasesPrevented = "Bạch hầu, Ho gà, Uốn ván",
                IsMandatory = true
            },
            new Vaccine
            {
                Name = "Sởi - Quai bị - Rubella (MMR) - Mũi 1",
                Description = "Vaccine 3 trong 1: Sởi, Quai bị, Rubella - Mũi 1",
                RequiredAgeMonths = 9,
                DiseasesPrevented = "Sởi, Quai bị, Rubella",
                IsMandatory = true
            },
            new Vaccine
            {
                Name = "Sởi - Quai bị - Rubella (MMR) - Mũi 2",
                Description = "Vaccine 3 trong 1: Sởi, Quai bị, Rubella - Mũi 2",
                RequiredAgeMonths = 18,
                DiseasesPrevented = "Sởi, Quai bị, Rubella",
                IsMandatory = true
            },
            new Vaccine
            {
                Name = "Viêm não Nhật Bản - Mũi 1",
                Description = "Vaccine phòng viêm não Nhật Bản - Mũi 1",
                RequiredAgeMonths = 12,
                DiseasesPrevented = "Viêm não Nhật Bản",
                IsMandatory = true
            },
            new Vaccine
            {
                Name = "Viêm não Nhật Bản - Mũi 2",
                Description = "Vaccine phòng viêm não Nhật Bản - Mũi 2",
                RequiredAgeMonths = 24,
                DiseasesPrevented = "Viêm não Nhật Bản",
                IsMandatory = true
            },
            new Vaccine
            {
                Name = "Thủy đậu",
                Description = "Vaccine phòng bệnh thủy đậu",
                RequiredAgeMonths = 12,
                DiseasesPrevented = "Thủy đậu",
                IsMandatory = false
            },
            new Vaccine
            {
                Name = "Cúm (Influenza)",
                Description = "Vaccine phòng cúm hàng năm",
                RequiredAgeMonths = 6,
                DiseasesPrevented = "Cúm",
                IsMandatory = false
            },
            new Vaccine
            {
                Name = "Viêm gan A",
                Description = "Vaccine phòng viêm gan A",
                RequiredAgeMonths = 18,
                DiseasesPrevented = "Viêm gan A",
                IsMandatory = false
            },
            new Vaccine
            {
                Name = "Phế cầu (Pneumococcal)",
                Description = "Vaccine phòng bệnh do vi khuẩn phế cầu",
                RequiredAgeMonths = 2,
                DiseasesPrevented = "Viêm phổi, Viêm tai giữa, Viêm màng não",
                IsMandatory = false
            },
            new Vaccine
            {
                Name = "Rotavirus",
                Description = "Vaccine phòng tiêu chảy do Rotavirus",
                RequiredAgeMonths = 2,
                DiseasesPrevented = "Tiêu chảy cấp",
                IsMandatory = false
            }
        };
        _context.Vaccines.AddRange(vaccines);
        await _context.SaveChangesAsync();

        // Seed some VaccinationRecords for existing students (if any)
        if (students.Any())
        {
            var vaccinationRecords = new List<VaccinationRecord>
            {
                // Student 1 vaccinations
                new VaccinationRecord
                {
                    StudentId = students[0].Id,
                    VaccineId = vaccines[0].Id, // BCG
                    Status = "Done",
                    VaccinationDate = students[0].DateOfBirth.AddDays(1),
                    MedicalUnit = "Bệnh viện Nhi Đồng 1",
                    LotNumber = "BCG-2024-001",
                    Notes = "Tiêm thành công, không có phản ứng phụ"
                },
                new VaccinationRecord
                {
                    StudentId = students[0].Id,
                    VaccineId = vaccines[1].Id, // Viêm gan B - Mũi 1
                    Status = "Done",
                    VaccinationDate = students[0].DateOfBirth.AddDays(1),
                    MedicalUnit = "Bệnh viện Nhi Đồng 1",
                    LotNumber = "HBV-2024-015",
                    Notes = "Tiêm thành công"
                },
                new VaccinationRecord
                {
                    StudentId = students[0].Id,
                    VaccineId = vaccines[2].Id, // Viêm gan B - Mũi 2
                    Status = "Done",
                    VaccinationDate = students[0].DateOfBirth.AddMonths(1),
                    MedicalUnit = "Trạm Y tế Phường 1",
                    LotNumber = "HBV-2024-020"
                },
                new VaccinationRecord
                {
                    StudentId = students[0].Id,
                    VaccineId = vaccines[10].Id, // MMR - Mũi 1
                    Status = "Done",
                    VaccinationDate = students[0].DateOfBirth.AddMonths(9),
                    MedicalUnit = "Trạm Y tế Phường 1",
                    LotNumber = "MMR-2024-045"
                },
                // Student 2 vaccinations
                new VaccinationRecord
                {
                    StudentId = students[1].Id,
                    VaccineId = vaccines[0].Id, // BCG
                    Status = "Done",
                    VaccinationDate = students[1].DateOfBirth.AddDays(2),
                    MedicalUnit = "Bệnh viện Nhi Đồng 2"
                },
                new VaccinationRecord
                {
                    StudentId = students[1].Id,
                    VaccineId = vaccines[14].Id, // Thủy đậu
                    Status = "Scheduled",
                    VaccinationDate = DateTime.Now.AddDays(30),
                    MedicalUnit = "Trạm Y tế Phường 2",
                    Notes = "Đã đặt lịch tiêm"
                },
                // Student 3 vaccinations
                new VaccinationRecord
                {
                    StudentId = students[2].Id,
                    VaccineId = vaccines[0].Id, // BCG
                    Status = "Done",
                    VaccinationDate = students[2].DateOfBirth.AddDays(1),
                    MedicalUnit = "Phòng khám Đa khoa Quốc tế"
                },
                new VaccinationRecord
                {
                    StudentId = students[2].Id,
                    VaccineId = vaccines[15].Id, // Cúm
                    Status = "Not Done",
                    Notes = "Chưa tiêm, cần theo dõi"
                }
            };
            _context.VaccinationRecords.AddRange(vaccinationRecords);
            await _context.SaveChangesAsync();
        }

        // Seed some HealthRecords (monthly tracking) for existing students
        if (students.Any())
        {
            var healthRecords = new List<HealthRecord>
            {
                new HealthRecord
                {
                    StudentId = students[0].Id,
                    Month = DateTime.Now.Month - 1 > 0 ? DateTime.Now.Month - 1 : 12,
                    Year = DateTime.Now.Month - 1 > 0 ? DateTime.Now.Year : DateTime.Now.Year - 1,
                    Height = 105.5m,
                    Weight = 18.2m,
                    GeneralHealth = "Tốt",
                    MedicalConditions = "Không có",
                    TeacherComments = "Bé năng động, vui vẻ, ăn uống tốt",
                    TeacherEvaluation = "Phát triển tốt, đúng chuẩn"
                },
                new HealthRecord
                {
                    StudentId = students[0].Id,
                    Month = DateTime.Now.Month,
                    Year = DateTime.Now.Year,
                    Height = 106.0m,
                    Weight = 18.5m,
                    GeneralHealth = "Tốt",
                    TeacherComments = "Bé vẫn khỏe mạnh, tăng cân đều",
                    TeacherEvaluation = "Tiếp tục theo dõi"
                },
                new HealthRecord
                {
                    StudentId = students[1].Id,
                    Month = DateTime.Now.Month,
                    Year = DateTime.Now.Year,
                    Height = 103.0m,
                    Weight = 16.8m,
                    GeneralHealth = "Khá",
                    MedicalConditions = "Dị ứng phấn hoa nhẹ",
                    TeacherComments = "Bé hơi nhút nhát nhưng ngoan ngoãn",
                    TeacherEvaluation = "Cần quan tâm thêm về tâm lý"
                },
                new HealthRecord
                {
                    StudentId = students[2].Id,
                    Month = DateTime.Now.Month,
                    Year = DateTime.Now.Year,
                    Height = 108.5m,
                    Weight = 19.5m,
                    GeneralHealth = "Tốt",
                    TeacherComments = "Bé rất năng động, thích vận động",
                    TeacherEvaluation = "Phát triển vượt chuẩn"
                }
            };
            _context.HealthRecords.AddRange(healthRecords);
            await _context.SaveChangesAsync();
        }

        Console.WriteLine("Database seeded successfully with all data including Phase 4!");
    }
}
