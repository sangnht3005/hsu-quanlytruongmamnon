using System.Collections.ObjectModel;
using System.Windows.Input;
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

    public InvoiceManagementViewModel(IInvoiceBlo invoiceBlo)
    {
        _invoiceBlo = invoiceBlo;
    }
}

public class StatusItem
{
    public string Value { get; set; } = string.Empty;
    public string Display { get; set; } = string.Empty;
}
