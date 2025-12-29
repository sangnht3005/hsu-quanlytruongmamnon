using KindergartenManagement.DTO;
using KindergartenManagement.DAO;

namespace KindergartenManagement.BLO;

public interface IClassBlo
{
    Task<Class?> GetByIdAsync(Guid id);
    Task<IEnumerable<Class>> GetAllAsync();
    Task<IEnumerable<Class>> GetByGradeIdAsync(Guid gradeId);
    Task<Class> CreateAsync(Class classEntity);
    Task<Class> UpdateAsync(Class classEntity);
    Task DeleteAsync(Guid id);
}

public class ClassBlo : IClassBlo
{
    private readonly IClassDao _classDao;
    private readonly IGradeDao _gradeDao;

    public ClassBlo(IClassDao classDao, IGradeDao gradeDao)
    {
        _classDao = classDao;
        _gradeDao = gradeDao;
    }

    public async Task<Class?> GetByIdAsync(Guid id)
    {
        return await _classDao.GetByIdAsync(id);
    }

    public async Task<IEnumerable<Class>> GetAllAsync()
    {
        return await _classDao.GetAllAsync();
    }

    public async Task<IEnumerable<Class>> GetByGradeIdAsync(Guid gradeId)
    {
        return await _classDao.GetByGradeIdAsync(gradeId);
    }

    public async Task<Class> CreateAsync(Class classEntity)
    {
        if (string.IsNullOrWhiteSpace(classEntity.Name))
        {
            throw new ArgumentException("Class name is required");
        }

        if (classEntity.Capacity <= 0)
        {
            throw new ArgumentException("Capacity must be greater than 0");
        }

        var grade = await _gradeDao.GetByIdAsync(classEntity.GradeId);
        if (grade == null)
        {
            throw new ArgumentException("Grade not found");
        }

        // Kiểm tra giáo viên đã được gán làm chủ nhiệm lớp khác chưa
        if (classEntity.TeacherId.HasValue)
        {
            var existingClass = await _classDao.GetByTeacherIdAsync(classEntity.TeacherId.Value);
            if (existingClass != null)
            {
                throw new InvalidOperationException($"Giáo viên này đã làm chủ nhiệm lớp {existingClass.Name}");
            }
        }

        return await _classDao.CreateAsync(classEntity);
    }

    public async Task<Class> UpdateAsync(Class classEntity)
    {
        if (classEntity == null)
        {
            throw new ArgumentNullException(nameof(classEntity));
        }

        // Kiểm tra giáo viên đã được gán làm chủ nhiệm lớp khác chưa
        if (classEntity.TeacherId.HasValue)
        {
            var existingClass = await _classDao.GetByTeacherIdAsync(classEntity.TeacherId.Value);
            if (existingClass != null && existingClass.Id != classEntity.Id)
            {
                throw new InvalidOperationException($"Giáo viên này đã làm chủ nhiệm lớp {existingClass.Name}");
            }
        }

        return await _classDao.UpdateAsync(classEntity);
    }

    public async Task DeleteAsync(Guid id)
    {
        await _classDao.DeleteAsync(id);
    }
}

public interface IGradeBlo
{
    Task<Grade?> GetByIdAsync(Guid id);
    Task<IEnumerable<Grade>> GetAllAsync();
    Task<Grade> CreateAsync(Grade grade);
    Task<Grade> UpdateAsync(Grade grade);
    Task DeleteAsync(Guid id);
}

public class GradeBlo : IGradeBlo
{
    private readonly IGradeDao _gradeDao;

    public GradeBlo(IGradeDao gradeDao)
    {
        _gradeDao = gradeDao;
    }

    public async Task<Grade?> GetByIdAsync(Guid id)
    {
        return await _gradeDao.GetByIdAsync(id);
    }

    public async Task<IEnumerable<Grade>> GetAllAsync()
    {
        return await _gradeDao.GetAllAsync();
    }

