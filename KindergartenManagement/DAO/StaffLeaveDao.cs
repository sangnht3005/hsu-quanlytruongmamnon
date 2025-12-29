using Microsoft.EntityFrameworkCore;
using KindergartenManagement.DTO;

namespace KindergartenManagement.DAO;

public interface IStaffLeaveDao
{
    Task<StaffLeave?> GetByIdAsync(Guid id);
    Task<IEnumerable<StaffLeave>> GetAllAsync();
    Task<IEnumerable<StaffLeave>> GetByUserIdAsync(Guid userId);
    Task<IEnumerable<StaffLeave>> GetByStatusAsync(string status);
    Task<StaffLeave> CreateAsync(StaffLeave staffLeave);
    Task<StaffLeave> UpdateAsync(StaffLeave staffLeave);
    Task DeleteAsync(Guid id);
}

public class StaffLeaveDao : IStaffLeaveDao
{
    private readonly KindergartenDbContext _context;

    public StaffLeaveDao(KindergartenDbContext context)
    {
        _context = context;
    }

    public async Task<StaffLeave?> GetByIdAsync(Guid id)
    {
        return await _context.StaffLeaves
            .Include(sl => sl.User)
            .Include(sl => sl.Approver)
            .FirstOrDefaultAsync(sl => sl.Id == id);
    }

    public async Task<IEnumerable<StaffLeave>> GetAllAsync()
    {
        return await _context.StaffLeaves
            .Include(sl => sl.User)
            .Include(sl => sl.Approver)
            .OrderByDescending(sl => sl.StartDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<StaffLeave>> GetByUserIdAsync(Guid userId)
    {
        return await _context.StaffLeaves
            .Include(sl => sl.User)
            .Include(sl => sl.Approver)
            .Where(sl => sl.UserId == userId)
            .OrderByDescending(sl => sl.StartDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<StaffLeave>> GetByStatusAsync(string status)
    {
        return await _context.StaffLeaves
            .Include(sl => sl.User)
            .Include(sl => sl.Approver)
            .Where(sl => sl.Status == status)
            .OrderByDescending(sl => sl.StartDate)
            .ToListAsync();
    }

    public async Task<StaffLeave> CreateAsync(StaffLeave staffLeave)
    {
        _context.StaffLeaves.Add(staffLeave);
        await _context.SaveChangesAsync();
        return staffLeave;
    }

    public async Task<StaffLeave> UpdateAsync(StaffLeave staffLeave)
    {
        staffLeave.UpdatedAt = DateTime.Now;
        _context.StaffLeaves.Update(staffLeave);
        await _context.SaveChangesAsync();
        return staffLeave;
    }

    public async Task DeleteAsync(Guid id)
    {
        var staffLeave = await _context.StaffLeaves.FindAsync(id);
        if (staffLeave != null)
        {
            _context.StaffLeaves.Remove(staffLeave);
            await _context.SaveChangesAsync();
        }
    }
}
