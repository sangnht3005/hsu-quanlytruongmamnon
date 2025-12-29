using Microsoft.EntityFrameworkCore;
using KindergartenManagement.DTO;

namespace KindergartenManagement.DAO;

public interface IClassDao
{
    Task<Class?> GetByIdAsync(Guid id);
    Task<IEnumerable<Class>> GetAllAsync();
    Task<IEnumerable<Class>> GetByGradeIdAsync(Guid gradeId);
    Task<Class?> GetByTeacherIdAsync(Guid teacherId);
    Task<Class> CreateAsync(Class classEntity);
    Task<Class> UpdateAsync(Class classEntity);
    Task DeleteAsync(Guid id);
}

public class ClassDao : IClassDao
{
    private readonly KindergartenDbContext _context;

    public ClassDao(KindergartenDbContext context)
    {
        _context = context;
    }

    public async Task<Class?> GetByIdAsync(Guid id)
    {
        return await _context.Classes
            .Include(c => c.Grade)
            .Include(c => c.Teacher)
            .Include(c => c.Students)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<IEnumerable<Class>> GetAllAsync()
    {
        return await _context.Classes
            .Include(c => c.Grade)
            .Include(c => c.Teacher)
            .Include(c => c.Students)
            .ToListAsync();
    }

    public async Task<IEnumerable<Class>> GetByGradeIdAsync(Guid gradeId)
    {
        return await _context.Classes
            .Include(c => c.Grade)
            .Include(c => c.Teacher)
            .Where(c => c.GradeId == gradeId)
            .ToListAsync();
    }

    public async Task<Class?> GetByTeacherIdAsync(Guid teacherId)
    {
        return await _context.Classes
            .Include(c => c.Grade)
            .Include(c => c.Teacher)
            .FirstOrDefaultAsync(c => c.TeacherId == teacherId);
    }

    public async Task<Class> CreateAsync(Class classEntity)
    {
        _context.Classes.Add(classEntity);
        await _context.SaveChangesAsync();
        return classEntity;
    }

    public async Task<Class> UpdateAsync(Class classEntity)
    {
        classEntity.UpdatedAt = DateTime.Now;
        _context.Classes.Update(classEntity);
        await _context.SaveChangesAsync();
        return classEntity;
    }

    public async Task DeleteAsync(Guid id)
    {
        var classEntity = await _context.Classes.FindAsync(id);
        if (classEntity != null)
        {
            _context.Classes.Remove(classEntity);
            await _context.SaveChangesAsync();
        }
    }
}

public interface IGradeDao
{
    Task<Grade?> GetByIdAsync(Guid id);
    Task<IEnumerable<Grade>> GetAllAsync();
    Task<Grade> CreateAsync(Grade grade);
    Task<Grade> UpdateAsync(Grade grade);
    Task DeleteAsync(Guid id);
}

public class GradeDao : IGradeDao
{
    private readonly KindergartenDbContext _context;

    public GradeDao(KindergartenDbContext context)
    {
        _context = context;
    }

    public async Task<Grade?> GetByIdAsync(Guid id)
    {
        return await _context.Grades
            .Include(g => g.Classes)
            .FirstOrDefaultAsync(g => g.Id == id);
    }

    public async Task<IEnumerable<Grade>> GetAllAsync()
    {
        return await _context.Grades
            .Include(g => g.Classes)
            .ToListAsync();
    }

    public async Task<Grade> CreateAsync(Grade grade)
    {
        _context.Grades.Add(grade);
        await _context.SaveChangesAsync();
        return grade;
    }

    public async Task<Grade> UpdateAsync(Grade grade)
    {
        grade.UpdatedAt = DateTime.Now;
        _context.Grades.Update(grade);
        await _context.SaveChangesAsync();
        return grade;
    }