    public async Task<Grade> CreateAsync(Grade grade)
    {
        if (string.IsNullOrWhiteSpace(grade.Name))
        {
            throw new ArgumentException("Grade name is required");
        }

        if (grade.MinAge < 0 || grade.MaxAge < 0 || grade.MinAge > grade.MaxAge)
        {
            throw new ArgumentException("Invalid age range");
        }

        return await _gradeDao.CreateAsync(grade);
    }

    public async Task<Grade> UpdateAsync(Grade grade)
    {
        if (grade == null)
        {
            throw new ArgumentNullException(nameof(grade));
        }

        return await _gradeDao.UpdateAsync(grade);
    }

    public async Task DeleteAsync(Guid id)
    {
        await _gradeDao.DeleteAsync(id);
    }
}

public interface IAttendanceBlo
{
    Task<Attendance?> GetByIdAsync(Guid id);
    Task<IEnumerable<Attendance>> GetByStudentIdAsync(Guid studentId);
    Task<IEnumerable<Attendance>> GetByClassIdAsync(Guid classId, DateTime date);
    Task<Attendance> CreateAsync(Attendance attendance);
    Task<Attendance> UpdateAsync(Attendance attendance);
    Task DeleteAsync(Guid id);
    Task<IEnumerable<Attendance>> CreateClassAttendanceAsync(Guid classId, DateTime date);
}

public class AttendanceBlo : IAttendanceBlo
{
    private readonly IAttendanceDao _attendanceDao;
    private readonly IStudentDao _studentDao;
    private readonly IClassDao _classDao;
    private readonly IFoodManagementBlo _foodBlo;

    public AttendanceBlo(IAttendanceDao attendanceDao, IStudentDao studentDao, IClassDao classDao, IFoodManagementBlo foodBlo)
    {
        _attendanceDao = attendanceDao;
        _studentDao = studentDao;
        _classDao = classDao;
        _foodBlo = foodBlo;
    }

    public async Task<Attendance?> GetByIdAsync(Guid id)
    {
        return await _attendanceDao.GetByIdAsync(id);
    }

    public async Task<IEnumerable<Attendance>> GetByStudentIdAsync(Guid studentId)
    {
        return await _attendanceDao.GetByStudentIdAsync(studentId);
    }

    public async Task<IEnumerable<Attendance>> GetByClassIdAsync(Guid classId, DateTime date)
    {
        return await _attendanceDao.GetByClassIdAsync(classId, date);
    }

    public async Task<Attendance> CreateAsync(Attendance attendance)
    {
        var student = await _studentDao.GetByIdAsync(attendance.StudentId);
        if (student == null)
        {
            throw new ArgumentException("Student not found");
        }

        var classEntity = await _classDao.GetByIdAsync(attendance.ClassId);
        if (classEntity == null)
        {
            throw new ArgumentException("Class not found");
        }

        // Enforce uniqueness: StudentId + Date
        var existing = await _attendanceDao.GetByStudentAndDateAsync(attendance.StudentId, attendance.Date);
        if (existing != null)
        {
            throw new InvalidOperationException($"Điểm danh cho học sinh này đã tồn tại trong ngày {attendance.Date:dd/MM/yyyy}");
        }

        var created = await _attendanceDao.CreateAsync(attendance);

        // Tự động tạo phiếu ăn nếu học sinh có mặt
        if (created.Status == "Present")
        {
            await CreateMealTicketsForAttendanceAsync(created);
        }

        return created;
    }

