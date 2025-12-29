using KindergartenManagement.DTO;
using KindergartenManagement.DAO;
using BCrypt.Net;

namespace KindergartenManagement.BLO;

public interface IAccountBlo
{
    Task<Account?> LoginAsync(string username, string password);
    Task<Account?> GetByIdAsync(Guid id);
    Task<IEnumerable<Account>> GetAllAsync();
    Task<Account> CreateAsync(string username, string password, Guid roleId);
    Task<Account> UpdateAsync(Account account);
    Task<bool> ChangePasswordAsync(Guid accountId, string oldPassword, string newPassword);
    Task DeactivateAsync(Guid id);
    Task DeleteAsync(Guid id);
    Task<bool> HasPermissionAsync(Guid accountId, string permissionName);
}

public class AccountBlo : IAccountBlo
{
    private readonly IAccountDao _accountDao;
    private readonly IRoleDao _roleDao;

    public AccountBlo(IAccountDao accountDao, IRoleDao roleDao)
    {
        _accountDao = accountDao;
        _roleDao = roleDao;
    }

    public async Task<Account?> LoginAsync(string username, string password)
    {
        // Business validation
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            throw new ArgumentException("Username and password are required");
        }

        var account = await _accountDao.GetByUsernameAsync(username);
        
        if (account == null || !account.IsActive)
        {
            return null;
        }

        // Verify password
        if (!BCrypt.Net.BCrypt.Verify(password, account.PasswordHash))
        {
            return null;
        }

        return account;
    }

    public async Task<Account?> GetByIdAsync(Guid id)
    {
        return await _accountDao.GetByIdAsync(id);
    }

    public async Task<IEnumerable<Account>> GetAllAsync()
    {
        return await _accountDao.GetAllAsync();
    }

    public async Task<Account> CreateAsync(string username, string password, Guid roleId)
    {
        // Business validation
        if (string.IsNullOrWhiteSpace(username))
        {
            throw new ArgumentException("Username is required");
        }

        if (string.IsNullOrWhiteSpace(password) || password.Length < 6)
        {
            throw new ArgumentException("Password must be at least 6 characters");
        }

        // Check if username already exists
        var existing = await _accountDao.GetByUsernameAsync(username);
        if (existing != null)
        {
            throw new InvalidOperationException("Username already exists");
        }

        // Verify role exists
        var role = await _roleDao.GetByIdAsync(roleId);
        if (role == null)
        {
            throw new ArgumentException("Invalid role");
        }

        // Hash password
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

        var account = new Account
        {
            Username = username,
            PasswordHash = passwordHash,
            RoleId = roleId,
            IsActive = true
        };

        return await _accountDao.CreateAsync(account);
    }

    public async Task<Account> UpdateAsync(Account account)
    {
        if (account == null)
        {
            throw new ArgumentNullException(nameof(account));
        }

        var existing = await _accountDao.GetByIdAsync(account.Id);
        if (existing == null)
        {
            throw new InvalidOperationException("Account not found");
        }

        return await _accountDao.UpdateAsync(account);
    }

    public async Task<bool> ChangePasswordAsync(Guid accountId, string oldPassword, string newPassword)
    {
        if (string.IsNullOrWhiteSpace(oldPassword) || string.IsNullOrWhiteSpace(newPassword))
        {
            throw new ArgumentException("Passwords are required");
        }

        if (newPassword.Length < 6)
        {
            throw new ArgumentException("New password must be at least 6 characters");
        }

        var account = await _accountDao.GetByIdAsync(accountId);
        if (account == null)
        {
            throw new InvalidOperationException("Account not found");
        }

        // Verify old password
        if (!BCrypt.Net.BCrypt.Verify(oldPassword, account.PasswordHash))
        {
            return false;
        }

        // Hash and update new password
        account.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
        await _accountDao.UpdateAsync(account);

        return true;
    }

    public async Task DeactivateAsync(Guid id)
    {
        var account = await _accountDao.GetByIdAsync(id);
        if (account == null)
        {
            throw new InvalidOperationException("Account not found");
        }

        account.IsActive = false;
        await _accountDao.UpdateAsync(account);
    }

    public async Task DeleteAsync(Guid id)
    {
        var account = await _accountDao.GetByIdAsync(id);
        if (account == null)
        {
            throw new InvalidOperationException("Account not found");
        }

        await _accountDao.DeleteAsync(id);
    }

    public async Task<bool> HasPermissionAsync(Guid accountId, string permissionName)
    {
        var account = await _accountDao.GetByIdAsync(accountId);
        if (account == null || account.Role == null)
        {
            return false;
        }

        return account.Role.RolePermissions
            .Any(rp => rp.Permission?.Name == permissionName);
    }
}
