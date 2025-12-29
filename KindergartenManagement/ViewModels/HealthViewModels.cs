using System.Collections.ObjectModel;
using System.Windows.Input;
using KindergartenManagement.DTO;
using KindergartenManagement.BLO;
using KindergartenManagement.DAO;

namespace KindergartenManagement.ViewModels;

// Vaccine Management ViewModel
public class VaccineManagementViewModel : ViewModelBase
{
    private readonly IVaccineBlo _vaccineBlo;
    
    private ObservableCollection<Vaccine> _vaccines = new();
    private Vaccine? _selectedVaccine;
    private bool _isLoading;

    public VaccineManagementViewModel(IVaccineBlo vaccineBlo)
    {
        _vaccineBlo = vaccineBlo;

        LoadDataCommand = new AsyncRelayCommand(LoadDataAsync);
        AddVaccineCommand = new RelayCommand(AddVaccine);
        SaveVaccineCommand = new AsyncRelayCommand(SaveVaccineAsync);
        DeleteVaccineCommand = new AsyncRelayCommand(DeleteVaccineAsync, CanDelete);

        SelectedVaccine = new Vaccine
        {
            Id = Guid.Empty,
            Name = string.Empty,
            Description = string.Empty,
            RequiredAgeMonths = 0,
            IsMandatory = true
        };

        _ = LoadDataAsync(null);
    }

    public ObservableCollection<Vaccine> Vaccines
    {
        get => _vaccines;
        set => SetProperty(ref _vaccines, value);
    }

    public Vaccine? SelectedVaccine
    {
        get => _selectedVaccine;
        set
        {
            SetProperty(ref _selectedVaccine, value);
            // Nếu bỏ chọn (click ra ngoài), clear form
            if (value == null)
            {
                _selectedVaccine = new Vaccine
                {
                    Id = Guid.Empty,
                    Name = string.Empty,
                    Description = string.Empty,
                    RequiredAgeMonths = 0,
                    IsMandatory = true
                };
                OnPropertyChanged(nameof(SelectedVaccine));
            }
        }
    }

    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    public ICommand LoadDataCommand { get; }
    public ICommand AddVaccineCommand { get; }
    public ICommand SaveVaccineCommand { get; }
    public ICommand DeleteVaccineCommand { get; }

