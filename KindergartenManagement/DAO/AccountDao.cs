using Microsoft.EntityFrameworkCore;
using KindergartenManagement.DTO;

namespace KindergartenManagement.DAO;

public interface IAccountDao
{
    Task<Account?> GetByIdAsync(Guid id);
    Task<Account?> GetByUsernameAsync(string username);
    Task<IEnumerable<Account>> GetAllAsync();
    Task<Account> CreateAsync(Account account);
    Task<Account> UpdateAsync(Account account);
    Task DeleteAsync(Guid id);
}

public class AccountDao : IAccountDao
{
    private readonly KindergartenDbContext _context;

    public AccountDao(KindergartenDbContext context)
    {
        _context = context;
    }

    public async Task<Account?> GetByIdAsync(Guid id)
    {
        return await _context.Accounts
            .Include(a => a.Role)
            .Include(a => a.User)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<Account?> GetByUsernameAsync(string username)
    {
        return await _context.Accounts
            .Include(a => a.Role)
            .ThenInclude(r => r!.RolePermissions)
            .ThenInclude(rp => rp.Permission)
            .Include(a => a.User)
            .FirstOrDefaultAsync(a => a.Username == username);
    }

    public async Task<IEnumerable<Account>> GetAllAsync()
    {
        return await _context.Accounts
            .Include(a => a.Role)
            .Include(a => a.User)
            .ToListAsync();
    }

    public async Task<Account> CreateAsync(Account account)
    {
        _context.Accounts.Add(account);
        await _context.SaveChangesAsync();
        return account;
    }

    public async Task<Account> UpdateAsync(Account account)
    {
        account.UpdatedAt = DateTime.Now;
        _context.Accounts.Update(account);
        await _context.SaveChangesAsync();
        return account;
    }

    public async Task DeleteAsync(Guid id)
    {
        var account = await _context.Accounts.FindAsync(id);
        if (account != null)
        {
            _context.Accounts.Remove(account);
            await _context.SaveChangesAsync();
        }
    }
}
