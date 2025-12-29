using Microsoft.EntityFrameworkCore;
using KindergartenManagement.DTO;

namespace KindergartenManagement.DAO;

public interface IStudentDao
{
    Task<Student?> GetByIdAsync(Guid id);
    Task<IEnumerable<Student>> GetAllAsync();
    Task<IEnumerable<Student>> GetByClassIdAsync(Guid classId);
    Task<IEnumerable<Student>> GetByParentIdAsync(Guid parentId);
    Task<Student> CreateAsync(Student student);
    Task<Student> UpdateAsync(Student student);
    Task DeleteAsync(Guid id);
}

public class StudentDao : IStudentDao
{
    private readonly KindergartenDbContext _context;

    public StudentDao(KindergartenDbContext context)
    {
        _context = context;
    }

    public async Task<Student?> GetByIdAsync(Guid id)
    {
        return await _context.Students
            .Include(s => s.Parent)
            .Include(s => s.Class)
            .ThenInclude(c => c!.Grade)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<IEnumerable<Student>> GetAllAsync()
    {
        return await _context.Students
            .Include(s => s.Parent)
            .Include(s => s.Class)
            .ThenInclude(c => c!.Grade)
            .ToListAsync();
    }

    public async Task<IEnumerable<Student>> GetByClassIdAsync(Guid classId)
    {
        return await _context.Students
            .Include(s => s.Parent)
            .Include(s => s.Class)
            .Where(s => s.ClassId == classId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Student>> GetByParentIdAsync(Guid parentId)
    {
        return await _context.Students
            .Include(s => s.Parent)
            .Include(s => s.Class)
            .Where(s => s.ParentId == parentId)
            .ToListAsync();
    }

    public async Task<Student> CreateAsync(Student student)
    {
        _context.Students.Add(student);
        await _context.SaveChangesAsync();
        return student;
    }

    public async Task<Student> UpdateAsync(Student student)
    {
        student.UpdatedAt = DateTime.Now;
        _context.Students.Update(student);
        await _context.SaveChangesAsync();
        return student;
    }

    public async Task DeleteAsync(Guid id)
    {
        var student = await _context.Students.FindAsync(id);
        if (student != null)
        {
            _context.Students.Remove(student);
            await _context.SaveChangesAsync();
        }
    }
}