    public async Task<Attendance> UpdateAsync(Attendance attendance)
    {
        if (attendance == null)
        {
            throw new ArgumentNullException(nameof(attendance));
        }

        // Lấy trạng thái cũ bằng cách query riêng với AsNoTracking
        var oldStatus = await _attendanceDao.GetStatusByIdAsync(attendance.Id);

        // Update attendance
        var updated = await _attendanceDao.UpdateAsync(attendance);

        // Xử lý phiếu ăn khi thay đổi trạng thái
        if (oldStatus != null)
        {
            // Nếu chuyển từ không có mặt -> có mặt: Tạo phiếu ăn
            if (oldStatus != "Present" && updated.Status == "Present")
            {
                await CreateMealTicketsForAttendanceAsync(updated);
            }
            // Nếu chuyển từ có mặt -> không có mặt: Xóa phiếu ăn
            else if (oldStatus == "Present" && updated.Status != "Present")
            {
                await DeleteMealTicketsForAttendanceAsync(updated);
            }
        }

        return updated;
    }

    public async Task DeleteAsync(Guid id)
    {
        await _attendanceDao.DeleteAsync(id);
    }

    private async Task CreateMealTicketsForAttendanceAsync(Attendance attendance)
    {
        try
        {
            // Lấy ngày trong tuần từ attendance date
            int dayOfWeek = (int)attendance.Date.DayOfWeek;
            
            // Tìm tất cả menu cho ngày này
            var menus = await _foodBlo.GetMenusByDayOfWeekAsync(dayOfWeek);
            // Lọc theo cấu hình bữa được bật
            menus = menus
                .Where(m => KindergartenManagement.Utilities.AutoMealSettings.IsEnabled(m.MealType))
                .ToList();
            
            if (!menus.Any())
            {
                // Không có menu cho ngày này, bỏ qua
                return;
            }
            
            // Lấy tất cả phiếu ăn hiện có của học sinh trong ngày này
            var existingTickets = await _foodBlo.GetMealTicketsByDateAsync(attendance.Date);
            var studentExistingTickets = existingTickets
                .Where(mt => mt.StudentId == attendance.StudentId && mt.Date.Date == attendance.Date.Date)
                .ToList();
            
            // Tạo phiếu ăn cho mỗi menu (Breakfast, Lunch, Snack, etc.) đã bật trong cấu hình
            foreach (var menu in menus)
            {
                // Kiểm tra xem phiếu ăn đã tồn tại chưa
                var exists = studentExistingTickets.Any(mt => mt.MenuId == menu.Id);

                if (!exists)
                {
                    var mealTicket = new MealTicket
                    {
                        StudentId = attendance.StudentId,
                        MenuId = menu.Id,
                        Date = attendance.Date,
                        IsConsumed = false
                    };
                    await _foodBlo.SaveMealTicketAsync(mealTicket);
                }
            }
        }
        catch (Exception ex)
        {
            // Log lỗi nhưng không throw để không chặn điểm danh
            System.Diagnostics.Debug.WriteLine($"Lỗi tạo phiếu ăn: {ex.Message}");
        }
    }

    private async Task DeleteMealTicketsForAttendanceAsync(Attendance attendance)
    {
        try
        {
            // Lấy tất cả phiếu ăn của học sinh trong ngày
            var tickets = await _foodBlo.GetMealTicketsByDateAsync(attendance.Date);
            var studentTickets = tickets.Where(mt => 
                mt.StudentId == attendance.StudentId && 
                mt.Date.Date == attendance.Date.Date)
                .ToList();

            // Xóa tất cả phiếu ăn chưa sử dụng
            foreach (var ticket in studentTickets)
            {
                if (!ticket.IsConsumed)
                {
                    await _foodBlo.DeleteMealTicketAsync(ticket.Id);
                }
            }
        }
        catch (Exception ex)
        {
            // Log lỗi nhưng không throw
            System.Diagnostics.Debug.WriteLine($"Lỗi xóa phiếu ăn: {ex.Message}");
        }
    }

