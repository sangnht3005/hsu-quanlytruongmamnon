using System.Collections.ObjectModel;
using System.Windows.Input;
using KindergartenManagement.BLO;
using KindergartenManagement.DAO;

namespace KindergartenManagement.ViewModels;

public class ReportingViewModel : ViewModelBase
{
    private readonly IReportingBlo _reportingBlo;
    private readonly IClassBlo _classBlo;

    private ObservableCollection<ClassReportDto> _classReports = new();
    private ObservableCollection<StudentReportDto> _studentReports = new();
    private HealthStatisticsDto? _healthStatistics;
    private VaccinationStatisticsDto? _vaccinationStatistics;
    private FinancialReportDto? _financialReport;
    private ClassReportDto? _selectedClassReport;
    private FinancialReportByClassDto? _selectedClassFinancialReport;

    private DateTime _fromDate = DateTime.Now.AddMonths(-1);
    private DateTime _toDate = DateTime.Now;
    private bool _isLoading;

    public ReportingViewModel(IReportingBlo reportingBlo, IClassBlo classBlo)
    {
        _reportingBlo = reportingBlo;
        _classBlo = classBlo;

        LoadReportsCommand = new AsyncRelayCommand(async _ => await LoadReportsAsync(null));
        ExportReportCommand = new RelayCommand(_ => ExportReport());

        _ = LoadReportsAsync(null);
    }

    public ObservableCollection<ClassReportDto> ClassReports
    {
        get => _classReports;
        set => SetProperty(ref _classReports, value);
    }

    public ObservableCollection<StudentReportDto> StudentReports
    {
        get => _studentReports;
        set => SetProperty(ref _studentReports, value);
    }

    public HealthStatisticsDto? HealthStatistics
    {
        get => _healthStatistics;
        set => SetProperty(ref _healthStatistics, value);
    }

    public VaccinationStatisticsDto? VaccinationStatistics
    {
        get => _vaccinationStatistics;
        set => SetProperty(ref _vaccinationStatistics, value);
    }

    public FinancialReportDto? FinancialReport
    {
        get => _financialReport;
        set => SetProperty(ref _financialReport, value);
    }

    public ClassReportDto? SelectedClassReport
    {
        get => _selectedClassReport;
        set => SetProperty(ref _selectedClassReport, value);
    }

    public FinancialReportByClassDto? SelectedClassFinancialReport
    {
        get => _selectedClassFinancialReport;
        set => SetProperty(ref _selectedClassFinancialReport, value);
    }

    public DateTime FromDate
    {
        get => _fromDate;
        set => SetProperty(ref _fromDate, value);
    }

    public DateTime ToDate
    {
        get => _toDate;
        set => SetProperty(ref _toDate, value);
    }

    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    public ICommand LoadReportsCommand { get; }
    public ICommand ExportReportCommand { get; }

    private async Task LoadReportsAsync(object? parameter)
    {
        try
        {
            IsLoading = true;

            // Load Class Reports
            var classReports = await _reportingBlo.GetAllClassReportsAsync(FromDate, ToDate);
            ClassReports = new ObservableCollection<ClassReportDto>(classReports);

            // Load Health Statistics
            HealthStatistics = await _reportingBlo.GetHealthStatisticsAsync(null, FromDate, ToDate);

            // Load Vaccination Statistics
            VaccinationStatistics = await _reportingBlo.GetVaccinationStatisticsAsync();

            // Load Financial Report
            FinancialReport = await _reportingBlo.GetFinancialReportAsync(FromDate, ToDate);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading reports: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void ExportReport()
    {
        // TODO: Implement CSV/Excel export functionality
        System.Diagnostics.Debug.WriteLine("Export report functionality to be implemented");
    }
}
