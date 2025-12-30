using System.Collections.ObjectModel;
using System.Windows.Input;
using KindergartenManagement.BLO;
using KindergartenManagement.DTO;

namespace KindergartenManagement.ViewModels;

public class TuitionFeeManagementViewModel : ViewModelBase
{
    private readonly ITuitionFeeBlo _tuitionFeeBlo;
    private readonly IGradeBlo _gradeBlo;

    private ObservableCollection<TuitionFeeDto> _tuitionFees = new();
    private ObservableCollection<Grade> _grades = new();
    private TuitionFeeDto? _selectedTuitionFee;
    private bool _isLoading;

    public TuitionFeeManagementViewModel(ITuitionFeeBlo tuitionFeeBlo, IGradeBlo gradeBlo)
    {
        _tuitionFeeBlo = tuitionFeeBlo;
        _gradeBlo = gradeBlo;

        LoadDataCommand = new AsyncRelayCommand(async _ => await LoadDataAsync());
        AddTuitionFeeCommand = new RelayCommand(_ => AddTuitionFee());
        SaveTuitionFeeCommand = new AsyncRelayCommand(async _ => await SaveTuitionFeeAsync());
        DeleteTuitionFeeCommand = new AsyncRelayCommand(async _ => await DeleteTuitionFeeAsync());

        _ = LoadDataAsync();
    }

    public ObservableCollection<TuitionFeeDto> TuitionFees
    {
        get => _tuitionFees;
        set => SetProperty(ref _tuitionFees, value);
    }

    public ObservableCollection<Grade> Grades
    {
        get => _grades;
        set => SetProperty(ref _grades, value);
    }

    public TuitionFeeDto? SelectedTuitionFee
    {
        get => _selectedTuitionFee;
        set => SetProperty(ref _selectedTuitionFee, value);
    }

    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    public ICommand LoadDataCommand { get; }
    public ICommand AddTuitionFeeCommand { get; }
    public ICommand SaveTuitionFeeCommand { get; }
    public ICommand DeleteTuitionFeeCommand { get; }

    private async Task LoadDataAsync()
    {
        try
        {
            IsLoading = true;

            // Load grades first
            var grades = await _gradeBlo.GetAllAsync();
            if (grades != null)
            {
                Grades = new ObservableCollection<Grade>(grades);
                System.Diagnostics.Debug.WriteLine($"✓ Loaded {Grades.Count} grades");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("✗ Grades collection is null");
                Grades = new ObservableCollection<Grade>();
            }

            // Then load tuition fees
            var tuitionFees = await _tuitionFeeBlo.GetAllAsync();
            if (tuitionFees != null)
            {
                TuitionFees = new ObservableCollection<TuitionFeeDto>(tuitionFees);
                System.Diagnostics.Debug.WriteLine($"✓ Loaded {TuitionFees.Count} tuition fees");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("✗ TuitionFees collection is null");
                TuitionFees = new ObservableCollection<TuitionFeeDto>();
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"✗ Error loading data: {ex.Message}");
            System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
            
            // Show error message
            System.Windows.MessageBox.Show(
                $"Lỗi tải dữ liệu:\n{ex.Message}", 
                "Lỗi",
                System.Windows.MessageBoxButton.OK,
                System.Windows.MessageBoxImage.Error);
            
            Grades = new ObservableCollection<Grade>();
            TuitionFees = new ObservableCollection<TuitionFeeDto>();
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void AddTuitionFee()
    {
        SelectedTuitionFee = new TuitionFeeDto
        {
            Id = Guid.NewGuid(),
            SchoolDaysPerMonth = 20,
            EffectiveDate = DateTime.Now
        };
    }

    private async Task SaveTuitionFeeAsync()
    {
        if (SelectedTuitionFee == null || SelectedTuitionFee.GradeId == Guid.Empty)
            return;

        try
        {
            IsLoading = true;

            if (SelectedTuitionFee.Id == Guid.Empty)
                SelectedTuitionFee.Id = Guid.NewGuid();

            var existing = TuitionFees.FirstOrDefault(t => t.GradeId == SelectedTuitionFee.GradeId && t.Id != SelectedTuitionFee.Id);
            if (existing != null)
            {
                await _tuitionFeeBlo.UpdateAsync(SelectedTuitionFee);
                TuitionFees[TuitionFees.IndexOf(existing)] = SelectedTuitionFee;
            }
            else
            {
                await _tuitionFeeBlo.CreateAsync(SelectedTuitionFee);
                TuitionFees.Add(SelectedTuitionFee);
            }

            SelectedTuitionFee = null;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error saving tuition fee: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task DeleteTuitionFeeAsync()
    {
        if (SelectedTuitionFee == null)
            return;

        try
        {
            IsLoading = true;
            await _tuitionFeeBlo.DeleteAsync(SelectedTuitionFee.Id);
            TuitionFees.Remove(SelectedTuitionFee);
            SelectedTuitionFee = null;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error deleting tuition fee: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }
}
