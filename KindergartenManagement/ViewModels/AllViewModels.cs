using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Text.RegularExpressions;
using KindergartenManagement.BLO;
using KindergartenManagement.DTO;

namespace KindergartenManagement.ViewModels;

public class StudentManagementViewModel : ViewModelBase
{
    private readonly IStudentBlo _studentBlo;
    private readonly IParentBlo _parentBlo;
    private readonly IClassBlo _classBlo;
    
    private ObservableCollection<Student> _students = new();
    private ObservableCollection<Parent> _parents = new();
    private ObservableCollection<Class> _classes = new();
    private Student? _selectedStudent;
    private bool _isLoading;

    public StudentManagementViewModel(IStudentBlo studentBlo, IParentBlo parentBlo, IClassBlo classBlo)
    {
        _studentBlo = studentBlo;
        _parentBlo = parentBlo;
        _classBlo = classBlo;

        LoadDataCommand = new AsyncRelayCommand(LoadDataAsync);
        AddStudentCommand = new RelayCommand(AddStudent);
        EditStudentCommand = new RelayCommand(EditStudent, CanEditOrDelete);
        DeleteStudentCommand = new AsyncRelayCommand(DeleteStudentAsync, CanEditOrDelete);
        SaveStudentCommand = new AsyncRelayCommand(SaveStudentAsync);

        // Initialize with empty student for user to input directly
        SelectedStudent = new Student
        {
            Id = Guid.Empty,
            FullName = string.Empty,
            DateOfBirth = DateTime.Now.AddYears(-3),
            Gender = string.Empty,
            Address = string.Empty
        };

        // Auto-load data on initialization
        _ = LoadDataAsync(null);
    }

    public ObservableCollection<Student> Students
    {
        get => _students;
        set => SetProperty(ref _students, value);
    }

    public ObservableCollection<Parent> Parents
    {
        get => _parents;
        set => SetProperty(ref _parents, value);
    }

    public ObservableCollection<Class> Classes
    {
        get => _classes;
        set => SetProperty(ref _classes, value);
    }

    public Student? SelectedStudent
    {
        get => _selectedStudent;
        set
        {
            SetProperty(ref _selectedStudent, value);
            // Nếu bỏ chọn (click ra ngoài), clear form
            if (value == null)
            {
                _selectedStudent = new Student
                {
                    Id = Guid.Empty,
                    FullName = string.Empty,
                    DateOfBirth = DateTime.Now.AddYears(-3),
                    Gender = string.Empty,
                    Address = string.Empty
                };
                OnPropertyChanged(nameof(SelectedStudent));
            }
        }
    }

    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    public ICommand LoadDataCommand { get; }
    public ICommand AddStudentCommand { get; }
    public ICommand EditStudentCommand { get; }
    public ICommand DeleteStudentCommand { get; }
    public ICommand SaveStudentCommand { get; }

