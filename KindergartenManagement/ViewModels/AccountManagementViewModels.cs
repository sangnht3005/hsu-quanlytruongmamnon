using System.Collections.ObjectModel;
using System.Windows.Input;
using KindergartenManagement.BLO;
using KindergartenManagement.DTO;

namespace KindergartenManagement.ViewModels;

public class AccountManagementViewModel : ViewModelBase
{
    private readonly IAccountBlo _accountBlo;
    private readonly IRoleBlo _roleBlo;
    private readonly IUserBlo _userBlo;
    
    private ObservableCollection<Account> _accounts = new();
    private ObservableCollection<Role> _roles = new();
    private ObservableCollection<User> _users = new();
    private Account? _selectedAccount;
    private string _newPassword = string.Empty;
    private bool _isLoading;

    public AccountManagementViewModel(IAccountBlo accountBlo, IRoleBlo roleBlo, IUserBlo userBlo)
    {
        _accountBlo = accountBlo;
        _roleBlo = roleBlo;
        _userBlo = userBlo;

        LoadDataCommand = new AsyncRelayCommand(LoadDataAsync);
        AddAccountCommand = new RelayCommand(AddAccount);
        EditAccountCommand = new RelayCommand(EditAccount, CanEditOrDelete);
        DeleteAccountCommand = new AsyncRelayCommand(DeleteAccountAsync, CanEditOrDelete);
        SaveAccountCommand = new AsyncRelayCommand(SaveAccountAsync);
        ResetPasswordCommand = new AsyncRelayCommand(ResetPasswordAsync, CanEditOrDelete);

        // Initialize with empty account
        SelectedAccount = new Account 
        { 
            Id = Guid.Empty,
            Username = string.Empty,
            IsActive = true
        };

        // Auto-load data on initialization
        _ = LoadDataAsync(null);
    }

    public ObservableCollection<Account> Accounts
    {
        get => _accounts;
        set => SetProperty(ref _accounts, value);
    }

    public ObservableCollection<Role> Roles
    {
        get => _roles;
        set => SetProperty(ref _roles, value);
    }

    public ObservableCollection<User> Users
    {
        get => _users;
        set => SetProperty(ref _users, value);
    }

    public Account? SelectedAccount
    {
        get => _selectedAccount;
        set => SetProperty(ref _selectedAccount, value);
    }

    public Guid? SelectedUserId
    {
        get => SelectedAccount?.UserId;
        set
        {
            if (SelectedAccount != null && SelectedAccount.UserId != value)
            {
                SelectedAccount.UserId = value;
                OnPropertyChanged(nameof(SelectedUserId));
                OnPropertyChanged(nameof(SelectedAccount));
                OnPropertyChanged(nameof(SelectedUserEmail));
                OnPropertyChanged(nameof(SelectedUserPosition));
                
                // Auto-select role when user is selected
                if (value != null && value != Guid.Empty)
                {
                    AutoSelectRoleForUser();
                }
            }
        }
    }

    public string SelectedUserEmail
    {
        get
        {
            if (SelectedUserId == null || SelectedUserId == Guid.Empty) return string.Empty;
            var user = Users.FirstOrDefault(u => u.Id == SelectedUserId);
            return user?.Email ?? string.Empty;
        }
    }

    public string SelectedUserPosition
    {
        get
        {
            if (SelectedUserId == null || SelectedUserId == Guid.Empty) return string.Empty;
            var user = Users.FirstOrDefault(u => u.Id == SelectedUserId);
            return user?.Position ?? string.Empty;
        }
    }

    public string NewPassword
    {
        get => _newPassword;
        set => SetProperty(ref _newPassword, value);
    }

    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    public ICommand LoadDataCommand { get; }
    public ICommand AddAccountCommand { get; }
    public ICommand EditAccountCommand { get; }
    public ICommand DeleteAccountCommand { get; }
    public ICommand SaveAccountCommand { get; }
    public ICommand ResetPasswordCommand { get; }