    private async Task LoadDataAsync(object? parameter)
    {
        try
        {
            IsLoading = true;
            var vaccines = await _vaccineBlo.GetAllAsync();
            Vaccines = new ObservableCollection<Vaccine>(vaccines);
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

    private void AddVaccine(object? parameter)
    {
        SelectedVaccine = new Vaccine
        {
            Id = Guid.Empty,
            Name = string.Empty,
            Description = string.Empty,
            RequiredAgeMonths = 0,
            IsMandatory = true
        };
    }

    private bool CanDelete(object? parameter)
    {
        return SelectedVaccine != null && SelectedVaccine.Id != Guid.Empty;
    }

    private bool ValidateVaccine()
    {
        if (SelectedVaccine == null) return false;

        if (string.IsNullOrWhiteSpace(SelectedVaccine.Name))
        {
            System.Windows.MessageBox.Show("Vui lòng nhập tên vaccine!", "Lỗi validation",
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
            return false;
        }

        if (SelectedVaccine.RequiredAgeMonths < 0)
        {
            System.Windows.MessageBox.Show("Tuổi yêu cầu phải lớn hơn hoặc bằng 0!", "Lỗi validation",
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
            return false;
        }

        return true;
    }

    private async Task SaveVaccineAsync(object? parameter)
    {
        if (SelectedVaccine == null) return;

        if (!ValidateVaccine())
        {
            return;
        }

        try
        {
            if (SelectedVaccine.Id == Guid.Empty)
            {
                await _vaccineBlo.CreateAsync(SelectedVaccine);
            }
            else
            {
                await _vaccineBlo.UpdateAsync(SelectedVaccine);
            }

            System.Windows.MessageBox.Show("Lưu vaccine thành công!", "Thành công",
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);

            await LoadDataAsync(null);

            SelectedVaccine = new Vaccine
            {
                Id = Guid.Empty,
                Name = string.Empty,
                Description = string.Empty,
                RequiredAgeMonths = 0,
                IsMandatory = true
            };
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi",
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
        }
    }

    private async Task DeleteVaccineAsync(object? parameter)
    {
        if (SelectedVaccine == null) return;

        var result = System.Windows.MessageBox.Show(
            $"Bạn có chắc chắn muốn xóa vaccine {SelectedVaccine.Name}?",
            "Xác nhận",
            System.Windows.MessageBoxButton.YesNo,
            System.Windows.MessageBoxImage.Question);

        if (result == System.Windows.MessageBoxResult.Yes)
        {
            try
            {
                await _vaccineBlo.DeleteAsync(SelectedVaccine.Id);
                Vaccines.Remove(SelectedVaccine);
                SelectedVaccine = null;

                System.Windows.MessageBox.Show("Xóa vaccine thành công!", "Thành công",
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

// VaccinationRecord Management ViewModel
public class VaccinationRecordManagementViewModel : ViewModelBase
{
    private readonly IVaccinationRecordBlo _vaccinationRecordBlo;
    private readonly IStudentDao _studentDao;
    private readonly IVaccineDao _vaccineDao;

    private ObservableCollection<VaccinationRecord> _vaccinationRecords = new();
    private ObservableCollection<VaccinationRecord> _filteredRecords = new();
    private ObservableCollection<Student> _students = new();
    private ObservableCollection<Vaccine> _vaccines = new();
    private VaccinationRecord? _selectedRecord;
    private bool _isLoading;
    private Guid? _filterStudentId;
    private Guid? _filterVaccineId;

    public VaccinationRecordManagementViewModel(IVaccinationRecordBlo vaccinationRecordBlo, IStudentDao studentDao, IVaccineDao vaccineDao)
    {
        _vaccinationRecordBlo = vaccinationRecordBlo;
        _studentDao = studentDao;
        _vaccineDao = vaccineDao;

        LoadDataCommand = new AsyncRelayCommand(LoadDataAsync);
        AddRecordCommand = new RelayCommand(AddRecord);
        SaveRecordCommand = new AsyncRelayCommand(SaveRecordAsync);
        DeleteRecordCommand = new AsyncRelayCommand(DeleteRecordAsync, CanDelete);
        ApplyFilterCommand = new RelayCommand(ApplyFilter);
        ClearFilterCommand = new RelayCommand(ClearFilter);

        SelectedRecord = new VaccinationRecord
        {
            Id = Guid.Empty,
            StudentId = Guid.Empty,
            VaccineId = Guid.Empty,
            Status = "Not Done"
        };

        _ = LoadDataAsync(null);
    }

    public ObservableCollection<VaccinationRecord> VaccinationRecords
    {
        get => _vaccinationRecords;
        set => SetProperty(ref _vaccinationRecords, value);
    }

    public ObservableCollection<VaccinationRecord> FilteredRecords
    {
        get => _filteredRecords;
        set => SetProperty(ref _filteredRecords, value);
    }

    public ObservableCollection<Student> Students
    {
        get => _students;
        set => SetProperty(ref _students, value);
    }

    public ObservableCollection<Vaccine> Vaccines
    {
        get => _vaccines;
        set => SetProperty(ref _vaccines, value);
    }

    public VaccinationRecord? SelectedRecord
    {
        get => _selectedRecord;
        set
        {
            SetProperty(ref _selectedRecord, value);
            // Nếu bỏ chọn (click ra ngoài), clear form
            if (value == null)
            {
                _selectedRecord = new VaccinationRecord
                {
                    Id = Guid.Empty,
                    StudentId = Guid.Empty,
                    VaccineId = Guid.Empty,
                    Status = "Not Done"
                };
                OnPropertyChanged(nameof(SelectedRecord));
            }
        }
    }

    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    public Guid? FilterStudentId
    {
        get => _filterStudentId;
        set => SetProperty(ref _filterStudentId, value);
    }

    public Guid? FilterVaccineId
    {
        get => _filterVaccineId;
        set => SetProperty(ref _filterVaccineId, value);
    }

    public ICommand LoadDataCommand { get; }
    public ICommand AddRecordCommand { get; }
    public ICommand SaveRecordCommand { get; }
    public ICommand DeleteRecordCommand { get; }
    public ICommand ApplyFilterCommand { get; }
    public ICommand ClearFilterCommand { get; }

    private async Task LoadDataAsync(object? parameter)
    {
        try
        {
            IsLoading = true;

            var records = await _vaccinationRecordBlo.GetAllAsync();
            VaccinationRecords = new ObservableCollection<VaccinationRecord>(records);
            FilteredRecords = new ObservableCollection<VaccinationRecord>(records);

            var students = await _studentDao.GetAllAsync();
            Students = new ObservableCollection<Student>(students);

            var vaccines = await _vaccineDao.GetAllAsync();
            Vaccines = new ObservableCollection<Vaccine>(vaccines);
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

    private void AddRecord(object? parameter)
    {
        SelectedRecord = new VaccinationRecord
        {
            Id = Guid.Empty,
            StudentId = Guid.Empty,
            VaccineId = Guid.Empty,
            Status = "Not Done"
        };
    }

    private bool CanDelete(object? parameter)
    {
        return SelectedRecord != null && SelectedRecord.Id != Guid.Empty;
    }

    private bool ValidateRecord()
    {
        if (SelectedRecord == null) return false;

        if (SelectedRecord.StudentId == Guid.Empty)
        {
            System.Windows.MessageBox.Show("Vui lòng chọn học sinh!", "Lỗi validation",
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
            return false;
        }

        if (SelectedRecord.VaccineId == Guid.Empty)
        {
            System.Windows.MessageBox.Show("Vui lòng chọn vaccine!", "Lỗi validation",
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
            return false;
        }

        return true;
    }

    private async Task SaveRecordAsync(object? parameter)
    {
        if (SelectedRecord == null) return;

        if (!ValidateRecord())
        {
            return;
        }

        try
        {
            if (SelectedRecord.Id == Guid.Empty)
            {
                await _vaccinationRecordBlo.CreateAsync(SelectedRecord);
            }
            else
            {
                await _vaccinationRecordBlo.UpdateAsync(SelectedRecord);
            }

            System.Windows.MessageBox.Show("Lưu hồ sơ tiêm chủng thành công!", "Thành công",
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);

            await LoadDataAsync(null);

            SelectedRecord = new VaccinationRecord
            {
                Id = Guid.Empty,
                StudentId = Guid.Empty,
                VaccineId = Guid.Empty,
                Status = "Not Done"
            };
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi",
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
        }
    }

    private async Task DeleteRecordAsync(object? parameter)
    {
        if (SelectedRecord == null) return;

        var result = System.Windows.MessageBox.Show(
            "Bạn có chắc chắn muốn xóa hồ sơ tiêm chủng này?",
            "Xác nhận",
            System.Windows.MessageBoxButton.YesNo,
            System.Windows.MessageBoxImage.Question);

        if (result == System.Windows.MessageBoxResult.Yes)
        {
            try
            {
                await _vaccinationRecordBlo.DeleteAsync(SelectedRecord.Id);
                VaccinationRecords.Remove(SelectedRecord);
                SelectedRecord = null;

                System.Windows.MessageBox.Show("Xóa hồ sơ tiêm chủng thành công!", "Thành công",
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi",
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }
    }

    private void ApplyFilter(object? parameter)
    {
        var filtered = VaccinationRecords.AsEnumerable();

        if (FilterStudentId.HasValue && FilterStudentId.Value != Guid.Empty)
        {
            filtered = filtered.Where(r => r.StudentId == FilterStudentId.Value);
        }

        if (FilterVaccineId.HasValue && FilterVaccineId.Value != Guid.Empty)
        {
            filtered = filtered.Where(r => r.VaccineId == FilterVaccineId.Value);
        }

        FilteredRecords = new ObservableCollection<VaccinationRecord>(filtered);
    }

    private void ClearFilter(object? parameter)
    {
        FilterStudentId = null;
        FilterVaccineId = null;
        FilteredRecords = new ObservableCollection<VaccinationRecord>(VaccinationRecords);
    }
}

// Health Record Management ViewModel
public class HealthRecordManagementViewModel : ViewModelBase
{
    private readonly IHealthRecordBlo _healthRecordBlo;
    private readonly IStudentDao _studentDao;
    private readonly IClassDao _classDao;

    private ObservableCollection<HealthRecord> _healthRecords = new();
    private ObservableCollection<HealthRecord> _filteredRecords = new();
    private ObservableCollection<Student> _students = new();
    private ObservableCollection<Student> _filteredStudents = new();
    private ObservableCollection<Class> _classes = new();
    private HealthRecord? _selectedRecord;
    private Student? _selectedStudent;
    private bool _isLoading;
    private Guid? _filterClassId;
    private Guid? _filterStudentId;

    public HealthRecordManagementViewModel(IHealthRecordBlo healthRecordBlo, IStudentDao studentDao, IClassDao classDao)
    {
        _healthRecordBlo = healthRecordBlo;
        _studentDao = studentDao;
        _classDao = classDao;

        LoadDataCommand = new AsyncRelayCommand(LoadDataAsync);
        AddRecordCommand = new RelayCommand(AddRecord);
        SaveRecordCommand = new AsyncRelayCommand(SaveRecordAsync);
        DeleteRecordCommand = new AsyncRelayCommand(DeleteRecordAsync, CanDelete);
        ApplyFilterCommand = new RelayCommand(ApplyFilter);
        ClearFilterCommand = new RelayCommand(ClearFilter);

        SelectedRecord = new HealthRecord
        {
            Id = Guid.Empty,
            StudentId = Guid.Empty,
            Month = DateTime.Now.Month,
            Year = DateTime.Now.Year
        };

        _ = LoadDataAsync(null);
    }

    public ObservableCollection<HealthRecord> HealthRecords
    {
        get => _healthRecords;
        set => SetProperty(ref _healthRecords, value);
    }

    public ObservableCollection<HealthRecord> FilteredRecords
    {
        get => _filteredRecords;
        set => SetProperty(ref _filteredRecords, value);
    }

    public ObservableCollection<Student> Students
    {
        get => _students;
        set => SetProperty(ref _students, value);
    }

    public ObservableCollection<Student> FilteredStudents
    {
        get => _filteredStudents;
        set => SetProperty(ref _filteredStudents, value);
    }

    public ObservableCollection<Class> Classes
    {
        get => _classes;
        set => SetProperty(ref _classes, value);
    }

    public HealthRecord? SelectedRecord
    {
        get => _selectedRecord;
        set
        {
            SetProperty(ref _selectedRecord, value);
            // Nếu bỏ chọn (click ra ngoài), clear form
            if (value == null)
            {
                _selectedRecord = new HealthRecord
                {
                    Id = Guid.Empty,
                    StudentId = Guid.Empty,
                    Month = DateTime.Now.Month,
                    Year = DateTime.Now.Year
                };
                OnPropertyChanged(nameof(SelectedRecord));
            }
        }
    }

    public Student? SelectedStudent
    {
        get => _selectedStudent;
        set
        {
            if (SetProperty(ref _selectedStudent, value))
            {
                // Khi chọn học sinh, tự động lọc hồ sơ sức khỏe của học sinh đó
                if (_selectedStudent != null)
                {
                    FilterStudentId = _selectedStudent.Id;
                    ApplyFilter(null);
                }
            }
        }
    }

    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    public Guid? FilterClassId
    {
        get => _filterClassId;
        set => SetProperty(ref _filterClassId, value);
    }

    public Guid? FilterStudentId
    {
        get => _filterStudentId;
        set => SetProperty(ref _filterStudentId, value);
    }

    public ICommand LoadDataCommand { get; }
    public ICommand AddRecordCommand { get; }
    public ICommand SaveRecordCommand { get; }
    public ICommand DeleteRecordCommand { get; }
    public ICommand ApplyFilterCommand { get; }
    public ICommand ClearFilterCommand { get; }

    private async Task LoadDataAsync(object? parameter)
    {
        try
        {
            IsLoading = true;

            var records = await _healthRecordBlo.GetAllAsync();
            HealthRecords = new ObservableCollection<HealthRecord>(records);
            FilteredRecords = new ObservableCollection<HealthRecord>(records);

            var students = await _studentDao.GetAllAsync();
            Students = new ObservableCollection<Student>(students);

            var classes = await _classDao.GetAllAsync();
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

    private void AddRecord(object? parameter)
    {
        SelectedRecord = new HealthRecord
        {
            Id = Guid.Empty,
            StudentId = Guid.Empty,
            Month = DateTime.Now.Month,
            Year = DateTime.Now.Year
        };
    }

    private bool CanDelete(object? parameter)
    {
        return SelectedRecord != null && SelectedRecord.Id != Guid.Empty;
    }

    private bool ValidateRecord()
    {
        if (SelectedRecord == null) return false;

        if (SelectedRecord.StudentId == Guid.Empty)
        {
            System.Windows.MessageBox.Show("Vui lòng chọn học sinh!", "Lỗi validation",
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
            return false;
        }

        if (SelectedRecord.Month < 1 || SelectedRecord.Month > 12)
        {
            System.Windows.MessageBox.Show("Tháng phải từ 1 đến 12!", "Lỗi validation",
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
            return false;
        }

        if (SelectedRecord.Year < 2000 || SelectedRecord.Year > DateTime.Now.Year)
        {
            System.Windows.MessageBox.Show($"Năm phải từ 2000 đến {DateTime.Now.Year}!", "Lỗi validation",
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
            return false;
        }

        if (SelectedRecord.Height.HasValue && SelectedRecord.Height.Value <= 0)
        {
            System.Windows.MessageBox.Show("Chiều cao phải lớn hơn 0!", "Lỗi validation",
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
            return false;
        }

        if (SelectedRecord.Weight.HasValue && SelectedRecord.Weight.Value <= 0)
        {
            System.Windows.MessageBox.Show("Cân nặng phải lớn hơn 0!", "Lỗi validation",
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
            return false;
        }

        return true;
    }

    private async Task SaveRecordAsync(object? parameter)
    {
        if (SelectedRecord == null) return;

        if (!ValidateRecord())
        {
            return;
        }

        try
        {
            if (SelectedRecord.Id == Guid.Empty)
            {
                await _healthRecordBlo.CreateAsync(SelectedRecord);
            }
            else
            {
                await _healthRecordBlo.UpdateAsync(SelectedRecord);
            }

            System.Windows.MessageBox.Show("Lưu hồ sơ sức khỏe thành công!", "Thành công",
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);

            await LoadDataAsync(null);

            SelectedRecord = new HealthRecord
            {
                Id = Guid.Empty,
                StudentId = Guid.Empty,
                Month = DateTime.Now.Month,
                Year = DateTime.Now.Year
            };
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi",
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
        }
    }

    private async Task DeleteRecordAsync(object? parameter)
    {
        if (SelectedRecord == null) return;

        var result = System.Windows.MessageBox.Show(
            "Bạn có chắc chắn muốn xóa hồ sơ sức khỏe này?",
            "Xác nhận",
            System.Windows.MessageBoxButton.YesNo,
            System.Windows.MessageBoxImage.Question);

        if (result == System.Windows.MessageBoxResult.Yes)
        {
            try
            {
                await _healthRecordBlo.DeleteAsync(SelectedRecord.Id);
                HealthRecords.Remove(SelectedRecord);
                SelectedRecord = null;

                System.Windows.MessageBox.Show("Xóa hồ sơ sức khỏe thành công!", "Thành công",
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi",
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }
    }

    private void ApplyFilter(object? parameter)
    {
        // Nếu chọn lớp: Hiển thị danh sách học sinh của lớp đó
        if (FilterClassId.HasValue && FilterClassId.Value != Guid.Empty)
        {
            var studentsInClass = Students.Where(s => s.ClassId == FilterClassId.Value).OrderBy(s => s.FullName);
            FilteredStudents = new ObservableCollection<Student>(studentsInClass);
            
            // Nếu chưa chọn học sinh, hiển thị tất cả hồ sơ của lớp
            if (!FilterStudentId.HasValue || FilterStudentId.Value == Guid.Empty)
            {
                var recordsInClass = HealthRecords.Where(r => r.Student?.ClassId == FilterClassId.Value)
                    .OrderByDescending(r => r.Year)
                    .ThenByDescending(r => r.Month);
                FilteredRecords = new ObservableCollection<HealthRecord>(recordsInClass);
                return;
            }
        }
        else
        {
            // Nếu không chọn lớp, xóa danh sách học sinh đã lọc
            FilteredStudents.Clear();
        }

        // Nếu chọn học sinh: Hiển thị tất cả hồ sơ sức khỏe (hiện tại + quá khứ) của học sinh đó
        if (FilterStudentId.HasValue && FilterStudentId.Value != Guid.Empty)
        {
            var studentRecords = HealthRecords
                .Where(r => r.StudentId == FilterStudentId.Value)
                .OrderByDescending(r => r.Year)
                .ThenByDescending(r => r.Month);
            FilteredRecords = new ObservableCollection<HealthRecord>(studentRecords);
        }
        else
        {
            // Nếu không chọn gì, hiển thị tất cả
            FilteredRecords = new ObservableCollection<HealthRecord>(
                HealthRecords.OrderByDescending(r => r.Year).ThenByDescending(r => r.Month));
        }
    }

    private void ClearFilter(object? parameter)
    {
        FilterClassId = null;
        FilterStudentId = null;
        SelectedStudent = null;
        FilteredStudents.Clear();
        FilteredRecords = new ObservableCollection<HealthRecord>(HealthRecords.OrderByDescending(r => r.Year).ThenByDescending(r => r.Month));
    }
}