    public async Task<IEnumerable<Attendance>> CreateClassAttendanceAsync(Guid classId, DateTime date)
    {
        // Idempotent creation: create only missing records and return full list
        var existing = await _attendanceDao.GetByClassIdAsync(classId, date);
        var existingByStudent = existing.ToDictionary(a => a.StudentId, a => a);

        var students = await _studentDao.GetByClassIdAsync(classId);
        var createdList = new List<Attendance>();

        foreach (var student in students)
        {
            if (!existingByStudent.ContainsKey(student.Id))
            {
                var attendance = new Attendance
                {
                    StudentId = student.Id,
                    ClassId = classId,
                    Date = date,
                    Status = "Present"
                };

                var created = await _attendanceDao.CreateAsync(attendance);
                createdList.Add(created);

                // Tự động tạo phiếu ăn cho học sinh có mặt
                await CreateMealTicketsForAttendanceAsync(created);
            }
        }

        // Trả về đầy đủ danh sách điểm danh (đã có + mới tạo)
        return existing.Concat(createdList).ToList();
    }
}

public interface IHealthRecordBlo
{
    Task<HealthRecord?> GetByIdAsync(Guid id);
    Task<IEnumerable<HealthRecord>> GetAllAsync();
    Task<IEnumerable<HealthRecord>> GetByStudentIdAsync(Guid studentId);
    Task<HealthRecord> CreateAsync(HealthRecord healthRecord);
    Task<HealthRecord> UpdateAsync(HealthRecord healthRecord);
    Task DeleteAsync(Guid id);
}

public class HealthRecordBlo : IHealthRecordBlo
{
    private readonly IHealthRecordDao _healthRecordDao;
    private readonly IStudentDao _studentDao;

    public HealthRecordBlo(IHealthRecordDao healthRecordDao, IStudentDao studentDao)
    {
        _healthRecordDao = healthRecordDao;
        _studentDao = studentDao;
    }

    public async Task<HealthRecord?> GetByIdAsync(Guid id)
    {
        return await _healthRecordDao.GetByIdAsync(id);
    }

    public async Task<IEnumerable<HealthRecord>> GetAllAsync()
    {
        return await _healthRecordDao.GetAllAsync();
    }

    public async Task<IEnumerable<HealthRecord>> GetByStudentIdAsync(Guid studentId)
    {
        return await _healthRecordDao.GetByStudentIdAsync(studentId);
    }

    public async Task<HealthRecord> CreateAsync(HealthRecord healthRecord)
    {
        // Business validation
        var student = await _studentDao.GetByIdAsync(healthRecord.StudentId);
        if (student == null)
        {
            throw new ArgumentException("Học sinh không tồn tại");
        }

        if (healthRecord.Month < 1 || healthRecord.Month > 12)
        {
            throw new ArgumentException("Tháng phải từ 1 đến 12");
        }

        if (healthRecord.Year < 2000 || healthRecord.Year > DateTime.Now.Year)
        {
            throw new ArgumentException($"Năm phải từ 2000 đến {DateTime.Now.Year}");
        }

        // Check for duplicate monthly record
        var existing = await _healthRecordDao.GetByStudentAndMonthAsync(healthRecord.StudentId, healthRecord.Month, healthRecord.Year);
        if (existing != null)
        {
            throw new InvalidOperationException($"Học sinh đã có hồ sơ sức khỏe cho tháng {healthRecord.Month}/{healthRecord.Year}");
        }

        if (healthRecord.Height.HasValue && healthRecord.Height.Value <= 0)
        {
            throw new ArgumentException("Chiều cao phải lớn hơn 0");
        }

        if (healthRecord.Weight.HasValue && healthRecord.Weight.Value <= 0)
        {
            throw new ArgumentException("Cân nặng phải lớn hơn 0");
        }

        return await _healthRecordDao.CreateAsync(healthRecord);
    }