    private async Task LoadDataAsync(object? parameter)
    {
        try
        {
            IsLoading = true;

            var accounts = await _accountBlo.GetAllAsync();
            Accounts = new ObservableCollection<Account>(accounts);

            var roles = await _roleBlo.GetAllAsync();
            Roles = new ObservableCollection<Role>(roles);

            var users = await _userBlo.GetAllAsync();
            Users = new ObservableCollection<User>(users);
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show($"Lỗi tải dữ liệu: {ex.Message}", "Lỗi", 
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void AddAccount(object? parameter)
    {
        SelectedAccount = new Account { Id = Guid.Empty };
    }

    private void AutoSelectRoleForUser()
    {
        if (SelectedUserId == null || SelectedUserId == Guid.Empty) return;

        var selectedUser = Users.FirstOrDefault(u => u.Id == SelectedUserId);
        if (selectedUser == null || string.IsNullOrEmpty(selectedUser.Position)) return;

        // Map position to role name
        string roleName = selectedUser.Position switch
        {
            "Hiệu trưởng" => "Admin",
            "Phó hiệu trưởng" => "Manager",
            "Giáo viên" => "Teacher",
            "Kế toán" => "Accountant",
            "Y tá" => "Nurse",
            _ => "Teacher" // Default to Teacher
        };

        // Find and set the role
        var role = Roles.FirstOrDefault(r => r.Name.Equals(roleName, StringComparison.OrdinalIgnoreCase));
        if (role != null && SelectedAccount != null)
        {
            SelectedAccount.RoleId = role.Id;
            OnPropertyChanged(nameof(SelectedAccount));
            System.Diagnostics.Debug.WriteLine($"Auto-selected role: {role.Name} for user: {selectedUser.FullName}");
        }
    }

    private void EditAccount(object? parameter)
    {
        // Edit logic handled in view
    }

    private bool CanEditOrDelete(object? parameter)
    {
        return SelectedAccount != null && SelectedAccount.Id != Guid.Empty;
    }

    private async Task DeleteAccountAsync(object? parameter)
    {
        if (SelectedAccount == null) return;

        var result = System.Windows.MessageBox.Show(
            $"Bạn có chắc chắn muốn xóa tài khoản {SelectedAccount.Username}?",
            "Xác nhận",
            System.Windows.MessageBoxButton.YesNo,
            System.Windows.MessageBoxImage.Question);

        if (result == System.Windows.MessageBoxResult.Yes)
        {
            try
            {
                await _accountBlo.DeleteAsync(SelectedAccount.Id);
                await LoadDataAsync(null);

                System.Windows.MessageBox.Show("Xóa tài khoản thành công!", "Thành công",
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi",
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }
    }

    private bool ValidateAccount()
    {
        if (SelectedAccount == null) return false;

        if (string.IsNullOrWhiteSpace(SelectedAccount.Username))
        {
            System.Windows.MessageBox.Show("Vui lòng nhập tên đăng nhập!", "Lỗi validation",
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
            return false;
        }

        if (SelectedAccount.Username.Length < 3)
        {
            System.Windows.MessageBox.Show("Tên đăng nhập phải ít nhất 3 ký tự!", "Lỗi validation",
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
            return false;
        }

        if (SelectedAccount.UserId == null || SelectedAccount.UserId == Guid.Empty)
        {
            System.Windows.MessageBox.Show("Vui lòng chọn nhân viên!", "Lỗi validation",
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
            return false;
        }

        if (SelectedAccount.RoleId == Guid.Empty)
        {
            System.Windows.MessageBox.Show("Vui lòng chọn vai trò!", "Lỗi validation",
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
            return false;
        }

        return true;
    }

    private async Task SaveAccountAsync(object? parameter)
    {
        if (SelectedAccount == null) return;

        if (!ValidateAccount())
        {
            return;
        }

        try
        {
            if (SelectedAccount.Id == Guid.Empty)
            {
                // Creating new account requires password
                if (string.IsNullOrWhiteSpace(NewPassword))
                {
                    System.Windows.MessageBox.Show("Vui lòng nhập mật khẩu!", "Lỗi",
                        System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
                    return;
                }

                if (NewPassword.Length < 6)
                {
                    System.Windows.MessageBox.Show("Mật khẩu phải ít nhất 6 ký tự!", "Lỗi",
                        System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
                    return;
                }

                await _accountBlo.CreateAsync(SelectedAccount.Username, NewPassword, SelectedAccount.RoleId);
            }
            else
            {
                await _accountBlo.UpdateAsync(SelectedAccount);
            }

            System.Windows.MessageBox.Show("Lưu tài khoản thành công!", "Thành công",
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);

            NewPassword = string.Empty;
            await LoadDataAsync(null);
            
            // Clear form for next entry
            SelectedAccount = new Account 
            { 
                Id = Guid.Empty,
                Username = string.Empty,
                IsActive = true
            };
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi",
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
        }
    }

    private Task ResetPasswordAsync(object? parameter)
    {
        if (SelectedAccount == null || string.IsNullOrWhiteSpace(NewPassword)) return Task.CompletedTask;

        try
        {
            // This would require a ResetPassword method in BLO
            System.Windows.MessageBox.Show("Chức năng đặt lại mật khẩu đang được phát triển", "Thông báo",
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi",
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
        }
        
        return Task.CompletedTask;
    }
}

public class RolePermissionManagementViewModel : ViewModelBase
{
    private readonly IRoleBlo _roleBlo;
    private readonly IPermissionBlo _permissionBlo;
    
    private ObservableCollection<Role> _roles = new();
    private ObservableCollection<Permission> _permissions = new();
    private Role? _selectedRole;

    public RolePermissionManagementViewModel(IRoleBlo roleBlo, IPermissionBlo permissionBlo)
    {
        _roleBlo = roleBlo;
        _permissionBlo = permissionBlo;

        LoadDataCommand = new AsyncRelayCommand(LoadDataAsync);
    }

    public ObservableCollection<Role> Roles
    {
        get => _roles;
        set => SetProperty(ref _roles, value);
    }

    public ObservableCollection<Permission> Permissions
    {
        get => _permissions;
        set => SetProperty(ref _permissions, value);
    }

    public Role? SelectedRole
    {
        get => _selectedRole;
        set => SetProperty(ref _selectedRole, value);
    }

    public ICommand LoadDataCommand { get; }

    private async Task LoadDataAsync(object? parameter)
    {
        try
        {
            var roles = await _roleBlo.GetAllAsync();
            Roles = new ObservableCollection<Role>(roles);

            var permissions = await _permissionBlo.GetAllAsync();
            Permissions = new ObservableCollection<Permission>(permissions);
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show($"Lỗi tải dữ liệu: {ex.Message}", "Lỗi", 
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
        }
    }
}

public class StaffManagementViewModel : ViewModelBase
{
    private readonly IUserBlo _userBlo;
    private readonly IAccountBlo _accountBlo;
    
    private ObservableCollection<User> _staff = new();
    private User? _selectedStaff;
    private bool _isLoading;

    public StaffManagementViewModel(IUserBlo userBlo, IAccountBlo accountBlo)
    {
        _userBlo = userBlo;
        _accountBlo = accountBlo;

        LoadDataCommand = new AsyncRelayCommand(LoadDataAsync);
        AddStaffCommand = new RelayCommand(AddStaff);
        EditStaffCommand = new RelayCommand(EditStaff, CanEditOrDelete);
        DeleteStaffCommand = new AsyncRelayCommand(DeleteStaffAsync, CanEditOrDelete);
        SaveStaffCommand = new AsyncRelayCommand(SaveStaffAsync);

        // Initialize with empty staff for user to input directly
        SelectedStaff = new User 
        { 
            Id = Guid.Empty,
            FullName = string.Empty,
            Email = string.Empty,
            PhoneNumber = string.Empty,
            Position = string.Empty,
            Salary = 0,
            Allowance = 0,
            Specialization = string.Empty
        };

        // Auto-load data on initialization
        _ = LoadDataAsync(null);
    }

    public ObservableCollection<User> Staff
    {
        get => _staff;
        set => SetProperty(ref _staff, value);
    }

    public User? SelectedStaff
    {
        get => _selectedStaff;
        set => SetProperty(ref _selectedStaff, value);
    }

    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    public ICommand LoadDataCommand { get; }
    public ICommand AddStaffCommand { get; }
    public ICommand EditStaffCommand { get; }
    public ICommand DeleteStaffCommand { get; }
    public ICommand SaveStaffCommand { get; }

    private async Task LoadDataAsync(object? parameter)
    {
        try
        {
            IsLoading = true;

            var staff = await _userBlo.GetAllAsync();
            Staff = new ObservableCollection<User>(staff);
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show($"Lỗi tải dữ liệu: {ex.Message}", "Lỗi", 
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void AddStaff(object? parameter)
    {
        SelectedStaff = new User 
        { 
            Id = Guid.Empty,
            FullName = string.Empty,
            Email = string.Empty,
            PhoneNumber = string.Empty,
            Position = string.Empty,
            Salary = 0,
            Allowance = 0,
            Specialization = string.Empty
        };
        System.Diagnostics.Debug.WriteLine("AddStaff: Created new Staff with Id = Guid.Empty");
    }

    private void EditStaff(object? parameter)
    {
        // Edit logic handled in view
    }

    private bool CanEditOrDelete(object? parameter)
    {
        return SelectedStaff != null && SelectedStaff.Id != Guid.Empty;
    }

    private async Task DeleteStaffAsync(object? parameter)
    {
        if (SelectedStaff == null) return;

        var result = System.Windows.MessageBox.Show(
            $"Bạn có chắc chắn muốn xóa nhân viên {SelectedStaff.FullName}?",
            "Xác nhận",
            System.Windows.MessageBoxButton.YesNo,
            System.Windows.MessageBoxImage.Question);

        if (result == System.Windows.MessageBoxResult.Yes)
        {
            try
            {
                await _userBlo.DeleteAsync(SelectedStaff.Id);
                Staff.Remove(SelectedStaff);
                SelectedStaff = null;

                System.Windows.MessageBox.Show("Xóa nhân viên thành công!", "Thành công",
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi",
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }
    }

    private bool ValidateStaff()
    {
        if (SelectedStaff == null) return false;

        if (string.IsNullOrWhiteSpace(SelectedStaff.FullName))
        {
            System.Windows.MessageBox.Show("Vui lòng nhập họ và tên nhân viên!", "Lỗi validation",
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
            return false;
        }

        if (string.IsNullOrWhiteSpace(SelectedStaff.Email))
        {
            System.Windows.MessageBox.Show("Vui lòng nhập email!", "Lỗi validation",
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
            return false;
        }

        if (!System.Text.RegularExpressions.Regex.IsMatch(SelectedStaff.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
        {
            System.Windows.MessageBox.Show("Email không đúng định dạng!", "Lỗi validation",
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
            return false;
        }

        if (string.IsNullOrWhiteSpace(SelectedStaff.Position))
        {
            System.Windows.MessageBox.Show("Vui lòng chọn chức vụ!", "Lỗi validation",
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
            return false;
        }

        if (SelectedStaff.Salary <= 0)
        {
            System.Windows.MessageBox.Show("Lương cơ bản phải lớn hơn 0!", "Lỗi validation",
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
            return false;
        }

        return true;
    }

    private async Task SaveStaffAsync(object? parameter)
    {
        try
        {
            System.Diagnostics.Debug.WriteLine("SaveStaffAsync called");
            System.Diagnostics.Debug.WriteLine($"Staff Id: {SelectedStaff?.Id}");
            System.Diagnostics.Debug.WriteLine($"Staff FullName: '{SelectedStaff?.FullName}'");
            System.Diagnostics.Debug.WriteLine($"Staff Email: '{SelectedStaff?.Email}'");
            System.Diagnostics.Debug.WriteLine($"Staff Position: '{SelectedStaff?.Position}'");
            System.Diagnostics.Debug.WriteLine($"Staff Salary: {SelectedStaff?.Salary}");

            if (!ValidateStaff())
            {
                return;
            }

            if (SelectedStaff == null) return; // Safety check

            System.Diagnostics.Debug.WriteLine("Validation passed, saving...");

            if (SelectedStaff.Id == Guid.Empty)
            {
                System.Diagnostics.Debug.WriteLine("Creating new staff...");
                var newStaff = await _userBlo.CreateAsync(SelectedStaff);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Updating existing staff...");
                await _userBlo.UpdateAsync(SelectedStaff);
            }

            System.Windows.MessageBox.Show("Lưu nhân viên thành công!", "Thành công",
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);

            await LoadDataAsync(null);
            
            // Clear form for next entry
            SelectedStaff = new User 
            { 
                Id = Guid.Empty,
                FullName = string.Empty,
                Email = string.Empty,
                PhoneNumber = string.Empty,
                Position = string.Empty,
                Salary = 0,
                Allowance = 0,
                Specialization = string.Empty
            };
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Exception: {ex}");
            System.Windows.MessageBox.Show($"Lỗi: {ex.Message}\n\nStack trace: {ex.StackTrace}", "Lỗi",
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
        }
    }
}

public class ParentManagementViewModel : ViewModelBase
{
    private readonly IParentBlo _parentBlo;
    
    private ObservableCollection<Parent> _parents = new();
    private Parent? _selectedParent;
    private bool _isLoading;

    public ParentManagementViewModel(IParentBlo parentBlo)
    {
        _parentBlo = parentBlo;

        LoadDataCommand = new AsyncRelayCommand(LoadDataAsync);
        AddParentCommand = new RelayCommand(AddParent);
        EditParentCommand = new RelayCommand(EditParent, CanEditOrDelete);
        DeleteParentCommand = new AsyncRelayCommand(DeleteParentAsync, CanEditOrDelete);
        SaveParentCommand = new AsyncRelayCommand(SaveParentAsync);

        // Initialize with empty parent for user to input directly
        SelectedParent = new Parent 
        { 
            Id = Guid.Empty,
            FullName = string.Empty,
            Email = string.Empty,
            PhoneNumber = string.Empty,
            Occupation = string.Empty,
            Address = string.Empty
        };

        // Auto-load data on initialization
        _ = LoadDataAsync(null);
    }

    public ObservableCollection<Parent> Parents
    {
        get => _parents;
        set => SetProperty(ref _parents, value);
    }

    public Parent? SelectedParent
    {
        get => _selectedParent;
        set => SetProperty(ref _selectedParent, value);
    }

    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    public ICommand LoadDataCommand { get; }
    public ICommand AddParentCommand { get; }
    public ICommand EditParentCommand { get; }
    public ICommand DeleteParentCommand { get; }
    public ICommand SaveParentCommand { get; }

    private async Task LoadDataAsync(object? parameter)
    {
        try
        {
            IsLoading = true;

            var parents = await _parentBlo.GetAllAsync();
            Parents = new ObservableCollection<Parent>(parents);
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show($"Lỗi tải dữ liệu: {ex.Message}", "Lỗi", 
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void AddParent(object? parameter)
    {
        SelectedParent = new Parent 
        { 
            Id = Guid.Empty,
            FullName = string.Empty,
            Email = string.Empty,
            PhoneNumber = string.Empty,
            Occupation = string.Empty,
            Address = string.Empty
        };
        System.Diagnostics.Debug.WriteLine("AddParent: Created new Parent with Id = Guid.Empty");
    }

    private void EditParent(object? parameter)
    {
        // Edit logic handled in view
    }

    private bool CanEditOrDelete(object? parameter)
    {
        return SelectedParent != null && SelectedParent.Id != Guid.Empty;
    }

    private async Task DeleteParentAsync(object? parameter)
    {
        if (SelectedParent == null) return;

        var result = System.Windows.MessageBox.Show(
            $"Bạn có chắc chắn muốn xóa phụ huynh {SelectedParent.FullName}?",
            "Xác nhận",
            System.Windows.MessageBoxButton.YesNo,
            System.Windows.MessageBoxImage.Question);

        if (result == System.Windows.MessageBoxResult.Yes)
        {
            try
            {
                await _parentBlo.DeleteAsync(SelectedParent.Id);
                Parents.Remove(SelectedParent);
                SelectedParent = null;

                System.Windows.MessageBox.Show("Xóa phụ huynh thành công!", "Thành công",
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi",
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }
    }

    private bool ValidateParent()
    {
        if (SelectedParent == null) return false;

        if (string.IsNullOrWhiteSpace(SelectedParent.FullName))
        {
            System.Windows.MessageBox.Show("Vui lòng nhập họ và tên phụ huynh!", "Lỗi validation",
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
            return false;
        }

        if (string.IsNullOrWhiteSpace(SelectedParent.Email))
        {
            System.Windows.MessageBox.Show("Vui lòng nhập email!", "Lỗi validation",
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
            return false;
        }

        // Validate email format
        if (!System.Text.RegularExpressions.Regex.IsMatch(SelectedParent.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
        {
            System.Windows.MessageBox.Show("Email không đúng định dạng!", "Lỗi validation",
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
            return false;
        }

        if (string.IsNullOrWhiteSpace(SelectedParent.PhoneNumber))
        {
            System.Windows.MessageBox.Show("Vui lòng nhập số điện thoại!", "Lỗi validation",
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
            return false;
        }

        // Validate phone number format (10-11 digits)
        if (!System.Text.RegularExpressions.Regex.IsMatch(SelectedParent.PhoneNumber, @"^\d{10,11}$"))
        {
            System.Windows.MessageBox.Show("Số điện thoại phải là 10-11 chữ số!", "Lỗi validation",
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
            return false;
        }

        return true;
    }

    private async Task SaveParentAsync(object? parameter)
    {
        try
        {
            System.Diagnostics.Debug.WriteLine("SaveParentAsync called");
            System.Diagnostics.Debug.WriteLine($"Parent Id: {SelectedParent?.Id}");
            System.Diagnostics.Debug.WriteLine($"Parent FullName: '{SelectedParent?.FullName}'");
            System.Diagnostics.Debug.WriteLine($"Parent Email: '{SelectedParent?.Email}'");
            System.Diagnostics.Debug.WriteLine($"Parent PhoneNumber: '{SelectedParent?.PhoneNumber}'");
            System.Diagnostics.Debug.WriteLine($"Parent Occupation: '{SelectedParent?.Occupation}'");
            System.Diagnostics.Debug.WriteLine($"Parent Address: '{SelectedParent?.Address}'");

            // Validate before saving
            if (!ValidateParent())
            {
                return;
            }

            if (SelectedParent == null) return; // Safety check

            System.Diagnostics.Debug.WriteLine("Validation passed, saving...");

            if (SelectedParent.Id == Guid.Empty)
            {
                System.Diagnostics.Debug.WriteLine("Creating new parent...");
                var newParent = await _parentBlo.CreateAsync(SelectedParent);
                // Don't add directly, reload from database to get all data
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Updating existing parent...");
                await _parentBlo.UpdateAsync(SelectedParent);
            }

            System.Windows.MessageBox.Show("Lưu phụ huynh thành công!", "Thành công",
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);

            // Reload data to refresh DataGrid
            await LoadDataAsync(null);
            
            // Clear form for next entry
            SelectedParent = new Parent 
            { 
                Id = Guid.Empty,
                FullName = string.Empty,
                Email = string.Empty,
                PhoneNumber = string.Empty,
                Occupation = string.Empty,
                Address = string.Empty
            };
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Exception: {ex}");
            System.Windows.MessageBox.Show($"Lỗi: {ex.Message}\n\nStack trace: {ex.StackTrace}", "Lỗi",
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
        }
    }
}

public class StaffLeaveManagementViewModel : ViewModelBase
{
    private readonly IStaffLeaveBlo _staffLeaveBlo;
    private readonly IUserBlo _userBlo;
    private Account? _currentAccount;
    
    private ObservableCollection<StaffLeave> _staffLeaves = new();
    private ObservableCollection<User> _users = new();
    private StaffLeave? _selectedLeave;
    private string _tempStatus = "Pending";
    private bool _isLoading;

    public StaffLeaveManagementViewModel(IStaffLeaveBlo staffLeaveBlo, IUserBlo userBlo)
    {
        _staffLeaveBlo = staffLeaveBlo;
        _userBlo = userBlo;

        LoadDataCommand = new AsyncRelayCommand(LoadDataAsync);
        AddLeaveCommand = new RelayCommand(AddLeave);
        SaveLeaveCommand = new AsyncRelayCommand(SaveLeaveAsync);
        DeleteLeaveCommand = new AsyncRelayCommand(DeleteLeaveAsync, CanDelete);

        // Initialize with empty leave for user to input directly
        SelectedLeave = new StaffLeave
        {
            Id = Guid.Empty,
            StartDate = DateTime.Now,
            EndDate = DateTime.Now.AddDays(1),
            Status = "Pending",
            LeaveType = string.Empty,
            Reason = string.Empty
        };

        // Auto-load data on initialization
        _ = LoadDataAsync(null);
    }

    public Account? CurrentAccount
    {
        get => _currentAccount;
        set
        {
            if (SetProperty(ref _currentAccount, value))
            {
                OnPropertyChanged(nameof(IsAdminOrManager));
                OnPropertyChanged(nameof(CanSelectUser));
                OnPropertyChanged(nameof(CanEditStatus));
                OnPropertyChanged(nameof(CanEditApprovalNotes));
            }
        }
    }

    public bool IsAdminOrManager
    {
        get
        {
            if (CurrentAccount?.Role == null) return false;
            var roleName = CurrentAccount.Role.Name?.ToLower();
            return roleName == "admin" || roleName == "manager" || roleName == "hiệu trưởng";
        }
    }

    public bool CanSelectUser => IsAdminOrManager;
    public bool CanEditStatus => IsAdminOrManager;
    public bool CanEditApprovalNotes => IsAdminOrManager;

    public ObservableCollection<StaffLeave> StaffLeaves
    {
        get => _staffLeaves;
        set => SetProperty(ref _staffLeaves, value);
    }

    public ObservableCollection<User> Users
    {
        get => _users;
        set => SetProperty(ref _users, value);
    }

    public StaffLeave? SelectedLeave
    {
        get => _selectedLeave;
        set
        {
            if (SetProperty(ref _selectedLeave, value))
            {
                // When selecting a leave, copy its status to temp status
                if (_selectedLeave != null)
                {
                    TempStatus = _selectedLeave.Status;
                }
            }
        }
    }

    public string TempStatus
    {
        get => _tempStatus;
        set => SetProperty(ref _tempStatus, value);
    }

    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    public ICommand LoadDataCommand { get; }
    public ICommand AddLeaveCommand { get; }
    public ICommand SaveLeaveCommand { get; }
    public ICommand DeleteLeaveCommand { get; }

    private async Task LoadDataAsync(object? parameter)
    {
        try
        {
            IsLoading = true;

            // Fix any malformed status values in database (one-time cleanup)
            try
            {
                await _staffLeaveBlo.FixAllStatusValuesAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Warning: Could not fix status values: {ex.Message}");
            }

            var leaves = await _staffLeaveBlo.GetAllAsync();
            
            // If not admin/manager, only show current user's leaves
            if (!IsAdminOrManager && CurrentAccount?.UserId != null)
            {
                leaves = leaves.Where(l => l.UserId == CurrentAccount.UserId.Value).ToList();
            }
            
            StaffLeaves = new ObservableCollection<StaffLeave>(leaves);

            var users = await _userBlo.GetAllAsync();
            Users = new ObservableCollection<User>(users);
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show($"Lỗi tải dữ liệu: {ex.Message}", "Lỗi", 
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void AddLeave(object? parameter)
    {
        var newLeave = new StaffLeave 
        { 
            Id = Guid.Empty,
            StartDate = DateTime.Now, 
            EndDate = DateTime.Now.AddDays(1),
            Status = "Pending",
            LeaveType = string.Empty,
            Reason = string.Empty
        };
        
        // If not admin/manager, auto-set to current user
        if (!IsAdminOrManager && CurrentAccount?.UserId != null)
        {
            newLeave.UserId = CurrentAccount.UserId.Value;
        }
        
        SelectedLeave = newLeave;
        TempStatus = "Pending";
    }

    private bool CanDelete(object? parameter)
    {
        return SelectedLeave != null && SelectedLeave.Id != Guid.Empty;
    }

    private async Task ApproveLeaveAsync(object? parameter)
    {
        if (SelectedLeave == null) return;

        try
        {
            // TODO: Get the current logged-in user's ID from app context
            // For now, using a placeholder - you should get this from MainViewModel.CurrentAccount
            var approverId = Guid.Parse("00000000-0000-0000-0000-000000000001"); // Admin user from seeder
            await _staffLeaveBlo.ApproveLeaveAsync(SelectedLeave.Id, approverId, null);
            
            System.Windows.MessageBox.Show("Duyệt đơn nghỉ thành công!", "Thành công",
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);

            await LoadDataAsync(null);
            
            // Clear form
            SelectedLeave = new StaffLeave
            {
                Id = Guid.Empty,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(1),
                Status = "Pending",
                LeaveType = string.Empty,
                Reason = string.Empty
            };
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi",
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
        }
    }

    private async Task RejectLeaveAsync(object? parameter)
    {
        if (SelectedLeave == null) return;

        try
        {
            // TODO: Get the current logged-in user's ID from app context
            var approverId = Guid.Parse("00000000-0000-0000-0000-000000000001"); // Admin user from seeder
            await _staffLeaveBlo.RejectLeaveAsync(SelectedLeave.Id, approverId, null);
            
            System.Windows.MessageBox.Show("Từ chối đơn nghỉ thành công!", "Thành công",
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);

            await LoadDataAsync(null);
            
            // Clear form
            SelectedLeave = new StaffLeave
            {
                Id = Guid.Empty,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(1),
                Status = "Pending",
                LeaveType = string.Empty,
                Reason = string.Empty
            };
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi",
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
        }
    }

    private bool ValidateLeave()
    {
        if (SelectedLeave == null) return false;

        if (SelectedLeave.UserId == Guid.Empty)
        {
            System.Windows.MessageBox.Show("Vui lòng chọn nhân viên!", "Lỗi validation",
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
            return false;
        }

        if (SelectedLeave.StartDate == default)
        {
            System.Windows.MessageBox.Show("Vui lòng chọn ngày bắt đầu!", "Lỗi validation",
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
            return false;
        }

        if (SelectedLeave.EndDate == default)
        {
            System.Windows.MessageBox.Show("Vui lòng chọn ngày kết thúc!", "Lỗi validation",
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
            return false;
        }

        if (SelectedLeave.EndDate < SelectedLeave.StartDate)
        {
            System.Windows.MessageBox.Show("Ngày kết thúc phải sau ngày bắt đầu!", "Lỗi validation",
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
            return false;
        }

        if (string.IsNullOrWhiteSpace(SelectedLeave.LeaveType))
        {
            System.Windows.MessageBox.Show("Vui lòng chọn loại nghỉ!", "Lỗi validation",
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
            return false;
        }

        if (string.IsNullOrWhiteSpace(SelectedLeave.Reason))
        {
            System.Windows.MessageBox.Show("Vui lòng nhập lý do nghỉ!", "Lỗi validation",
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
            return false;
        }

        return true;
    }

    private async Task SaveLeaveAsync(object? parameter)
    {
        if (SelectedLeave == null) return;

        if (!ValidateLeave())
        {
            return;
        }

        try
        {
            // Update the actual Status from TempStatus before saving
            SelectedLeave.Status = TempStatus;
            
            System.Diagnostics.Debug.WriteLine($"Saving Leave - Id: {SelectedLeave.Id}, Status: {SelectedLeave.Status}, UserId: {SelectedLeave.UserId}");
            
            // If not admin/manager and creating new, ensure UserId is current user
            if (!IsAdminOrManager && SelectedLeave.Id == Guid.Empty && CurrentAccount?.UserId != null)
            {
                SelectedLeave.UserId = CurrentAccount.UserId.Value;
            }
            
            // If status changed to Approved/Rejected and user is admin/manager, set ApprovedBy
            if (IsAdminOrManager && (SelectedLeave.Status == "Approved" || SelectedLeave.Status == "Rejected"))
            {
                if (CurrentAccount?.UserId != null)
                {
                    SelectedLeave.ApprovedBy = CurrentAccount.UserId.Value;
                    SelectedLeave.ApprovalDate = DateTime.Now;
                }
            }
            
            if (SelectedLeave.Id == Guid.Empty)
            {
                System.Diagnostics.Debug.WriteLine("Calling CreateAsync");
                await _staffLeaveBlo.CreateAsync(SelectedLeave);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Calling UpdateAsync");
                await _staffLeaveBlo.UpdateAsync(SelectedLeave);
            }

            System.Windows.MessageBox.Show("Lưu đơn nghỉ thành công!", "Thành công",
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);

            await LoadDataAsync(null);
            
            // Clear form for next entry
            var newLeave = new StaffLeave
            {
                Id = Guid.Empty,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(1),
                Status = "Pending",
                LeaveType = string.Empty,
                Reason = string.Empty
            };
            
            // Auto-set UserId for non-admin
            if (!IsAdminOrManager && CurrentAccount?.UserId != null)
            {
                newLeave.UserId = CurrentAccount.UserId.Value;
            }
            
            SelectedLeave = newLeave;
            TempStatus = "Pending";
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi",
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
        }
    }

    private async Task DeleteLeaveAsync(object? parameter)
    {
        if (SelectedLeave == null) return;

        var result = System.Windows.MessageBox.Show(
            $"Bạn có chắc chắn muốn xóa đơn nghỉ này?",
            "Xác nhận",
            System.Windows.MessageBoxButton.YesNo,
            System.Windows.MessageBoxImage.Question);

        if (result == System.Windows.MessageBoxResult.Yes)
        {
            try
            {
                await _staffLeaveBlo.DeleteAsync(SelectedLeave.Id);
                StaffLeaves.Remove(SelectedLeave);
                SelectedLeave = null;

                System.Windows.MessageBox.Show("Xóa đơn nghỉ thành công!", "Thành công",
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi",
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }
    }
}
