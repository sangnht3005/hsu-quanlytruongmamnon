using KindergartenManagement.DTO;
using KindergartenManagement.DAO;
using Microsoft.EntityFrameworkCore;

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

        // ==================== Phase 5: Food Management ====================
        
        // Seed Suppliers
        var suppliers = new List<Supplier>
        {
            new Supplier 
            { 
                Name = "Công ty TNHH Thực phẩm Sạch Việt",
                ContactPerson = "Nguyễn Văn Sạch",
                Phone = "0901234567",
                Email = "info@thucphamsach.vn",
                Address = "123 Đường Nguyễn Trãi, Q.1, TP.HCM",
                IsActive = true
            },
            new Supplier 
            { 
                Name = "Công ty CP Rau Quả Đà Lạt",
                ContactPerson = "Trần Thị Hoa",
                Phone = "0902345678",
                Email = "contact@rauquadalat.vn",
                Address = "45 Lâm Đồng, Đà Lạt",
                IsActive = true
            },
            new Supplier 
            { 
                Name = "Trang trại Hữu cơ Xanh",
                ContactPerson = "Lê Văn Xanh",
                Phone = "0903456789",
                Email = "xanh@huuco.vn",
                Address = "789 Đường Lê Lợi, Q.3, TP.HCM",
                IsActive = true
            }
        };
        _context.Suppliers.AddRange(suppliers);
        await _context.SaveChangesAsync();

        // Seed Ingredients
        var ingredients = new List<Ingredient>
        {
            // Protein
            new Ingredient { Name = "Thịt gà", Category = "Protein", Unit = "kg", CaloriesPer100g = 165m, UnitPrice = 85000m, SupplierId = suppliers[0].Id, IsActive = true },
            new Ingredient { Name = "Thịt heo", Category = "Protein", Unit = "kg", CaloriesPer100g = 242m, UnitPrice = 120000m, SupplierId = suppliers[0].Id, IsActive = true },
            new Ingredient { Name = "Cá hồi", Category = "Protein", Unit = "kg", CaloriesPer100g = 208m, UnitPrice = 250000m, SupplierId = suppliers[0].Id, IsActive = true },
            new Ingredient { Name = "Trứng gà", Category = "Protein", Unit = "kg", CaloriesPer100g = 155m, UnitPrice = 35000m, SupplierId = suppliers[0].Id, IsActive = true },
            new Ingredient { Name = "Đậu hũ", Category = "Protein", Unit = "kg", CaloriesPer100g = 76m, UnitPrice = 25000m, SupplierId = suppliers[0].Id, IsActive = true },
            
            // Vegetables
            new Ingredient { Name = "Cà rót", Category = "Vegetable", Unit = "kg", CaloriesPer100g = 25m, UnitPrice = 18000m, SupplierId = suppliers[1].Id, IsActive = true },
            new Ingredient { Name = "Bông cải xanh", Category = "Vegetable", Unit = "kg", CaloriesPer100g = 34m, UnitPrice = 30000m, SupplierId = suppliers[1].Id, IsActive = true },
            new Ingredient { Name = "Cà chua", Category = "Vegetable", Unit = "kg", CaloriesPer100g = 18m, UnitPrice = 20000m, SupplierId = suppliers[1].Id, IsActive = true },
            new Ingredient { Name = "Súp lơ", Category = "Vegetable", Unit = "kg", CaloriesPer100g = 25m, UnitPrice = 35000m, SupplierId = suppliers[2].Id, IsActive = true },
            new Ingredient { Name = "Rau muống", Category = "Vegetable", Unit = "kg", CaloriesPer100g = 19m, UnitPrice = 12000m, SupplierId = suppliers[2].Id, IsActive = true },
            
            // Fruits
            new Ingredient { Name = "Chuối", Category = "Fruit", Unit = "kg", CaloriesPer100g = 89m, UnitPrice = 22000m, SupplierId = suppliers[1].Id, IsActive = true },
            new Ingredient { Name = "Táo", Category = "Fruit", Unit = "kg", CaloriesPer100g = 52m, UnitPrice = 45000m, SupplierId = suppliers[1].Id, IsActive = true },
            new Ingredient { Name = "Cam", Category = "Fruit", Unit = "kg", CaloriesPer100g = 47m, UnitPrice = 35000m, SupplierId = suppliers[1].Id, IsActive = true },
            
            // Grains
            new Ingredient { Name = "Gạo trắng", Category = "Grain", Unit = "kg", CaloriesPer100g = 130m, UnitPrice = 25000m, SupplierId = suppliers[0].Id, IsActive = true },
            new Ingredient { Name = "Mì ý", Category = "Grain", Unit = "kg", CaloriesPer100g = 131m, UnitPrice = 35000m, SupplierId = suppliers[0].Id, IsActive = true },
            
            // Dairy
            new Ingredient { Name = "Sữa tươi", Category = "Dairy", Unit = "liter", CaloriesPer100g = 61m, UnitPrice = 35000m, SupplierId = suppliers[0].Id, IsActive = true },
            new Ingredient { Name = "Sữa chua", Category = "Dairy", Unit = "kg", CaloriesPer100g = 59m, UnitPrice = 45000m, SupplierId = suppliers[0].Id, IsActive = true }
        };
        _context.Ingredients.AddRange(ingredients);
        await _context.SaveChangesAsync();

        // Seed Dishes with ingredients
        var dishes = new List<Dish>();
        
        // Dish 1: Cơm gà xào rau
        var dish1 = new Dish 
        { 
            Name = "Cơm gà xào rau",
            Category = "Main Course",
            Description = "Cơm trắng với thịt gà xào cùng rau củ",
            SellingPrice = 35000m,
            IsActive = true
        };
        _context.Dishes.Add(dish1);
        await _context.SaveChangesAsync();
        
        var dish1Ingredients = new List<DishIngredient>
        {
            new DishIngredient { DishId = dish1.Id, IngredientId = ingredients[13].Id, Quantity = 100 }, // Gạo
            new DishIngredient { DishId = dish1.Id, IngredientId = ingredients[0].Id, Quantity = 80 },  // Thịt gà
            new DishIngredient { DishId = dish1.Id, IngredientId = ingredients[6].Id, Quantity = 50 },  // Bông cải
            new DishIngredient { DishId = dish1.Id, IngredientId = ingredients[7].Id, Quantity = 30 }   // Cà chua
        };
        _context.DishIngredients.AddRange(dish1Ingredients);
        await _context.SaveChangesAsync();

        // Dish 2: Canh cá hồi nấu rau
        var dish2 = new Dish 
        { 
            Name = "Canh cá hồi nấu rau",
            Category = "Soup",
            Description = "Canh cá hồi thanh mát với rau củ",
            SellingPrice = 28000m,
            IsActive = true
        };
        _context.Dishes.Add(dish2);
        await _context.SaveChangesAsync();
        
        var dish2Ingredients = new List<DishIngredient>
        {
            new DishIngredient { DishId = dish2.Id, IngredientId = ingredients[2].Id, Quantity = 60 },  // Cá hồi
            new DishIngredient { DishId = dish2.Id, IngredientId = ingredients[7].Id, Quantity = 40 },  // Cà chua
            new DishIngredient { DishId = dish2.Id, IngredientId = ingredients[9].Id, Quantity = 30 }   // Rau muống
        };
        _context.DishIngredients.AddRange(dish2Ingredients);
        await _context.SaveChangesAsync();

        // Dish 3: Trứng chiên đậu hũ
        var dish3 = new Dish 
        { 
            Name = "Trứng chiên đậu hũ",
            Category = "Main Course",
            Description = "Món ăn giàu protein từ trứng và đậu hũ",
            SellingPrice = 20000m,
            IsActive = true
        };
        _context.Dishes.Add(dish3);
        await _context.SaveChangesAsync();
        
        var dish3Ingredients = new List<DishIngredient>
        {
            new DishIngredient { DishId = dish3.Id, IngredientId = ingredients[3].Id, Quantity = 100 }, // Trứng
            new DishIngredient { DishId = dish3.Id, IngredientId = ingredients[4].Id, Quantity = 80 }   // Đậu hũ
        };
        _context.DishIngredients.AddRange(dish3Ingredients);
        await _context.SaveChangesAsync();

        // Dish 4: Salad trái cây
        var dish4 = new Dish 
        { 
            Name = "Salad trái cây",
            Category = "Dessert",
            Description = "Hỗn hợp trái cây tươi ngon",
            SellingPrice = 18000m,
            IsActive = true
        };
        _context.Dishes.Add(dish4);
        await _context.SaveChangesAsync();
        
        var dish4Ingredients = new List<DishIngredient>
        {
            new DishIngredient { DishId = dish4.Id, IngredientId = ingredients[10].Id, Quantity = 50 }, // Chuối
            new DishIngredient { DishId = dish4.Id, IngredientId = ingredients[11].Id, Quantity = 60 }, // Táo
            new DishIngredient { DishId = dish4.Id, IngredientId = ingredients[12].Id, Quantity = 50 }, // Cam
            new DishIngredient { DishId = dish4.Id, IngredientId = ingredients[16].Id, Quantity = 30 }  // Sữa chua
        };
        _context.DishIngredients.AddRange(dish4Ingredients);
        await _context.SaveChangesAsync();

        dishes.AddRange(new[] { dish1, dish2, dish3, dish4 });

        // Recalculate all dishes cost and calories
        foreach (var dish in dishes)
        {
            var dishIngredients = await _context.DishIngredients
                .Where(di => di.DishId == dish.Id)
                .ToListAsync();
            
            decimal totalCalories = 0;
            decimal totalCost = 0;

            foreach (var di in dishIngredients)
            {
                var ingredient = ingredients.First(i => i.Id == di.IngredientId);
                totalCalories += (ingredient.CaloriesPer100g * di.Quantity) / 100;
                totalCost += (ingredient.UnitPrice * di.Quantity) / 1000;
            }

            dish.TotalCalories = Math.Round(totalCalories, 2);
            dish.TotalCost = Math.Round(totalCost, 2);
        }
        await _context.SaveChangesAsync();

        // Seed Menus - theo mẫu ngày trong tuần, tái sử dụng được
        var menus = new List<Menu>
        {
            // Thứ 2
            new Menu 
            { 
                Name = "Thực đơn sáng thứ 2",
                DayOfWeek = 1, // Monday
                MealType = "Breakfast",
                Description = "Bữa sáng thứ 2 - Trứng chiên đậu hũ"
            },
            new Menu 
            { 
                Name = "Thực đơn trưa thứ 2",
                DayOfWeek = 1, // Monday
                MealType = "Lunch",
                Description = "Bữa trưa thứ 2 - Cơm gà xào rau + Canh cá"
            },
            new Menu 
            { 
                Name = "Thực đơn xế thứ 2",
                DayOfWeek = 1, // Monday
                MealType = "Snack",
                Description = "Bữa phụ thứ 2 - Salad trái cây"
            },
            // Thứ 3
            new Menu 
            { 
                Name = "Thực đơn sáng thứ 3",
                DayOfWeek = 2, // Tuesday
                MealType = "Breakfast",
                Description = "Bữa sáng thứ 3 - Cháo gà"
            },
            new Menu 
            { 
                Name = "Thực đơn trưa thứ 3",
                DayOfWeek = 2, // Tuesday
                MealType = "Lunch",
                Description = "Bữa trưa thứ 3 - Cơm + Canh"
            },
        };
        _context.Menus.AddRange(menus);
        await _context.SaveChangesAsync();

        // Link dishes to menus
        var menuDishes = new List<MenuDish>
        {
            // Monday Breakfast
            new MenuDish { MenuId = menus[0].Id, DishId = dish3.Id }, // Trứng chiên đậu hũ
            
            // Monday Lunch
            new MenuDish { MenuId = menus[1].Id, DishId = dish1.Id }, // Cơm gà xào rau
            new MenuDish { MenuId = menus[1].Id, DishId = dish2.Id }, // Canh cá
            
            // Monday Snack
            new MenuDish { MenuId = menus[2].Id, DishId = dish4.Id }, // Salad trái cây
            
            // Tuesday Breakfast
            new MenuDish { MenuId = menus[3].Id, DishId = dish1.Id }, // Cơm gà
            
            // Tuesday Lunch
            new MenuDish { MenuId = menus[4].Id, DishId = dish2.Id }, // Canh cá
        };
        _context.MenuDishes.AddRange(menuDishes);
        await _context.SaveChangesAsync();

        Console.WriteLine("Database seeded successfully with all data including Phase 5!");
    }
}
