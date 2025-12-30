using Microsoft.EntityFrameworkCore;
using KindergartenManagement.DTO;

namespace KindergartenManagement.DAO;

public interface ITuitionFeeDao
{
    Task<TuitionFee?> GetByGradeIdAsync(Guid gradeId);
    Task<IEnumerable<TuitionFee>> GetAllAsync();
    Task<TuitionFee> CreateAsync(TuitionFee tuitionFee);
    Task<TuitionFee> UpdateAsync(TuitionFee tuitionFee);
    Task DeleteAsync(Guid id);
}

public class TuitionFeeDao : ITuitionFeeDao
{
    private readonly KindergartenDbContext _context;

    public TuitionFeeDao(KindergartenDbContext context)
    {
        _context = context;
    }

    public async Task<TuitionFee?> GetByGradeIdAsync(Guid gradeId)
    {
        return await _context.TuitionFees
            .Include(t => t.Grade)
            .FirstOrDefaultAsync(t => t.GradeId == gradeId);
    }

    public async Task<IEnumerable<TuitionFee>> GetAllAsync()
    {
        return await _context.TuitionFees
            .Include(t => t.Grade)
            .OrderBy(t => t.Grade!.Name)
            .ToListAsync();
    }

    public async Task<TuitionFee> CreateAsync(TuitionFee tuitionFee)
    {
        tuitionFee.CreatedAt = DateTime.Now;
        tuitionFee.UpdatedAt = DateTime.Now;
        _context.TuitionFees.Add(tuitionFee);
        await _context.SaveChangesAsync();
        return tuitionFee;
    }

    public async Task<TuitionFee> UpdateAsync(TuitionFee tuitionFee)
    {
        tuitionFee.UpdatedAt = DateTime.Now;
        _context.TuitionFees.Update(tuitionFee);
        await _context.SaveChangesAsync();
        return tuitionFee;
    }

    public async Task DeleteAsync(Guid id)
    {
        var tuitionFee = await _context.TuitionFees.FindAsync(id);
        if (tuitionFee != null)
        {
            _context.TuitionFees.Remove(tuitionFee);
            await _context.SaveChangesAsync();
        }
    }
}
