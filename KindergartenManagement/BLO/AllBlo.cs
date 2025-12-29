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

    public AttendanceBlo(IAttendanceDao attendanceDao, IStudentDao studentDao, IClassDao classDao)
    {
        _attendanceDao = attendanceDao;
        _studentDao = studentDao;
        _classDao = classDao;
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

        return await _attendanceDao.CreateAsync(attendance);
    }

    public async Task<Attendance> UpdateAsync(Attendance attendance)
    {
        if (attendance == null)
        {
            throw new ArgumentNullException(nameof(attendance));
        }

        return await _attendanceDao.UpdateAsync(attendance);
    }

    public async Task DeleteAsync(Guid id)
    {
        await _attendanceDao.DeleteAsync(id);
    }

    public async Task<IEnumerable<Attendance>> CreateClassAttendanceAsync(Guid classId, DateTime date)
    {
        // Check if attendance already exists for this class and date
        var existing = await _attendanceDao.GetByClassIdAsync(classId, date);
        if (existing.Any())
        {
            throw new InvalidOperationException("Attendance already exists for this date");
        }

        var students = await _studentDao.GetByClassIdAsync(classId);
        var attendances = new List<Attendance>();

        foreach (var student in students)
        {
            var attendance = new Attendance
            {
                StudentId = student.Id,
                ClassId = classId,
                Date = date,
                Status = "Present"
            };

            var created = await _attendanceDao.CreateAsync(attendance);
            attendances.Add(created);
        }

        return attendances;
    }
}

public interface IHealthRecordBlo
{
    Task<HealthRecord?> GetByIdAsync(Guid id);
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

    public async Task<IEnumerable<HealthRecord>> GetByStudentIdAsync(Guid studentId)
    {
        return await _healthRecordDao.GetByStudentIdAsync(studentId);
    }

    public async Task<HealthRecord> CreateAsync(HealthRecord healthRecord)
    {
        var student = await _studentDao.GetByIdAsync(healthRecord.StudentId);
        if (student == null)
        {
            throw new ArgumentException("Student not found");
        }

        return await _healthRecordDao.CreateAsync(healthRecord);
    }

    public async Task<HealthRecord> UpdateAsync(HealthRecord healthRecord)
    {
        if (healthRecord == null)
        {
            throw new ArgumentNullException(nameof(healthRecord));
        }

        return await _healthRecordDao.UpdateAsync(healthRecord);
    }

    public async Task DeleteAsync(Guid id)
    {
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
    Task<IEnumerable<Menu>> GetByDateAsync(DateTime date);
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

    public async Task<IEnumerable<Menu>> GetByDateAsync(DateTime date)
    {
        return await _menuDao.GetByDateAsync(date);
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
        var menus = await _menuDao.GetByDateAsync(date);
        if (!menus.Any())
        {
            throw new InvalidOperationException("No menu available for this date");
        }

        var mealTickets = new List<MealTicket>();

        // Get all attendances for the date where students are present
        // This is simplified - in reality you'd query all classes
        
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