    public async Task<HealthRecord> UpdateAsync(HealthRecord healthRecord)
    {
        if (healthRecord == null)
        {
            throw new ArgumentNullException(nameof(healthRecord));
        }

        var existing = await _healthRecordDao.GetByIdAsync(healthRecord.Id);
        if (existing == null)
        {
            throw new InvalidOperationException("Hồ sơ sức khỏe không tồn tại");
        }

        if (healthRecord.Month < 1 || healthRecord.Month > 12)
        {
            throw new ArgumentException("Tháng phải từ 1 đến 12");
        }

        if (healthRecord.Year < 2000 || healthRecord.Year > DateTime.Now.Year)
        {
            throw new ArgumentException($"Năm phải từ 2000 đến {DateTime.Now.Year}");
        }

        if (healthRecord.Height.HasValue && healthRecord.Height.Value <= 0)
        {
            throw new ArgumentException("Chiều cao phải lớn hơn 0");
        }

        if (healthRecord.Weight.HasValue && healthRecord.Weight.Value <= 0)
        {
            throw new ArgumentException("Cân nặng phải lớn hơn 0");
        }

        return await _healthRecordDao.UpdateAsync(healthRecord);
    }

    public async Task DeleteAsync(Guid id)
    {
        var healthRecord = await _healthRecordDao.GetByIdAsync(id);
        if (healthRecord == null)
        {
            throw new InvalidOperationException("Hồ sơ sức khỏe không tồn tại");
        }

        await _healthRecordDao.DeleteAsync(id);
    }
}

public interface IFoodBlo
{
    Task<Food?> GetByIdAsync(Guid id);
    Task<IEnumerable<Food>> GetAllAsync();
    Task<Food> CreateAsync(Food food);
    Task<Food> UpdateAsync(Food food);
    Task DeleteAsync(Guid id);
}

public class FoodBlo : IFoodBlo
{
    private readonly IFoodDao _foodDao;

    public FoodBlo(IFoodDao foodDao)
    {
        _foodDao = foodDao;
    }

    public async Task<Food?> GetByIdAsync(Guid id)
    {
        return await _foodDao.GetByIdAsync(id);
    }

    public async Task<IEnumerable<Food>> GetAllAsync()
    {
        return await _foodDao.GetAllAsync();
    }

    public async Task<Food> CreateAsync(Food food)
    {
        if (string.IsNullOrWhiteSpace(food.Name))
        {
            throw new ArgumentException("Food name is required");
        }

        return await _foodDao.CreateAsync(food);
    }

    public async Task<Food> UpdateAsync(Food food)
    {
        if (food == null)
        {
            throw new ArgumentNullException(nameof(food));
        }

        return await _foodDao.UpdateAsync(food);
    }

    public async Task DeleteAsync(Guid id)
    {
        await _foodDao.DeleteAsync(id);
    }
}

public interface IMenuBlo
{
    Task<Menu?> GetByIdAsync(Guid id);
    Task<IEnumerable<Menu>> GetAllAsync();
    Task<IEnumerable<Menu>> GetByDayOfWeekAsync(int dayOfWeek);
    Task<Menu> CreateAsync(Menu menu);
    Task<Menu> UpdateAsync(Menu menu);
    Task DeleteAsync(Guid id);
}

public class MenuBlo : IMenuBlo
{
    private readonly IMenuDao _menuDao;

    public MenuBlo(IMenuDao menuDao)
    {
        _menuDao = menuDao;
    }

    public async Task<Menu?> GetByIdAsync(Guid id)
    {
        return await _menuDao.GetByIdAsync(id);
    }

    public async Task<IEnumerable<Menu>> GetAllAsync()
    {
        return await _menuDao.GetAllAsync();
    }

    public async Task<IEnumerable<Menu>> GetByDayOfWeekAsync(int dayOfWeek)
    {
        return await _menuDao.GetByDayOfWeekAsync(dayOfWeek);
    }

    public async Task<Menu> CreateAsync(Menu menu)
    {
        if (string.IsNullOrWhiteSpace(menu.Name))
        {
            throw new ArgumentException("Menu name is required");
        }

        return await _menuDao.CreateAsync(menu);
    }

    public async Task<Menu> UpdateAsync(Menu menu)
    {
        if (menu == null)
        {
            throw new ArgumentNullException(nameof(menu));
        }

        return await _menuDao.UpdateAsync(menu);
    }

