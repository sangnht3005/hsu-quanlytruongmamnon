using KindergartenManagement.DTO;
using KindergartenManagement.DAO;

namespace KindergartenManagement.BLO;

public interface IStudentBlo
{
    Task<Student?> GetByIdAsync(Guid id);
    Task<IEnumerable<Student>> GetAllAsync();
    Task<IEnumerable<Student>> GetByClassIdAsync(Guid classId);
    Task<IEnumerable<Student>> GetByParentIdAsync(Guid parentId);
    Task<Student> CreateAsync(Student student);
    Task<Student> UpdateAsync(Student student);
    Task DeleteAsync(Guid id);
    Task<int> GetAgeAsync(Guid studentId);
}

public class StudentBlo : IStudentBlo
{
    private readonly IStudentDao _studentDao;
    private readonly IParentDao _parentDao;
    private readonly IClassDao _classDao;

    public StudentBlo(IStudentDao studentDao, IParentDao parentDao, IClassDao classDao)
    {
        _studentDao = studentDao;
        _parentDao = parentDao;
        _classDao = classDao;
    }

    public async Task<Student?> GetByIdAsync(Guid id)
    {
        return await _studentDao.GetByIdAsync(id);
    }

    public async Task<IEnumerable<Student>> GetAllAsync()
    {
        return await _studentDao.GetAllAsync();
    }

    public async Task<IEnumerable<Student>> GetByClassIdAsync(Guid classId)
    {
        return await _studentDao.GetByClassIdAsync(classId);
    }

    public async Task<IEnumerable<Student>> GetByParentIdAsync(Guid parentId)
    {
        return await _studentDao.GetByParentIdAsync(parentId);
    }

    public async Task<Student> CreateAsync(Student student)
    {
        // Business validation
        if (string.IsNullOrWhiteSpace(student.FullName))
        {
            throw new ArgumentException("Student name is required");
        }

        if (student.DateOfBirth >= DateTime.Now)
        {
            throw new ArgumentException("Invalid date of birth");
        }

        // Verify parent exists
        var parent = await _parentDao.GetByIdAsync(student.ParentId);
        if (parent == null)
        {
            throw new ArgumentException("Parent not found");
        }

        // Verify class if assigned
        if (student.ClassId.HasValue)
        {
            var classEntity = await _classDao.GetByIdAsync(student.ClassId.Value);
            if (classEntity == null)
            {
                throw new ArgumentException("Class not found");
            }

            // Check class capacity
            var studentsInClass = await _studentDao.GetByClassIdAsync(student.ClassId.Value);
            if (studentsInClass.Count() >= classEntity.Capacity)
            {
                throw new InvalidOperationException("Class is at full capacity");
            }
        }

        return await _studentDao.CreateAsync(student);
    }

    public async Task<Student> UpdateAsync(Student student)
    {
        if (student == null)
        {
            throw new ArgumentNullException(nameof(student));
        }

        var existing = await _studentDao.GetByIdAsync(student.Id);
        if (existing == null)
        {
            throw new InvalidOperationException("Student not found");
        }

        // Verify class if assigned
        if (student.ClassId.HasValue && student.ClassId != existing.ClassId)
        {
            var classEntity = await _classDao.GetByIdAsync(student.ClassId.Value);
            if (classEntity == null)
            {
                throw new ArgumentException("Class not found");
            }

            // Check class capacity
            var studentsInClass = await _studentDao.GetByClassIdAsync(student.ClassId.Value);
            if (studentsInClass.Count() >= classEntity.Capacity)
            {
                throw new InvalidOperationException("Class is at full capacity");
            }
        }

        return await _studentDao.UpdateAsync(student);
    }

    public async Task DeleteAsync(Guid id)
    {
        var student = await _studentDao.GetByIdAsync(id);
        if (student == null)
        {
            throw new InvalidOperationException("Student not found");
        }

        await _studentDao.DeleteAsync(id);
    }

    public async Task<int> GetAgeAsync(Guid studentId)
    {
        var student = await _studentDao.GetByIdAsync(studentId);
        if (student == null)
        {
            throw new InvalidOperationException("Student not found");
        }

        var age = DateTime.Now.Year - student.DateOfBirth.Year;
        if (DateTime.Now < student.DateOfBirth.AddYears(age))
        {
            age--;
        }

        return age;
    }
}