    public async Task DeleteAsync(Guid id)
    {
        var grade = await _context.Grades.FindAsync(id);
        if (grade != null)
        {
            _context.Grades.Remove(grade);
            await _context.SaveChangesAsync();
        }
    }
}

public interface IAttendanceDao
{
    Task<Attendance?> GetByIdAsync(Guid id);
    Task<string?> GetStatusByIdAsync(Guid id);
    Task<IEnumerable<Attendance>> GetByStudentIdAsync(Guid studentId);
    Task<IEnumerable<Attendance>> GetByClassIdAsync(Guid classId, DateTime date);
    Task<Attendance?> GetByStudentAndDateAsync(Guid studentId, DateTime date);
    Task<Attendance> CreateAsync(Attendance attendance);
    Task<Attendance> UpdateAsync(Attendance attendance);
    Task DeleteAsync(Guid id);
}

public class AttendanceDao : IAttendanceDao
{
    private readonly KindergartenDbContext _context;

    public AttendanceDao(KindergartenDbContext context)
    {
        _context = context;
    }

    public async Task<Attendance?> GetByIdAsync(Guid id)
    {
        return await _context.Attendances
            .Include(a => a.Student)
            .Include(a => a.Class)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<string?> GetStatusByIdAsync(Guid id)
    {
        return await _context.Attendances
            .AsNoTracking()
            .Where(a => a.Id == id)
            .Select(a => a.Status)
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Attendance>> GetByStudentIdAsync(Guid studentId)
    {
        return await _context.Attendances
            .Include(a => a.Student)
            .Include(a => a.Class)
            .Where(a => a.StudentId == studentId)
            .OrderByDescending(a => a.Date)
            .ToListAsync();
    }

    public async Task<IEnumerable<Attendance>> GetByClassIdAsync(Guid classId, DateTime date)
    {
        return await _context.Attendances
            .AsNoTracking()
            .Include(a => a.Student)
            .Include(a => a.Class)
            .Where(a => a.ClassId == classId && a.Date.Date == date.Date)
            .ToListAsync();
    }

    public async Task<Attendance?> GetByStudentAndDateAsync(Guid studentId, DateTime date)
    {
        return await _context.Attendances
            .FirstOrDefaultAsync(a => a.StudentId == studentId && a.Date.Date == date.Date);
    }

    public async Task<Attendance> CreateAsync(Attendance attendance)
    {
        _context.Attendances.Add(attendance);
        await _context.SaveChangesAsync();
        return attendance;
    }

    public async Task<Attendance> UpdateAsync(Attendance attendance)
    {
        attendance.UpdatedAt = DateTime.Now;

        // Không cập nhật theo graph; tránh dính nav Student/Class
        attendance.Student = null;
        attendance.Class = null;

        // Nếu đã có entity được track, cập nhật trực tiếp; nếu không thì attach mới
        var tracked = _context.ChangeTracker.Entries<Attendance>()
            .FirstOrDefault(e => e.Entity.Id == attendance.Id);

        if (tracked != null)
        {
            tracked.Entity.Status = attendance.Status;
            tracked.Entity.Notes = attendance.Notes;
            tracked.Entity.UpdatedAt = attendance.UpdatedAt;

            _context.Entry(tracked.Entity).Property(a => a.Status).IsModified = true;
            _context.Entry(tracked.Entity).Property(a => a.Notes).IsModified = true;
            _context.Entry(tracked.Entity).Property(a => a.UpdatedAt).IsModified = true;
        }
        else
        {
            // Đính kèm và chỉ đánh dấu các trường cần cập nhật
            _context.Attendances.Attach(attendance);
            _context.Entry(attendance).Property(a => a.Status).IsModified = true;
            _context.Entry(attendance).Property(a => a.Notes).IsModified = true;
            _context.Entry(attendance).Property(a => a.UpdatedAt).IsModified = true;
        }

        await _context.SaveChangesAsync();
        return attendance;
    }

    public async Task DeleteAsync(Guid id)
    {
        var attendance = await _context.Attendances.FindAsync(id);
        if (attendance != null)
        {
            _context.Attendances.Remove(attendance);
            await _context.SaveChangesAsync();
        }
    }
}