    public async Task DeleteAsync(Guid id)
    {
        await _menuDao.DeleteAsync(id);
    }
}

public interface IMealTicketBlo
{
    Task<MealTicket?> GetByIdAsync(Guid id);
    Task<IEnumerable<MealTicket>> GetByStudentIdAsync(Guid studentId);
    Task<IEnumerable<MealTicket>> GenerateFromAttendanceAsync(DateTime date);
}

public class MealTicketBlo : IMealTicketBlo
{
    private readonly IMealTicketDao _mealTicketDao;
    private readonly IAttendanceDao _attendanceDao;
    private readonly IMenuDao _menuDao;

    public MealTicketBlo(IMealTicketDao mealTicketDao, IAttendanceDao attendanceDao, IMenuDao menuDao)
    {
        _mealTicketDao = mealTicketDao;
        _attendanceDao = attendanceDao;
        _menuDao = menuDao;
    }

    public async Task<MealTicket?> GetByIdAsync(Guid id)
    {
        return await _mealTicketDao.GetByIdAsync(id);
    }

    public async Task<IEnumerable<MealTicket>> GetByStudentIdAsync(Guid studentId)
    {
        return await _mealTicketDao.GetByStudentIdAsync(studentId);
    }

    public async Task<IEnumerable<MealTicket>> GenerateFromAttendanceAsync(DateTime date)
    {
        // Lấy menu dựa trên ngày trong tuần của date
        int dayOfWeek = (int)date.DayOfWeek;
        var menus = await _menuDao.GetByDayOfWeekAsync(dayOfWeek);
        if (!menus.Any())
        {
            throw new InvalidOperationException($"No menu available for day of week: {dayOfWeek}");
        }

        // Phiếu ăn sẽ được tạo tự động bởi GenerateMealTicketsForMenuAsync trong FoodManagementDao
        // Phương thức này chỉ trả về danh sách rỗng vì logic thay đổi
        var mealTickets = new List<MealTicket>();
        return mealTickets;
    }
}

public interface IInvoiceBlo
{
    Task<Invoice?> GetByIdAsync(Guid id);
    Task<IEnumerable<Invoice>> GetAllAsync();
    Task<IEnumerable<Invoice>> GetByStudentIdAsync(Guid studentId);
    Task<Invoice> CreateAsync(Invoice invoice);
    Task<Invoice> UpdateAsync(Invoice invoice);
    Task DeleteAsync(Guid id);
}

public class InvoiceBlo : IInvoiceBlo
{
    private readonly IInvoiceDao _invoiceDao;

    public InvoiceBlo(IInvoiceDao invoiceDao)
    {
        _invoiceDao = invoiceDao;
    }

    public async Task<Invoice?> GetByIdAsync(Guid id)
    {
        return await _invoiceDao.GetByIdAsync(id);
    }

    public async Task<IEnumerable<Invoice>> GetAllAsync()
    {
        return await _invoiceDao.GetAllAsync();
    }

    public async Task<IEnumerable<Invoice>> GetByStudentIdAsync(Guid studentId)
    {
        return await _invoiceDao.GetByStudentIdAsync(studentId);
    }

    public async Task<Invoice> CreateAsync(Invoice invoice)
    {
        if (string.IsNullOrWhiteSpace(invoice.InvoiceNumber))
        {
            throw new ArgumentException("Invoice number is required");
        }

        if (invoice.Amount <= 0)
        {
            throw new ArgumentException("Amount must be greater than 0");
        }

        return await _invoiceDao.CreateAsync(invoice);
    }

    public async Task<Invoice> UpdateAsync(Invoice invoice)
    {
        if (invoice == null)
        {
            throw new ArgumentNullException(nameof(invoice));
        }

        return await _invoiceDao.UpdateAsync(invoice);
    }

    public async Task DeleteAsync(Guid id)
    {
        await _invoiceDao.DeleteAsync(id);
    }
}