public interface IRoleBlo
{
    Task<Role?> GetByIdAsync(Guid id);
    Task<IEnumerable<Role>> GetAllAsync();
}

public class RoleBlo : IRoleBlo
{
    private readonly IRoleDao _roleDao;

    public RoleBlo(IRoleDao roleDao)
    {
        _roleDao = roleDao;
    }

    public async Task<Role?> GetByIdAsync(Guid id)
    {
        return await _roleDao.GetByIdAsync(id);
    }

    public async Task<IEnumerable<Role>> GetAllAsync()
    {
        return await _roleDao.GetAllAsync();
    }
}

public interface IPermissionBlo
{
    Task<Permission?> GetByIdAsync(Guid id);
    Task<IEnumerable<Permission>> GetAllAsync();
}

public class PermissionBlo : IPermissionBlo
{
    private readonly IPermissionDao _permissionDao;

    public PermissionBlo(IPermissionDao permissionDao)
    {
        _permissionDao = permissionDao;
    }

    public async Task<Permission?> GetByIdAsync(Guid id)
    {
        return await _permissionDao.GetByIdAsync(id);
    }

    public async Task<IEnumerable<Permission>> GetAllAsync()
    {
        return await _permissionDao.GetAllAsync();
    }
}

public interface IUserBlo
{
    Task<User?> GetByIdAsync(Guid id);
    Task<IEnumerable<User>> GetAllAsync();
    Task<User> CreateAsync(User user);
    Task<User> UpdateAsync(User user);
    Task DeleteAsync(Guid id);
}

public class UserBlo : IUserBlo
{
    private readonly IUserDao _userDao;

    public UserBlo(IUserDao userDao)
    {
        _userDao = userDao;
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await _userDao.GetByIdAsync(id);
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        return await _userDao.GetAllAsync();
    }

    public async Task<User> CreateAsync(User user)
    {
        if (string.IsNullOrWhiteSpace(user.FullName))
        {
            throw new ArgumentException("User name is required");
        }

        if (string.IsNullOrWhiteSpace(user.Email))
        {
            throw new ArgumentException("Email is required");
        }

        return await _userDao.CreateAsync(user);
    }

    public async Task<User> UpdateAsync(User user)
    {
        if (user == null)
        {
            throw new ArgumentNullException(nameof(user));
        }

        return await _userDao.UpdateAsync(user);
    }

    public async Task DeleteAsync(Guid id)
    {
        await _userDao.DeleteAsync(id);
    }
}

public interface IParentBlo
{
    Task<Parent?> GetByIdAsync(Guid id);
    Task<IEnumerable<Parent>> GetAllAsync();
    Task<Parent> CreateAsync(Parent parent);
    Task<Parent> UpdateAsync(Parent parent);
    Task DeleteAsync(Guid id);
}

public class ParentBlo : IParentBlo
{
    private readonly IParentDao _parentDao;

    public ParentBlo(IParentDao parentDao)
    {
        _parentDao = parentDao;
    }

    public async Task<Parent?> GetByIdAsync(Guid id)
    {
        return await _parentDao.GetByIdAsync(id);
    }

    public async Task<IEnumerable<Parent>> GetAllAsync()
    {
        return await _parentDao.GetAllAsync();
    }

    public async Task<Parent> CreateAsync(Parent parent)
    {
        if (string.IsNullOrWhiteSpace(parent.FullName))
        {
            throw new ArgumentException("Parent name is required");
        }

        if (string.IsNullOrWhiteSpace(parent.Email))
        {
            throw new ArgumentException("Email is required");
        }

        return await _parentDao.CreateAsync(parent);
    }

    public async Task<Parent> UpdateAsync(Parent parent)
    {
        if (parent == null)
        {
            throw new ArgumentNullException(nameof(parent));
        }

        return await _parentDao.UpdateAsync(parent);
    }

    public async Task DeleteAsync(Guid id)
    {
        await _parentDao.DeleteAsync(id);
    }
}
