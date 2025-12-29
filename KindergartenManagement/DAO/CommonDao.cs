using Microsoft.EntityFrameworkCore;
using KindergartenManagement.DTO;

namespace KindergartenManagement.DAO;

public interface IRoleDao
{
    Task<Role?> GetByIdAsync(Guid id);
    Task<IEnumerable<Role>> GetAllAsync();
    Task<Role> CreateAsync(Role role);
    Task<Role> UpdateAsync(Role role);
    Task DeleteAsync(Guid id);
}

public class RoleDao : IRoleDao
{
    private readonly KindergartenDbContext _context;

    public RoleDao(KindergartenDbContext context)
    {
        _context = context;
    }

    public async Task<Role?> GetByIdAsync(Guid id)
    {
        return await _context.Roles
            .Include(r => r.RolePermissions)
            .ThenInclude(rp => rp.Permission)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<IEnumerable<Role>> GetAllAsync()
    {
        return await _context.Roles
            .Include(r => r.RolePermissions)
            .ThenInclude(rp => rp.Permission)
            .ToListAsync();
    }

    public async Task<Role> CreateAsync(Role role)
    {
        _context.Roles.Add(role);
        await _context.SaveChangesAsync();
        return role;
    }

    public async Task<Role> UpdateAsync(Role role)
    {
        role.UpdatedAt = DateTime.Now;
        _context.Roles.Update(role);
        await _context.SaveChangesAsync();
        return role;
    }

    public async Task DeleteAsync(Guid id)
    {
        var role = await _context.Roles.FindAsync(id);
        if (role != null)
        {
            _context.Roles.Remove(role);
            await _context.SaveChangesAsync();
        }
    }
}

public interface IPermissionDao
{
    Task<Permission?> GetByIdAsync(Guid id);
    Task<IEnumerable<Permission>> GetAllAsync();
}

public class PermissionDao : IPermissionDao
{
    private readonly KindergartenDbContext _context;

    public PermissionDao(KindergartenDbContext context)
    {
        _context = context;
    }

    public async Task<Permission?> GetByIdAsync(Guid id)
    {
        return await _context.Permissions.FindAsync(id);
    }

    public async Task<IEnumerable<Permission>> GetAllAsync()
    {
        return await _context.Permissions.ToListAsync();
    }
}

public interface IUserDao
{
    Task<User?> GetByIdAsync(Guid id);
    Task<IEnumerable<User>> GetAllAsync();
    Task<User> CreateAsync(User user);
    Task<User> UpdateAsync(User user);
    Task DeleteAsync(Guid id);
}

public class UserDao : IUserDao
{
    private readonly KindergartenDbContext _context;

    public UserDao(KindergartenDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await _context.Users
            .Include(u => u.Account)
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        return await _context.Users
            .Include(u => u.Account)
            .ToListAsync();
    }

    public async Task<User> CreateAsync(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<User> UpdateAsync(User user)
    {
        user.UpdatedAt = DateTime.Now;
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task DeleteAsync(Guid id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user != null)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }
    }
}

public interface IParentDao
{
    Task<Parent?> GetByIdAsync(Guid id);
    Task<IEnumerable<Parent>> GetAllAsync();
    Task<Parent> CreateAsync(Parent parent);
    Task<Parent> UpdateAsync(Parent parent);
    Task DeleteAsync(Guid id);
}

public class ParentDao : IParentDao
{
    private readonly KindergartenDbContext _context;

    public ParentDao(KindergartenDbContext context)
    {
        _context = context;
    }

    public async Task<Parent?> GetByIdAsync(Guid id)
    {
        return await _context.Parents
            .Include(p => p.Students)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<IEnumerable<Parent>> GetAllAsync()
    {
        return await _context.Parents
            .Include(p => p.Students)
            .ToListAsync();
    }

    public async Task<Parent> CreateAsync(Parent parent)
    {
        _context.Parents.Add(parent);
        await _context.SaveChangesAsync();
        return parent;
    }

    public async Task<Parent> UpdateAsync(Parent parent)
    {
        parent.UpdatedAt = DateTime.Now;
        _context.Parents.Update(parent);
        await _context.SaveChangesAsync();
        return parent;
    }

    public async Task DeleteAsync(Guid id)
    {
        var parent = await _context.Parents.FindAsync(id);
        if (parent != null)
        {
            _context.Parents.Remove(parent);
            await _context.SaveChangesAsync();
        }
    }
}