    private async Task LoadDataAsync(object? parameter)
    {
        try
        {
            IsLoading = true;

            var students = await _studentBlo.GetAllAsync();
            Students = new ObservableCollection<Student>(students);

            var parents = await _parentBlo.GetAllAsync();
            Parents = new ObservableCollection<Parent>(parents);

            var classes = await _classBlo.GetAllAsync();
            Classes = new ObservableCollection<Class>(classes);
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

    private void AddStudent(object? parameter)
    {
        SelectedStudent = new Student 
        { 
            Id = Guid.Empty,
            DateOfBirth = DateTime.Now.AddYears(-3)
        };
    }

    private void EditStudent(object? parameter)
    {
        // Edit logic will be handled in the view
    }

    private bool CanEditOrDelete(object? parameter)
    {
        return SelectedStudent != null && SelectedStudent.Id != Guid.Empty;
    }

    private async Task DeleteStudentAsync(object? parameter)
    {
        if (SelectedStudent == null) return;

        var result = System.Windows.MessageBox.Show(
            $"Bạn có chắc chắn muốn xóa học sinh {SelectedStudent.FullName}?",
            "Xác nhận",
            System.Windows.MessageBoxButton.YesNo,
            System.Windows.MessageBoxImage.Question);

        if (result == System.Windows.MessageBoxResult.Yes)
        {
            try
            {
                await _studentBlo.DeleteAsync(SelectedStudent.Id);
                Students.Remove(SelectedStudent);
                SelectedStudent = null;

                System.Windows.MessageBox.Show("Xóa học sinh thành công!", "Thành công",
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Lỗi xóa học sinh: {ex.Message}", "Lỗi",
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }
    }

    private bool ValidateStudent()
    {
        if (SelectedStudent == null) return false;

        if (string.IsNullOrWhiteSpace(SelectedStudent.FullName))
        {
            System.Windows.MessageBox.Show("Vui lòng nhập họ và tên học sinh!", "Lỗi validation",
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
            return false;
        }

        if (SelectedStudent.DateOfBirth == default || SelectedStudent.DateOfBirth > DateTime.Now)
        {
            System.Windows.MessageBox.Show("Ngày sinh không hợp lệ!", "Lỗi validation",
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
            return false;
        }

        if (string.IsNullOrWhiteSpace(SelectedStudent.Gender))
        {
            System.Windows.MessageBox.Show("Vui lòng chọn giới tính!", "Lỗi validation",
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
            return false;
        }

        if (SelectedStudent.ParentId == Guid.Empty)
        {
            System.Windows.MessageBox.Show("Vui lòng chọn phụ huynh!", "Lỗi validation",
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
            return false;
        }

        return true;
    }

    private async Task SaveStudentAsync(object? parameter)
    {
        if (SelectedStudent == null) return;

        if (!ValidateStudent())
        {
            return;
        }

        try
        {
            if (SelectedStudent.Id == Guid.Empty)
            {
                var newStudent = await _studentBlo.CreateAsync(SelectedStudent);
            }
            else
            {
                await _studentBlo.UpdateAsync(SelectedStudent);
            }

            System.Windows.MessageBox.Show("Lưu học sinh thành công!", "Thành công",
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);

            await LoadDataAsync(null);
            
            // Clear form for next entry
            SelectedStudent = new Student
            {
                Id = Guid.Empty,
                FullName = string.Empty,
                DateOfBirth = DateTime.Now.AddYears(-3),
                Gender = string.Empty,
                Address = string.Empty
            };
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show($"Lỗi lưu học sinh: {ex.Message}", "Lỗi",
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
        }
    }
}

public class ClassManagementViewModel : ViewModelBase
{
    private readonly IClassBlo _classBlo;
    private readonly IGradeBlo _gradeBlo;
    private readonly IUserBlo _userBlo;
    private ObservableCollection<Class> _classes = new();
    private ObservableCollection<Grade> _grades = new();
    private ObservableCollection<User> _teachers = new();
    private Class? _selectedClass;

    public ClassManagementViewModel(IClassBlo classBlo, IGradeBlo gradeBlo, IUserBlo userBlo)
    {
        _classBlo = classBlo;
        _gradeBlo = gradeBlo;
        _userBlo = userBlo;
        
        LoadDataCommand = new AsyncRelayCommand(LoadDataAsync);
        AddClassCommand = new RelayCommand(AddClass);
        SaveClassCommand = new AsyncRelayCommand(SaveClassAsync);
        DeleteClassCommand = new AsyncRelayCommand(DeleteClassAsync, CanEditOrDelete);

        // Initialize with empty class for user to input directly
        SelectedClass = new Class
        {
            Id = Guid.Empty,
            Name = string.Empty,
            Capacity = 0
        };

        // Auto-load data on initialization
        _ = LoadDataAsync(null);
    }

    public ObservableCollection<Class> Classes
    {
        get => _classes;
        set => SetProperty(ref _classes, value);
    }

    public ObservableCollection<Grade> Grades
    {
        get => _grades;
        set => SetProperty(ref _grades, value);
    }

    public ObservableCollection<User> Teachers
    {
        get => _teachers;
        set => SetProperty(ref _teachers, value);
    }

    public Class? SelectedClass
    {
        get => _selectedClass;
        set
        {
            SetProperty(ref _selectedClass, value);
            // Nếu bỏ chọn (click ra ngoài), clear form
            if (value == null)
            {
                _selectedClass = new Class
                {
                    Id = Guid.Empty,
                    Name = string.Empty,
                    GradeId = Guid.Empty,
                    TeacherId = Guid.Empty
                };
                OnPropertyChanged(nameof(SelectedClass));
            }
        }
    }

    public ICommand LoadDataCommand { get; }
    public ICommand AddClassCommand { get; }
    public ICommand SaveClassCommand { get; }
    public ICommand DeleteClassCommand { get; }

    private async Task LoadDataAsync(object? parameter)
    {
        var classes = await _classBlo.GetAllAsync();
        Classes = new ObservableCollection<Class>(classes);

        var grades = await _gradeBlo.GetAllAsync();
        Grades = new ObservableCollection<Grade>(grades);

        var users = await _userBlo.GetAllAsync();
        Teachers = new ObservableCollection<User>(users);
    }

    private void AddClass(object? parameter)
    {
        SelectedClass = new Class 
        { 
            Id = Guid.Empty,
            Capacity = 30 
        };
    }

    private bool CanEditOrDelete(object? parameter)
    {
        return SelectedClass != null && SelectedClass.Id != Guid.Empty;
    }

    private bool ValidateClass()
    {
        if (SelectedClass == null) return false;

        if (string.IsNullOrWhiteSpace(SelectedClass.Name))
        {
            System.Windows.MessageBox.Show("Vui lòng nhập tên lớp!", "Lỗi validation",
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
            return false;
        }

        if (SelectedClass.GradeId == Guid.Empty)
        {
            System.Windows.MessageBox.Show("Vui lòng chọn khối học!", "Lỗi validation",
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
            return false;
        }

        if (SelectedClass.Capacity <= 0 || SelectedClass.Capacity > 50)
        {
            System.Windows.MessageBox.Show("Sĩ số tối đa phải từ 1 đến 50!", "Lỗi validation",
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
            return false;
        }

        return true;
    }

    private async Task SaveClassAsync(object? parameter)
    {
        if (SelectedClass == null) return;

        if (!ValidateClass())
        {
            return;
        }

        try
        {
            if (SelectedClass.Id == Guid.Empty)
            {
                var newClass = await _classBlo.CreateAsync(SelectedClass);
            }
            else
            {
                await _classBlo.UpdateAsync(SelectedClass);
            }

            System.Windows.MessageBox.Show("Lưu lớp học thành công!", "Thành công",
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);

            await LoadDataAsync(null);
            
            // Clear form for next entry
            SelectedClass = new Class
            {
                Id = Guid.Empty,
                Name = string.Empty,
                Capacity = 0
            };
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show($"Lỗi lưu lớp học: {ex.Message}", "Lỗi",
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
        }
    }

    private async Task DeleteClassAsync(object? parameter)
    {
        if (SelectedClass == null) return;

        var result = System.Windows.MessageBox.Show(
            $"Bạn có chắc chắn muốn xóa lớp {SelectedClass.Name}?",
            "Xác nhận",
            System.Windows.MessageBoxButton.YesNo,
            System.Windows.MessageBoxImage.Question);

        if (result == System.Windows.MessageBoxResult.Yes)
        {
            try
            {
                await _classBlo.DeleteAsync(SelectedClass.Id);
                Classes.Remove(SelectedClass);
                SelectedClass = null;

                System.Windows.MessageBox.Show("Xóa lớp học thành công!", "Thành công",
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Lỗi xóa lớp học: {ex.Message}", "Lỗi",
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }
    }
}

public class GradeManagementViewModel : ViewModelBase
{
    private readonly IGradeBlo _gradeBlo;
    private ObservableCollection<Grade> _grades = new();
    private Grade? _selectedGrade;

    public GradeManagementViewModel(IGradeBlo gradeBlo)
    {
        _gradeBlo = gradeBlo;

        LoadDataCommand = new AsyncRelayCommand(LoadDataAsync);
        AddGradeCommand = new RelayCommand(AddGrade);
        SaveGradeCommand = new AsyncRelayCommand(SaveGradeAsync);
        DeleteGradeCommand = new AsyncRelayCommand(DeleteGradeAsync, CanEditOrDelete);

        // Initialize with empty grade for user to input directly
        SelectedGrade = new Grade
        {
            Id = Guid.Empty,
            Name = string.Empty,
            MinAge = 0,
            MaxAge = 0
        };

        // Auto-load data on initialization
        _ = LoadDataAsync(null);
    }

    public ObservableCollection<Grade> Grades
    {
        get => _grades;
        set => SetProperty(ref _grades, value);
    }

    public Grade? SelectedGrade
    {
        get => _selectedGrade;
        set => SetProperty(ref _selectedGrade, value);
    }

    public ICommand LoadDataCommand { get; }
    public ICommand AddGradeCommand { get; }
    public ICommand SaveGradeCommand { get; }
    public ICommand DeleteGradeCommand { get; }

    private async Task LoadDataAsync(object? parameter)
    {
        try
        {
            var grades = await _gradeBlo.GetAllAsync();
            Grades = new ObservableCollection<Grade>(grades);
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show($"Lỗi tải dữ liệu: {ex.Message}", "Lỗi",
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
        }
    }

    private void AddGrade(object? parameter)
    {
        SelectedGrade = new Grade
        {
            Id = Guid.Empty,
            Name = string.Empty,
            MinAge = 0,
            MaxAge = 0
        };
    }

    private bool CanEditOrDelete(object? parameter)
    {
        return SelectedGrade != null && SelectedGrade.Id != Guid.Empty;
    }

    private bool ValidateGrade()
    {
        if (SelectedGrade == null) return false;

        if (string.IsNullOrWhiteSpace(SelectedGrade.Name))
        {
            System.Windows.MessageBox.Show("Vui lòng nhập tên khối!", "Lỗi validation",
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
            return false;
        }

        if (SelectedGrade.MinAge < 0 || SelectedGrade.MaxAge < 0)
        {
            System.Windows.MessageBox.Show("Độ tuổi không được âm!", "Lỗi validation",
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
            return false;
        }

        if (SelectedGrade.MinAge > SelectedGrade.MaxAge)
        {
            System.Windows.MessageBox.Show("Độ tuổi tối thiểu không được lớn hơn độ tuổi tối đa!", "Lỗi validation",
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
            return false;
        }

        return true;
    }

    private async Task SaveGradeAsync(object? parameter)
    {
        if (SelectedGrade == null) return;

        if (!ValidateGrade())
        {
            return;
        }

        try
        {
            if (SelectedGrade.Id == Guid.Empty)
            {
                await _gradeBlo.CreateAsync(SelectedGrade);
            }
            else
            {
                await _gradeBlo.UpdateAsync(SelectedGrade);
            }

            System.Windows.MessageBox.Show("Lưu khối lớp thành công!", "Thành công",
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);

            await LoadDataAsync(null);

            // Clear form for next entry
            SelectedGrade = new Grade
            {
                Id = Guid.Empty,
                Name = string.Empty,
                MinAge = 0,
                MaxAge = 0
            };
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show($"Lỗi lưu khối lớp: {ex.Message}", "Lỗi",
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
        }
    }

    private async Task DeleteGradeAsync(object? parameter)
    {
        if (SelectedGrade == null) return;

        var result = System.Windows.MessageBox.Show(
            $"Bạn có chắc chắn muốn xóa khối {SelectedGrade.Name}?",
            "Xác nhận",
            System.Windows.MessageBoxButton.YesNo,
            System.Windows.MessageBoxImage.Question);

        if (result == System.Windows.MessageBoxResult.Yes)
        {
            try
            {
                await _gradeBlo.DeleteAsync(SelectedGrade.Id);
                Grades.Remove(SelectedGrade);
                SelectedGrade = new Grade
                {
                    Id = Guid.Empty,
                    Name = string.Empty,
                    MinAge = 0,
                    MaxAge = 0
                };

                System.Windows.MessageBox.Show("Xóa khối lớp thành công!", "Thành công",
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Lỗi xóa khối lớp: {ex.Message}", "Lỗi",
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }
    }
}

public class AttendanceManagementViewModel : ViewModelBase
{
    private readonly IAttendanceBlo _attendanceBlo;
    private readonly IClassBlo _classBlo;
    private ObservableCollection<Attendance> _attendances = new();
    private ObservableCollection<Class> _classes = new();
    private Class? _selectedClass;
    private DateTime _selectedDate = DateTime.Today;
    private bool _isLoading;

    public AttendanceManagementViewModel(IAttendanceBlo attendanceBlo, IClassBlo classBlo)
    {
        _attendanceBlo = attendanceBlo;
        _classBlo = classBlo;

        LoadClassesCommand = new AsyncRelayCommand(LoadClassesAsync);
        LoadAttendanceCommand = new AsyncRelayCommand(LoadAttendanceAsync);
        CreateAttendanceCommand = new AsyncRelayCommand(CreateAttendanceAsync);
        SaveAttendanceCommand = new AsyncRelayCommand(SaveAttendanceAsync);

        // Auto-load classes
        _ = LoadClassesAsync(null);
    }

    public ObservableCollection<Attendance> Attendances
    {
        get => _attendances;
        set => SetProperty(ref _attendances, value);
    }

    public ObservableCollection<Class> Classes
    {
        get => _classes;
        set => SetProperty(ref _classes, value);
    }

    public List<StatusItem> StatusList { get; } = new()
    {
        new StatusItem { Value = "Present", Display = "Có mặt" },
        new StatusItem { Value = "Absent", Display = "Vắng" },
        new StatusItem { Value = "Excused", Display = "Vắng có phép" },
        new StatusItem { Value = "Late", Display = "Muộn" }
    };

    public Class? SelectedClass
    {
        get => _selectedClass;
        set
        {
            if (SetProperty(ref _selectedClass, value))
            {
                _ = LoadAttendanceAsync(null);
            }
        }
    }

    public DateTime SelectedDate
    {
        get => _selectedDate;
        set
        {
            if (SetProperty(ref _selectedDate, value))
            {
                _ = LoadAttendanceAsync(null);
            }
        }
    }

    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    public ICommand LoadClassesCommand { get; }
    public ICommand LoadAttendanceCommand { get; }
    public ICommand CreateAttendanceCommand { get; }
    public ICommand SaveAttendanceCommand { get; }

    private async Task LoadClassesAsync(object? parameter)
    {
        try
        {
            var classes = await _classBlo.GetAllAsync();
            Classes = new ObservableCollection<Class>(classes);

            // Tự động chọn lớp đầu tiên và load dữ liệu điểm danh
            if (Classes.Count > 0 && SelectedClass == null)
            {
                SelectedClass = Classes.First();
            }
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show($"Lỗi tải danh sách lớp: {ex.Message}", "Lỗi",
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
        }
    }

    private async Task LoadAttendanceAsync(object? parameter)
    {
        if (SelectedClass == null) return;

        try
        {
            IsLoading = true;
            var attendances = await _attendanceBlo.GetByClassIdAsync(SelectedClass.Id, SelectedDate);
            Attendances = new ObservableCollection<Attendance>(attendances);
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show($"Lỗi tải điểm danh: {ex.Message}", "Lỗi",
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task CreateAttendanceAsync(object? parameter)
    {
        if (SelectedClass == null)
        {
            System.Windows.MessageBox.Show("Vui lòng chọn lớp học!", "Thông báo",
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
            return;
        }

        try
        {
            IsLoading = true;
            var currentCount = Attendances.Count;
            var allAttendances = await _attendanceBlo.CreateClassAttendanceAsync(SelectedClass.Id, SelectedDate);
            var newCount = allAttendances.Count();
            var createdCount = newCount - currentCount;

            // Cập nhật collection để UI refresh
            Attendances = new ObservableCollection<Attendance>(allAttendances);

            // Thông báo chi tiết với số lượng điểm danh được tạo
            var message = createdCount > 0
                ? $"✓ Tạo thành công {createdCount} bảng điểm danh cho lớp {SelectedClass.Name} ngày {SelectedDate:dd/MM/yyyy}.\n" +
                  $"  Phiếu ăn đã được tạo tự động cho các học sinh có mặt."
                : $"ℹ Bảng điểm danh cho lớp {SelectedClass.Name} ngày {SelectedDate:dd/MM/yyyy} đã tồn tại.\n" +
                  $"  Đã tải lên để bạn tiếp tục chỉnh sửa.";

            System.Windows.MessageBox.Show(
                message,
                "Thành công",
                System.Windows.MessageBoxButton.OK,
                System.Windows.MessageBoxImage.Information);
        }
        catch (InvalidOperationException ex)
        {
            // Trong trường hợp logic cũ còn sót lại ở nơi khác ném lỗi "đã tồn tại",
            // chuyển sang tải dữ liệu để người dùng tiếp tục làm việc
            await LoadAttendanceAsync(null);
            System.Windows.MessageBox.Show(
                $"{ex.Message}\nĐã tải bảng điểm danh hiện có để bạn tiếp tục chỉnh sửa.",
                "Thông báo",
                System.Windows.MessageBoxButton.OK,
                System.Windows.MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show($"Lỗi tạo điểm danh: {ex.Message}", "Lỗi",
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task SaveAttendanceAsync(object? parameter)
    {
        if (!Attendances.Any())
        {
            System.Windows.MessageBox.Show("Không có điểm danh nào để lưu!", "Thông báo",
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
            return;
        }

        try
        {
            IsLoading = true;
            foreach (var attendance in Attendances)
            {
                await _attendanceBlo.UpdateAsync(attendance);
            }

            System.Windows.MessageBox.Show("Lưu điểm danh thành công!", "Thành công",
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show($"Lỗi lưu điểm danh: {ex.Message}", "Lỗi",
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
        }
        finally
        {
            IsLoading = false;
        }
    }
}

public class HealthRecordViewModel : ViewModelBase
{
    private readonly IHealthRecordBlo _healthRecordBlo;

    public HealthRecordViewModel(IHealthRecordBlo healthRecordBlo)
    {
        _healthRecordBlo = healthRecordBlo;
    }
}

public class MenuManagementViewModel : ViewModelBase
{
    private readonly IMenuBlo _menuBlo;

    public MenuManagementViewModel(IMenuBlo menuBlo)
    {
        _menuBlo = menuBlo;
    }
}

public class InvoiceManagementViewModel : ViewModelBase
{
    private readonly IInvoiceBlo _invoiceBlo;
    private readonly IClassBlo _classBlo;
    private readonly IStudentBlo _studentBlo;
    private readonly IAttendanceBlo _attendanceBlo;
    private readonly IUserBlo _userBlo;

    private ObservableCollection<Invoice> _invoices = new();
    private ObservableCollection<Invoice> _tuitionInvoices = new();
    private ObservableCollection<Invoice> _salaryInvoices = new();
    private ObservableCollection<Invoice> _otherInvoices = new();
    private ObservableCollection<Class> _classes = new();
    private ObservableCollection<User> _users = new();
    private Invoice? _selectedInvoice;
    private Class? _selectedClass;
    private Class? _selectedClassFilter;
    private DateTime _invoiceMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
    private bool _isLoading;

    public InvoiceManagementViewModel(IInvoiceBlo invoiceBlo, IClassBlo classBlo, IStudentBlo studentBlo, IAttendanceBlo attendanceBlo, IUserBlo userBlo)
    {
        _invoiceBlo = invoiceBlo;
        _classBlo = classBlo;
        _studentBlo = studentBlo;
        _attendanceBlo = attendanceBlo;
        _userBlo = userBlo;

        LoadDataCommand = new AsyncRelayCommand(LoadDataAsync);
        CreateInvoiceCommand = new AsyncRelayCommand(CreateInvoiceAsync);
        SaveInvoiceCommand = new AsyncRelayCommand(SaveInvoiceAsync);
        DeleteInvoiceCommand = new AsyncRelayCommand(DeleteInvoiceAsync);
        CreateBatchTuitionInvoicesCommand = new AsyncRelayCommand(CreateBatchTuitionInvoicesAsync);
        CreateBatchSalaryInvoicesCommand = new AsyncRelayCommand(CreateBatchSalaryInvoicesAsync);

        // Auto load
        _ = LoadDataAsync(null);
    }

    public ObservableCollection<Invoice> Invoices
    {
        get => _invoices;
        set => SetProperty(ref _invoices, value);
    }

    public ObservableCollection<Invoice> TuitionInvoices
    {
        get => _tuitionInvoices;
        set => SetProperty(ref _tuitionInvoices, value);
    }

    public ObservableCollection<Invoice> SalaryInvoices
    {
        get => _salaryInvoices;
        set => SetProperty(ref _salaryInvoices, value);
    }

    public ObservableCollection<Invoice> OtherInvoices
    {
        get => _otherInvoices;
        set => SetProperty(ref _otherInvoices, value);
    }

    public ObservableCollection<Class> Classes
    {
        get => _classes;
        set => SetProperty(ref _classes, value);
    }

    public ObservableCollection<User> Users
    {
        get => _users;
        set => SetProperty(ref _users, value);
    }

    public Invoice? SelectedInvoice
    {
        get => _selectedInvoice;
        set => SetProperty(ref _selectedInvoice, value);
    }

    public Class? SelectedClass
    {
        get => _selectedClass;
        set => SetProperty(ref _selectedClass, value);
    }

    public Class? SelectedClassFilter
    {
        get => _selectedClassFilter;
        set
        {
            if (SetProperty(ref _selectedClassFilter, value))
            {
                FilterInvoicesByType();
            }
        }
    }

    public DateTime InvoiceMonth
    {
        get => _invoiceMonth;
        set => SetProperty(ref _invoiceMonth, value);
    }

    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    public List<string> InvoiceStatuses { get; } = new() { "Tất cả", "Chưa thanh toán", "Đã thanh toán", "Quá hạn", "Hủy" };
    public List<string> InvoiceTypes { get; } = new() { "Tất cả", "Học phí", "Lương", "Sửa chữa", "Khác" };
    public List<string> OtherInvoiceTypes { get; } = new() { "Sửa chữa", "Bảo trì", "Khác", "Đơn vị ngoài", "Phí dịch vụ" };

    public ICommand LoadDataCommand { get; }
    public ICommand CreateInvoiceCommand { get; }
    public ICommand SaveInvoiceCommand { get; }
    public ICommand DeleteInvoiceCommand { get; }
    public ICommand CreateBatchTuitionInvoicesCommand { get; }
    public ICommand CreateBatchSalaryInvoicesCommand { get; }

    private async Task LoadDataAsync(object? parameter)
    {
        try
        {
            IsLoading = true;
            var all = await _invoiceBlo.GetAllAsync();
            Invoices = new ObservableCollection<Invoice>(all ?? Enumerable.Empty<Invoice>());

            var classes = await _classBlo.GetAllAsync();
            Classes = new ObservableCollection<Class>(classes ?? Enumerable.Empty<Class>());

            var users = await _userBlo.GetAllAsync();
            Users = new ObservableCollection<User>(users ?? Enumerable.Empty<User>());

            FilterInvoicesByType();
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show($"Lỗi tải hóa đơn: {ex.Message}", "Lỗi", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
        }
        finally
        {
            IsLoading = false;
        }
    }

    private Task CreateInvoiceAsync(object? parameter)
    {
        SelectedInvoice = new Invoice
        {
            InvoiceNumber = GenerateInvoiceNumber("HD"),
            Type = "Học phí",
            Amount = 0,
            IssueDate = DateTime.Today,
            DueDate = DateTime.Today.AddDays(7),
            Status = "Chưa thanh toán",
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };
        System.Windows.MessageBox.Show("Hãy điền thông tin và bấm Lưu để lưu hóa đơn", "Thông báo", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
        return Task.CompletedTask;
    }

    private async Task SaveInvoiceAsync(object? parameter)
    {
        if (SelectedInvoice == null)
        {
            System.Windows.MessageBox.Show("Vui lòng chọn hóa đơn!", "Thông báo", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
            return;
        }

        if (SelectedInvoice.Amount <= 0)
        {
            System.Windows.MessageBox.Show("Số tiền phải lớn hơn 0", "Thông báo", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
            return;
        }

        try
        {
            IsLoading = true;
            if (SelectedInvoice.Id == Guid.Empty)
            {
                var created = await _invoiceBlo.CreateAsync(SelectedInvoice);
                Invoices.Insert(0, created);
                SelectedInvoice = created;
                System.Windows.MessageBox.Show("Tạo hóa đơn thành công", "Thành công", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
            }
            else
            {
                await _invoiceBlo.UpdateAsync(SelectedInvoice);
                System.Windows.MessageBox.Show("Cập nhật hóa đơn thành công", "Thành công", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
            }
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show($"Lỗi lưu hóa đơn: {ex.Message}", "Lỗi", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
        }
        finally
        {
            FilterInvoicesByType();
            IsLoading = false;
        }
    }

    private async Task DeleteInvoiceAsync(object? parameter)
    {
        if (SelectedInvoice == null)
        {
            System.Windows.MessageBox.Show("Vui lòng chọn hóa đơn để xóa", "Thông báo", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
            return;
        }

        var confirm = System.Windows.MessageBox.Show($"Bạn có chắc chắn xóa hóa đơn {SelectedInvoice.InvoiceNumber}?", "Xác nhận", System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Question);
        if (confirm != System.Windows.MessageBoxResult.Yes) return;

        try
        {
            IsLoading = true;
            await _invoiceBlo.DeleteAsync(SelectedInvoice.Id);
            Invoices.Remove(SelectedInvoice);
            SelectedInvoice = null;
            System.Windows.MessageBox.Show("Đã xóa hóa đơn", "Thành công", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show($"Lỗi xóa hóa đơn: {ex.Message}", "Lỗi", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
        }
        finally
        {
            FilterInvoicesByType();
            IsLoading = false;
        }
    }

    private void FilterInvoicesByType()
    {
        try
        {
            var tuitionInvoices = Invoices.Where(i => i.Type == "Học phí");
            
            // Apply class filter if selected
            if (SelectedClassFilter != null)
            {
                tuitionInvoices = tuitionInvoices.Where(i => i.ClassId == SelectedClassFilter.Id);
            }
            
            TuitionInvoices = new ObservableCollection<Invoice>(tuitionInvoices);
            SalaryInvoices = new ObservableCollection<Invoice>(Invoices.Where(i => i.Type == "Lương"));
            OtherInvoices = new ObservableCollection<Invoice>(Invoices.Where(i => i.Type != "Học phí" && i.Type != "Lương"));
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"FilterInvoicesByType error: {ex}");
        }
    }

    private async Task CreateBatchTuitionInvoicesAsync(object? parameter)
    {
        try
        {
            IsLoading = true;

            var allClasses = await _classBlo.GetAllAsync();
            if (allClasses == null || !allClasses.Any())
            {
                System.Windows.MessageBox.Show("Không có lớp nào trong hệ thống.", "Thông báo", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
                return;
            }

            int totalCreatedCount = 0;

            foreach (var selectedClass in allClasses)
            {
                var students = await _studentBlo.GetByClassIdAsync(selectedClass.Id);
                if (students == null || !students.Any())
                {
                    continue; // Skip classes with no students
                }

                int createdCount = 0;

                foreach (var student in students)
                {
                    // Bỏ qua nếu đã có hóa đơn học phí trong tháng này cho học sinh
                    bool exists = Invoices.Any(i => i.Type == "Học phí"
                        && i.StudentId == student.Id
                        && i.IssueDate.Year == InvoiceMonth.Year
                        && i.IssueDate.Month == InvoiceMonth.Month);
                    if (exists) continue;

                    var absences = await _attendanceBlo.GetByStudentIdAsync(student.Id);
                    var monthAbsences = absences
                        .Where(a => a.Date.Year == InvoiceMonth.Year && a.Date.Month == InvoiceMonth.Month && a.IsExcusedAbsence)
                        .ToList();

                    var prevMonth = InvoiceMonth.AddMonths(-1);
                    var prevMonthAbsences = absences
                        .Where(a => a.Date.Year == prevMonth.Year && a.Date.Month == prevMonth.Month && a.IsExcusedAbsence)
                        .ToList();

                    decimal totalMealRefund = monthAbsences.Sum(a => a.DailyMealRefund);
                    decimal previousBalance = prevMonthAbsences.Sum(a => a.DailyMealRefund);
                    decimal baseTuition = selectedClass.TuitionFee;
                    decimal mealFee = selectedClass.MealFee;

                    decimal totalAmount = baseTuition + mealFee - totalMealRefund - previousBalance;

                    var invoice = new Invoice
                    {
                        InvoiceNumber = GenerateInvoiceNumber($"HP-{SanitizeCode(selectedClass.Name)}-{InvoiceMonth:yyyyMM}"),
                        Type = "Học phí",
                        Amount = Math.Max(totalAmount, 0),
                        MonthlyTuitionFee = baseTuition,
                        MealBalanceFromPreviousMonth = previousBalance,
                    FinalAmount = totalAmount,
                    IssueDate = InvoiceMonth,
                    DueDate = InvoiceMonth.AddDays(7),
                    Status = "Chưa thanh toán",
                    StudentId = student.Id,
                    ClassId = selectedClass.Id,
                    Description = $"Học phí tháng {InvoiceMonth:MM/yyyy} - Trừ {monthAbsences.Count} ngày nghỉ có phép"
                    };

                    var created = await _invoiceBlo.CreateAsync(invoice);
                    Invoices.Insert(0, created);
                    createdCount++;
                }

                totalCreatedCount += createdCount;
            }

            FilterInvoicesByType();

            System.Windows.MessageBox.Show($"Đã tạo {totalCreatedCount} hóa đơn học phí cho tất cả các lớp trong tháng {InvoiceMonth:MM/yyyy}.", "Thành công", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show($"Lỗi tạo hóa đơn hàng loạt: {ex.Message}", "Lỗi", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
        }
        finally
        {
            IsLoading = false;
        }
    }

    private string GenerateInvoiceNumber(string prefix)
    {
        string number;
        do
        {
            number = $"{prefix}-{DateTime.Now:yyyyMMddHHmmssfff}-{Guid.NewGuid():N}";
        } while (Invoices.Any(i => i.InvoiceNumber == number));

        return number;
    }

    private async Task CreateBatchSalaryInvoicesAsync(object? parameter)
    {
        try
        {
            IsLoading = true;

            var staffMembers = Users.Where(u => u.Salary.HasValue || u.Allowance.HasValue).ToList();
            if (!staffMembers.Any())
            {
                System.Windows.MessageBox.Show("Không có nhân viên nào có lương hoặc phụ cấp.", "Thông báo", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
                return;
            }

            int createdCount = 0;

            foreach (var staff in staffMembers)
            {
                // Bỏ qua nếu đã có phiếu lương trong tháng này
                bool exists = Invoices.Any(i => i.Type == "Lương"
                    && i.UserId == staff.Id
                    && i.IssueDate.Year == InvoiceMonth.Year
                    && i.IssueDate.Month == InvoiceMonth.Month);
                if (exists) continue;

                decimal baseSalary = staff.Salary ?? 0;
                decimal allowance = staff.Allowance ?? 0;
                decimal totalAmount = baseSalary + allowance;

                var invoice = new Invoice
                {
                    InvoiceNumber = GenerateInvoiceNumber($"PS-{SanitizeCode(staff.FullName)}-{InvoiceMonth:yyyyMM}"),
                    Type = "Lương",
                    Amount = totalAmount,
                    MonthlyTuitionFee = baseSalary,
                    MealBalanceFromPreviousMonth = allowance,
                    FinalAmount = totalAmount,
                    IssueDate = InvoiceMonth,
                    DueDate = InvoiceMonth.AddDays(0), // Same day
                    Status = "Chưa thanh toán",
                    UserId = staff.Id,
                    Description = $"Phiếu lương tháng {InvoiceMonth:MM/yyyy} - {staff.Position} - Lương: {baseSalary:N0} + Phụ cấp: {allowance:N0}"
                };

                var created = await _invoiceBlo.CreateAsync(invoice);
                Invoices.Insert(0, created);
                createdCount++;
            }

            FilterInvoicesByType();

            System.Windows.MessageBox.Show($"Đã tạo {createdCount} phiếu lương cho tháng {InvoiceMonth:MM/yyyy}.", "Thành công", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show($"Lỗi tạo phiếu lương hàng loạt: {ex.Message}", "Lỗi", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
        }
        finally
        {
            IsLoading = false;
        }
    }

    private static string SanitizeCode(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return "UNK";
        var cleaned = Regex.Replace(input, "[^A-Za-z0-9]", "");
        return string.IsNullOrWhiteSpace(cleaned) ? "UNK" : cleaned.ToUpperInvariant();
    }
}

public class StatusItem
{
    public string Value { get; set; } = string.Empty;
    public string Display { get; set; } = string.Empty;
}
